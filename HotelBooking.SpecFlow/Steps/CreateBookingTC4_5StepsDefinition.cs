using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelBooking.Core;
using Moq;
using TechTalk.SpecFlow;
using Xunit;

namespace HotelBooking.SpecFlow.Steps;

[Binding]
public sealed class CreateBookingTC4_5StepsDefiniton
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly ScenarioContext _scenarioContext;
    
    // Fake rep created
    private IBookingManager bookingManager;
    private Mock<IRepository<Booking>> fakeBookingRepository;
    private Mock<IRepository<Room>> fakeRoomRepository;
    
    // Dates for occupied period
    private DateTime fullOccupiedPeriodStartDate;
    private DateTime fullOccupiedPeriodEndDate;

    public CreateBookingTC4_5StepsDefiniton(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        
        // Fake rep init
        fakeBookingRepository = new Mock<IRepository<Booking>>();
        fakeRoomRepository = new Mock<IRepository<Room>>();
        
        List<Room> roomsFake = new List<Room>
        {
            new Room { Id = 1, Description = "A" }
        };

        List<Booking> bookingsFake = new List<Booking>();
        fakeBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingsFake);
        fakeBookingRepository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
            bookingsFake.FirstOrDefault(b => b.Id == id));
            
        fakeRoomRepository.Setup(x => x.GetAll()).Returns(() => roomsFake);
        fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
            roomsFake.FirstOrDefault(r => r.Id == id));
            
        bookingManager = new BookingManager(fakeBookingRepository.Object, fakeRoomRepository.Object);

    }
    
    [Given(@"the booking manager has the following occupied period:")]
    public void GivenTheBookingManagerHasTheFollowingOccupiedPeriod(Table table)
    {
        var period = table.Rows.First();
        fullOccupiedPeriodStartDate = DateTime.Parse(period["Occupied Start Date"]);
        fullOccupiedPeriodEndDate = DateTime.Parse(period["Occupied End Date"]);
        
        var newBooking = new Booking { Id= 1, StartDate = fullOccupiedPeriodStartDate, EndDate = fullOccupiedPeriodEndDate, IsActive = true, RoomId = 1};
        var bookingsFake = fakeBookingRepository.Object.GetAll().ToList();
        bookingsFake.Add(newBooking);

        // Updating the fake booking rep
        fakeBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingsFake);

    }
    
    [When(@"the user attempts to create a booking with start date ""(.*)"" before full occupied period and end date ""(.*)"" in occupied period")]
    public void WhenTheUserAttemptsToCreateABookingWithStartDateBeforeFullOccupiedPeriodAndEndDateInOccupiedPeriod(string startDate, string endDate)
    {
        DateTime start = DateTime.Parse(startDate);
        DateTime slut = DateTime.Parse(endDate);

        int result = bookingManager.FindAvailableRoom(start, slut);
        
        _scenarioContext["BookingCreationResult"] = result;

    }

    [Then(@"the booking should not be created")]
    public void ThenTheBookingShouldNotBeCreated()
    {
        int result = (int)_scenarioContext["BookingCreationResult"];
        Assert.Equal(-1,result);
    }

   
}