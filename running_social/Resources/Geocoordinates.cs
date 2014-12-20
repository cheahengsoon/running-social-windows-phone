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
        private Geolocator myGeolocator = new Geolocator();
        public List<Tuple<DateTime, Geopoint>> locationsList = new List<Tuple<DateTime, Geopoint>>();
        /// <summary>
        /// The singleton object.
        /// </summary>
        private static Geocoordinates instance = null;
        /// <summary>
        /// The interval between calculating a new geolocation coordinate, in seconds.
        /// </summary>
        public uint coordinatesInterval = 5;
        /// <summary>
        /// To remember if this object has already been subscribed to updates.
        /// </summary>
        private bool subscribedToUpdates = false;

        /// <summary>
        /// Use <see cref="GetGeolocator"/>
        /// </summary>
        public Geocoordinates()
        {
            try
            {
                myGeolocator.DesiredAccuracyInMeters = 5;
                myGeolocator.MovementThreshold = 5;
                myGeolocator.ReportInterval = coordinatesInterval*1000;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                new MessageDialog(ex.Message).ShowAsync();
            }
        }

        public async void SubscribeToUpdates()
        {
            string exceptionMsg = "";
            try
            {
                instance.myGeolocator.PositionChanged += new TypedEventHandler<Geolocator, PositionChangedEventArgs>(OnLocationChanged);                
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
        /// <see cref="coordinatesInterval"/>
        /// </summary>
        /// <param name="loc">The geolocator</param>
        /// <param name="args">Arguments created by the geolocator, including the geopoint</param>
        public static void OnLocationChanged(Geolocator loc, PositionChangedEventArgs args)
        {
            Geocoordinates instance = Geocoordinates.GetGeolocator();
            instance.locationsList.Add(
                new Tuple<DateTime, Geopoint>( new DateTime(), args.Position.Coordinate.Point ));
            Debug.WriteLine(instance.locationsList.Count);
        }

        /// <summary>
        /// Get the Geocoordinates singleton object.
        /// </summary>
        /// <returns>The Geocoordinates singleton object.</returns>
        public static Geocoordinates GetGeolocator()
        {
            if (instance == null)
            {
                instance = new Geocoordinates();
            }
            return instance;
        }

        /// <summary>
        /// Gets the the coordinate pair closest in time to the given time.
        /// </summary>
        /// <param name="dt">The datetime to get an approximate answer from</param>
        /// <returns>The closest datetime/geopoint pair.</returns>
        public Tuple<DateTime, Geopoint> GetCoordinatePair(DateTime dt)
        {
            if (locationsList.Count == 0)
            {
                CalculateSingeCoordinate();
            }

            Tuple<DateTime, Geopoint> closestPair = locationsList[0];
            int closestDiff = Math.Abs((int) (closestPair.Item1 - dt).TotalSeconds);
            foreach (var pair in locationsList)
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
        /// Calculate the next datetime/geopoint pair and add it to locationsList
        /// From http://msdn.microsoft.com/en-us/library/windows/apps/jj244363(v=vs.105).aspx
        /// </summary>
        private async void CalculateSingeCoordinate()
        {
            // Get the phone's current location.
            string exceptionMsg = "";
            try
            {
                Geoposition myGeoPosition = await myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                locationsList.Add(
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
