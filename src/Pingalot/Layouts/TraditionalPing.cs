using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Pingalot.Layouts
{
	public static class TraditionalPing
	{
		public static async Task<PingSession> StartAsync(PingRequestOptions options)
		{
			var cancellationTokenSource = new CancellationTokenSource();

			Console.CancelKeyPress += (sender, e) =>
			{
				e.Cancel = true;
				cancellationTokenSource.Cancel();
			};

			var pingRequestAgent = new PingRequestAgent();

			pingRequestAgent.PingCompleted += (sender, e) =>
			{
				var result = e.CompletedPing;
				if (result.Status == IPStatus.Success)
				{
					Console.WriteLine("Reply from {0}: bytes={1} time={2}ms TTL={3} when={4:yyyy-MM-ddTHH:mm:ss}", result.Address.ToString(), result.BufferLength, result.RoundtripTime, result.TimeToLive, result.RequestTime);
				}
				else
				{
					Console.WriteLine(result.Status.ToString());
				}

			};

			Console.WriteLine();
			Console.WriteLine("Pinging {0} with {1} bytes of data:", options.Address, options.BufferSize);

			var results = await pingRequestAgent.StartAsync(options, cancellationTokenSource.Token);

			if (results.PacketsSent > 0)
			{
				Console.WriteLine();
				Console.WriteLine("Ping stats for {0}", options.Address);
				Console.WriteLine("    Packets: Sent = {0}, Received = {1}, Lost = {2} ({3:0}% loss),", results.PacketsSent, results.PacketsReceived, results.PacketsLost, results.PacketsLostPercentage);
				Console.WriteLine("    Time: Start = {0:yyyy-MM-ddTHH:mm:ss}, End = {1:yyyy-MM-ddTHH:mm:ss}, Elapsed = {2:0}s", results.StartTime, results.EndTime, results.Elapsed.TotalSeconds);

				if (results.PacketsReceived > 0)
				{
					Console.WriteLine("Approximate round trip times in milli-seconds:");
					Console.WriteLine("    Minimum = {0}ms, Maximum = {1}ms, Average = {2:0}ms", results.MinimumRoundtrip, results.MaximumRoundtrip, results.AverageRoundtrip);
				}
			}

			Console.WriteLine();

			return results;
		}
	}
}
