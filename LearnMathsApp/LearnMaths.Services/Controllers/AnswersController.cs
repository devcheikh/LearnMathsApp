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
    public class AnswersController : BaseApiController
    {
        [ActionName("list")]
        [HttpGet]
        public IEnumerable<AnswerModel> GetListOfAnswers(int id,
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

                var answers = this.Data.Answers.All().Where(x => x.QuestionId == id).
                    Select(AnswerModel.FromAnswer).ToList();

                return answers;
            });

            return responseMsg;
        }
    }
}
