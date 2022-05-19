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
