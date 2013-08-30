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
        public HttpResponseMessage Add([FromBody]AdvertFullModel advertModel, string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new RealEstateContext();

                    var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid user");
                    }

                    if (advertModel.Headline == null || advertModel.Headline.Length < HeadlineMinLength)
                    {
                        throw new ArgumentException(
                            string.Format("Advert headline should be at least {0} characters long", HeadlineMinLength));
                    }

                    var advertEntity = GenerateAdvertEntity(advertModel, context, user);

                    context.Adverts.Add(advertEntity);
                    context.SaveChanges();

                    var resultAdvertModel = new AdvertResponseModel()
                    {
                        Id = advertEntity.Id,
                        Headline = advertEntity.Headline
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.Created, resultAdvertModel);

                    return response;
                });

            return responseMsg;
        }

        [HttpGet]
        public IQueryable<AdvertModel> GetAll()
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new RealEstateContext();

                    var advertEntities = context.Adverts;

                    var models = GetAdvertModels(advertEntities);

                    return models.OrderByDescending(p => p.PostDate);
                });

            return responseMsg;
        }

        private IQueryable<AdvertModel> GetAdvertModels(System.Data.Entity.DbSet<Advert> advertEntities)
        {
            var models =
                from advert in advertEntities
                select new AdvertModel
                {
                    Headline = advert.Headline,
                    PostDate = advert.PostDate,
                    Town = advert.Town.Name,
                    Tags = 
                        from tag in advert.Tags
                        select tag.Name
                };

            return models;
        }

        //public IQueryable<PostFullModel> Get(int page, int count,
        //    [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        //{
        //    var responseMsg = this.PerformOperationAndHandleExceptions(() =>
        //        {

        //            var models = this.GetAll(sessionKey);

        //            var result = models.Skip(page * count).Take(count);

        //            return result;
        //        });

        //    return responseMsg;
        //}


        public IQueryable<AdvertModel> GetByTown(string town, string sessionKey)
        {

            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new RealEstateContext();

                    var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid user");
                    }

                    var models = this.GetAll();                    

                    var result = models.Where(advert => advert.Town.ToLower().Contains(town.ToLower()));

                    return result.OrderByDescending(post => post.PostDate);
                });

            return responseMsg;
        }


        public IEnumerable<AdvertModel> GetByTags(string tags, string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new RealEstateContext();
                    var user = context.Users.FirstOrDefault(usr => usr.SessionKey == sessionKey);

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid user");
                    }

                    var allTags = tags.Split(',');

                    var models = this.GetAll();

                    var result = models.ToList().Where(advert => advert.Tags.Intersect(allTags).Count() == allTags.Length);

                    return result.OrderByDescending(post => post.PostDate);
                });

            return responseMsg;
        }

        public HttpResponseMessage GetById(int id, string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
                {
                    var context = new RealEstateContext();

                    var existingAdvert = context.Adverts.FirstOrDefault(adv => adv.Id == id);

                    var resultAdvertModel = GetAdvertFullModel(existingAdvert);
                    
                    var response = this.Request.CreateResponse(HttpStatusCode.OK, resultAdvertModel);

                    return response;
                });

            return responseMsg;
        }

        private object GetAdvertFullModel(Advert existingAdvert)
        {
            var model = new AdvertFullModel
                {
                    Id = existingAdvert.Id,
                    PostDate = existingAdvert.PostDate,
                    Headline = existingAdvert.Headline,
                    Pictures =
                        (from pic in existingAdvert.Pictures
                         select pic.Url),
                    Price = existingAdvert.Price,
                    Town = existingAdvert.Town.Name,
                    Text = existingAdvert.Text,
                    Address = existingAdvert.Address,
                    Tags =
                        (from t in existingAdvert.Tags
                         select t.Name)
                };

            return model;
        }

        //[ActionName("comment")]
        //[HttpPut]


        //private static IQueryable<AdvertFullModel> GetById(System.Data.Entity.DbSet<Advert> advertEntities)
        //{
        //    var models =
        //        from post in advertEntities
        //        select new PostFullModel
        //        {
        //            Id = post.Id,
        //            PostDate = post.PostDate,
        //            PostedBy = post.PostedBy,
        //            Text = post.Text,
        //            Title = post.Title,
        //            Comments =
        //                (from c in post.Comments
        //                 select new CommentModel
        //                 {
        //                     CommentedBy = c.CommentedBy,
        //                     PostDate = c.PostDate,
        //                     Text = c.Text
        //                 }),
        //            Tags =
        //                (from t in post.Tags
        //                 select t.Name)
        //        };
        //    return models;
        //}

        private static Advert GenerateAdvertEntity(AdvertFullModel advertModel, RealEstateContext context, User user)
        {
            var advertEntity = new Advert()
            {
                Headline = advertModel.Headline,
                Text = advertModel.Text,
                User = user,
                PostDate = DateTime.Now,
                Address = advertModel.Address,
                Tags = AddOrUpdateTags(advertModel.Tags, context, advertModel.Headline),
                Town = AddOrUpdateTown(context, advertModel.Town),
                Pictures =
                    (from pic in advertModel.Pictures
                     select new Picture
                     {
                         Url = pic
                     }).ToList(),
                Price = advertModel.Price,
            };

            return advertEntity;
        }

        private static Town AddOrUpdateTown(RealEstateContext context, string town)
        {
            var existing = context.Towns.FirstOrDefault(t => t.Name == town);

            if (existing == null)
            {
                existing = new Town()
                {
                    Name = town
                };

                context.Towns.Add(existing);
                
                context.SaveChanges();
            }

            return existing;
        }

        private static ICollection<Tag> AddOrUpdateTags(IEnumerable<string> tagNames, RealEstateContext context, string title)
        {

            string[] tagsFromTheTitle = title.Split(SplitSymbols, StringSplitOptions.RemoveEmptyEntries);

            HashSet<string> allTags = new HashSet<string>();

            for (int i = 0; i < tagNames.Count(); i++)
            {
                allTags.Add(tagNames.ElementAt(i));
            }

            for (int i = 0; i < tagsFromTheTitle.Length; i++)
            {
                allTags.Add(tagsFromTheTitle[i]);
            }

            ICollection<Tag> tags = new HashSet<Tag>();
            foreach (var name in allTags)
            {
                var tag = context.Tags.FirstOrDefault(t => t.Name == name);
                if (tag == null)
                {
                    var newTag = new Tag
                    {
                        Name = name
                    };
                    tags.Add(newTag);
                    context.Tags.Add(newTag);
                }
                else
                {
                    tags.Add(tag);
                }
            }
            context.SaveChanges();

            return tags;

        }
    }

}
