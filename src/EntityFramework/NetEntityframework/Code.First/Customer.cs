using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.First
{
    public class Customer
    {
        [Key]//标识这是对应数据库表的主键  
        public int Id { get; set; }

        public string CusName { get; set; }

        public string CusId { get; set; }
    }
}
