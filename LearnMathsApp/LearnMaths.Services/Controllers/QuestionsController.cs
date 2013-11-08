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

                var questions = this.Data.Questions.All().Where(x => x.CategoryId == id).
                    OrderBy(x => new Guid()).Take(NumberQuestions).Select(QuestionModel.FromQuestion).ToList();

                return questions;
            });

            return responseMsg;
        }
    }
}
