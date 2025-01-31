using Microsoft.EntityFrameworkCore;
using TrybeHotel.Dto;
using TrybeHotel.Models;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;

        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 7. Refatore o endpoint GET /room
        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            return _context
                .Rooms.Where(room => room.HotelId == HotelId)
                .Include(room => room.Hotel)
                .Select(room => new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.Hotel!.HotelId,
                        Name = room.Hotel.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City!.Name,
                        State = room.Hotel.City!.State,
                    },
                });
        }

        // 8. Refatore o endpoint POST /room
        public RoomDto AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();

            return _context
                .Rooms.Where(r => r.RoomId == room.RoomId)
                .Include(r => r.Hotel)
                .Select(r => new RoomDto
                {
                    RoomId = r.RoomId,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    Image = r.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = r.Hotel!.HotelId,
                        Name = r.Hotel.Name,
                        Address = r.Hotel.Address,
                        CityId = r.Hotel.CityId,
                        CityName = r.Hotel.City!.Name,
                        State = r.Hotel.City!.State,
                    },
                })
                .FirstOrDefault()!;
        }

        public void DeleteRoom(int RoomId)
        {
            var roomToDelete = _context.Rooms.Find(RoomId);
            if (roomToDelete != null)
            {
                _context.Rooms.Remove(roomToDelete);
            }
            _context.SaveChanges();
        }
    }
}
