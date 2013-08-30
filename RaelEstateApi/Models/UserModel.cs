using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RaelEstateApi.Models
{
    [DataContract]
    public class UserModel
    {
        [DataMember(Name="username")]
        public string Username { get; set; }

        [DataMember(Name = "fullName")]
        public string FullName { get; set; }

        [DataMember(Name="authCode")]
        public string AuthCode { get; set; }
    }
}