using Homies.Data;
using Homies.Data.Models;
using Homies.Models.Event;
using Homies.Services;
using Microsoft.EntityFrameworkCore;

namespace Homies.Tests
{
    [TestFixture]
    internal class EventServiceTests
    {
        private HomiesDbContext _dbContext;
        private EventService _eventService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HomiesDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique database name to avoid conflicts
                .Options;
            _dbContext = new HomiesDbContext(options);

            _eventService = new EventService(_dbContext);
        }

        [Test]
        public async Task AddEventAsync_ShouldAddEvent_WhenValidEventModelAndUserId()
        {
            // Step 1: Arrange - Set up the initial conditions for the test
            // Create a new event model with test data

            var eventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2)
            };

            // Define a user ID for testing purposes

            string userId = "testUserId";

            // Step 2: Act - Perform the action being tested
            // Call the service method to add the event

            await _eventService.AddEventAsync(eventModel, userId);

            // Step 3: Assert - Verify the outcome of the action
            // Retrieve the added event from the database
            var eventInTheDataBase = await _dbContext.Events.FirstOrDefaultAsync(x => x.Name == eventModel.Name && x.OrganiserId == userId);


            // Assert that the added event is not null, indicating it was successfully added

            Assert.IsNotNull(eventInTheDataBase);

            // Assert that the description of the added event matches the description provided in the event model
            Assert.AreEqual(eventInTheDataBase.Description, eventModel.Description);
            Assert.That(eventInTheDataBase.Start, Is.EqualTo(eventModel.Start));
            Assert.That(eventInTheDataBase.End, Is.EqualTo(eventModel.End));
        }


        [Test]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            // Step 1: Arrange - Set up the initial conditions for the test
            // Create two event models with test data
            var firstEventModel = new EventFormModel
            {
                Name = "First new Event",
                Description = "new Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(7)
            };

            var secondEventModel = new EventFormModel
            {
                Name = "Second Test Event",
                Description = "Description",
                Start = DateTime.Now.AddDays(2),
                End = DateTime.Now.AddDays(3)
            };


            // Define a user ID for testing purposes
            string userId = "secondTestUserId";


            // Step 2: Act - Perform the action being tested
            // Add the two events to the database using the event service

            await _eventService.AddEventAsync(firstEventModel, userId);
            await _eventService.AddEventAsync(secondEventModel, userId);

            var result = await _dbContext.Events.CountAsync();


            // Assert that the count of events in the database is equal to the expected count (2)
           
            Assert.That(result, Is.EqualTo(2));


        }

        [Test]
        public async Task GetEventDetailsAsync_ShouldReturnAllEventDetails()
        {
            //Arrange
            var firstEventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = 2,

            };

            await _eventService.AddEventAsync(firstEventModel, "1");

            var eventInTheDb = await _dbContext.Events.FirstAsync();

            //Act

            var result = await _eventService.GetEventDetailsAsync(eventInTheDb.Id);

            //Assert

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo(firstEventModel.Name));
            Assert.That(result.Description, Is.EqualTo(firstEventModel.Description));
            //I should add aditional prop checks

        }

        [Test]
        public async Task GetEventForEditAsync_ShouldGetEventIfPresentInTheDb()
        {
            //Arrange
            var firstEventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = 2,

            };

            await _eventService.AddEventAsync(firstEventModel, "1");

            var eventInTheDb = await _dbContext.Events.FirstAsync();

            //Act
            var result = await _eventService.GetEventForEditAsync(eventInTheDb.Id);


            //Assert
            Assert.NotNull(result);

            Assert.AreEqual(firstEventModel.Description, result.Description);
            Assert.AreEqual(firstEventModel.Name, result.Name);
            Assert.AreEqual(firstEventModel.Start, result.Start);
            Assert.AreEqual(firstEventModel.End, result.End);

        }


        [Test]
        public async Task GetEventForEditAsync_ShouldReturnNullIfEventNotFound()
        {
            //Act
            var result = await _eventService.GetEventForEditAsync(23);
            //Assert
            Assert.Null(result);
        }

    }
}
