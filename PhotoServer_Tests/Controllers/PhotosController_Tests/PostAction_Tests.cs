using Highway.Data;
using NUnit.Framework;
using PhotoServer.Storage;
using PhotoServer2.Controllers;
using PhotoServer.DataAccessLayer;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Helpers;
using RacePhotosTestSupport;

namespace PhotoServer_Tests.Controllers.PhotosController_Tests
{
	[TestFixture]
	public class PostAction_Tests
	{
		#region SetUp / TearDown

		private PhotoController target;
		private string pathArgument = @"Test.5K\FinishLine\1\001.jpg";
		private int raceArgument = 1;
		private string eventName = "Test";
		private const string raceName = "Test.5K";
		private string stationArgument = "FinishLine";
		private string cardArgument = "1";
		private int seqArgument = 1;
		private int fileSize;

	    protected IStorageProvider provider;
		[TestFixtureSetUp]
		public virtual void InitFixture()
		{
			//PhotoServer.App_Start.InitializeMapper.MapClasses();
			provider = new AzureStorageProvider(@"UseDevelopmentStorage=true", "images");
			ObjectMother.SetPhotoPath();
		}
		private IRepository fakeDataSource;
		[SetUp]
		public void Init()
		{
			var fileName = @"..\..\TestFiles\Run For The Next Generation 2011 001.JPG";
			fakeDataSource =   new FakeRepository();
			target = new PhotoController(fakeDataSource, provider);
			target.ControllerContext = new FakeControllerContext();
			target.ControllerContext.Request = SetupContent(fileName);
			target.context = new  FakeHttpContext();
		}
		private HttpRequestMessage SetupContent(string fileName)
		{

			var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost:6471/api/Photos");
			using (var file = new BinaryReader(new FileStream(fileName, FileMode.Open)))
			{
				fileSize = (int) new FileInfo(fileName).Length;
				var image = file.ReadBytes( fileSize);
				req.Content = new ByteArrayContent(image);
				req.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
			}

			return req;
		}

		[TearDown]
		public void Dispose()
		{ }

		#endregion

		#region Tests
		
	
		[Test]
		public void Post_WithNewPhoto_ShouldAddPhotosDataItemToDB()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			//Assert
			Assert.AreEqual(1, fakeDataSource.Photos.FindAll().Count((item) =>true));
		}

		
		[Test]
		public void Post_WithNewPhoto_ReturnsLocationHeaderWithGuid()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			var hdrs = result.Headers;
			
			//Assert
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Did not return Created status code");
			Assert.IsNotNull(hdrs.Location, "Location Header is null");
			Assert.That(hdrs.Location.OriginalString.StartsWith("/api/Photos/"));
		}

		
		[Test]
		public  void  Post_WithNewData_ReturnsPhotoDataItemInMessageBody()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			var dataItem = fakeDataSource.Photos.FindAll().FirstOrDefault();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "request failed to return Created status code");
			var body = result.Content;
			var bodyString = body.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);
			//Assert
			Assert.IsNotNull(dataItem, "returned null dataItem");
			Assert.AreEqual(dataItem.Id, resultData.Id, "Item Id in HttpContent not equal to data Item Id");
			Assert.IsNullOrEmpty(resultData.Race);
			Assert.IsNullOrEmpty(resultData.Station);
			Assert.IsNullOrEmpty(resultData.Card);
			Assert.IsNullOrEmpty(resultData.PhotographerInitials);
			Assert.IsNull(resultData.Sequence);
		}

		
		[Test]
		public void Post_WithAttachedPhoto_ResultsInPhotoInDirectory()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);

			//Act
			var result = target.Post();
			//Assert
			var resultPath = Path.Combine("originals", GetResultGuid(result));
			Assert.That(provider.FileExists(resultPath));
		}

		
		[Test]
		public void Post_WithAttachedPhoto_ReturnsDataWithTimeAndResolution()
		{
			//Arrange
			
			DateTime expectedTimeStamp = new DateTime(2011, 10, 22, 8, 28, 59, 60);
			int expectedHres = 3008;
			int expectedVres = 2000;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			var bodyString = result.Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);

			//Assert
			Assert.AreEqual(expectedTimeStamp, resultData.TimeStamp, "TimeStamp");
			Assert.AreEqual(expectedHres, resultData.Hres, "Hres");
			Assert.AreEqual(expectedVres, resultData.Vres, "Vres");
		}

		
		[Test]
		public void Post_WithAttachedPhoto_SetsServerInPhotoData()
		{
			//Arrange
			string expected = "localhost";
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			var resultId = GetResultGuid(result);
			var serverResult = fakeDataSource.Photos.FindById(new Guid(resultId)).Server;
			//Assert
			Assert.AreEqual(expected, serverResult, "failure message");
		}

		private static string GetResultGuid(HttpResponseMessage result)
		{
			var resultLocation = result.Headers.Location.ToString();
			var resultId = resultLocation.Substring(resultLocation.LastIndexOf('/') + 1);
			return resultId;
		}


		[Test]
		public void Post_WithAttachedPhoto_RecordsFileLengthInDB()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			var resultId = GetResultGuid(result);
			var fileLen = fakeDataSource.Photos.FindById(new Guid(resultId)).FileSize;
			//Assert
			Assert.AreEqual(fileSize, fileLen, "Recorded fileLen differs ");
		}

		
		[Test]
		public void Post_WithAttachedPhoto_SetsLastAccessTimeToCurrentTime()
		{
			//Arrange
			DateTime expectedAccessTime = DateTime.Now;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Failed to return Created status");
			var resultId = GetResultGuid(result);
			var accessTime = fakeDataSource.Photos.FindById(new Guid(resultId)).LastAccessed;
			//Assert
			Assert.IsNotNull(accessTime, "accessTime in DB is null");
			Assert.That(accessTime,Is.GreaterThanOrEqualTo(expectedAccessTime));
			Assert.That(accessTime, Is.LessThanOrEqualTo(DateTime.Now));
		}

		
		[Test]
		public void Post_WithAttachedPhoto_SetsFStopInDataReturned()
		{
			//Arrange
			string expected = "f/6.3";
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Failed to return Created status");
			var bodyString = result.Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);
			//Assert
			Assert.AreEqual(expected, resultData.FStop, "FStop");
		}
		[Test]
		public void Post_WithAttachedPhoto_SetsShutterSpeedInDataReturned()
		{
			//Arrange
			string expected = "1/160";
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Failed to return Created status");
			var bodyString = result.Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);
			//Assert
			Assert.AreEqual(expected, resultData.ShutterSpeed, "ShutterSpeed");
		}
		[Test]
		public void Post_WithAttachedPhoto_SetsISOSpeedInDataReturned()
		{
			//Arrange
			short expected = 200;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Failed to return Created status");
			var bodyString = result.Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);
			//Assert
			Assert.AreEqual(expected, resultData.ISOSpeed, "ISOSpeed");
		}

		[Test]
		public void Post_WithAttachedPhoto_SetsFocalLengthInDataReturned()
		{
			//Arrange
			short expected = 26;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Failed to return Created status");
			var bodyString = result.Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);
			//Assert
			Assert.AreEqual(expected, resultData.FocalLength, "FocalLength");
		}

		
		[Test]
		public void Post_PostNewPhoto_SetsCreatedByToUser()
		{
			//Arrange
			string expected = "FinishLineAdmin";
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Failed to return Created status");
			var bodyString = result.Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);
			//Assert
			Assert.AreEqual(expected, resultData.CreatedBy, "CreatedBy not set correctly");
		}

		
		[Test]
		public void Post_PostNewPhoto_SetCreatedDateToNow()
		{
			//Arrange
			DateTime expectedTime = DateTime.Now;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.Post();
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "Failed to return Created status");
			var bodyString = result.Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer.Models.PhotoData>(bodyString);
			//Assert
			Assert.IsNotNull(resultData.CreatedDate);
			Assert.That(resultData.CreatedDate, Is.GreaterThanOrEqualTo(expectedTime));
			Assert.That(resultData.CreatedDate, Is.LessThanOrEqualTo(DateTime.Now));
		}
		#endregion
	}

	
}
