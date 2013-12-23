using System.Threading;
using PhotoServer.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using PhotoServer.Storage;

namespace RacePhotosTestSupport
{
	public static class ObjectMother
	{
		public static string photoPath;
		public const string testDirectory = "Test.5K";

		public static void ClearDirectory(IStorageProvider provider)
		{
			// In this case, since the way we set up the test is different depending on 
			// whether we're using file storage or Azure storage, I'll switch based on what 
			// the provider is.

			if (provider is FileStorageProvider)
			{
				ClearFileStorageDirectory();
				return;
			}
			photoPath = string.Empty;
			var files = provider.GetFiles(Path.Combine(photoPath, testDirectory));
			foreach (var file in files)
			{

				provider.DeleteFile(Path.Combine(testDirectory, file));
			}

		}

		public static void ClearFileStorageDirectory()
		{
			SetPhotoPath();
			var directoryPath = Path.Combine(photoPath, testDirectory);
			var directoryInfo = new DirectoryInfo(directoryPath);
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete(true);
			}

		}

		public static void SetPhotoPath()
		{
			if (string.IsNullOrWhiteSpace(photoPath))
			{
				photoPath = ConfigurationManager.AppSettings["PhotosPhysicalDirectory"];
			}
		}

		private static Race TestRace = new Race()
			{
				Distance = new Distance {Id = 3, RaceDistance = "5K"},
				DistanceId = 3,
				Event = new Event() {EventName = "Test", Id = 1},
				EventId = 1,
				Id = 1
			};

		private static PhotoServer.Domain.Photo[] testData =

			new PhotoServer.Domain.Photo[]
				{
					new Photo
						{
							Id = new Guid("E0CAF539-5C32-432B-AAC4-B01CD4EABB3A"),
							RaceId = 1,
							Race = TestRace,
							Station = "FinishLine",
							Card = "1",
							Sequence = 1,
							Path = @"Test.5K/FinishLine/1/001.JPG",
							Hres = 3008,
							Vres = 2000,
							TimeStamp = new DateTime(2011, 10, 22, 8, 48, 59, 60)
						},
					new Photo
						{
							Id = new Guid("24249DF5-C9FF-4B17-A259-514586F07C6B"),
							RaceId = 1,
							Race = TestRace,
							Station = "FinishLine",
							Card = "1",
							Sequence = 3,
							Path = @"Test.5K/FinishLine/1/003.JPG",
							Hres = 3008,
							Vres = 2000,
							TimeStamp = new DateTime(2011, 10, 22, 8, 29, 0)
						},
					new Photo
						{
							Id = new Guid("E5034D6B-3B26-4F03-8F9E-0EA1F5BE55C7"),
							RaceId = 1,
							Race = TestRace,
							Station = "FinishLine",
							Card = "1",
							Sequence = 4,
							Path = @"Test.5K/FinishLine/1/004.JPG",
							Hres = 3008,
							Vres = 2000,
							TimeStamp = new DateTime(2011, 10, 22, 8, 29, 0)
						}
				};


		public static List<PhotoServer.Domain.Photo> ReturnPhotoDataRecord(int count)
		{
			var returnList = new List<Photo>();
			if (count <= 3)
				for (int ix = 0; ix < count; ix++)
				{
					returnList.Add(testData[ix]);
				}

			return returnList;

		}

		public static void CopyTestFiles(IStorageProvider provider)
		{
			SetPhotoPath();
			var sourcePhotos = new DirectoryInfo(@"..\..\..\PhotoServer_Tests\TestFiles").EnumerateFiles().ToList();
			foreach (var photoData in testData)
			{
			    var count = 0;
			    var success = false;
			    while (!success && count < 3)
			    {
			        var destFile = photoData.Path;
			        var fileName = Path.GetFileName(destFile);
			        var sourceFile = sourcePhotos.Where(p => p.Name.EndsWith(fileName)).Select(f => f.FullName).Single();
			        try
			        {
                        var fileStream = new FileStream(sourceFile, FileMode.Open);
                        var fileData = new BinaryReader(fileStream).ReadBytes((int) fileStream.Length);
                        provider.WriteFile(destFile, fileData);
			            success = true;

			        }
			        catch (IOException ex)
			        {
			            count++;
                        Thread.Sleep(500);
			        }
			    }
			}
		}
	}
}