﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateModels
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string AuthCode { get; set; }
        public string SessionKey { get; set; }
        public virtual Role Role { get; set; }

        public virtual ICollection<Advert> Adverts { get; set; }

        public User()
        {

            this.Adverts = new HashSet<Advert>();
        }

    }
}
