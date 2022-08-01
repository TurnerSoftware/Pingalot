using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;

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

			//if (options.ExportFileFullPath != null)
			//{
			//	var pingRequestFileExporter = new PingRequestFileExporter(options.ExportFileFullPath);
			//	PingCompleted += pingRequestFileExporter.onPingCompletedExportResultToFile;
			//}

			var buffer = CreateBuffer(options.BufferSize);
			var startTime = DateTime.Now;
			var pingSession = new PingSession(startTime);

			var timer = new Stopwatch();
			timer.Start();

			while (!cancellationToken.IsCancellationRequested && (options.NumberOfPings == -1 || pingSession.PacketsSent < options.NumberOfPings))
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


				PingCompleted?.Invoke(this, new PingCompletedEventArgs
				{
					CompletedPing = pingRequest,
					Session = pingSession
				});

				//if (ExportFile != null)
				//{
				//	WriteRecordToExportFile(ExportFile, pingRequest);
				//}

				pingSession.AddSinglePingResult(timer.Elapsed, pingRequest);

				try
				{
					await Task.Delay(options.DelayBetweenPings, cancellationToken);
				}
				catch { }
			}

			timer.Stop();
			var endTime = DateTime.Now;

			pingSession.CalculateFinalPingStats(endTime, timer.Elapsed);
			return pingSession;
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
