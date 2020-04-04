using antons_auto.mvc.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace antons_auto.mvc.ServiceProxies
{
    public class DawaServiceProxy : IDawaServiceProxy
    {
        private readonly HttpClient _client = new HttpClient
        {
            Timeout = new TimeSpan(10, 0, 0),
            BaseAddress = new Uri("http://dawa.aws.dk/")
        };

        private readonly Dictionary<string, LocationModel> _locationCache = new Dictionary<string, LocationModel>();

        public LocationModel GetLocation(string streetName, string streetNo, int postalCode)
        {
            var key = $"{streetName}#{streetNo}#{postalCode}";

            if (!_locationCache.ContainsKey(key))
            {
                var location = QueryLocation(key);
                _locationCache.Add(key, location);
            }

            return _locationCache[key];
        }

        internal LocationModel QueryLocation(string key)
        {
            var keySplit = key.Split('#');
            var query = $"adgangsadresser?vejnavn={keySplit[0]}&husnr={keySplit[1]}&postnr={keySplit[2]}&struktur=mini";

            var response = GetData(query);

            if (!string.IsNullOrEmpty(response))
            {
                var jArray = JArray.Parse(response);
                if (jArray.Count > 0)
                {
                    return new LocationModel
                    {
                        City = jArray[0]["postnrnavn"].ToString(),
                        Longitude = float.Parse(jArray[0]["x"].ToString()),
                        Latitude = float.Parse(jArray[0]["y"].ToString())
                    };
                }
            }

            return new LocationModel { City = "Ukendt", Longitude = 0, Latitude = 0 };
        }

        internal string GetData(string query)
        {
            var response = _client.GetAsync(query).Result;

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var responseBody = response.Content.ReadAsStringAsync().Result;

                return responseBody;
            }

            return string.Empty;
        }
    }
}