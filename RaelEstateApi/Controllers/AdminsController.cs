using RaelEstateApi.Models;
using RealEstateData;
using RealEstateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RaelEstateApi.Controllers
{
    public class AdminsController : BaseApiController
    {
        public IQueryable<LoggedUserModel> GetAll(string sessionKey)
        {

            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new RealEstateContext();

                var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                if (user == null)
                {
                    throw new InvalidOperationException("Invalid user");
                }

                if (user.Role.UserRole == "admin")
                {
                    var models = this.GetAllUsers(context, user);

                    return models.OrderByDescending(u => u.FullName);
                }
                else
                {
                    throw new ArgumentException("The user is not admin. Don't have permissions");
                }

            });

            return responseMsg;
        }

        public UserModel GetById(int id, string sessionKey)
        {

            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new RealEstateContext();

                var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                if (user == null)
                {
                    throw new InvalidOperationException("Invalid user");
                }

                if (user.Role.UserRole == "admin")
                {
                    UserModel resultUser = this.GetUserById(context, id);

                    return resultUser;
                }
                else
                {
                    throw new ArgumentException("The user is not admin. Don't have permissions");
                }

            });

            return responseMsg;
        }

        [HttpPut]
        public HttpResponseMessage UpdateUserById(int id,[FromBody] UserModel updatedUser, string sessionKey)
        {

            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new RealEstateContext();

                var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                if (user == null)
                {
                    throw new InvalidOperationException("Invalid user");
                }

                if (user.Role.UserRole == "admin")
                {
                    UpdateUser(context, updatedUser, id);

                    var response =
                       this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
                else
                {
                    throw new ArgumentException("The user is not admin. Don't have permissions");
                }

            });

            return responseMsg;
        }

        // if admin changes the username wont't work
        private void UpdateUser(RealEstateContext context, UserModel updatedUser,int id)
        {
            var userDb = context.Users.FirstOrDefault(u =>u.Id==id);
            if (userDb!=null)
            {
                userDb.FullName = updatedUser.FullName;
                userDb.Username = updatedUser.Username;
                userDb.Role = UpdateOrEdinRole(context, updatedUser.Role);
                userDb.AuthCode = updatedUser.AuthCode;
                context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The user did't exist. Probably u have changed his user name and fullname");
            }
        }

        private Role UpdateOrEdinRole(RealEstateContext context, string role)
        {
            Role roleDb = new Role();
            var doRoleExist = context.Roles.FirstOrDefault(r => r.UserRole == role);
            if (doRoleExist==null)
            {
                roleDb.UserRole= role;
                context.Roles.Add(roleDb);
                context.SaveChanges();
                return roleDb;
            }
            return doRoleExist;
            
        }

        private UserModel GetUserById(RealEstateContext context, int id)
        {
            var userDb = context.Users.First(u => u.Id == id);
            UserModel userToReturn = new UserModel()
            {
                Username=userDb.Username,
                FullName= userDb.FullName,
                Role=userDb.Role!=null?userDb.Role.UserRole:"No role",
                AuthCode = userDb.AuthCode
            };
            
            
            return userToReturn;

        }


        private IQueryable<LoggedUserModel> GetAllUsers(RealEstateContext context, User user)
        {
            var users = from u in context.Users
                        select new LoggedUserModel
                        {
                            FullName = u.FullName,
                            SessionKey = u.SessionKey
                        };
           // users = users.Where(u => u.FullName != user.FullName);
            return users;
        }
    }
}
