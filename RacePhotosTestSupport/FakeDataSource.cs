using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoServer.DataAccessLayer;
using PhotoServer.Domain;

namespace RacePhotosTestSupport
{
    public class FakeDataSource : IPhotoDataSource
    {
        private FakeGuidRepository<Photo>  _photoData;
        public IRepository<Photo, Guid> Photos { get { return _photoData; } }
	    private FakeIntRepository<Race> _raceData;
		public IRepository<Race, int> Races { get { return _raceData; } }
	    private FakeIntRepository<Event> _eventData;
		public IRepository<Event, int> Events { get { return _eventData; } }
	    private FakeIntRepository<Distance> _distanceData;
		public IRepository<Distance, int> Distances { get { return _distanceData; } }
 		

        public FakeDataSource()
        {
            _photoData = new FakeGuidRepository<Photo>();
	        InitPhotoData();
			_distanceData = new FakeIntRepository<Distance>();
	        InitDistanceData();
			_eventData = new FakeIntRepository<Event>();
	        InitEventData();
			_raceData = new FakeIntRepository<Race>();
	        InitRaceData();
        }

	    private void InitRaceData()
	    {
		    var distance = _distanceData.Find(d => d.RaceDistance == "5K").SingleOrDefault();
			var _event = _eventData.Find(e => e.EventName == "Test").SingleOrDefault();
			if (distance == null || _event == null)
				throw new InvalidOperationException("Cannont initialize Races - Distance or Event cannot be found");
		    _raceData.Add(new Race() {Distance = distance , DistanceId = distance.Id, Event = _event, EventId = _event.Id});
		    _raceData.SaveChanges();
	    }

	    private void InitEventData()
	    {
		    _eventData.Add(new Event() { EventName = "Test"});
		    _eventData.SaveChanges();
	    }

	    private void InitDistanceData()
	    {
		    _distanceData.Add(new Distance(){RaceDistance = "1 Mile"});
			_distanceData.Add(new Distance() {RaceDistance = "2 Mile"});
			_distanceData.Add(new Distance() {RaceDistance = "5K"}) ;
		    _distanceData.SaveChanges();
	    }

	    private void InitPhotoData()
	    {
		    return;
	    }

	    public int SaveChanges()
	    {
		    int numChanges = _distanceData.SaveChanges() + _eventData.SaveChanges() + _raceData.SaveChanges() +
		                     _photoData.SaveChanges();

		    return numChanges;

	    }
    }

   
}
