using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Pingalot
{
	public class PingSession
	{
		public DateTime StartTime { get; }
		public DateTime? EndTime { get; private set; }
		public TimeSpan Elapsed { get; private set; }

		public PingRequest PingResult { get; private set; }

		public int PacketsSent { get; private set; }
		public int PacketsReceived { get; private set; }
		public int PacketsLost { get; private set; }
		public double PacketsLostPercentage { get; private set; }
		public long MinimumRoundtrip { get; private set; }
		public long MaximumRoundtrip { get; private set; }
		public long TotalRoundtrip { get; private set; }
		public double AverageRoundtrip { get; private set; }

		public PingSession(DateTime startTime)
		{
			StartTime = startTime;
			PacketsSent = 0;
			PacketsReceived = 0;
			PacketsLost = 0;
			PacketsLostPercentage = 0D;
			MinimumRoundtrip = 0L;
			MaximumRoundtrip = 0L;
			TotalRoundtrip = 0L;
			AverageRoundtrip = 0D;
		}

		public void AddSinglePingResult(TimeSpan elapsed, PingRequest pingResult)
		{
			Elapsed = elapsed;
			PingResult = pingResult;

			PacketsSent++;

			if (PingResult.Status == IPStatus.Success)
			{
				PacketsReceived++;

				if (PacketsReceived == 1)
				{
					MinimumRoundtrip = PingResult.RoundtripTime;
					MaximumRoundtrip = PingResult.RoundtripTime;
				}
				else
				{
					MinimumRoundtrip = Math.Min(MinimumRoundtrip, PingResult.RoundtripTime);
					MaximumRoundtrip = Math.Max(MaximumRoundtrip, PingResult.RoundtripTime);
				}

				TotalRoundtrip += PingResult.RoundtripTime;
			}

			if (PacketsSent > 0)
			{
				PacketsLost = PacketsSent - PacketsReceived;
				PacketsLostPercentage = (double)PacketsLost / PacketsSent * 100;
				PacketsLostPercentage = Math.Round(PacketsLostPercentage, 2);


				if (PacketsReceived > 0)
				{
					AverageRoundtrip = TotalRoundtrip / PacketsReceived;
					AverageRoundtrip = Math.Round(AverageRoundtrip, 2);
				}
				else
				{
					AverageRoundtrip = 0;
				}
			}
		}

		public void CalculateFinalPingStats(DateTime endTime, TimeSpan elapsed)
		{
			EndTime = endTime;
			Elapsed = elapsed;
		}

	}
}