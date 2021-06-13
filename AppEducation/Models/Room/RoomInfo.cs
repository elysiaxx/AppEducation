using AppEducation.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppEducation.Models.RoomInfo
{
        public class RoomMember
        {
            [Key]
            public string RoomMemberID { get; set; }

            public virtual ICollection<AppUser> Members { get; set; }

            public string ClassID { get; set; }
            public Classes Classes { get; set; }
        }
        public class RoomDocument
        {
            [Key]
            public string RoomDocumentID { get; set; }

            public string ClassInfoID { get; set; }
            public Classes Classes { get; set; }
        }
}
