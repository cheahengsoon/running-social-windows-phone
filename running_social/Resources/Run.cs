using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Popups;

namespace running_social.Resources
{
    class Run
    {
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
        /// The interval between calculating a new geolocation coordinate, in seconds.
        /// </summary>
        public uint CoordinatesInterval = 5;

        /// <summary>
        /// Use <see cref="GetGeolocator"/>
        /// </summary>
        public Run()
        {
            StartNewLocationsList();
        }

        /// <summary>
        /// Gets the the coordinate pair closest in time to the given time.
        /// </summary>
        /// <param name="dt">The datetime to get an approximate answer from</param>
        /// <returns>The closest datetime/geopoint pair, or null if there are no pairs.</returns>
        public Tuple<DateTime, Geopoint> GetCoordinatePair(DateTime dt)
        {
            if (LocationsList.Count == 0)
            {
                return null;
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
    }
}
