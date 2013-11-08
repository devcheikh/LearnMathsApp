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
    public class RecordsController : BaseApiController
    {
        private const string StateFail = "Fail";
        private const string StatePass = "Pass";
        private const string StateDone = "Done";
        private const int PassLimit = 50;
        private const int DoneLimit = 80;

        [ActionName("create")]
        [HttpPost]
        public HttpResponseMessage PostRecord(RecordPostModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var user = this.Data.Users.All().FirstOrDefault(
                      x => x.SessionKey == sessionKey);

                  if (user == null || sessionKey == string.Empty)
                  {
                      throw new InvalidOperationException("Invalid sessionKey");
                  }

                  var categoryRank = this.Data.Records.All().Where(x => x.UserId == user.Id).
                      Select(x => x.Category).OrderByDescending(x => x.Rank).FirstOrDefault().Rank;

                  var nextCategory = this.Data.Categories.All().
                      Where(x => x.Level.Name == model.Level).FirstOrDefault(x => x.Rank == categoryRank + 1);
                  var responseModel = new RecordResponseModel()
                  {
                      Id = null
                  };

                  if (nextCategory != null)
                  {
                      var record = new Record()
                      {
                          Category = nextCategory,
                          User = user,
                          CoverLevel = this.Data.CoverLevels.All().Where(x => x.State == StateFail).FirstOrDefault()
                      };

                      this.Data.Records.Add(record);
                      this.Data.SaveChanges();
                      responseModel.Id = record.Id.ToString();
                  }

                  var response =
                      this.Request.CreateResponse(HttpStatusCode.OK, responseModel);
                  return response;
              });

            return responseMsg;
        }

        [ActionName("cover")]
        [HttpPost]
        public HttpResponseMessage PostChangeCover(RecordCoverModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var user = this.Data.Users.All().FirstOrDefault(
                      x => x.SessionKey == sessionKey);

                  if (user == null || sessionKey == string.Empty)
                  {
                      throw new InvalidOperationException("Invalid sessionKey");
                  }

                  var record = this.Data.Records.All().Where(x => x.Id == model.Id).FirstOrDefault();
                  if (record == null)
                  {
                      throw new InvalidOperationException("Invalid record");
                  }

                  var state = string.Empty;
                  if (model.Score >= DoneLimit)
                  {
                      state = StateDone;
                  }
                  else if (model.Score >= PassLimit && model.Score < DoneLimit)
                  {
                      state = StatePass;
                  }
                  else if (model.Score < PassLimit)
                  {
                      state = StateFail;
                  }

                  record.CoverLevel = this.Data.CoverLevels.All().Where(x => x.State == state).FirstOrDefault();
                  this.Data.SaveChanges();

                  var responseModel = new RecordCoverResponseModel()
                  {
                      State = state
                  };

                  var response =
                      this.Request.CreateResponse(HttpStatusCode.OK, responseModel);
                  return response;
              });

            return responseMsg;
        }

        [ActionName("clear")]
        [HttpPost]
        public HttpResponseMessage PostClearRecords(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var user = this.Data.Users.All().FirstOrDefault(
                      x => x.SessionKey == sessionKey);

                  if (user == null || sessionKey == string.Empty)
                  {
                      throw new InvalidOperationException("Invalid sessionKey");
                  }

                  var records = this.Data.Records.All().Where(x => x.UserId == user.Id).ToList();
                  foreach (Record record in records)
                  {
                      this.Data.Records.Delete(record);
                  }

                  this.Data.SaveChanges();
                  var response =
                      this.Request.CreateResponse(HttpStatusCode.OK);
                  return response;
              });

            return responseMsg;
        }

        [ActionName("list")]
        [HttpGet]
        public IEnumerable<RecordModel> GetListOfRecords(
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

                var records = this.Data.Records.All().Where(x => x.UserId == user.Id).
                    Select(RecordModel.FromRecord).ToList();

                return records;
            });

            return responseMsg;
        }
    }
}
