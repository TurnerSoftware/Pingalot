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
			var ExportFile = options.ExportFile; // https://github.com/TurnerSoftware/Pingalot/issues/6
			var pingSender = new Ping();
			var pingOptions = new PingOptions
			{
				Ttl = options.TimeTolive
			};

			if (ExportFile != null)
			{
				try
				{
					SetupExportFile(ExportFile);
				}
				catch
				{
					// something went wrong with using the provided export file path\filename - so lets setup one local to exe
					var fileNameDate = DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss");
					ExportFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\results_" + fileNameDate + ".csv";
					SetupExportFile(ExportFile);
				}

			}

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

				if (ExportFile != null)
				{
					WriteRecordToExportFile(ExportFile, pingRequest);
				}

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

		private void SetupExportFile(string exportFile)
		{
			// open file and write out the csv file headers - just once
			// we use append as file may already exist - thats ok still write to it
			using (var stream = File.Open(exportFile, FileMode.Append))
			using (var writer = new StreamWriter(stream))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteHeader<PingRequestExportModel>();
				csv.NextRecord();
			}
		}

		private void WriteRecordToExportFile(string exportFile, PingRequest pingRequest)
		{
			// write a single pingrequest record to export file
			using (var stream = File.Open(exportFile, FileMode.Append))
			using (var writer = new StreamWriter(stream))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				var singleExportablePingResult = new PingRequestExportModel(pingRequest);
				csv.WriteRecord(singleExportablePingResult);
				csv.NextRecord();
			}
		}
	}
}
