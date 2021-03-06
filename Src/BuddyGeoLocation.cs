using Newtonsoft.Json;
using System;
using System.Globalization;

namespace BuddySDK
{
    [JsonConverter(typeof(BuddySDK.BuddyServiceClient.BuddyLocationGeoConverter))]
    public class BuddyGeoLocation
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }

        public string LocationID { get; private set; }

        public BuddyGeoLocation()
        {
        }

		public static BuddyGeoLocation Parse(object latLng)
		{
            return JsonConvert.DeserializeObject<BuddyGeoLocation>(latLng.ToString());
        }

        public BuddyGeoLocation(string locationId)
        {
            LocationID = locationId;
        }

        public BuddyGeoLocation(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
        public override string ToString()
        {
            if (LocationID == null)
            {
                return String.Format(CultureInfo.InvariantCulture.NumberFormat, "{0},{1}", Latitude, Longitude);
            }
            return LocationID;
        }
    }



    public class BuddyGeoLocationRange : BuddyGeoLocation 
    {
        public int DistanceInMeters {get;set;}

        public BuddyGeoLocationRange(double lat, double lng, int distanceInMeters) : base(lat, lng)
        {
            DistanceInMeters = distanceInMeters;
        } 

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture.NumberFormat, "{0},{1},{2}", Latitude, Longitude, DistanceInMeters);
        }
    }
}

