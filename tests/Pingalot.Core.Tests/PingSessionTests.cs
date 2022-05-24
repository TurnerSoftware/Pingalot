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

			var startTime = DateTime.Now;
			TimeSpan duration = new System.TimeSpan(0, 0, 0, 30);
			var endTime = startTime.Add(duration);

			// Setup a single failed ping(TimedOut)
			var pingRequest = new PingRequest
			{
				Address = IPAddress.Loopback,
				Status = IPStatus.TimedOut,
				RoundtripTime = 0,
				TimeToLive = 0,
				BufferLength = 0,
				HasMatchingBuffer = false,
				RequestTime = DateTime.Now
			};

			pingRequests.Add(pingRequest);

			var testPingSession = new PingSession(startTime, endTime, duration, pingRequests);

			// When we have a single failed ping request - our AverageRoundtrip should be 0
			Assert.AreEqual(testPingSession.AverageRoundtrip, 0, "If PacketsReceived is 0, when we attempt to calculate AverageRoundTrip we may trigger a divide by zero exception.");
		}



		[TestMethod]
		public void ConfirmPingSessionStatistics()
		{
			// Mock up PingRequests with specific values
			// Manually calculate the expected statistics 
			// Assert stats are correct

			var pingRequests = new List<PingRequest>();

			var startTime = DateTime.Now;
			TimeSpan duration = new System.TimeSpan(0, 0, 0, 30);
			var endTime = startTime.Add(duration);

			// Create 6 successful pings 10ms RT
			for (int i = 0; i < 6; i++)
			{
				var pingRequest = new PingRequest
				{
					Address = new IPAddress(16843009),
					Status = IPStatus.Success,
					RoundtripTime = 10,
					TimeToLive = 58,
					BufferLength = 32,
					HasMatchingBuffer = true,
					RequestTime = DateTime.Now
				};

				pingRequests.Add(pingRequest);
			}

			// Create 2 successful pings 5ms RT 
			for (int i = 0; i < 3; i++)
			{
				var pingRequest = new PingRequest
				{
					Address = new IPAddress(16843009),
					Status = IPStatus.Success,
					RoundtripTime = 5,
					TimeToLive = 58,
					BufferLength = 32,
					HasMatchingBuffer = true,
					RequestTime = DateTime.Now
				};

				pingRequests.Add(pingRequest);
			}

			// Create 2 TimedOut pings
			for (int i = 0; i < 2; i++)
			{
				var pingRequest = new PingRequest
				{
					Address = new IPAddress(16843009),
					Status = IPStatus.TimedOut,
					RoundtripTime = 0,
					TimeToLive = 0,
					BufferLength = 0,
					HasMatchingBuffer = false,
					RequestTime = DateTime.Now
				};

				pingRequests.Add(pingRequest);
			}

			var testPingSession = new PingSession(startTime, endTime, duration, pingRequests);

			// assert that stats add up properly - calc them manually and not via debug
			Assert.IsTrue(testPingSession.PacketsReceived == 9, "PacketsReceived should be 9.");
			Assert.IsTrue(testPingSession.PacketsSent == 11, "PacketsSent should be 11.");
			Assert.IsTrue(testPingSession.PacketsLost == 2, "PacketsLost should be 2.");
			Assert.IsTrue(testPingSession.PacketsLostPercentage == 18.18, "PacketsLostPercentage should be 18.18%");
			Assert.IsTrue(testPingSession.MinimumRoundtrip == 5, "MinimumRoundtrip should be 5");
			Assert.IsTrue(testPingSession.MaximumRoundtrip == 10, "MinimumRoundtrip should be 10");
			Assert.IsTrue(testPingSession.AverageRoundtrip == 8.33, "AverageRoundtrip should equal 75 divided by 9 = 8.33 rounded to two decimal places.");
		}
	}
}
