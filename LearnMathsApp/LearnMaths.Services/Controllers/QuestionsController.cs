using LearnMaths.Data;
using LearnMaths.Services.Attributes;
using LearnMaths.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;

namespace LearnMaths.Services.Controllers
{
    public class QuestionsController : BaseApiController
    {
        private const int NumberQuestions = 5;

        [ActionName("list")]
        [HttpGet]
        public IEnumerable<QuestionModel> GetListOfQuestions(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.Data.Users.All().FirstOrDefault(
                    x => x.SessionKey == sessionKey);

                if (user == null || sessionKey == string.Empty)
                {
                    throw new InvalidOperationException("Invalid sessionKey");
                }
				
				var categoryId = this.Data.Records.All().Where(x => x.Id == id).FirstOrDefault().CategoryId;
                var questions = this.Data.Questions.All().Where(x => x.CategoryId == categoryId).
                    OrderBy(x => Guid.NewGuid()).Take(NumberQuestions).Select(QuestionModel.FromQuestion).ToList();

                return questions;
            });

            return responseMsg;
        }

        [ActionName("all")]
        [HttpGet]
        public IEnumerable<QuestionModel> GetAllQuestions(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.Data.Users.All().FirstOrDefault(
                    x => x.SessionKey == sessionKey);

                if (user == null || sessionKey == string.Empty || user.UserRole.Role != "Admin")
                {
                    throw new InvalidOperationException("Invalid sessionKey");
                }

                var questions = this.Data.Questions.All().OrderBy(x => x.CategoryId).
                    Select(QuestionModel.FromQuestion).ToList();

                return questions;
            });

            return responseMsg;
        }

        [ActionName("byid")]
        [HttpGet]
        public QuestionFullModel GetByIdQuestion(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.Data.Users.All().FirstOrDefault(
                    x => x.SessionKey == sessionKey);

                if (user == null || sessionKey == string.Empty)
                {
                    throw new InvalidOperationException("Invalid sessionKey");
                }

                var question = this.Data.Questions.All().Where(x => x.Id == id).
                    Select(QuestionFullModel.FromQuestion).FirstOrDefault();

                return question;
            });

            return responseMsg;
        }

        [ActionName("create")]
        [HttpPost]
        public HttpResponseMessage PostCreateQuestion(QuestionFullModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var user = this.Data.Users.All().FirstOrDefault(
                      x => x.SessionKey == sessionKey);

                  if (user == null || sessionKey == string.Empty || user.UserRole.Role != "Admin")
                  {
                      throw new InvalidOperationException("Invalid sessionKey");
                  }

                  ValidateModel(model);
                  var category = this.Data.Categories.All().Where(x => x.Name == model.Category).FirstOrDefault();
                  var question = new Question()
                  {
                      Text = model.Text,
                      Category = category
                  };

                  this.Data.Questions.Add(question);
                  this.Data.SaveChanges();

                  var answerA = new Answer()
                  {
                      Text = model.AText,
                      IsCorrect = false
                  };

                  var answerB = new Answer()
                  {
                      Text = model.BText,
                      IsCorrect = false
                  };

                  var answerC = new Answer()
                  {
                      Text = model.CText,
                      IsCorrect = false
                  };

                  if (model.Correct == "A")
                  {
                      answerA.IsCorrect = true;
                  }
                  else if (model.Correct == "B")
                  {
                      answerB.IsCorrect = true;
                  }
                  else if (model.Correct == "C")
                  {
                      answerC.IsCorrect = true;
                  }

                  question.Answers.Add(answerA);
                  question.Answers.Add(answerB);
                  question.Answers.Add(answerC);
                  this.Data.SaveChanges();

                  var response =
                      this.Request.CreateResponse(HttpStatusCode.OK);
                  return response;
              });

            return responseMsg;
        }

        [ActionName("edit")]
        [HttpPost]
        public HttpResponseMessage PostEditQuestion(QuestionFullModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var user = this.Data.Users.All().FirstOrDefault(
                      x => x.SessionKey == sessionKey);

                  if (user == null || sessionKey == string.Empty || user.UserRole.Role != "Admin")
                  {
                      throw new InvalidOperationException("Invalid sessionKey");
                  }

                  ValidateModel(model);
                  var category = this.Data.Categories.All().Where(x => x.Name == model.Category).FirstOrDefault();
                  var question = this.Data.Questions.All().Where(x => x.Id == model.Id).FirstOrDefault();
                  question.Text = model.Text;
                  question.Category = category;
                  var answers = question.Answers.OrderBy(x => x.Id).ToList();
                  answers[0].Text = model.AText;
                  answers[1].Text = model.BText;
                  answers[2].Text = model.CText;

                  if (model.Correct == "A")
                  {
                      answers[0].IsCorrect = true;
                      answers[1].IsCorrect = false;
                      answers[2].IsCorrect = false;
                  }
                  else if (model.Correct == "B")
                  {
                      answers[0].IsCorrect = false;
                      answers[1].IsCorrect = true;
                      answers[2].IsCorrect = false;
                  }
                  else if (model.Correct == "C")
                  {
                      answers[0].IsCorrect = false;
                      answers[1].IsCorrect = false;
                      answers[2].IsCorrect = true;
                  }

                  this.Data.SaveChanges();

                  var response =
                      this.Request.CreateResponse(HttpStatusCode.OK);
                  return response;
              });

            return responseMsg;
        }

        [ActionName("delete")]
        [HttpPost]
        public HttpResponseMessage PostDeleteQuestion(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var user = this.Data.Users.All().FirstOrDefault(
                      x => x.SessionKey == sessionKey);

                  if (user == null || sessionKey == string.Empty || user.UserRole.Role != "Admin")
                  {
                      throw new InvalidOperationException("Invalid sessionKey");
                  }

                  var question = this.Data.Questions.All().Where(x => x.Id == id).FirstOrDefault();
                  var answers = question.Answers.ToList();
                  for (int count = 0; count < answers.Count; count++)
                  {
                      this.Data.Answers.Delete(answers[count]);
                      this.Data.SaveChanges();
                  }

                  this.Data.Questions.Delete(question);
                  this.Data.SaveChanges();

                  var response =
                      this.Request.CreateResponse(HttpStatusCode.OK);
                  return response;
              });

            return responseMsg;
        }

        private void ValidateModel(QuestionFullModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                throw new InvalidOperationException("Question Text should be not empty or white space");
            }

            if (string.IsNullOrWhiteSpace(model.AText) || string.IsNullOrWhiteSpace(model.BText) || 
                string.IsNullOrWhiteSpace(model.CText))
            {
                throw new InvalidOperationException("Answer Text should be not empty or white space");
            }

            if (string.IsNullOrWhiteSpace(model.Correct))
            {
                throw new InvalidOperationException("You should choose correct answer");
            }

            if (string.IsNullOrWhiteSpace(model.Category))
            {
                throw new InvalidOperationException("You should choose question's category");
            }
        }
    }
}
