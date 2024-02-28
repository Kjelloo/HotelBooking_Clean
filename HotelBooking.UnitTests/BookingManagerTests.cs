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
        private Mock<IRepository<Booking>> fakeBookingRespository;
        private Mock<IRepository<Room>> fakeRoomRespository;
        
        public BookingManagerTests(){
            fakeBookingRespository = new Mock<IRepository<Booking>>();
            fakeRoomRespository = new Mock<IRepository<Room>>();
            
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
            
            fakeBookingRespository.Setup(x => x.GetAll()).Returns(() => bookingsFake);
            fakeBookingRespository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
                bookingsFake.FirstOrDefault(b => b.Id == id));
            
            fakeRoomRespository.Setup(x => x.GetAll()).Returns(() => roomsFake);
            fakeRoomRespository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
                roomsFake.FirstOrDefault(r => r.Id == id));
            
            bookingManager = new BookingManager(fakeBookingRespository.Object, fakeRoomRespository.Object);
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date, date);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
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
            var bookingForReturnedRoomId = fakeBookingRespository.Object.GetAll().Where(
                b => b.RoomId == roomId
                && b.StartDate <= date
                && b.EndDate >= date
                && b.IsActive);

            Assert.Empty(bookingForReturnedRoomId);
        }
        
        [Fact]
        public void FindAvailableRoom_RoomAvailable_ReturnsRoomId()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.InRange<int>(roomId, 1, 3);
        }
        
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
        
        [Fact]
        public void FindAvailableRoom_StartAndEndInTheFuture()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(21);
            DateTime endDate = DateTime.Today.AddDays(25);

            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.NotEqual(-1, roomId);
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
    }
}
