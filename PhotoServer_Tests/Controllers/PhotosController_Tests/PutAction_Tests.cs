using Highway.Data;
using NUnit.Framework;
using PhotoServer.Domain;
using PhotoServer.Storage;
using PhotoServer2.Controllers;
using RacePhotosTestSupport;

namespace PhotoServer_Tests.Controllers.PhotosController_Tests
{
	[TestFixture]
	public class PutAction_Tests
	{
		#region SetUp / TearDown

		private PhotoController target;
		private IRepository fakeDataSource;
	    protected IStorageProvider provider;
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			//PhotoServer.App_Start.InitializeMapper.MapClasses();
			provider = new AzureStorageProvider(@"UseDevelopmentStorage=true", "images");	
		}

		[SetUp]
		public void Init()
		{
			fakeDataSource = new Repository();
			var photoRecordList = ObjectMother.ReturnPhotoDataRecord(3);
			foreach (var photo in photoRecordList)
			{
				fakeDataSource.Context.Add(photo);
			}
			fakeDataSource.Context.Commit();
			target = new PhotoController(fakeDataSource, provider);
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
			string expected = "FillsInDataFields";
			var photoRecord = fakeDataSource.Photos.FindAll().FirstOrDefault();
			var photoData = AutoMapper.Mapper.Map<Photo, PhotoData>(photoRecord);
			photoData.Race = "Test.5K";
			photoData.Station = "FinishLine";
			photoData.Card = "1";
			photoData.Sequence = 1;
			//Act

			target.Put(photoRecord.Id, photoData);

			//Assert
			var resultPhotoRecord = fakeDataSource.Photos.FindById(photoRecord.Id);
			Assert.IsNotNull(resultPhotoRecord);
			Assert.AreEqual(1, resultPhotoRecord.RaceId,  "Race");
			Assert.AreEqual("FinishLine", resultPhotoRecord.Station, "Station");
			Assert.AreEqual("1", resultPhotoRecord.Card, "Card");
			Assert.AreEqual(1, resultPhotoRecord.Sequence, "Sequence");

		}

		#endregion
	}
}
