using RealEstateModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateData
{
    public class RealEstateContext : DbContext
    {
        public RealEstateContext()
            : base("RealEstateContext")
        {

        }

        public DbSet<Advert> Adverts { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
