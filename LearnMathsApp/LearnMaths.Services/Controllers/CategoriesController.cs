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
    public class CategoriesController : BaseApiController
    {
        [ActionName("all")]
        [HttpGet]
        public IEnumerable<CategoryModel> GetAllCategories(
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

                var categories = this.Data.Categories.All().OrderBy(x => x.LevelId).
                    Select(CategoryModel.FromCategory).ToList();

                return categories;
            });

            return responseMsg;
        }
    }
}
