using LearnMaths.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web;

namespace LearnMaths.Services.Models
{
    [DataContract]
    public class RecordModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "cover")]
        public string Cover { get; set; }

        public static Expression<Func<Record, RecordModel>> FromRecord
        {
            get
            {
                return record => new RecordModel
                {
                    Id = record.Id,
                    Category = record.Category.Name,
                    Cover = record.CoverLevel.State
                };
            }
        }
    }
}