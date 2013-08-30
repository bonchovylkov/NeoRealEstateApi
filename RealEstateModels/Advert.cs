using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealEstateModels
{
    public class Advert
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string Text { get; set; }
        public string  Picture { get; set; }
        public virtual Town Town { get; set; }
        public string Adress { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public Advert()
        {
            this.Tags = new HashSet<Tag>();
        }
    }
}
