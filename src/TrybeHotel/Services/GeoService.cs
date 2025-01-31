using System.Net.Http;
using Microsoft.Identity.Client;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;

        public GeoService(HttpClient client)
        {
            _client = client;

            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "aspnet-user-agent");
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object> GetGeoStatus()
        {
            var request = await _client.GetAsync(
                "https://nominatim.openstreetmap.org/status.php?format=json"
            );

            if (request.IsSuccessStatusCode)
            {
                var response = await request.Content.ReadFromJsonAsync<object>();
                return response!;
            }

            return default!;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            string uri =
                $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1";

            var request = await _client.GetAsync(uri);

            if (request.IsSuccessStatusCode)
            {
                var response = await request.Content.ReadFromJsonAsync<GeoDtoResponse[]>();
                if (response is not null && response.Length > 0)
                    return new GeoDtoResponse { lat = response[0].lat, lon = response[0].lon };

                return default!;
            }

            return default!;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(
            GeoDto geoDto,
            IHotelRepository repository
        )
        {
            List<HotelDto> hotels = repository.GetHotels().ToList();
            List<GeoDtoHotelResponse> response = new();

            foreach (HotelDto hotel in hotels)
            {
                GeoDtoResponse geoHotel = await GetGeoLocation(
                    new GeoDto
                    {
                        Address = hotel.Address,
                        City = hotel.CityName,
                        State = hotel.State,
                    }
                );

                GeoDtoResponse geoSearch = await GetGeoLocation(geoDto);

                if (geoHotel is not null)
                {
                    int distance = CalculateDistance(
                        geoHotel.lat!,
                        geoHotel.lon!,
                        geoSearch.lat!,
                        geoSearch.lon!
                    );

                    response.Add(
                        new GeoDtoHotelResponse
                        {
                            HotelId = hotel.HotelId,
                            Name = hotel.Name,
                            Address = hotel.Address,
                            CityName = hotel.CityName,
                            State = hotel.State,
                            Distance = distance,
                        }
                    );
                }
            }

            response.Sort((hotel1, hotel2) => hotel1.Distance.CompareTo(hotel2.Distance));

            return response;
        }

        public int CalculateDistance(
            string latitudeOrigin,
            string longitudeOrigin,
            string latitudeDestiny,
            string longitudeDestiny
        )
        {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.', ','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.', ','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.', ','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.', ','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(radiano(latOrigin))
                    * Math.Cos(radiano(latDestiny))
                    * Math.Sin(dLon / 2)
                    * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return int.Parse(Math.Round(distance, 0).ToString());
        }

        public double radiano(double degree)
        {
            return degree * Math.PI / 180;
        }
    }
}
