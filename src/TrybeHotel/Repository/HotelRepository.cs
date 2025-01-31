using Microsoft.EntityFrameworkCore;
using TrybeHotel.Dto;
using TrybeHotel.Models;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;

        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        //  5. Refatore o endpoint GET /hotel
        public IEnumerable<HotelDto> GetHotels()
        {
            return _context
                .Hotels.Include(hotel => hotel.City)
                .Select(hotel => new HotelDto
                {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityId = hotel.CityId,
                    CityName = hotel.City!.Name,
                    State = hotel.City!.State,
                });
        }

        // 6. Refatore o endpoint POST /hotel
        public HotelDto AddHotel(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            _context.SaveChanges();

            return _context
                .Hotels.Where(h => h.HotelId == hotel.HotelId)
                .Include(hotel => hotel.City)
                .Select(hotel => new HotelDto
                {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityId = hotel.CityId,
                    CityName = hotel.City!.Name,
                    State = hotel.City!.State,
                })
                .FirstOrDefault()!;
        }
    }
}
