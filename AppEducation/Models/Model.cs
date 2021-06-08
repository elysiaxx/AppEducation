using AppEducation.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppEducation.Models
{
    public class Classes
    {
        [Key]
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public string Topic { get; set; }
        public bool isActive { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
    public class HistoryOfClass
    {
        public string hocID { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
    public class Room
    {
        public Classes RoomIF { get; set; }
        public List<UserCall> UserCalls { get; set; }
    }
    public class UserCall
    {
        public string FullName { get; set; }
        public string ConnectionID { get; set; }
        public bool InCall { get; set; }
        public bool IsCaller { get; set; }
    }
}
