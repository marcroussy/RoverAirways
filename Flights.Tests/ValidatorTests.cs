using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Common;
using Flights.Store;
using Flights.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Flights.Tests
{
    [TestClass]
    public class ValidatorTests
    {
        private Validator _sut;
        private Mock<IFlightStore> _mockStore;
        private Mock<ILogger> _mockLogger;
        private Mock<IWarningGenerator> _mockWarnings;

        [TestInitialize]
        public void Init()
        {
            _mockStore = new Mock<IFlightStore>();
            _mockLogger = new Mock<ILogger>();
            _mockWarnings = new Mock<IWarningGenerator>();
            _sut = new Validator(_mockStore.Object, _mockWarnings.Object);
        }

        [TestMethod]
        public void GivenRevisedTime_EarlierThanScheduled_LogWarning()
        {
            var mockTime = new Mock<TimerSchedule>();
            var timer = new TimerInfo(mockTime.Object, new ScheduleStatus(), true);
            _mockStore
                .Setup(f => f.Get())
                .Returns(Task.FromResult(MockFlights()));

            _mockWarnings
                .Setup(w => w.Send());

            _sut.Run(timer, _mockLogger.Object);

            _mockWarnings
                .Verify(w => w.Send());
        }

        private ImmutableList<Flight> MockFlights()
        {
            return new List<Flight>()
            {
                 new Flight(
                     1, 
                     DateTimeOffset.UtcNow.ToString(), 
                     DateTimeOffset.UtcNow.ToString(),
                     "Airbus A380",
                     1000,
                     999)
            }.ToImmutableList();
        }
    }
}
