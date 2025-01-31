using TrybeHotel.Dto;
using TrybeHotel.Models;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;

        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public UserDto GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public UserDto Login(LoginDto login)
        {
            User? foundUser = _context
                .Users.Where(user => user.Email == login.Email)
                .FirstOrDefault();

            if (foundUser == null || foundUser.Password != login.Password!)
            {
                throw new InvalidOperationException("Incorrect e-mail or password");
            }

            return new UserDto
            {
                UserId = foundUser.UserId,
                Name = foundUser.Name,
                Email = foundUser.Email,
                UserType = foundUser.UserType,
            };
        }

        public UserDto Add(UserDtoInsert user)
        {
            var userExists = _context.Users.Where(u => u.Email == user.Email).FirstOrDefault();

            if (userExists != null)
            {
                throw new InvalidOperationException("User email already exists");
            }

            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                UserType = "client",
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return new UserDto
            {
                UserId = newUser.UserId,
                Name = newUser.Name,
                Email = newUser.Email,
                UserType = newUser.UserType,
            };
        }

        public UserDto GetUserByEmail(string userEmail)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _context
                .Users.Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    UserType = u.UserType,
                })
                .ToList();
        }
    }
}
