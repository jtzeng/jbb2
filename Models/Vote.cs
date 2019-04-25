using System;
using System.ComponentModel.DataAnnotations;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

// using System.ComponentModel.DataAnnotations.Schema;

namespace JustinBB.Models
{
    public class Vote
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(typeof(Post), OnDelete = "CASCADE")]
        public int PostID { get; set; }

        [ForeignKey(typeof(User), OnDelete = "CASCADE")]
        public int UserID { get; set; }

        [Unique]
        public int Direction { get; set; }
    }
}
