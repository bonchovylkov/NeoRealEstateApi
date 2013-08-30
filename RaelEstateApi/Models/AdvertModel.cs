﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RaelEstateApi.Models
{
    [DataContract]
    public class AdvertModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name="headline")]
        public string Headline { get; set; }
        
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "town")]
        public string Town { get; set; }
    }
}