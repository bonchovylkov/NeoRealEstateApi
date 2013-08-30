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
        public int Id { get; set; }

        public string Headline { get; set; }

        public string Text { get; set; }

        public string[] Pictures { get; set; }

        public string Town { get; set; }

        public string Address { get; set; }

        public string PostedBy { get; set; }

        public string[] Tags { get; set; }
    }
}