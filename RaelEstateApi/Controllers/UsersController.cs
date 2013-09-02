using RaelEstateApi.Models;
using RealEstateData;
using RealEstateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace RaelEstateApi.Controllers
{
    public class UsersController : BaseApiController
    {
        private const int MinUsernameLength = 6;
        private const int MaxUsernameLength = 30;
        private const string ValidUsernameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";
        private const string ValidNicknameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_. -";

        private const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
        private static readonly Random rand = new Random();

        private const int SessionKeyLength = 50;

        private const int Sha1Length = 40;

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage PostRegisterUser(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var context = new RealEstateContext();
                    using (context)
                    {
                        this.ValidateUsername(model.Username);
                        this.ValidateNickname(model.FullName);
                        this.ValidateAuthCode(model.AuthCode);
                        var usernameToLower = model.Username.ToLower();
                        var nicknameToLower = model.FullName.ToLower();
                        var user = context.Users.FirstOrDefault(
                            usr => usr.Username == usernameToLower
                            || usr.FullName.ToLower() == nicknameToLower);

                        if (user != null)
                        {
                            throw new InvalidOperationException("Users exists");
                        }

                        user = new User()
                        {
                            Username = usernameToLower,
                            FullName = model.FullName,
                            AuthCode = model.AuthCode,
                            Role= new Role{UserRole= model.Role}
                        };

                        context.Users.Add(user);
                        context.SaveChanges();

                        user.SessionKey = this.GenerateSessionKey(user.Id);
                        context.SaveChanges();

                        var loggedModel = new LoggedUserModel()
                        {
                            FullName = user.FullName,
                            SessionKey = user.SessionKey
                        };

                        var response =
                            this.Request.CreateResponse(HttpStatusCode.Created,
                                            loggedModel);
                        return response;
                    }
                });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage PostLoginUser(UserModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var context = new RealEstateContext();
                  using (context)
                  {
                      this.ValidateUsername(model.Username);
                      this.ValidateAuthCode(model.AuthCode);
                      var usernameToLower = model.Username.ToLower();
                      var user = context.Users.FirstOrDefault(
                          usr => usr.Username == usernameToLower
                          && usr.AuthCode == model.AuthCode);

                      if (user == null)
                      {
                          throw new InvalidOperationException("Invalid username or password");
                      }

                      if (user.SessionKey == null)
                      {
                          user.SessionKey = this.GenerateSessionKey(user.Id);
                          context.SaveChanges();
                      }

                      var loggedModel = new LoggedUserModel()
                      {
                          FullName = user.FullName,
                          SessionKey = user.SessionKey
                      };

                      var response =
                          this.Request.CreateResponse(HttpStatusCode.Created,
                                          loggedModel);
                      return response;
                  }
              });

            return responseMsg;
        }


        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage LogOutUser(string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
              () =>
              {
                  var context = new RealEstateContext();
                  using (context)
                  {
                      var user = context.Users.FirstOrDefault(
                          usr => usr.SessionKey == sessionKey);

                      if (user == null)
                      {
                          throw new InvalidOperationException("Invalid session key");
                      }

                      if (user.SessionKey != null)
                      {
                          user.SessionKey = null;
                          context.SaveChanges();
                      }
                      else
                      {
                          throw new InvalidOperationException("The user is already logged out");
                      }


                      var response =
                          this.Request.CreateResponse(HttpStatusCode.OK);
                      return response;
                  }
              });

            return responseMsg;
        }

        [HttpGet]
        public HttpResponseMessage Get(string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new RealEstateContext();

                var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                var userModel = new LoggedUserModel() {
                    FullName = user.FullName,
                    Role = user.Role.UserRole,
                    SessionKey = user.SessionKey
                };

                var response = this.Request.CreateResponse(HttpStatusCode.OK, userModel);

                return response;
            });

            return responseMsg;
        }

        private void ValidateAuthCode(string authCode)
        {
            if (authCode == null || authCode.Length != Sha1Length)
            {
                throw new ArgumentOutOfRangeException("Password should be encrypted");
            }
        }

        private void ValidateNickname(string nickname)
        {
            if (nickname == null)
            {
                throw new ArgumentNullException("nickname cannot be null");
            }
            else if (nickname.Length < MinUsernameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("nickname must be at least {0} characters long",
                    MinUsernameLength));
            }
            else if (nickname.Length > MaxUsernameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("nickname must be less than {0} characters long",
                    MaxUsernameLength));
            }
            else if (nickname.Any(ch => !ValidNicknameCharacters.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException(
                    "nickname must contain only Latin letters, digits .,_");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("Username cannot be null");
            }
            else if (username.Length < MinUsernameLength)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Username must be at least {0} characters long",
                    MinUsernameLength));
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





        private string GenerateSessionKey(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }
            return skeyBuilder.ToString();
        }
    }
}
