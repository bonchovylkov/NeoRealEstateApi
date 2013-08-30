using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealEstateModels
{
  public class Tag
    {
      public int Id { get; set; }
      public string Name { get; set; }

      public virtual ICollection<Advert> Adverts { get; set; }

      public Tag()
      {
          this.Adverts = new HashSet<Advert>();
      }
    }
}
