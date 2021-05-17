using AppEducation.Models;
using AppEducation.Models.Users;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Globalization;

namespace AppEducation.Hubs
{
    // connectionhub
    public class ConnectionHub : Hub<IConnectionHub>
    {
        private readonly List<Room> _rooms;
        private readonly AppIdentityDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor; 
         
        public ConnectionHub(List<Room> rooms, AppIdentityDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _rooms = rooms;
            _context = context;
        }
        public async Task Join(string username, string classid,bool isCaller)
        {
            UserCall usr = new UserCall { FullName = username, ConnectionID = Context.ConnectionId ,IsCaller = isCaller,InCall= false};
            Classes clr = _context.Classes.Find(classid);
            if (clr == null)
            {
                //do something if no class has been found
                return;
            }
            else
            {
                Room room = GetRoomByClassID(classid);
                if (room == null)
                {
                    _rooms.Add(new Room
                    {
                        RoomIF = clr,
                        UserCalls = new List<UserCall> { usr }
                    });
                    await SendUserListUpdate(GetRoomByClassID(classid));
                    //await Clients.Client(usr.ConnectionID).initDevices(usr);
                }
                else
                {
                    room.UserCalls.Add(usr);
                    await SendUserListUpdate(room);
                    //await Clients.Client(usr.ConnectionID).initDevices(usr);
                    room.UserCalls.ForEach(async u =>
                    {
                        if (u != usr)
                        {
                            await Clients.Client(u.ConnectionID).NotifyNewMember(usr);
                        }
                    });
                }
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            // Hang up any calls the user is in
            await HangUp(); // Gets the user from "Context" which is available in the whole hub

            // Remove the user
            callingRoom.UserCalls.RemoveAll(u => u.ConnectionID == Context.ConnectionId);

            // Send down the new user list to all clients
            await SendUserListUpdate(callingRoom);

            await base.OnDisconnectedAsync(exception);
        }


        public async Task CallUser(UserCall targetUser)
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            UserCall callingUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            // Make sure the person we are trying to call is still here
            if (targetUser == null)
            {
                // If not, let the caller know
                await Clients.Caller.CallDeclined(targetUser,"The user you called has left.");
                return;
            }
            // They are here, so tell them someone wants to talk
            await Clients.Client(targetUser.ConnectionID).IncomingCall(callingUser);
        }

        public async Task AnswerCall(bool acceptCall, UserCall targetConnectionId)
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            UserCall callingUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            var targetUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == targetConnectionId.ConnectionID);

            // This can only happen if the server-side came down and clients were cleared, while the user
            // still held their browser session.
            if (callingUser == null)
            {
                return;
            }

            // Make sure the original caller has not left the page yet
            if (targetUser == null)
            {
                await Clients.Caller.CallEnded(targetConnectionId, "The other user in your call has left.");
                return;
            }

            // Send a decline message if the callee said no
            if (acceptCall == false)
            {
                await Clients.Client(targetConnectionId.ConnectionID).CallDeclined(callingUser, string.Format("{0} did not accept your call.", callingUser.FullName));
                return;
            }

            callingUser.InCall = true;
            targetUser.InCall = true;
            // Make sure there is still an active offer.  If there isn't, then the other use hung up before the Callee answered.
            // Tell the original caller that the call was accepted
            await Clients.Client(targetConnectionId.ConnectionID).CallAccepted(callingUser);

            // Update the user list, since thes two are now in a call
            //await SendUserListUpdate();
        }

        public async Task HangUp()
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            UserCall callingUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            // if room is mine . Remove all user in call
            
            if (callingRoom.UserCalls.Count <= 1)
            {
                // do something
                _context.Classes.Find(callingRoom.RoomIF.ClassID).isActive = false;
                _context.Classes.Find(callingRoom.RoomIF.ClassID).OnlineStudent = 0;
                var hoc = _context.HOClasses.Find(callingRoom.RoomIF.hocID);
                hoc.endTime = DateTime.Now;

                _rooms.Remove(callingRoom);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Classes.SingleOrDefault(c => c.ClassID == callingRoom.RoomIF.ClassID).OnlineStudent -= 1;
                _context.SaveChanges();
                callingRoom.UserCalls.Remove(callingUser);
            }

            // Send a hang up message to each user in the call, if there is one
            if (callingRoom != null)
            {
                foreach (UserCall user in callingRoom.UserCalls.Where(u => u.ConnectionID != callingUser.ConnectionID))
                {
                    await Clients.Client(user.ConnectionID).CallEnded(callingUser, string.Format("{0} has hung up.", callingUser.FullName));

                }
                await SendUserListUpdate(callingRoom);
            }
        }

        // WebRTC Signal Handler
        public async Task SendSignal(string signal, string targetConnectionId)
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            UserCall callingUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            UserCall targetUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == targetConnectionId);
            // Make sure both users are valid
            if (callingUser == null || targetUser == null)
            {
                return;
            }

            // These folks are in a call together, let's let em talk WebRTC
            await Clients.Client(targetConnectionId).ReceiveSignal(callingUser, signal);
        }
        public async Task getConnectionID()
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            UserCall callingUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).getConnectionID(callingUser);
        }
        #region chat method
        /// <summary>
        /// Send message to All people in this room
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string message)
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            UserCall callingUser = callingRoom.UserCalls.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            await Clients.All.ReceiveMessageByAllAsync(callingUser,message);
        }
        /// <summary>
        /// Send message to a person in this rom with his connectionid 
        /// </summary>
        /// <returns></returns>
        public async Task SendMessageToClient(string targetConnectionId,string message)
        {
            Room callingRoom = GetRoomByConnectionID(Context.ConnectionId);
            UserCall callingUser = callingRoom.UserCalls.SingleOrDefault(uc => uc.ConnectionID == Context.ConnectionId);
            if(callingUser == null)
            {
                // does not find out user that did this process 
                return;
            }
            UserCall targetUser = callingRoom.UserCalls.SingleOrDefault(uc => uc.ConnectionID == targetConnectionId);
            if(targetUser == null)
            {
                // doesn't find out target user
                await Clients.Client(callingUser.ConnectionID).SendMessageOnError();
            }
            await Clients.Client(callingUser.ConnectionID).ReceiveMessageByOneAsync(callingUser,message);
        }
        #endregion
        #region Private Helpers

        private async Task SendUserListUpdate(Room room)
        {
            foreach (UserCall u in room.UserCalls)
            {
                await Clients.Client(u.ConnectionID).UpdateUserList(room.UserCalls);
            }
        }
    
        private Room GetRoomByConnectionID(string cid)
        {
            Room matchingRoom = _rooms.SingleOrDefault(r => r.UserCalls.SingleOrDefault(u => u.ConnectionID == cid) != null);
            return matchingRoom;
        }
        private Room GetRoomByClassID(string classid)
        {
            Room matchingRoom = _rooms.SingleOrDefault<Room>(r => r.RoomIF.ClassID == classid);
            return matchingRoom;
        }
        #endregion
    }

    public interface IConnectionHub
    {
        Task CallAccepted(UserCall callingUser);
        Task CallDeclined(UserCall u, string v);
        Task CallEnded(UserCall targetConnectionId, string v);
        Task getConnectionID(UserCall callingUser);
        Task IncomingCall(UserCall callingUser);
        Task initDevices(UserCall UserCalls);
        Task NotifyNewMember(UserCall usr);
        Task ReceiveSignal(UserCall callingUser, string signal);
        Task UpdateUserList(List<UserCall> userCalls);
        Task ReceiveMessageByAllAsync(UserCall callingUser, string message);
        Task ReceiveMessageByOneAsync(UserCall callingUser, string message);
        Task SendMessageOnError();
    }
}
