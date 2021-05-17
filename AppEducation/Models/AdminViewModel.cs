using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using AppEducation.Models.Users;
namespace AppEducation.Models
{
  public class TotalInformation {
      public List<Room> Rooms {get; set;}
    
      public IEnumerable<AppUser> Users {get; set;}
  }
    public class ClassInfo
    {
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public string Topic { get; set; }
        public string TeacherId { get; set; }
        [DefaultValue(0)]
        public int OnlineStudent { get; set; }
    }
}
