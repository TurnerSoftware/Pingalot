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
		public void ConfirmPingSessionStatistics()
		{
			// Mock up PingRequests with specific values
			// Manually calculate the expected statistics 
			// Assert stats are correct

			var pingRequests = new List<PingRequest>();

			var startTime = DateTime.Now;
			TimeSpan duration = new System.TimeSpan(0, 0, 0, 30);
			var endTime = startTime.Add(duration);

			// Create 3 successful pings 10ms RT
			for (int i = 0; i < 3; i++)
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

			// Create 3 successful pings 5ms RT 
			for (int i = 0; i < 2; i++)
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
		}


		[TestMethod]
		public void TestPingStatisticsDivideByZero()
		{
			// https://github.com/TurnerSoftware/Pingalot/issues/10

			var pingRequests = new List<PingRequest>();

			var startTime = DateTime.Now;
			TimeSpan duration = new System.TimeSpan(0, 0, 0, 30);
			var endTime = startTime.Add(duration);


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

			var testPingSession = new PingSession(startTime, endTime, duration, pingRequests);

			Assert.IsTrue(testPingSession.AverageRoundtrip == 0, "If AverageRoundtrip is 0, we will attempt a divide by zero.");
		}
	}
}
