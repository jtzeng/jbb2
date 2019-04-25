using System;
using System.ComponentModel.DataAnnotations;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

using System.Linq;

namespace JustinBB.Models
{
    public class Post
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(typeof(Topic), OnDelete = "CASCADE")]
        public int TopicID { get; set; }

        [ForeignKey(typeof(User), OnDelete = "CASCADE")]
        public int UserID { get; set; }

        public string Contents { get; set; }

        public string ImageID { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Ignore]
        public string Score
        {
            get
            {
                var votes = Votes;
                // Console.WriteLine(votes + " votes");
                if (votes > 0)
                {
                    return "+" + votes;
                }
                return votes.ToString();
            }
        }

        [Ignore]
        public string Username { get; set; } = "";

        [Ignore]
        public int Votes
        {
            get
            {
                return Program.DB.Select<Vote>(x => x.PostID == ID).Sum(x => x.Direction);
            }
        }

        public void SetVote(User u, int direction)
        {
            var votes = Program.DB.Select<Vote>(x => x.PostID == ID && x.UserID == u.ID);
            Program.DB.DeleteAll(votes);

            var v = new Vote();
            v.PostID = ID;
            v.UserID = u.ID;
            v.Direction = direction;
            Program.DB.Insert<Vote>(v);
        }

        public static Post GetPost(int PostID)
        {
            var posts = Program.DB.Select<Post>(x => x.ID == PostID);
            if (posts.Count > 0)
            {
                return posts[0];
            }
            return null;
        }
    }
}
