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
    public class AdvertsController : BaseApiController
    {
        private const int TextMinLength = 10;
        private const int HeadlineMinLength = 6;
        private const int TagNameMinLength = 2;
        private static char[] SplitSymbols = new char[] { ' ', ',', '!', '.', '-', '?', '\'' };

        [HttpPost]
        public HttpResponseMessage Create([FromBody]AdvertModel advertModel, string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new RealEstateContext();

                    var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid user");
                    }

                    // The following could be implemented in the future

                    //if (advertModel.Text == null || advertModel.Text == string.Empty)
                    //{
                    //    throw new ArgumentException(
                    //        string.Format("Post text should be at least {0} characters long", TextMinLength));
                    //}

                    if (advertModel.Headline == null || advertModel.Headline.Length < HeadlineMinLength)
                    {
                        throw new ArgumentException(
                            string.Format("Advert headline should be at least {0} characters long", HeadlineMinLength));
                    }

                    var advertEntity = GenerateAdvertEntity(advertModel, context, user);

                    context.Posts.Add(advertEntity);
                    context.SaveChanges();

                    var resultPostModel = new PostResponseModel()
                    {
                        Id = advertEntity.Id,
                        Title = advertEntity.Title
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.Created, resultPostModel);

                    return response;
                });

            return responseMsg;
        }

        [HttpGet]
        public IQueryable<PostFullModel> GetAll(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new BlogContext();

                    var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid user");
                    }

                    var postEntities = context.Posts;

                    var models = GetPostModels(postEntities);

                    return models.OrderByDescending(p => p.PostDate);
                });

            return responseMsg;
        }

        public IQueryable<PostFullModel> Get(int page, int count,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {

                    var models = this.GetAll(sessionKey);

                    var result = models.Skip(page * count).Take(count);

                    return result;
                });

            return responseMsg;
        }

        public IQueryable<PostFullModel> GetByKeyword(string keyword,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var models = this.GetAll(sessionKey);

                    var result = models.Where(post => post.Title.ToLower().Contains(keyword.ToLower()));

                    return result.OrderByDescending(post => post.PostDate);
                });

            return responseMsg;
        }

        public IEnumerable<PostFullModel> GetByTags(string tags,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var allTags = tags.Split(',');

                    var models = this.GetAll(sessionKey);

                    var result = models.ToList().Where(post => post.Tags.Intersect(allTags).Count() == allTags.Length);

                    return result.OrderByDescending(post => post.PostDate);
                });

            return responseMsg;
        }

        [ActionName("comment")]
        [HttpPut]
        public HttpResponseMessage AddComment(int postId, [FromBody]CommentModel commentModel,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new BlogContext();

                    var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid user");
                    }

                    var postEntity = context.Posts.FirstOrDefault(post => post.Id == postId);

                    if (postEntity == null)
                    {
                        throw new InvalidOperationException("Post does not exist");
                    }
                    
                    if(commentModel.Text.Trim().Length < CommentMinLength)
                    {
                        throw new ArgumentException(
                            string.Format("Comment text should be at least {0} characters long", CommentMinLength));
                    }

                    var comment = new Comment()
                    {
                        CommentedBy = user.DisplayName,
                        PostDate = DateTime.Now,
                        Text = commentModel.Text,
                        User = user
                    };

                    postEntity.Comments.Add(comment);
                    context.SaveChanges();

                    var response = this.Request.CreateResponse(HttpStatusCode.OK);

                    return response;
                });

            return responseMsg;
        }

        private static IQueryable<PostFullModel> GetPostModels(System.Data.Entity.DbSet<Post> postEntities)
        {
            var models =
                from post in postEntities
                select new PostFullModel
                {
                    Id = post.Id,
                    PostDate = post.PostDate,
                    PostedBy = post.PostedBy,
                    Text = post.Text,
                    Title = post.Title,
                    Comments =
                        (from c in post.Comments
                         select new CommentModel
                         {
                             CommentedBy = c.CommentedBy,
                             PostDate = c.PostDate,
                             Text = c.Text
                         }),
                    Tags =
                        (from t in post.Tags
                         select t.Name)
                };
            return models;
        }

        private static Advert GenerateAdvertEntity(PostModel advertModel, BlogContext context, BlogSystem.Models.User user)
        {
            var postEntity = new Post()
            {
                Title = advertModel.Title,
                Text = advertModel.Text,
                User = user,
                PostedBy = user.DisplayName,
                PostDate = DateTime.Now
            };

            foreach (var tagName in advertModel.Tags)
            {
                if (tagName.Length < TagNameMinLength || tagName == null)
                {
                    throw new ArgumentException(
                        string.Format("Tag name should be at least {0} characters long", TagNameMinLength));
                }
                var tagNameToLower = tagName.ToLower();

                var existingTag = postEntity.Tags.FirstOrDefault(tag => tag.Name == tagNameToLower);

                if (existingTag == null)
                {
                    existingTag = context.Tags.FirstOrDefault(tag => tag.Name == tagNameToLower);

                    if (existingTag == null)
                    {
                        existingTag = new Tag()
                        {
                            Name = tagNameToLower
                        };
                    }
                }

                postEntity.Tags.Add(existingTag);
            }

            foreach (var word in advertModel.Title.Split(SplitSymbols, StringSplitOptions.RemoveEmptyEntries))
            {
                if (word.Length < TagNameMinLength)
                {
                    continue;
                }

                var tagNameToLower = word.ToLower();

                var existingTag = postEntity.Tags.FirstOrDefault(tag => tag.Name == tagNameToLower);

                if (existingTag == null)
                {
                    existingTag = context.Tags.FirstOrDefault(tag => tag.Name == tagNameToLower);

                    if (existingTag == null)
                    {
                        existingTag = new Tag()
                        {
                            Name = tagNameToLower
                        };
                    }
                }

                postEntity.Tags.Add(existingTag);
            }
            return postEntity;
        }

    }
    }
}
