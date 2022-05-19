using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Pingalot
{
	public class PingSession
	{
		public DateTime StartTime { get; }
		public DateTime? EndTime { get; }
		public TimeSpan Elapsed { get; }
		public IReadOnlyList<PingRequest> Requests { get; }

		public PingSession(DateTime startTime, TimeSpan elapsed, IReadOnlyList<PingRequest> results)
		{
			StartTime = startTime;
			Elapsed = elapsed;
			Requests = results;

			CalculateStatistics();
		}

		public PingSession(DateTime startTime, DateTime endTime, TimeSpan elapsed, IReadOnlyList<PingRequest> results)
		{
			StartTime = startTime;
			EndTime = endTime;
			Elapsed = elapsed;
			Requests = results;

			CalculateStatistics();
		}

		private void CalculateStatistics()
		{
			PacketsSent = Requests.Count;
			double totalRoundtrip = 0;

			for (var i = 0; i < Requests.Count; i++)
			{
				var result = Requests[i];

				if (result.Status == IPStatus.Success)
				{
					PacketsReceived++;
					if (PacketsReceived == 1)
					{
						MinimumRoundtrip = result.RoundtripTime;
						MaximumRoundtrip = result.RoundtripTime;
					}
					else
					{
						MinimumRoundtrip = Math.Min(MinimumRoundtrip, result.RoundtripTime);
						MaximumRoundtrip = Math.Max(MaximumRoundtrip, result.RoundtripTime);
					}

					totalRoundtrip += result.RoundtripTime;
				}
			}

			if (PacketsSent > 0)
			{
				PacketsLost = PacketsSent - PacketsReceived;
				PacketsLostPercentage = (double)PacketsLost / PacketsSent * 100;
				PacketsLostPercentage = Math.Round(PacketsLostPercentage, 2);


				if (PacketsReceived > 0)
				{
					AverageRoundtrip = totalRoundtrip / PacketsReceived;
					AverageRoundtrip = Math.Round(AverageRoundtrip, 2);
				}
				else
				{
					AverageRoundtrip = 0;
				}
			}
		}

		public int PacketsSent { get; private set; }
		public int PacketsReceived { get; private set; }
		public int PacketsLost { get; private set; }
		public double PacketsLostPercentage { get; private set; }
		public long MinimumRoundtrip { get; private set; }
		public long MaximumRoundtrip { get; private set; }
		public double AverageRoundtrip { get; private set; }
	}
}