using System.Text.Json;
using System.Text.Json.Serialization;

namespace STech.Services.Services
{
    public class GeocodioService : IGeocodioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeocodioService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public class Geometry
        {
            [JsonPropertyName("lat")]
            public double Lat { get; set; }

            [JsonPropertyName("lng")]
            public double Lng { get; set; }
        }

        public class Result
        {
            [JsonPropertyName("geometry")]
            public Geometry Geometry { get; set; } = new Geometry();
        }

        public class OpenCageResponse
        {
            [JsonPropertyName("results")]
            public IEnumerable<Result> Results { get; set; } = new List<Result>();
        }

        public async Task<(double? Latitude, double? Longtitude)> GetLocation(string city, string district, string ward)
        {
            string address = $"{ward}, {district}, {city}, Vietnam";

            HttpResponseMessage response = await _httpClient.GetAsync($"https://api.opencagedata.com/geocode/v1/json?q={address}&key={_apiKey}");
            response.EnsureSuccessStatusCode();

            if(response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                OpenCageResponse? data = JsonSerializer.Deserialize<OpenCageResponse>(json);

                if(data != null && data.Results.Count() > 0)
                {
                    return (data.Results.ElementAt(0).Geometry.Lat, data.Results.ElementAt(0).Geometry.Lng);
                }
            }

            return (null, null);
        }

        public async Task<(double? Latitude, double? Longtitude)> GetLocation(string address)
        {
            //    HttpResponseMessage response = await _httpClient.GetAsync($"https://api.opencagedata.com/geocode/v1/json?q={address}&key={_apiKey}");
            //    response.EnsureSuccessStatusCode();

            //    if (response.IsSuccessStatusCode)
            //    {
            //        string json = await response.Content.ReadAsStringAsync();
            //        OpenCageResponse? data = JsonSerializer.Deserialize<OpenCageResponse>(json);

            //        if (data != null && data.Results.Count() > 0)
            //        {
            //            return (data.Results.ElementAt(0).Geometry.Lat, data.Results.ElementAt(0).Geometry.Lng);
            //        }
            //    }

            return (null, null);
        }
    }
}
