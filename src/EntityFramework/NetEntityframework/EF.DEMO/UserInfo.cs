using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.First
{
    [Table("UserInfo",Schema ="dbo")]
    public class UserInfo
    {
        [Key,Column("Id")]
        public string Id { get; set; }
        [Column("UserName")]
        public string UserName { get; set; }
        [Column("UserCode")]
        public string UserCode { get; set; }
        [Column("PassWord")]
        public string PassWord { get; set; }
        [Column("PassWord2")]
        public string PassWord2 { get; set; }
        [Column("PassWord3")]
        public string PassWord3 { get; set; }
        [Column("PassWord4")]
        public string PassWord4 { get; set; }
    }
}
