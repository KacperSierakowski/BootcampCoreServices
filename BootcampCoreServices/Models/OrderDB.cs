using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootcampCoreServices
{
    class OrderDB : DbContext
    {
        public DbSet<Request> Orders { get; set; }
    }
}
