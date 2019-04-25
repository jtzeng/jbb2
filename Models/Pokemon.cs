using System;
using System.ComponentModel.DataAnnotations;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

namespace JustinBB.Models
{
    public class Pokemon
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        public string Owner { get; set; }

        public string[] Moves { get; set; }
    }
}
