using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.NetworkInformation;
using System.Net;
using System.Collections.Generic;

namespace Pingalot.Core.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void ConfirmPingSessionStatistics()
		{
			// One good place to start though would be when you have a PR up for your PingStats changes is testing that it does what we expect. If we pass in ping results of different states, do the statistics add up correctly.

			var pingRequests = new List<PingRequest>();

			var startTime = DateTime.Now;
			TimeSpan duration = new System.TimeSpan(0, 0, 0, 30);
			var endTime = startTime.Add(duration);

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
			// found a bug where if the ping fails on first ping then we attempt to divide by zero in ping statistics
			// need to cover this specific scenario with a test and possibly other divide by zero scenarios
			// to test simply ping an IP that never responds e.g 56.56.56.56
			// if packets received is zero then PingSession.cs:line 65 produces divide by zero error
			// 				AverageRoundtrip = totalRoundtrip / PacketsReceived;

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

			// assert that AverageRoundtrip = 0
		}
	}
}
