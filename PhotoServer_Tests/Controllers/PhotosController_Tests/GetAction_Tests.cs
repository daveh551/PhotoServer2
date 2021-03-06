using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;
using PhotoServer.Domain;
using PhotoServer.Storage;
using PhotoServer2.Controllers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using RacePhotosTestSupport;

namespace PhotoServer_Tests.Controllers.PhotosController_Tests
{
	[TestFixture]
	public class GetAction_Tests
	{
		#region SetUp / TearDown

		private PhotosController target;
		protected IStorageProvider provider;

		[TestFixtureSetUp]
		public virtual void InitFixture()
		{
		    try
		    {
                //PhotoServer.App_Start.InitializeMapper.MapClasses();
                provider = new AzureStorageProvider(@"UseDevelopmentStorage=true", "images");
                ObjectMother.ClearDirectory(provider);
                ObjectMother.CopyTestFiles(provider);

		    }
		    catch (Exception ex)
		    {
		        var msg = ex.Message;
		        throw;
		    }
		}
		[SetUp]
		public void Init()
		{
		    try
		    {
                var db = new FakeRepository();
                var testRecords = ObjectMother.ReturnPhotoDataRecord(3);
                testRecords.ForEach( r => db.Context.Add(r));
                db.Context.Commit();
                target = new PhotosController(db, provider);
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Photos");
                target.ControllerContext = new FakeControllerContext(target, request);
		        target.Request = request;

		    }
		    catch (Exception ex)
		    {
		        var msg = ex.Message;
		        throw;
		    }

		}

		[TearDown]
		public void Dispose()
		{ }

		#endregion

		
		[Test]
		public void Get_CalledWithNoArgument_ReturnsListOf3PhotoDatas()
		{
			//Arrange
			var expectedCount = 3;
			//Act
			var result = target.GetPhotos().ExecuteAsync(new CancellationToken()).Result;
			//Assert
            var bodyString = result.Content.ReadAsStringAsync().Result;
            var resultData = Json.Decode<IEnumerable<Photo>>(bodyString);

            Assert.IsInstanceOf(typeof(IEnumerable<Photo>), resultData, "returned wrong type");
		    var count =  resultData.ToList().Count;
			Assert.AreEqual(expectedCount, count, "Return record count");
		}

		
		[Test]
		public void Get_CalledWithNonExistentGuid_ReturnsNotFound()
		{
			//Arrange
			var expectedStatus = HttpStatusCode.NotFound;
			Guid recordID = new Guid();
            target.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Photos/" + recordID.ToString());
			//Act
			HttpResponseMessage result = target.GetPhoto(recordID).ExecuteAsync(new CancellationToken()).Result;
			//Assert
			Assert.AreEqual(expectedStatus, result.StatusCode, "Status Code");
		}

		
		[Test]
		public void Get_CalledWithValidGuid_ReturnsOKWithExpectedFile()
		{
			//Arrange
			var expectedStatus = HttpStatusCode.OK;
			Guid recordId = ObjectMother.ReturnPhotoDataRecord(1)[0].Id;
            target.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Photos/" + recordId.ToString());
			//Act
			var result = target.GetPhoto(recordId).ExecuteAsync(new CancellationToken()).Result;
			//Assert
			Assert.AreEqual(expectedStatus, result.StatusCode, "Status Code");
			Assert.IsNotNull(result.Content, "Content is null");
		}
	}
}
