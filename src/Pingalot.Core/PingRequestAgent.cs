using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Pingalot
{
	public class PingRequestAgent
	{
		public event PingCompletedEventHandler PingCompleted;

		public async Task<PingSession> StartAsync(PingRequestOptions options, CancellationToken cancellationToken)
		{
			var pingSender = new Ping();
			var pingOptions = new PingOptions
			{
				Ttl = options.TimeTolive
			};
			var pingRequests = new List<PingRequest>();

			var buffer = CreateBuffer(options.BufferSize);

			var startTime = DateTime.Now;
			var timer = new Stopwatch();
			timer.Start();

			while (!cancellationToken.IsCancellationRequested && (options.NumberOfPings == -1 || pingRequests.Count < options.NumberOfPings))
			{
				var requestTime = DateTime.Now;
				var pingReply = await pingSender.SendPingAsync(options.Address, (int)options.PingTimeout.TotalMilliseconds, buffer, pingOptions);
				var pingRequest = new PingRequest
				{
					Address = pingReply.Address,
					Status = pingReply.Status,
					RoundtripTime = pingReply.RoundtripTime,
					TimeToLive = pingReply.Options?.Ttl ?? 0,
					BufferLength = pingReply.Buffer.Length,
					HasMatchingBuffer = CheckBuffer(buffer, pingReply.Buffer),
					RequestTime = requestTime
				};

				pingRequests.Add(pingRequest);

				var partialSession = new PingSession(startTime, timer.Elapsed, pingRequests);
				PingCompleted?.Invoke(this, new PingCompletedEventArgs
				{
					CompletedPing = pingRequest,
					Session = partialSession
				});

				try
				{
					await Task.Delay(options.DelayBetweenPings, cancellationToken);
				}
				catch { }
			}

			timer.Stop();
			var endTime = DateTime.Now;

			return new PingSession(startTime, endTime, timer.Elapsed, pingRequests);
		}

		private static byte[] CreateBuffer(int size)
		{
			var buffer = new byte[size];
			for (var i = 0; i < size; i++)
			{
				buffer[i] = (byte)'a';
			}
			return buffer;
		}

		private static bool CheckBuffer(byte[] expected, byte[] actual)
		{
			if (actual == null)
			{
				return false;
			}

			if (expected.Length != actual.Length)
			{
				return false;
			}

			for (var i = 0; i < expected.Length; i++)
			{
				if (expected[i] != actual[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}
