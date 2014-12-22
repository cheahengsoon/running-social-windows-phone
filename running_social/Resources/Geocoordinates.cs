using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Popups;

namespace running_social.Resources
{
    /// <summary>
    /// Used to interface with the GPS, to find the coordinates at any given time during the application's lifetime.
    /// </summary>
    class Geocoordinates
    {
        private Geolocator _myGeolocator = new Geolocator();
        /// <summary>
        /// List of sublists of locations points and times.
        /// Each sublist represents a time period from when "play" was pushed until "pause" was pushed.
        /// </summary>
        public List<List<Tuple<DateTime, Geopoint>>> LocationsSetsList =
            new List<List<Tuple<DateTime, Geopoint>>>();
        /// <summary>
        /// Quick reference to the the current sublist of LocationsSetsList.
        /// </summary>
        public List<Tuple<DateTime, Geopoint>> LocationsList = null;
        /// <summary>
        /// The singleton object.
        /// </summary>
        private static Geocoordinates _instance = null;
        /// <summary>
        /// The interval between calculating a new geolocation coordinate, in seconds.
        /// </summary>
        public uint CoordinatesInterval = 5;
        /// <summary>
        /// To remember if this object has already been subscribed to updates.
        /// </summary>
        private bool _subscribedToUpdates = false;
        /// <summary>
        /// The subscription generated from SubscribeToUpdate().
        /// Stored so that events can be unsubscribed from.
        /// </summary>
        private TypedEventHandler<Geolocator, PositionChangedEventArgs> _subscription = null;

        /// <summary>
        /// Use <see cref="GetGeolocator"/>
        /// </summary>
        public Geocoordinates()
        {
            try
            {
                _myGeolocator.DesiredAccuracyInMeters = 5;
                _myGeolocator.MovementThreshold = 5;
                _myGeolocator.ReportInterval = CoordinatesInterval*1000;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                new MessageDialog(ex.Message).ShowAsync();
            }
        }

        public async void StopSubscription()
        {
            if (_subscription == null)
            {
                return;
            }
            _myGeolocator.PositionChanged -= _subscription;
            _subscription = null;
        }

        public async void SubscribeToUpdates()
        {
            // check that there isn't currently a subscription
            if (_subscription != null)
            {
                return;
            }

            // subscribe to geolocation updates
            string exceptionMsg = "";
            try
            {
                _subscription = new TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnLocationChanged);
                _myGeolocator.PositionChanged += _subscription;
            }
            catch (UnauthorizedAccessException ex)
            {
                exceptionMsg = "Location is disabled in phone settings or capabilities are not checked";
                exceptionMsg += " (" + ex.Message + ")";
            }
            catch (Exception ex)
            {
                // Something else happened while acquiring the location.
                exceptionMsg = ex.Message;
            }
            if (exceptionMsg != "")
            {
                await new MessageDialog(exceptionMsg).ShowAsync();
            }
        }

        /// <summary>
        /// Gathers a new geopoint and datetime to add to the list of geopoints.
        /// <see cref="CoordinatesInterval"/>
        /// </summary>
        /// <param name="loc">The geolocator</param>
        /// <param name="args">Arguments created by the geolocator, including the geopoint</param>
        public static void OnLocationChanged(Geolocator loc, PositionChangedEventArgs args)
        {
            _instance.LocationsList.Add(
                new Tuple<DateTime, Geopoint>( new DateTime(), args.Position.Coordinate.Point ));
            Debug.WriteLine(_instance.LocationsList.Count);
        }

        /// <summary>
        /// Get the Geocoordinates singleton object.
        /// </summary>
        /// <returns>The Geocoordinates singleton object.</returns>
        public static Geocoordinates GetGeolocator()
        {
            if (_instance == null)
            {
                _instance = new Geocoordinates();
            }
            return _instance;
        }

        /// <summary>
        /// Gets the the coordinate pair closest in time to the given time.
        /// </summary>
        /// <param name="dt">The datetime to get an approximate answer from</param>
        /// <returns>The closest datetime/geopoint pair.</returns>
        public Tuple<DateTime, Geopoint> GetCoordinatePair(DateTime dt)
        {
            if (LocationsList.Count == 0)
            {
                CalculateSingeCoordinate();
            }

            Tuple<DateTime, Geopoint> closestPair = LocationsList[0];
            int closestDiff = Math.Abs((int) (closestPair.Item1 - dt).TotalSeconds);
            foreach (var pair in LocationsList)
            {
                int diff = Math.Abs( (int)(pair.Item1 - dt).TotalSeconds );
                if (diff < closestDiff)
                {
                    closestPair = pair;
                    closestDiff = diff;
                }
            }
            return closestPair;
        }

        /// <summary>
        /// Used to start a new list every time "play" is pushed after "pause".
        /// </summary>
        public void StartNewLocationsList()
        {
            LocationsSetsList.Add(new List<Tuple<DateTime, Geopoint>>());
            LocationsList = LocationsSetsList[LocationsSetsList.Count - 1];
        }

        /// <summary>
        /// Used to start a new set of lists every time "play" is pushed after "stop"
        /// or the first time "play" is pushed.
        /// </summary>
        public void StartNewLocationsSetList()
        {
            LocationsSetsList = new List<List<Tuple<DateTime, Geopoint>>>();
            StartNewLocationsList();
        }

        /// <summary>
        /// Calculate the next datetime/geopoint pair and add it to locationsList
        /// From http://msdn.microsoft.com/en-us/library/windows/apps/jj244363(v=vs.105).aspx
        /// </summary>
        private async void CalculateSingeCoordinate()
        {
            // Get the phone's current location.
            string exceptionMsg = "";
            try
            {
                Geoposition myGeoPosition = await _myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                LocationsList.Add(
                    new Tuple<DateTime, Geopoint>(new DateTime(), myGeoPosition.Coordinate.Point));
            }
            catch (UnauthorizedAccessException ex)
            {
                exceptionMsg = "Location is disabled in phone settings or capabilities are not checked";
                exceptionMsg += " (" + ex.Message + ")";
            }
            catch (Exception ex)
            {
                // Something else happened while acquiring the location.
                exceptionMsg = ex.Message;
            }
            if (exceptionMsg != "")
            {
                await new MessageDialog(exceptionMsg).ShowAsync();
            }
        }
    }
}
