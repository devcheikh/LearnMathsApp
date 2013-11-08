using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LearnMaths.Services.Models
{
    [DataContract]
    public class RecordResponseModel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
    }
}