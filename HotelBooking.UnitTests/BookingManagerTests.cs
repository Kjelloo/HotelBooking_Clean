using System;
using System.Collections.Generic;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using HotelBooking.Core;
using HotelBooking.Infrastructure.Repositories;
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
            
            List<Room> roomsFake =
            [
                new Room { Id = 1, Description = "A" },
                new Room { Id = 2, Description = "B" },
                new Room { Id = 3, Description = "C" }
            ];
            
            List<Booking> bookingsFake =
            [
                new Booking { Id = 1, StartDate = start, EndDate = end, IsActive = true, RoomId = 1 },
                new Booking { Id = 2, StartDate = DateTime.Today.AddDays(-10), EndDate = DateTime.Today.AddDays(-5), IsActive = false, RoomId = 2 },
                new Booking { Id = 3, StartDate = start, EndDate = end.AddDays(10), IsActive = true, RoomId = 3 }
            ];
            
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
            var bookingForReturnedRoomId = bookingRepository.GetAll().Where(
                b => b.RoomId == roomId
                && b.StartDate <= date
                && b.EndDate >= date
                && b.IsActive);

            Assert.Empty(bookingForReturnedRoomId);
        }

    }
}
