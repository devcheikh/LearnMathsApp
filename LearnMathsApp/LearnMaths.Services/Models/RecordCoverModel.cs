using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LearnMaths.Services.Models
{
    [DataContract]
    public class RecordCoverModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }
    }
}