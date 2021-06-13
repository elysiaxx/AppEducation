using AppEducation.Models.RoomInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppEducation.Models
{
    public class DocumentViewModel
    {
        public Classes myClass { get; set; }
        public IEnumerable<Document> myDocuments { get; set; }
    }
}
