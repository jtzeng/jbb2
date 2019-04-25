using System;
using System.ComponentModel.DataAnnotations;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace JustinBB.Models
{
    public class NewPost
    {
        public int TopicID { get; set; }

        public string Contents { get; set; }

        public IFormFile File { get; set; }
    }
}
