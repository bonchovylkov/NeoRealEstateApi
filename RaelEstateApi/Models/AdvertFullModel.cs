using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RaelEstateApi.Models
{
    [DataContract]
    public class AdvertFullModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name = "headline")]
        public string Headline { get; set; }

        [DataMember(Name = "content")]
        public string Text { get; set; }

        [DataMember(Name = "pictures")]
        public IEnumerable<string> Pictures { get; set; }

        [DataMember(Name = "town")]
        public string Town { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "tags")]
        public IEnumerable<string> Tags { get; set; }

        [DataMember(Name="postDate")]
        public DateTime PostDate { get; set; }

        [DataMember(Name="price")]
        public decimal Price { get; set; }
    }
}