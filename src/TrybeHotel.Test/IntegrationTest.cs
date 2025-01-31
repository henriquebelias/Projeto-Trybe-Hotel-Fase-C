namespace TrybeHotel.Test;

using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TrybeHotel.Dto;
using TrybeHotel.Models;
using TrybeHotel.Repository;

public class LoginJson
{
    public string? token { get; set; }
}

public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>)
                    );
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ContextTest>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryTestDatabase");
                    });
                    services.AddScoped<ITrybeHotelContext, ContextTest>();
                    services.AddScoped<ICityRepository, CityRepository>();
                    services.AddScoped<IHotelRepository, HotelRepository>();
                    services.AddScoped<IRoomRepository, RoomRepository>();
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                    {
                        appContext.Database.EnsureCreated();
                        appContext.Database.EnsureDeleted();
                        appContext.Database.EnsureCreated();
                        appContext.Cities.Add(
                            new City
                            {
                                CityId = 1,
                                Name = "Manaus",
                                State = "AM",
                            }
                        );
                        appContext.Cities.Add(
                            new City
                            {
                                CityId = 2,
                                Name = "Palmas",
                                State = "TO",
                            }
                        );
                        appContext.SaveChanges();
                        appContext.Hotels.Add(
                            new Hotel
                            {
                                HotelId = 1,
                                Name = "Trybe Hotel Manaus",
                                Address = "Address 1",
                                CityId = 1,
                            }
                        );
                        appContext.Hotels.Add(
                            new Hotel
                            {
                                HotelId = 2,
                                Name = "Trybe Hotel Palmas",
                                Address = "Address 2",
                                CityId = 2,
                            }
                        );
                        appContext.Hotels.Add(
                            new Hotel
                            {
                                HotelId = 3,
                                Name = "Trybe Hotel Ponta Negra",
                                Address = "Addres 3",
                                CityId = 1,
                            }
                        );
                        appContext.SaveChanges();
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 1,
                                Name = "Room 1",
                                Capacity = 2,
                                Image = "Image 1",
                                HotelId = 1,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 2,
                                Name = "Room 2",
                                Capacity = 3,
                                Image = "Image 2",
                                HotelId = 1,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 3,
                                Name = "Room 3",
                                Capacity = 4,
                                Image = "Image 3",
                                HotelId = 1,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 4,
                                Name = "Room 4",
                                Capacity = 2,
                                Image = "Image 4",
                                HotelId = 2,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 5,
                                Name = "Room 5",
                                Capacity = 3,
                                Image = "Image 5",
                                HotelId = 2,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 6,
                                Name = "Room 6",
                                Capacity = 4,
                                Image = "Image 6",
                                HotelId = 2,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 7,
                                Name = "Room 7",
                                Capacity = 2,
                                Image = "Image 7",
                                HotelId = 3,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 8,
                                Name = "Room 8",
                                Capacity = 3,
                                Image = "Image 8",
                                HotelId = 3,
                            }
                        );
                        appContext.Rooms.Add(
                            new Room
                            {
                                RoomId = 9,
                                Name = "Room 9",
                                Capacity = 4,
                                Image = "Image 9",
                                HotelId = 3,
                            }
                        );
                        appContext.SaveChanges();
                        appContext.Users.Add(
                            new User
                            {
                                UserId = 1,
                                Name = "Ana",
                                Email = "ana@trybehotel.com",
                                Password = "Senha1",
                                UserType = "admin",
                            }
                        );
                        appContext.Users.Add(
                            new User
                            {
                                UserId = 2,
                                Name = "Beatriz",
                                Email = "beatriz@trybehotel.com",
                                Password = "Senha2",
                                UserType = "client",
                            }
                        );
                        appContext.Users.Add(
                            new User
                            {
                                UserId = 3,
                                Name = "Laura",
                                Email = "laura@trybehotel.com",
                                Password = "Senha3",
                                UserType = "client",
                            }
                        );
                        appContext.SaveChanges();
                        appContext.Bookings.Add(
                            new Booking
                            {
                                BookingId = 1,
                                CheckIn = new DateTime(2023, 07, 02),
                                CheckOut = new DateTime(2023, 07, 03),
                                GuestQuant = 1,
                                UserId = 2,
                                RoomId = 1,
                            }
                        );
                        appContext.Bookings.Add(
                            new Booking
                            {
                                BookingId = 2,
                                CheckIn = new DateTime(2023, 07, 02),
                                CheckOut = new DateTime(2023, 07, 03),
                                GuestQuant = 1,
                                UserId = 3,
                                RoomId = 4,
                            }
                        );
                        appContext.SaveChanges();
                    }
                });
            })
            .CreateClient();
    }

    [Trait("Category", "GET /city")]
    [Theory(DisplayName = "Deve retornar status code 200")]
    [InlineData("/city")]
    public async Task TestGetCity(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "POST /city")]
    [Theory(DisplayName = "Deve retornar status code 201")]
    [InlineData("/city")]
    public async Task TestPostCity(string url)
    {
        City city = new() { CityId = 3, Name = "Recife" };

        var response = await _clientTest.PostAsync(
            url,
            new StringContent(JsonConvert.SerializeObject(city), Encoding.UTF8, "application/json")
        );
        Assert.Equal(HttpStatusCode.Created, response?.StatusCode);

        string responseString = await response!.Content.ReadAsStringAsync();
        City cityResponse = JsonConvert.DeserializeObject<City>(responseString)!;

        Assert.Equal(city.CityId, cityResponse.CityId);
        Assert.Equal(city.Name, cityResponse.Name);
    }

    [Trait("Category", "GET /hotel")]
    [Theory(DisplayName = "Deve retornar status code 200")]
    [InlineData("/hotel")]
    public async Task TestGetHotelStatusCodeOk(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response?.StatusCode);

        string responseString = await response!.Content.ReadAsStringAsync();
        List<Hotel> responseObject = JsonConvert.DeserializeObject<List<Hotel>>(responseString)!;

        Assert.NotNull(responseObject);
        Assert.True(responseObject!.Count > 0);
    }

    [Trait("Category", "POST /hotel")]
    [Theory(DisplayName = "Deve retornar status code 201")]
    [InlineData("/hotel")]
    public async Task TestPostHotelStatusCodeCreated(string url)
    {
        LoginDto loginDto = new() { Email = "ana@trybehotel.com", Password = "Senha1" };

        var responseLogin = await _clientTest.PostAsync(
            "/login",
            new StringContent(
                JsonConvert.SerializeObject(loginDto),
                Encoding.UTF8,
                "application/json"
            )
        );

        string userToken = await responseLogin.Content.ReadAsStringAsync();
        LoginJson tokenResponse = JsonConvert.DeserializeObject<LoginJson>(userToken)!;

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokenResponse.token
        );

        Hotel hotel = new()
        {
            HotelId = 4,
            Name = "Hotel A",
            Address = "Rua A",
            CityId = 1,
        };

        var response = await _clientTest.PostAsync(
            url,
            new StringContent(JsonConvert.SerializeObject(hotel), Encoding.UTF8, "application/json")
        );
        Assert.Equal(HttpStatusCode.Created, response?.StatusCode);

        string responseString = await response!.Content.ReadAsStringAsync();
        Hotel responseObject = JsonConvert.DeserializeObject<Hotel>(responseString)!;

        Assert.Equal(hotel.HotelId, responseObject.HotelId);
        Assert.Equal(hotel.Name, responseObject.Name);
    }

    [Trait("Category", "GET /room/{hotelId}")]
    [Theory(DisplayName = "Deve retornar status code 200")]
    [InlineData("/room/1")]
    public async Task TestGetRoomByHotelIdStatusCodeOk(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response?.StatusCode);

        string responseString = await response!.Content.ReadAsStringAsync();
        List<Room> responseObject = JsonConvert.DeserializeObject<List<Room>>(responseString)!;

        Assert.NotNull(responseObject);
        Assert.True(responseObject.Count > 0);
    }

    [Trait("Category", "POST /room")]
    [Theory(DisplayName = "Deve retornar status code 201")]
    [InlineData("/room")]
    public async Task TestPostRoomStatusCodeCreated(string url)
    {
        LoginDto loginDto = new() { Email = "ana@trybehotel.com", Password = "Senha1" };

        var responseLogin = await _clientTest.PostAsync(
            "/login",
            new StringContent(
                JsonConvert.SerializeObject(loginDto),
                Encoding.UTF8,
                "application/json"
            )
        );

        string userToken = await responseLogin.Content.ReadAsStringAsync();
        LoginJson tokenResponse = JsonConvert.DeserializeObject<LoginJson>(userToken)!;

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokenResponse.token
        );

        Room newRoom = new()
        {
            RoomId = 10,
            Name = "Room 10",
            Capacity = 2,
            Image = "Image 10",
            HotelId = 1,
        };

        var response = await _clientTest.PostAsync(
            url,
            new StringContent(
                JsonConvert.SerializeObject(newRoom),
                Encoding.UTF8,
                "application/json"
            )
        );
        Assert.Equal(HttpStatusCode.Created, response?.StatusCode);

        string responseString = await response!.Content.ReadAsStringAsync();
        Room responseObject = JsonConvert.DeserializeObject<Room>(responseString)!;

        Assert.Equal(newRoom.RoomId, responseObject!.RoomId);
        Assert.Equal(newRoom.Name, responseObject.Name);
    }

    [Trait("Category", "DELETE /room/{roomId}")]
    [Theory(DisplayName = "Deve retornar status code 204")]
    [InlineData("/room/1")]
    public async Task TestDeleteRoomStatusCodeNoContent(string url)
    {
        LoginDto loginDto = new() { Email = "ana@trybehotel.com", Password = "Senha1" };

        var login = await _clientTest.PostAsync(
            "/login",
            new StringContent(
                JsonConvert.SerializeObject(loginDto),
                Encoding.UTF8,
                "application/json"
            )
        );

        string userToken = await login.Content.ReadAsStringAsync();
        LoginJson tokenResponse = JsonConvert.DeserializeObject<LoginJson>(userToken)!;

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokenResponse.token
        );

        var response = await _clientTest.DeleteAsync(url);
        Assert.Equal(HttpStatusCode.NoContent, response?.StatusCode);
    }

    [Trait("Category", "POST /login")]
    [Theory(DisplayName = "Deve retornar status code 200")]
    [InlineData("/login")]
    public async Task TestLogin(string url)
    {
        LoginDto inputObj = new() { Email = "ana@trybehotel.com", Password = "Senha1" };

        var response = await _clientTest.PostAsync(
            url,
            new StringContent(
                JsonConvert.SerializeObject(inputObj),
                Encoding.UTF8,
                "application/json"
            )
        );

        string responseString = await response.Content.ReadAsStringAsync();

        LoginJson tokenResponse = JsonConvert.DeserializeObject<LoginJson>(responseString)!;

        Assert.Equal(HttpStatusCode.OK, response?.StatusCode);
        Assert.NotEmpty(tokenResponse.token);
    }

    [Trait("Category", "POST /user")]
    [Theory(DisplayName = "Deve retornar status code 201")]
    [InlineData("/user")]
    public async Task TestPostUserSuccess(string url)
    {
        UserDtoInsert userInsert = new()
        {
            Name = "Paulo",
            Email = "Paulo@trybehotel.com",
            Password = "Pass4",
        };

        var response = await _clientTest.PostAsync(
            url,
            new StringContent(
                JsonConvert.SerializeObject(userInsert),
                Encoding.UTF8,
                "application/json"
            )
        );

        Assert.Equal(HttpStatusCode.Created, response?.StatusCode);
    }

    [Trait("Category", "GET /user")]
    [Theory(DisplayName = "Devem retornar status code 200")]
    [InlineData("/user")]
    public async Task TestGetUsers(string url)
    {
        LoginDto loginDto = new() { Email = "ana@trybehotel.com", Password = "Senha1" };

        var login = await _clientTest.PostAsync(
            "/login",
            new StringContent(
                JsonConvert.SerializeObject(loginDto),
                Encoding.UTF8,
                "application/json"
            )
        );

        string responseLoginString = await login.Content.ReadAsStringAsync();
        LoginJson tokenResponse = JsonConvert.DeserializeObject<LoginJson>(responseLoginString)!;

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokenResponse.token
        );

        var response = await _clientTest.GetAsync(url);
        string responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response?.StatusCode);

        List<UserDto> userList = JsonConvert.DeserializeObject<List<UserDto>>(responseString)!;

        Assert.NotNull(userList);
        Assert.True(userList.Count > 0);
    }

    [Trait("Category", "POST /booking")]
    [Theory(DisplayName = "Deve retornar status code 201")]
    [InlineData("/booking")]
    public async Task TestPostSuccessBooking(string url)
    {
        LoginDto loginDto = new() { Email = "ana@trybehotel.com", Password = "Senha1" };

        var login = await _clientTest.PostAsync(
            "/login",
            new StringContent(
                JsonConvert.SerializeObject(loginDto),
                Encoding.UTF8,
                "application/json"
            )
        );

        string readToken = await login.Content.ReadAsStringAsync();
        LoginJson tokenResponse = JsonConvert.DeserializeObject<LoginJson>(readToken)!;

        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            tokenResponse.token
        );

        BookingDtoInsert bookingInsert = new()
        {
            CheckIn = DateTime.Now,
            CheckOut = DateTime.Now.AddDays(3),
            GuestQuant = 1,
            RoomId = 1,
        };

        var response = await _clientTest.PostAsync(
            url,
            new StringContent(
                JsonConvert.SerializeObject(bookingInsert),
                Encoding.UTF8,
                "application/json"
            )
        );

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
