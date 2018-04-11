using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.First
{
    public class HotelDBContext:DbContext
    {
        public HotelDBContext()
            : base("name=ConnCodeFirst")
        {
            //Database.SetInitializer<HotelDBContext>(null);
        }
        public DbSet<Customer> customer { get; set; }
    }
}
