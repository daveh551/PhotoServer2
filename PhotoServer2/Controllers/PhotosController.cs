using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Highway.Data;
using PhotoServer.Domain;
using PhotoServer.DataAccessLayer.Queries;
using PhotoServer.Storage;
using PhotoServer2.Models;

namespace PhotoServer2.Controllers
{
    public class PhotosController : ApiController
    {
        private IRepository _repo = null;
        private IStorageProvider _store = null;

        public PhotosController(IRepository repo, IStorageProvider provider)
        {
            _repo = repo;
            _store = provider;
        }

        // GET api/Photo
        public IHttpActionResult GetPhotos()
        {
            var result =  _repo.Find(new PhotoServer.DataAccessLayer.Queries.FindAllPhotos());
            return Ok(result);
        }

        // GET api/Photo/5
        [ResponseType(typeof(Photo))]
        public IHttpActionResult GetPhoto(Guid id)
        {
            Photo photo = _repo.Find(new FindPhotoById(id));
            if (photo == null)
            {
                return NotFound();
            }

            return Ok(photo);
        }

        // PUT api/Photo/5
        public IHttpActionResult PutPhoto(Guid id, PhotoData photo)
        {
            // Merge photo[ID] with photo argument and validate changes
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != photo.Id)
            {
                return BadRequest();
            }

            _repo.Context.Update(photo);

            try
            {
                _repo.Context.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Photo
        [ResponseType(typeof(PhotoData))]
        public IHttpActionResult PostPhoto([FromBody] byte[] photoImage)
        {
           
            //Write Photo to storage
            string storagePath = StoreImage(photoImage);
            if (!string.IsNullOrEmpty(storagePath))
            {
                var photoId = new Guid(storagePath.Substring(storagePath.LastIndexOf('/') + 1));

                var photo = new Photo
                {
                    BasedOn = photoId,
                    Path = storagePath,
                    Id = photoId,
                    FileSize = photoImage.LongLength,
                };
                
                // Extract ExifData from photo

                _repo.Context.Add(photo);

                try
                {
                    _repo.Context.Commit();
                }
                catch (DbUpdateException)
                {
                    if (PhotoExists(photo.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                var helper = new UrlHelper(Request);
                var routeValues = new Dictionary<string, object>();
                routeValues.Add ("id", photo.Id);

                //var route = helper.Route("DefaultApi", routeValues);
                var config = Request.GetConfiguration();
                var routeData = config.Routes.GetRouteData(Request);
                var route = routeData.Route;
                var vp = route.GetVirtualPath(Request, routeValues);
                var vpr = Request.GetRequestContext().VirtualPathRoot;
                var vpd = config.Routes.GetVirtualPath(Request, "DefaultApi", routeValues);
                return CreatedAtRoute("DefaultApi", new Dictionary<string, object>(), photo) ;
            }
            else
            {
                return BadRequest("Failed to store photo");
            }
        }

        private string StoreImage(byte[] photoImage)
        {
            var path = "originals/" + Guid.NewGuid().ToString();
            return path;
        }

        // DELETE api/Photo/5
        [ResponseType(typeof(Photo))]
        public IHttpActionResult DeletePhoto(Guid id)
        {
            Photo photo = _repo.Find(new FindPhotoById(id));
            if (photo == null)
            {
                return NotFound();
            }

            _repo.Context.Remove(photo);
            _repo.Context.Commit();

            return Ok(photo);
        }

        protected override void Dispose(bool disposing)
        {
            // don't really need to do this - the IoC will manage the repo
            //if (disposing)
            //{
            //    db.Dispose();
            //}
            base.Dispose(disposing);
        }

        private bool PhotoExists(Guid id)
        {
            return _repo.Find(new FindPhotoById(id)) != null;
        }
    }
}