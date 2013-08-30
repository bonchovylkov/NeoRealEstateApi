using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace RaelEstateApi.Models
{
    [DataContract(Name = "LoggedUser")]
    public class LoggedUserModel
    {
        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }

        [DataMember(Name = "displayName")]
        public string Nickname { get; set; }
    }
}