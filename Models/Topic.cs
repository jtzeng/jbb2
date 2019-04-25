using System;
using System.ComponentModel.DataAnnotations;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

using System.Collections.Generic;
using System.Linq;

namespace JustinBB.Models
{
    public class Topic
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(typeof(User), OnDelete = "CASCADE")]
        public int UserID { get; set; }

        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Ignore]
        public Post FirstPost
        {
            get
            {
                var posts = Program.DB.Select<Post>(x => x.TopicID == ID).OrderBy(x => x.Date);
                // Console.WriteLine($"Topic {ID} has (first) {posts.Count()} posts.");
                return posts.First();
            }
        }

        [Ignore]
        public Post LastPost
        {
            get
            {
                var posts = Program.DB.Select<Post>(x => x.TopicID == ID).OrderByDescending(x => x.Date);
                // Console.WriteLine($"Topic {ID} has (last) {posts.Count()} posts.");
                return posts.First();
            }
        }

        [Ignore]
        public List<Post> Posts
        {
            get
            {
                var posts = Program.DB.Select<Post>(x => x.TopicID == ID);
                foreach (var p in posts)
                {
                    // p.Score = p.GetVotesFormatted();
                    p.Username = User.GetUser(p.UserID).Username;
                }
                return posts;
            }
        }

        [Ignore]
        public int Count
        {
            get
            {
               return Program.DB.Select<Post>(x => x.TopicID == ID).Count;
            }
        }

        public static Topic GetTopic(int TopicID)
        {
            var topics = Program.DB.Select<Topic>(x => x.ID == TopicID);
            if (topics.Count > 0)
            {
                return topics[0];
            }
            return null;
        }

        public static List<Topic> GetTopics()
        {
            var topics = Program.DB.Select<Topic>().OrderByDescending(t => t.LastPost.Date).ToList();
            foreach (var t in topics)
            {
                // t.FirstPost.Score = t.GetFirstPost().GetVotesFormatted();


                // t.FirstPost = t.GetFirstPost();
                // t.FirstPost.Score = t.GetFirstPost().GetVotesFormatted();
                // t.LastPost = GetLastPost(t.ID);
                // t.Count = CountPosts(t.ID);
            }
            return topics;
        }
    }
}
