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
        /// <summary>
        /// Contains all the static settings for the geolocator.
        /// </summary>
        private Geolocator _myGeolocator = null;
        /// <summary>
        /// The current run to record geolocation info into.
        /// </summary>
        private Run _run = null;
        /// <summary>
        /// The singleton object.
        /// </summary>
        private static Geocoordinates _instance = null;
        /// <summary>
        /// The interval between calculating a new geolocation coordinate, in seconds.
        /// </summary>
        public uint DefaultCoordinatesInterval = 5;
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
                _myGeolocator = new Geolocator
                {
                    DesiredAccuracyInMeters = 5,
                    MovementThreshold = 5,
                    ReportInterval = DefaultCoordinatesInterval*1000
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                new MessageDialog(ex.Message).ShowAsync();
            }
        }

        public async void PauseRun()
        {
            if (_subscription == null)
            {
                return;
            }
            _myGeolocator.PositionChanged -= _subscription;
            _subscription = null;
        }

        public async void StopRun()
        {
            PauseRun();
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
            // check that the singleton has been created
            if (_instance == null)
            {
                GetGeolocator();
            } else {
                // add a new geocoordinate to the locations list for this run
                _instance._run.LocationsList.Add(
                    new Tuple<DateTime, Geopoint>(new DateTime(), args.Position.Coordinate.Point));
                Debug.WriteLine(_instance._run.LocationsList.Count);
            }
        }

        /// <summary>
        /// Get the Geocoordinates singleton object.
        /// </summary>
        /// <returns>The Geocoordinates singleton object.</returns>
        public static Geocoordinates GetGeolocator()
        {
            return _instance ?? (_instance = new Geocoordinates());
        }

        /// <summary>
        /// Gets the the coordinate pair closest in time to the given time.
        /// </summary>
        /// <param name="dt">The datetime to get an approximate answer from</param>
        /// <returns>The closest datetime/geopoint pair.</returns>
        public async Task<Tuple<DateTime, Geopoint>> GetCoordinatePair(DateTime dt)
        {
            if (_run == null || _run.LocationsList.Count == 0)
            {
                return await CalculateSingeCoordinate();
            }

            Tuple<DateTime, Geopoint> closestPair = _run.LocationsList[0];
            int closestDiff = Math.Abs((int) (closestPair.Item1 - dt).TotalSeconds);
            foreach (var pair in _run.LocationsList)
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
        public void ResumeRun()
        {
            if (_run != null)
            {
                _run.StartNewLocationsList();
            } else {
                throw new InvalidOperationException(
                    "No run selected when trying to start a new locations list.");
            }
        }

        /// <summary>
        /// Used to start a new set of lists/runs every time "play" is pushed after "stop"
        /// or the first time "play" is pushed.
        /// </summary>
        public void StartNewRun()
        {
            _instance._run = new Run();
        }

        /// <summary>
        /// Calculate the next datetime/geopoint pair and add it to locationsList
        /// From http://msdn.microsoft.com/en-us/library/windows/apps/jj244363(v=vs.105).aspx
        /// </summary>
        private async Task<Tuple<DateTime,Geopoint>> CalculateSingeCoordinate()
        {
            Tuple<DateTime, Geopoint> retval = null;

            // Get the phone's current location.
            string exceptionMsg = "";
            try
            {
                Geoposition myGeoPosition = await _myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                retval = new Tuple<DateTime, Geopoint>(new DateTime(), myGeoPosition.Coordinate.Point);
                if (_run != null)
                {
                    _run.LocationsList.Add(retval);
                }
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

            return retval;
        }
    }
}
