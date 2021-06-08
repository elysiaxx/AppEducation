using AppEducation.Models.RoomInfo;
using AppEducation.Models.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace AppEducation.Models.RoomInfo
{
    public class Document
    {
        [Key]
        public string DocumentID { get; set; }
        public string Path { get; set; }
        public string Describe { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RoomDocumentID { get; set; }
        public RoomDocument RoomDocuments { get; set; }
    }
    public class File
    {
        [Key]
        public string FileID { get; set; }
        public string Path { get; set; }
        public string UserID { get; set; }
    }
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
        public string Filename { get; set; }
    }
}
