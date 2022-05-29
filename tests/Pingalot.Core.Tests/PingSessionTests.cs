using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.NetworkInformation;
using System.Net;
using System.Collections.Generic;

namespace Pingalot.Core.Tests
{
	[TestClass]
	public class PingSessionTests
	{
		[TestMethod]
		public void TestPingStatisticsDivideByZero()
		{
			// https://github.com/TurnerSoftware/Pingalot/issues/10

			var pingRequests = new List<PingRequest>();

			var startTime = new DateTime(2022, 5, 19, 14, 30, 0);
			var duration = new TimeSpan(0, 0, 0, 30);

			var testPingSession = new PingSession(startTime);

			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.TimedOut,0) );

			// When we have a single failed ping request - our AverageRoundtrip should be 0
			Assert.AreEqual(testPingSession.AverageRoundtrip, 0, "If PacketsReceived is 0, when we attempt to calculate AverageRoundTrip we may trigger a divide by zero exception.");
		}

		[TestMethod]
		public void ConfirmPingSessionStatistics()
		{
			var startTime = new DateTime(2022, 5, 19, 14, 30, 0);
			var duration = new TimeSpan(0, 0, 0, 30);

			var testPingSession = new PingSession(startTime);

			// Create and add 6 successful pings 10ms RT
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 10));
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 10));
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 10));
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 10));
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 10));
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 10));

			// Create and add 2 successful pings 5ms RT 
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 5));
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.Success, 5));

			// Create and add 2 TimedOut pings
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.TimedOut, 0));
			testPingSession.AddSinglePingResult(duration, CreateTestPingResult(IPStatus.TimedOut, 0));

			// assert that stats add up properly - calc them manually and not via debug
			Assert.AreEqual(testPingSession.PacketsReceived, 9, "PacketsReceived should be 9.");
			Assert.AreEqual(testPingSession.PacketsSent, 11, "PacketsSent should be 11.");
			Assert.AreEqual(testPingSession.PacketsLost, 2, "PacketsLost should be 2.");
			Assert.AreEqual(testPingSession.PacketsLostPercentage, 18.18, "PacketsLostPercentage should be 18.18%");
			Assert.AreEqual(testPingSession.MinimumRoundtrip, 5, "MinimumRoundtrip should be 5");
			Assert.AreEqual(testPingSession.MaximumRoundtrip, 10, "MinimumRoundtrip should be 10");
			Assert.AreEqual(testPingSession.AverageRoundtrip, 8.33, "AverageRoundtrip should equal 75 divided by 9 = 8.33 rounded to two decimal places.");
		}


		private static PingRequest CreateTestPingResult(IPStatus status, long roundtripTime)
		{

			var newPingRequest = new PingRequest
			{
				Address = IPAddress.Loopback,
				Status = status,
				RoundtripTime = roundtripTime,
				TimeToLive = (status == IPStatus.Success) ? 60 : 0,
				BufferLength = (status == IPStatus.Success) ? 32 : 0,
				HasMatchingBuffer = (status == IPStatus.Success),
				RequestTime = null
			};

			return newPingRequest;
		}
	}
}
