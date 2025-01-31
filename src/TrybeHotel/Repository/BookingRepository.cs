using Microsoft.EntityFrameworkCore;
using TrybeHotel.Dto;
using TrybeHotel.Models;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;

        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 9. Refatore o endpoint POST /booking
        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            User? user = _context.Users.Where(user => user.Email == email).FirstOrDefault();

            Room? room = _context
                .Rooms.Include(room => room.Hotel)
                .ThenInclude(hotel => hotel!.City)
                .Where(room => room.RoomId == booking.RoomId)
                .FirstOrDefault();

            if (booking.GuestQuant > room!.Capacity)
            {
                throw new Exception("Guest quantity over room capacity");
            }

            Booking bookingToAdd = new()
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                RoomId = booking.RoomId,
                UserId = user!.UserId,
            };

            _context.Bookings.Add(bookingToAdd);
            _context.SaveChanges();

            return new BookingResponse
            {
                BookingId = bookingToAdd.BookingId,
                CheckIn = bookingToAdd.CheckIn,
                CheckOut = bookingToAdd.CheckOut,
                GuestQuant = bookingToAdd.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.HotelId,
                        Name = room.Hotel!.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City!.Name,
                        State = room.Hotel.City!.State,
                    },
                },
            };
        }

        // 10. Refatore o endpoint GET /booking
        public BookingResponse GetBooking(int bookingId, string email)
        {
            User? user = _context.Users.FirstOrDefault(user => user.Email == email);
            Booking? booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            Room? room = _context
                .Rooms.Include(r => r.Hotel)
                .ThenInclude(h => h!.City)
                .FirstOrDefault(r => r.RoomId == booking!.RoomId);

            if (booking!.UserId != user!.UserId)
            {
                throw new UnauthorizedAccessException();
            }

            return new BookingResponse
            {
                BookingId = booking.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room!.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.HotelId,
                        Name = room.Hotel!.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City!.Name,
                        State = room.Hotel.City!.State,
                    },
                },
            };
        }

        public Room GetRoomById(int RoomId)
        {
            throw new NotImplementedException();
        }
    }
}
