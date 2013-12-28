using System.Net.Mime;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Highway.Data;
using Microsoft.WindowsAzure.Storage.Core;
using NUnit.Framework;
using PhotoServer.DataAccessLayer.Queries;
using PhotoServer.Domain;
using PhotoServer.Storage;
using PhotoServer2.App_Start;
using PhotoServer2.Controllers;
using PhotoServer.DataAccessLayer;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Results;
using System.Web.Helpers;
using PhotoServer2.Models;
using RacePhotosTestSupport;

namespace PhotoServer_Tests.Controllers.PhotosController_Tests
{
	[TestFixture]
	public class PostAction_Tests
	{
		#region SetUp / TearDown

		private const string host = "http://localhost:6471";
		private PhotosController target;
		private byte[] image;  // the photo being sent to  post
		//private string pathArgument = @"Test.5K\FinishLine\1\001.jpg";
		//private int raceArgument = 1;
		//private string eventName = "Test";
		//private const string raceName = "Test.5K";
		//private string stationArgument = "FinishLine";
		//private string cardArgument = "1";
		//private int seqArgument = 1;
		//private const string AZUREDEVCONNECTION = @"DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;";
		//private const string AZUREDEVCONNECTION = @"UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1";
		//private const string AZUREDEVCONNECTION = @"UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://ipv4.fiddler";
		private const string AZUREDEVCONNECTION = @"UseDevelopmentStorage=true";
		//private const string AZUREDEVCONNECTION = @"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DevelopmentStorageProxyUri=http://ipv4.fiddler";
		private int fileSize;

		protected IStorageProvider provider;
		[TestFixtureSetUp]
		public virtual void InitFixture()
		{
			//PhotoServer.App_Start.InitializeMapper.MapClasses();
			try
			{
				provider = new AzureStorageProvider(AZUREDEVCONNECTION, "images");

			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				throw;
			}
			ObjectMother.SetPhotoPath();
		}
		private IRepository fakeDataSource;
		[SetUp]
		public void Init()
		{
			var fileName = @"..\..\TestFiles\Run For The Next Generation 2011 001.JPG";
			fakeDataSource =   new FakeRepository();
			InitializeMapper.MapClasses();

			target = new PhotosController(fakeDataSource, provider);
			target.ControllerContext = new FakeControllerContext();
			target.Configuration = target.ControllerContext.Configuration;
			target.ControllerContext.Request = SetupContent(fileName);
			target.Request = target.ControllerContext.Request;
			var requestContext = new HttpRequestContext();
			requestContext.Principal = new GenericPrincipal(new GenericIdentity("FinishLineAdmin"), new string[0]) ;

			var routeValue = new HttpRouteValueDictionary();
			routeValue.Add("controller", "Photos");
			var routeData = new HttpRouteData(target.Request.GetConfiguration().Routes["DefaultApi"], routeValue);
			target.Request.SetRequestContext(requestContext);
			target.Request.SetRouteData(routeData);
			target.Request.SetConfiguration(target.Configuration);
			target.RequestContext = target.Request.GetRequestContext();
			target.Configuration.EnsureInitialized();
			target.Url = new UrlHelper(target.Request);

			//target.context = new  FakeHttpContext();
		}
		private HttpRequestMessage SetupContent(string fileName)
		{

			var req = new HttpRequestMessage(HttpMethod.Post, host + "/api/Photos");
			using (var file = new BinaryReader(new FileStream(fileName, FileMode.Open)))
			{
				fileSize = (int) new FileInfo(fileName).Length;
				image = file.ReadBytes( fileSize);
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
		public void PostPhoto_NoPhotoSupplied_ReturnsBadRequest()
		{
			//Arrange
			var expectedStatus = HttpStatusCode.BadRequest;
			// Default setup creates a photo in the content, so we need to
			// remove it for this test.
			target.Request.Content = null;
			image = null;
			//Act
			var result = target.PostPhoto(image).ExecuteAsync(new CancellationToken()).Result;
			//Assert

			Assert.AreEqual(expectedStatus, result.StatusCode, "Expected status was not returned.");
		}
		
	
		[Test]
		public void Post_WithNewPhoto_ShouldAddPhotosDataItemToDB()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.PostPhoto(image);
			//Assert
			Assert.AreEqual(1, fakeDataSource.Find<Photo>(new FindAllPhotos()).Count((item) =>true));
		}

		
		[Test]
		public void Post_WithNewPhoto_ReturnsLocationHeaderWithGuid()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.PostPhoto(image);
			
			//Assert
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "request failed to return Created status code");
			var hdr = GetHttpResult(result).Headers;
			Assert.IsNotNull(hdr.Location, "Location Header is null");
			Assert.That(hdr.Location.OriginalString.StartsWith(host + "/api/Photos/"));
		}

		
		[Test]
		public  void  Post_WithNewData_ReturnsPhotoDataItemInMessageBody()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.PostPhoto(image).ExecuteAsync(new CancellationToken()).Result;
			var dataItem = fakeDataSource.Find(new FindFirstPhoto());
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, "request failed to return Created status code");
			var body = result.Content;
			var bodyString = body.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);
			//Assert
			Assert.IsNotNull(dataItem, "returned null dataItem");
			Assert.AreEqual(dataItem.Id, resultData.Id, "Item Id in HttpContent not equal to data Item Id");
			Assert.IsNullOrEmpty(resultData.Race, "Race is not null");
			Assert.IsNullOrEmpty(resultData.Station, "Station is not null");
			Assert.IsNullOrEmpty(resultData.Card, "Card is not null");
			Assert.IsNullOrEmpty(resultData.PhotographerInitials, "PhotographerInitials is not null");
			Assert.IsNull(resultData.Sequence, "Sequence is not null");
		}

		
		[Test]
		public void Post_WithAttachedPhoto_ResultsInPhotoInDirectory()
		{
			//Arrange
			ObjectMother.ClearDirectory(provider);

			//Act
			var result = target.PostPhoto(image);
			//Assert
			var resultPath = Path.Combine("originals", GetResultGuid(GetHttpResult(result)));
			Assert.That(provider.FileExists(resultPath), "resultPath does not exist");
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
			var result = target.PostPhoto(image);
			var bodyString = GetHttpResult(result).Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);

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
			var result = target.PostPhoto(image);
			var resultId = GetResultGuid(GetHttpResult(result));
			var serverResult = fakeDataSource.Find(new FindPhotoById(new Guid(resultId))).Server;
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
			var result = target.PostPhoto(image);
			var resultId = GetResultGuid(GetHttpResult(result));
			var fileLen = fakeDataSource.Find(new FindPhotoById(new Guid(resultId))).FileSize;
			//Assert
			Assert.AreEqual(fileSize, fileLen, "Recorded fileLen differs ");
		}

		
		[Test]
		public void Post_WithAttachedPhoto_SetsLastAccessTimeToCurrentTime()
		{
			//Arrange
			DateTime expectedAccessTime = DateTime.UtcNow;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.PostPhoto(image);
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "Failed to return Created status");
			var resultId = GetResultGuid(GetHttpResult(result));
			var accessTime = fakeDataSource.Find(new FindPhotoById(new Guid(resultId))).LastAccessed;
			//Assert
			Assert.IsNotNull(accessTime, "accessTime in DB is null");
			Assert.That(accessTime,Is.GreaterThanOrEqualTo(expectedAccessTime));
			Assert.That(accessTime, Is.LessThanOrEqualTo(DateTime.UtcNow));
		}

		
		[Test]
		public void Post_WithAttachedPhoto_SetsFStopInDataReturned()
		{
			//Arrange
			string expected = "f/6.3";
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.PostPhoto(image);
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "Failed to return Created status");
			var bodyString = GetHttpResult(result).Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);
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
			var result = target.PostPhoto(image);
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "Failed to return Created status");
			
			var bodyString = GetHttpResult(result).Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);
			//Assert
			Assert.AreEqual(expected, resultData.ShutterSpeed, "ShutterSpeed");
		}

		private HttpResponseMessage GetHttpResult(IHttpActionResult result)
		{
			return result.ExecuteAsync(new CancellationToken()).Result;
		}

		[Test]
		public void Post_WithAttachedPhoto_SetsISOSpeedInDataReturned()
		{
			//Arrange
			short expected = 200;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.PostPhoto(image);
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "Failed to return Created status");
			var bodyString =GetHttpResult(result).Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);
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
			var result = target.PostPhoto(image);
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "Failed to return Created status");
			var bodyString = GetHttpResult(result).Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);
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
			var result = target.PostPhoto(image);
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "Failed to return Created status");
			var bodyString = GetHttpResult(result).Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);
			//Assert
			Assert.AreEqual(expected, resultData.CreatedBy, "CreatedBy not set correctly");
		}

		
		[Test]
		public void Post_PostNewPhoto_SetCreatedDateToNow()
		{
			//Arrange
			// Be aware: the CreatedDate is returned as UTC, but Json.Decode converts to Local
			// That may be a problem we need to deal with later
			DateTime expectedTime = DateTime.Now;
			ObjectMother.ClearDirectory(provider);
			//Act
			var result = target.PostPhoto(image);
			Assert.IsInstanceOf(typeof(CreatedAtRouteNegotiatedContentResult<PhotoData>), result, "Failed to return Created status");
			var bodyString = GetHttpResult(result).Content.ReadAsStringAsync().Result;
			var resultData = Json.Decode<PhotoServer2.Models.PhotoData>(bodyString);
			//Assert
			Assert.IsNotNull(resultData.CreatedDate);
			Assert.That(resultData.CreatedDate, Is.GreaterThanOrEqualTo(expectedTime));
			Assert.That(resultData.CreatedDate, Is.LessThanOrEqualTo(DateTime.Now));
		}

       
        [Test]
        public void Post_StoresPhotoThatCanBeRetrieved()
        {
            //Arrange
            ObjectMother.ClearDirectory(provider);
            //Act
            var result = target.PostPhoto(image);
			var hdr = GetHttpResult(result).Headers;
            var location = hdr.Location.AbsolutePath;
            var guid = new Guid(location.Substring(location.LastIndexOf('/') +1));
            target.Request = new HttpRequestMessage(HttpMethod.Get, location);
            target.Request.Headers.Clear();
            target.Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/jpg", 1.0));
            result = target.GetPhoto(guid);
            //var returnedImage = GetHttpResult(result).Content.ReadAsByteArrayAsync().Result;
            var content = GetHttpResult(result).Content;
            
            var length = content.Headers.ContentLength;
            var returnedImage = content.ReadAsByteArrayAsync().Result;

            //Assert
            Assert.IsInstanceOf(typeof(byte[]), returnedImage);
            Assert.AreEqual(image.Length, returnedImage.Length, "Image has wrong length");
            Assert.AreEqual(image, returnedImage, "images not equal");
        }
		#endregion
	}

	
}
