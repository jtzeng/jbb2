using System;
using System.ComponentModel.DataAnnotations;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

namespace JustinBB.Models
{
    public class User
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public long Cash { get; set; }

        public int[] Slots { get; set; }

        [Ignore]
        public bool Register { get; set; }

        public static User GetUser(string username)
        {
            var rows = Program.DB.Select<User>(x => x.Username == username);
            if (rows.Count > 0)
            {
                return rows[0];
            }
            return null;
        }

        public static User GetUser(int UserID)
        {
            var rows = Program.DB.Select<User>(x => x.ID == UserID);
            if (rows.Count > 0)
            {
                return rows[0];
            }
            return null;
        }
    }
}
