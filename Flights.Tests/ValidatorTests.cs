using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Common;
using Flights.Store;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace Flights.Tests
{
    [TestClass]
    public class ValidatorTests
    {
        private Validator _sut;
        private Mock<IFlightStore> _mockStore;
        private Mock<ILogger> _mockLogger;

        // Points of interest: 
        //   much easier to do thanks to injection
        //   can check results of output of function
        //   otherwise need to depend on mocks, which isn't great as it relies on implementation details
        //   mocking the TimerInfo object

        [TestInitialize]
        public void Init()
        {
            _mockStore = new Mock<IFlightStore>();
            _mockLogger = new Mock<ILogger>();
            _sut = new Validator(_mockStore.Object);
        }

        [TestMethod]
        public async Task GivenRevisedTime_WhenEarlierThanScheduled_ThenFlightIsInvalid()
        {
            var mockTime = new Mock<TimerSchedule>();
            var timer = new TimerInfo(mockTime.Object, new ScheduleStatus(), true);
            _mockStore
                .Setup(f => f.Get())
                .Returns(Task.FromResult(MockFlights(MockFlight("CYUL", "KLAX", 1566434251, 1566430000))));

            var actual = await _sut.Run(timer, _mockLogger.Object);

            Assert.AreEqual(1, actual.InvalidFlightIds.Count());
            Assert.AreEqual(1, actual.InvalidFlightIds.First());
        }

        [TestMethod]
        public async Task GivenRevisedTime_WhenDepartingAndArrivalMatch_ThenFlightIsInvalid()
        {
            var mockTime = new Mock<TimerSchedule>();
            var timer = new TimerInfo(mockTime.Object, new ScheduleStatus(), true);
            _mockStore
                .Setup(f => f.Get())
                .Returns(Task.FromResult(MockFlights(MockFlight("CYUL", "CYUL", 1566434251, 1566434351))));

            var actual = await _sut.Run(timer, _mockLogger.Object);

            Assert.AreEqual(1, actual.InvalidFlightIds.Count());
            Assert.AreEqual(1, actual.InvalidFlightIds.First());
        }
        private Flight MockFlight(
            string departingAirport,
            string arrivalAirport,
            long scheduled,
            long revised)
        {
            return new Flight(1, departingAirport, arrivalAirport, scheduled, revised);
        }

        private ImmutableList<Flight> MockFlights(params Flight[] flights)
        {
            return new List<Flight>(flights).ToImmutableList();
        }
    }
}
