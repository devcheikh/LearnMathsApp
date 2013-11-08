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
    public class UsersController : BaseApiController
    {
        private const int MinLength = 6;
        private const int MaxPasswordNameLength = 30;
        private const int MaxUsernameLength = 50;
        private const string ValidUsernameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";
        private const string ValidNameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
        private const string RoleUser = "User";

//{
//"username": "ssssss",
//"name": "SSSSSS",
//"password": "password"
//}

        [ActionName("register")]
        [HttpPost]
        public HttpResponseMessage PostRegisterUser(UserPostModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    this.ValidateUsername(model.Username);
                    this.ValidateName(model.Name);
                    this.ValidatePassword(model.Password);
                    var usernameToLower = model.Username.ToLower();
                    User user = this.Data.Users.All().FirstOrDefault(
                        x => x.Username == usernameToLower);

                    if (user != null)
                    {
                        throw new InvalidOperationException("User already exists");
                    }

                    user = new User()
                    {
                        Username = usernameToLower,
                        Name = model.Name,
                        Password = model.Password,
                        SessionKey = string.Empty,
                        Level = this.Data.Levels.All().OrderBy(x => x.Rank).FirstOrDefault(),
                        UserRole = this.Data.UserRoles.All().Where(x => x.Role == RoleUser).FirstOrDefault()
                    };

                    this.Data.Users.Add(user);
                    this.Data.SaveChanges();

                    user.SessionKey = user.Id.ToString();
                    this.Data.SaveChanges();

                    var loggedModel = new UserLoggedModel()
                    {
                        SessionKey = user.SessionKey
                    };

                    var response =
                        this.Request.CreateResponse(HttpStatusCode.Created,
                                        loggedModel);
                    return response;
                });

            return responseMsg;
        }

        [ActionName("login")]
        [HttpPost]
        public HttpResponseMessage PostLoginUser(UserPostModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                    this.ValidateUsername(model.Username);
                    this.ValidatePassword(model.Password);
                    var usernameToLower = model.Username.ToLower();
                    var user = this.Data.Users.All().FirstOrDefault(
                        x => x.Username == usernameToLower
                        && x.Password == model.Password);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid username or password");
                    }

                    if (user.SessionKey == string.Empty)
                    {
                        user.SessionKey = user.Id.ToString();
                        this.Data.SaveChanges();
                    }

                    var loggedModel = new UserLoggedModel()
                    {
                        SessionKey = user.SessionKey
                    };

                    var response =
                        this.Request.CreateResponse(HttpStatusCode.Created,
                                        loggedModel);
                    return response;
            });

            return responseMsg;
        }

        [ActionName("logout")]
        [HttpPost]
        public HttpResponseMessage PostLogoutUser(
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

                    user.SessionKey = string.Empty;
                    this.Data.SaveChanges();

                    var response =
                        this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
              });

            return responseMsg;
        }

        [ActionName("levelup")]
        [HttpPost]
        public HttpResponseMessage PostLevelUpUser(
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

                  var levelRank = user.Level.Rank;
                  var upLevel = this.Data.Levels.All().Where(x => x.Rank == levelRank + 1).FirstOrDefault();
                  if (upLevel == null)
                  {
                      throw new InvalidOperationException("There are no more levels");
                  }

                  user.Level = upLevel;
                  this.Data.SaveChanges();

                  var model = new UserInfoModel()
                  {
                      Id = user.Id,
                      Name = user.Name,
                      Role = user.UserRole.Role,
                      Level = user.Level.Name
                  };

                  var response =
                      this.Request.CreateResponse(HttpStatusCode.OK, model);
                  return response;
              });

            return responseMsg;
        }

        [ActionName("info")]
        [HttpGet]
        public UserInfoModel GetUserInfo(
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

                var model = new UserInfoModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.UserRole.Role,
                    Level = user.Level.Name
                };

                return model;
            });

            return responseMsg;
        }

        private void ValidatePassword(string password)
        {
            if (password == null || password.Length < MinLength || password.Length > MaxPasswordNameLength)
            {
                throw new ArgumentOutOfRangeException("Password is invalid");
            }
        }

        private void ValidateName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("Nickname cannot be null");
            }
            else if (name.Length < MinLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Name must be at least {0} characters long",
                    MinLength));
            }
            else if (name.Length > MaxPasswordNameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Name must be less than {0} characters long",
                    MaxUsernameLength));
            }
            else if (name.Any(ch => !ValidNameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "Name must contain only Latin letters");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("Username cannot be null");
            }
            else if (username.Length < MinLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Username must be at least {0} characters long",
                    MinLength));
            }
            else if (username.Length > MaxUsernameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Username must be less than {0} characters long",
                    MaxUsernameLength));
            }
            else if (username.Any(ch => !ValidUsernameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "Username must contain only Latin letters, digits .,_");
            }
        }
    }
}
