using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using HotelBooking.Core;
using Moq;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private Mock<IRepository<Booking>> fakeBookingRepository;
        private Mock<IRepository<Room>> fakeRoomRepository;
        
        public BookingManagerTests(){
            fakeBookingRepository = new Mock<IRepository<Booking>>();
            fakeRoomRepository = new Mock<IRepository<Room>>();
            
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            
            List<Room> roomsFake = new List<Room>
            {
                new Room { Id = 1, Description = "A" },
                new Room { Id = 2, Description = "B" },
                new Room { Id = 3, Description = "C" }
            };

            List<Booking> bookingsFake = new List<Booking>
            {
                new Booking { Id = 1, StartDate = start.AddDays(-5), EndDate = start.AddDays(5), IsActive = true, RoomId = 1 },
                new Booking { Id = 2, StartDate = start, EndDate = end, IsActive = true, RoomId = 2 },
                new Booking { Id = 3, StartDate = start.AddDays(2), EndDate = end.AddDays(-2), IsActive = true, RoomId = 3 }
            }; 
            
            fakeBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingsFake);
            fakeBookingRepository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
                bookingsFake.FirstOrDefault(b => b.Id == id));
            
            fakeRoomRepository.Setup(x => x.GetAll()).Returns(() => roomsFake);
            fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
                roomsFake.FirstOrDefault(r => r.Id == id));
            
            bookingManager = new BookingManager(fakeBookingRepository.Object, fakeRoomRepository.Object);
        }

        // Test case 1: Start date in the past
        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            var start = DateTime.Today.AddDays(-2);
            var end = DateTime.Today.AddDays(-1);

            // Act
            Action act = () => bookingManager.FindAvailableRoom(start, end);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        // Start date later than end date
        [Fact]
        public void FindAvailableRoom_RoomAvailable_ThrowsArgumentExceptions()
        {
            // Arrange
            var start = DateTime.Today.AddDays(2);
            var end = DateTime.Today.AddDays(1);
            // Act
            Action act = () => bookingManager.FindAvailableRoom(start, end);
            
            // Assert
            Assert.Throws<ArgumentException>(act);
        }
        
        // Test case 2: Start date in past end date in future
        [Fact]
        public void FindAvailableRoom_StartDateInThePastEndDateInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            var start = DateTime.Today.AddDays(-2);
            var end = DateTime.Today.AddDays(2);

            // Act
            Action act = () => bookingManager.FindAvailableRoom(start, end);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        
        [Fact]
        public void FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);

            // Assert
            var bookingForReturnedRoomId = fakeBookingRepository.Object.GetAll().Where(
                b => b.RoomId == roomId
                && b.StartDate <= date
                && b.EndDate >= date
                && b.IsActive);

            Assert.Empty(bookingForReturnedRoomId);
        }
        
        // Test case 3 & 5: Booking in a valid timeslot
        [Theory]
        [InlineData(1, 8)]
        [InlineData(21, 25)]
        public void FindAvailableRoom_RoomAvailable_ReturnsRoomId(int start, int end)
        {
            // Arrange
            var startDateTime = DateTime.Today.AddDays(start);
            var endDateTime = DateTime.Today.AddDays(end);
            // Act
            int roomId = bookingManager.FindAvailableRoom(startDateTime, endDateTime);
            // Assert
            Assert.InRange<int>(roomId, 1, 3);
        }
        
        // Test case 4: start date before and end date after a fully booked period
        [Fact]
        public void FindAvailableRoom_DuringFullyOccupiedPeriod_ReturnsMinusOne()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(7);
            DateTime end = DateTime.Today.AddDays(17);
            
            // Act
            int roomId = bookingManager.FindAvailableRoom(start, end);
            
            // Assert
            Assert.Equal(-1, roomId);
        }

        // Test case 6: Start date before and during a fully booked period
        [Fact]
        public void FindAvailableRoom_StartBeforeAndDuringBookedPeriod()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(7);
            DateTime endDate = DateTime.Today.AddDays(14);

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(-1, roomId);
        }

        // Test case 7: Start and end date during a fully booked period
        [Fact]
        public void FindAvailableRoom_StartAndEndDuringBookedPeriod()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(12);
            DateTime endDate = DateTime.Today.AddDays(14);

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(-1, roomId);
        }

        // Test case 8: Start date during and end date after a fully booked period.
        [Fact]
        public void FindAvailableRoom_StartDuringBookedPeriodAndEndAfter()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(14);
            DateTime endDate = DateTime.Today.AddDays(20);

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(-1, roomId);
        }
        
        [Fact]
        public void GetFullyOccupiedDates_StartDateLaterThanEndDate_ThrowsArgumentException()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(10);
            var endDate = DateTime.Today.AddDays(5);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => bookingManager.GetFullyOccupiedDates(startDate, endDate));
        }
        
        [Fact]
        public void GetFullyOccupiedDates_NoBookings_ReturnsEmptyList()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(10);
            var endDate = DateTime.Today.AddDays(20);
            fakeBookingRepository.Setup(x => x.GetAll()).Returns(new List<Booking>());

            // Act
            var result = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void GetFullyOccupiedDates_BookingsExist_ReturnsFullyOccupiedDates()
        {
            // Arrange
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(20);
            
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, StartDate = startDate.AddDays(-5), EndDate = startDate.AddDays(5), IsActive = true, RoomId = 1 },
                new Booking { Id = 2, StartDate = startDate.AddDays(2), EndDate = startDate.AddDays(10), IsActive = true, RoomId = 2 },
                new Booking { Id = 3, StartDate = startDate.AddDays(4), EndDate = endDate.AddDays(8), IsActive = true, RoomId = 3 }
            };
            
            fakeBookingRepository.Setup(x => x.GetAll()).Returns(bookings);
            
            var expectedDates = new List<DateTime>
            {
                DateTime.Today.AddDays(4),
                DateTime.Today.AddDays(5)
            };

            // Act
            var result = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Equal(expectedDates, result);
        }
        
        [Fact]
        public void CreateBooking_RoomAvailable_ReturnsTrue()
        {
            // Arrange
            var booking = new Booking { StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20) };
            fakeBookingRepository.Setup(x => x.Add(It.IsAny<Booking>()));
            fakeBookingRepository.Setup(x => x.GetAll()).Returns(new List<Booking>());

            // Act
            var result = bookingManager.CreateBooking(booking);

            // Assert
            Assert.True(result);
            fakeBookingRepository.Verify(x => x.Add(booking), Times.Once);
        }
        
        [Fact]
        public void CreateBooking_NoRoomAvailable_ReturnsFalse()
        {
            // Arrange
            var booking = new Booking { StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20) };
            
            var availableRooms = new List<Room>
            {
                new Room { Id = 1, Description = "A" }
            };
            fakeRoomRepository.Setup(x => x.GetAll()).Returns(availableRooms);
            
            var existingBookings = new List<Booking> { 
                new Booking { 
                    RoomId = 1, StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(20), IsActive = true 
                }};
            
            fakeBookingRepository.Setup(x => x.GetAll()).Returns(existingBookings);
            
            // Act
            var result = bookingManager.CreateBooking(booking);

            // Assert
            Assert.False(result);
            fakeBookingRepository.Verify(x => x.Add(It.IsAny<Booking>()), Times.Never);
        }

    }
}
