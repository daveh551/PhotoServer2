using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Highway.Data;
using PhotoServer.Domain;
using PhotoServer.DataAccessLayer.Queries;
using PhotoServer.Storage;

namespace PhotoServer2.Controllers
{
    public class PhotoController : ApiController
    {
        private IRepository _repo = null;
        private IStorageProvider _store = null;

        public PhotoController(IRepository repo, IStorageProvider provider)
        {
            _repo = repo;
            _store = provider;
        }

        // GET api/Photo
        public IEnumerable<Photo> GetPhotos()
        {
            return _repo.Find(new PhotoServer.DataAccessLayer.Queries.FindAllPhotos());
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
        public IHttpActionResult PutPhoto(Guid id, Photo photo)
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
        [ResponseType(typeof(Photo))]
        public IHttpActionResult PostPhoto(Photo photo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return CreatedAtRoute("DefaultApi", new { id = photo.Id }, photo);
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