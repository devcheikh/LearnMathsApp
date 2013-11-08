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
    public class QuestionModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        public static Expression<Func<Question, QuestionModel>> FromQuestion
        {
            get
            {
                return question => new QuestionModel
                {
                    Id = question.Id,
                    Category = question.Category.Name,
                    Text = question.Text
                };
            }
        }
    }
}