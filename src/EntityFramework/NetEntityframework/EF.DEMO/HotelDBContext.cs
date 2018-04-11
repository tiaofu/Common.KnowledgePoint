using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.First
{
    public class HotelDBContext:DbContext
    {
        public HotelDBContext()
            : base("name=ConnCodeFirst")
        {
            //Database.SetInitializer<HotelDBContext>(null);
        }
        public DbSet<UserInfo> customer { get; set; }
    }
}
