using HotelBooking.Core;
using Moq;
using Xunit;

namespace HotelBooking.SpecFlow.Steps;

[Binding]
public class CreateBookingStepsDefinition
{
    private readonly ScenarioContext scenarioContext;
    private IBookingManager bookingManager;
    private Mock<IRepository<Booking>> fakeBookingRepository;
    private Mock<IRepository<Room>> fakeRoomRepository;

    public CreateBookingStepsDefinition(
        ScenarioContext scenarioContext)
    {
        this.scenarioContext = scenarioContext;
        
        fakeBookingRepository = new Mock<IRepository<Booking>>();
        fakeRoomRepository = new Mock<IRepository<Room>>();
    }

    [Given(@"the fully occupied period from day (.*) to day (.*)")]
    public void GivenTheStartDateAndEndDateIsBeforeTheFullyOccupiedPeriod(int a, int b)
    {
        var roomsFake = new List<Room>
        {
            new Room { Id = 1, Description = "Room 1" }
        };
        
        var bookingsFake = new List<Booking>
        {
            new Booking { Id = 1, StartDate = DateTime.Today.AddDays(a), EndDate = DateTime.Today.AddDays(b), IsActive = true, RoomId = 1 }
        }; 
        
        fakeBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingsFake);
        fakeBookingRepository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
            bookingsFake.FirstOrDefault(booking => booking.Id == id));
        
        fakeRoomRepository.Setup(x => x.GetAll()).Returns(() => roomsFake);
        fakeRoomRepository.Setup(x => x.Get(It.IsAny<int>())).Returns((int id) => 
            roomsFake.FirstOrDefault(room => room.Id == id));
        
        bookingManager = new BookingManager(fakeBookingRepository.Object, fakeRoomRepository.Object);
    }

    [When(@"the user attempts to create a booking with start date (.*) and end date (.*)")]
    public void WhenTheUserAttemptsToCreateABookingWithStartDateAndEndDate(int a, int b)
    {
        var bookingUser = new Booking { Id = 1, StartDate = DateTime.Today.AddDays(a), EndDate = DateTime.Today.AddDays(b), IsActive = true, RoomId = 1 };
            
        scenarioContext["BookingResult"] = bookingManager.CreateBooking(bookingUser);
    }

    [Then(@"the booking should be created successfully")]
    public void ThenTheBookingShouldBeCreatedSuccessfully()
    {
        var bookingResult = (bool)scenarioContext["BookingResult"];
        Assert.True(bookingResult);
    }

    [Then(@"the booking should be rejected")]
    public void ThenTheBookingShouldBeRejected()
    {
        var bookingResult = (bool)scenarioContext["BookingResult"];
        Assert.False(bookingResult);
    }
}