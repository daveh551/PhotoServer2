using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using AutoMapper;
using ExifLib;
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
                    CreatedDate = DateTime.UtcNow,
                    LastAccessed = DateTime.UtcNow,
                    FileSize = photoImage.LongLength,
                    CreatedBy = Request.GetRequestContext().Principal.Identity.Name,
                    Server = Request.RequestUri.Host,
                };
                
                // Extract ExifData from photo
                GetExifData(photoImage, photo);

                try
                {
                    _repo.Context.Add(photo);

                    _repo.Context.Commit();
                }
                catch (DbUpdateException ex)
                {
                    if (PhotoExists(photo.Id))
                    {
                        return Conflict();
                    }
                    else
                    {
                        string msg = ex.ToString();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    string generalMsg = ex.ToString();
                    throw;
                }

                var photoData = Mapper.Map<PhotoData>(photo);

              return CreatedAtRoute("DefaultApi", new {id = photo.Id}, photoData) ;
            }
            else
            {
                return BadRequest("Failed to store photo");
            }
        }

        

        private string StoreImage(byte[] photoImage)
        {

            if (photoImage == null || photoImage.Length == 0) return string.Empty;
            var path = "originals/" + Guid.NewGuid().ToString();
            _store.WriteFile(path, photoImage);
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

        private static void GetExifData(byte[] imageArray, Photo data)
        {

            MemoryStream image = new MemoryStream(imageArray);
            image.Seek(0, SeekOrigin.Begin);
            using (var reader = new ExifLib.ExifReader(image))
            {
                String timeString;
                if (reader.GetTagValue(ExifTags.DateTimeOriginal, out timeString))
                {
                    DateTime timeStamp;
                    if (DateTime.TryParseExact(timeString, "yyyy:MM:dd hh:mm:ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                        out timeStamp))
                        data.TimeStamp = timeStamp;
                }
                UInt16 hres, vres;
                object tagVal;
                if (reader.GetTagValue(ExifTags.PixelXDimension, out hres))
                    data.Hres = (int) hres;
                if (reader.GetTagValue(ExifTags.PixelYDimension, out vres))
                    data.Vres = (int) vres;
                if (reader.GetTagValue(ExifTags.FNumber, out tagVal))
                    data.FStop = string.Format("f/{0:g2}", tagVal.ToString());
                if (reader.GetTagValue(ExifTags.ExposureTime, out tagVal))
                    data.ShutterSpeed = string.Format("1/{0:g0}", 1/(double) tagVal);
                if (reader.GetTagValue(ExifTags.ISOSpeedRatings, out tagVal))
                    data.ISOSpeed = (short) (ushort) tagVal;
                if (reader.GetTagValue(ExifTags.FocalLength, out tagVal))
                    data.FocalLength = (short) (double) tagVal;

                if (reader.GetTagValue(ExifTags.SubsecTimeOriginal, out timeString))
                {
                    int msec;
                    if (int.TryParse(timeString, out msec))
                    {
                        data.TimeStamp += new TimeSpan(0, 0, 0, 0, msec);
                    }
                }
            }
        }

    }
}