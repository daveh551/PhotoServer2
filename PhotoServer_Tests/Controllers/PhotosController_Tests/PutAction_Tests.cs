using System.Configuration;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using Highway.Data;
using NUnit.Framework;
using PhotoServer.Domain;
using PhotoServer.Storage;
using PhotoServer2.Controllers;
using RacePhotosTestSupport;
using PhotoServer.DataAccessLayer.Queries;
using PhotoServer2.Models;

namespace PhotoServer_Tests.Controllers.PhotosController_Tests
{
	[TestFixture]
	public class PutAction_Tests
	{
		#region SetUp / TearDown

		private PhotosController target;
		private IRepository fakeDataSource;
	    protected IStorageProvider provider;
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			PhotoServer2.App_Start.InitializeMapper.MapClasses();
			provider = new AzureStorageProvider(@"UseDevelopmentStorage=true", "images");	
		}

		[SetUp]
		public void Init()
		{
			fakeDataSource = new FakeRepository();
			var photoRecordList = ObjectMother.ReturnPhotoDataRecord(3);
			foreach (var photo in photoRecordList)
			{
				fakeDataSource.Context.Add(photo);
			}
			fakeDataSource.Context.Commit();
			target = new PhotosController(fakeDataSource, provider);
		}


		[TearDown]
		public void Dispose()
		{ }

		#endregion

		#region Tests



		[Test]
		public void Put_PassingRaceData_FillsInDataFields()
		{
			//Arrange
		    var photoRecord = fakeDataSource.Find(new FindFirstPhoto());
			var photoData = AutoMapper.Mapper.Map<Photo, PhotoData>(photoRecord);
			photoData.Race = "Test.5K";
			photoData.Station = "FinishLine";
			photoData.Card = "1";
			photoData.Sequence = 1;
			//Act

			target.PutPhoto(photoRecord.Id, photoData);

			//Assert
			var resultPhotoRecord = fakeDataSource.Find( new FindPhotoById(photoRecord.Id));
			Assert.IsNotNull(resultPhotoRecord);
			Assert.AreEqual(1, resultPhotoRecord.RaceId,  "Race");
			Assert.AreEqual("FinishLine", resultPhotoRecord.Station, "Station");
			Assert.AreEqual("1", resultPhotoRecord.Card, "Card");
			Assert.AreEqual(1, resultPhotoRecord.Sequence, "Sequence");

		}

        
        [Test]
        public void Put_PassingPhotoDataWithModifiedExifData_ExifDataIsNotModified()
        {
            //Arrange
            var photoRecord = fakeDataSource.Find(new FindFirstPhoto());
			var photoData = AutoMapper.Mapper.Map<Photo, PhotoData>(photoRecord);
            var originalPhotoData = new PhotoData(photoData);
			photoData.Race = "Test.5K";
			photoData.Station = "FinishLine";
			photoData.Card = "1";
			photoData.Sequence = 1;
            photoData.Hres = 100;
            var req = new HttpRequestMessage(HttpMethod.Put, "/api/Photos/" + photoRecord.Id.ToString());
            PostAction_Tests.SetupContext(target, req);
            //Act

            var result = target.PutPhoto (photoData.Id, photoData);
            //Assert
            Assert.IsInstanceOf(typeof(OkNegotiatedContentResult<PhotoData>), result, "result is wrong type");
            var content = PostAction_Tests.GetHttpResult(result).Content;
            Assert.IsInstanceOf(typeof(System.Net.Http.ObjectContent<PhotoData>), content, "Content is wrong type");
            var jsonString = content.ReadAsStringAsync().Result;
            var resultPhotoRecord = Json.Decode<PhotoData>(jsonString);
            Assert.IsInstanceOf(typeof(PhotoData),resultPhotoRecord, "Wrong return type");
            Assert.AreEqual(originalPhotoData.Vres, resultPhotoRecord.Vres, "Vres");
            Assert.AreEqual(originalPhotoData.Hres, resultPhotoRecord.Hres, "Hres");
        }
		#endregion
	}
}
