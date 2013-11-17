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
using PhotoServer.Domain;
using PhotoServer2.Models;

namespace PhotoServer2.Controllers
{
    public class PhotoController : ApiController
    {
        private PhotoDbContext db = new PhotoDbContext();

        // GET api/Photo
        public IQueryable<Photo> GetPhotos()
        {
            return db.Photos;
        }

        // GET api/Photo/5
        [ResponseType(typeof(Photo))]
        public IHttpActionResult GetPhoto(Guid id)
        {
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return NotFound();
            }

            return Ok(photo);
        }

        // PUT api/Photo/5
        public IHttpActionResult PutPhoto(Guid id, Photo photo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != photo.Id)
            {
                return BadRequest();
            }

            db.Entry(photo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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

            db.Photos.Add(photo);

            try
            {
                db.SaveChanges();
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
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return NotFound();
            }

            db.Photos.Remove(photo);
            db.SaveChanges();

            return Ok(photo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PhotoExists(Guid id)
        {
            return db.Photos.Count(e => e.Id == id) > 0;
        }
    }
}