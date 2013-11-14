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
    public class QuestionFullModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "atext")]
        public string AText { get; set; }

        [DataMember(Name = "btext")]
        public string BText { get; set; }

        [DataMember(Name = "ctext")]
        public string CText { get; set; }

        [DataMember(Name = "correct")]
        public string Correct { get; set; }

        public static Expression<Func<Question, QuestionFullModel>> FromQuestion
        {
            get
            {
                return question => new QuestionFullModel
                {
                    Id = question.Id,
                    Category = question.Category.Name,
                    Text = question.Text,
                    AText = question.Answers.OrderBy(x => x.Id).Take(1).FirstOrDefault().Text,
                    BText = question.Answers.OrderBy(x => x.Id).Skip(1).Take(1).FirstOrDefault().Text,
                    CText = question.Answers.OrderBy(x => x.Id).Skip(2).Take(1).FirstOrDefault().Text,
                    Correct = 
                    question.Answers.OrderBy(x => x.Id).Take(1).FirstOrDefault().IsCorrect ? "A" : 
                    question.Answers.OrderBy(x => x.Id).Skip(1).Take(1).FirstOrDefault().IsCorrect ? "B" : "C"
                };
            }
        }

        private static string GetCorrectString(Question question)
        {
            string correct = string.Empty;
            var answers = question.Answers.OrderBy(x => x.Id).ToList();
            if (answers[0].IsCorrect)
            {
                correct = "A";
            }
            else if (answers[1].IsCorrect) 
            {
                correct = "A";
            }
            else if (answers[2].IsCorrect)
            {
                correct = "C";
            }

            return correct;
        }
    }
}