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
    public class AnswerModel
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "iscorrect")]
        public bool IsCorrect { get; set; }

        public static Expression<Func<Answer, AnswerModel>> FromAnswer
        {
            get
            {
                return answer => new AnswerModel
                {
                    Text = answer.Text,
                    IsCorrect = answer.IsCorrect
                };
            }
        }
    }
}