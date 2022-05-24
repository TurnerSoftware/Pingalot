using CommandLine;
using CsvHelper;
using Pingalot.Layouts;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pingalot
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await Parser.Default.ParseArguments<PingArguments>(args)
				.WithParsedAsync(async pingArgs =>
				{
					var options = new PingRequestOptions
					{
						Address = pingArgs.TargetName,
						BufferSize = pingArgs.BufferSize,
						DelayBetweenPings = pingArgs.BreakBetweenPings,
						PingTimeout = pingArgs.PingTimeout,
						TimeTolive = pingArgs.TimeToLive,
						NumberOfPings = pingArgs.PingUntilStopped ? -1 : pingArgs.NumberOfPings,
						ExportFile = pingArgs.ExportLocation
					};

					//TODO: Error on bad layout
					PingSession session = null;
					if (pingArgs.Layout == Layout.Traditional)
					{
						session = await TraditionalPing.StartAsync(options);
					}
					else if (pingArgs.Layout == Layout.Modern)
					{
						session = await ModernPing.StartAsync(options);
					}

					if (session != null)
					{
						if (!string.IsNullOrWhiteSpace(pingArgs.ExportLocation))
						{
							Console.WriteLine("Exporting results to {0}...", pingArgs.ExportLocation);
							using (var file = File.Open(pingArgs.ExportLocation, FileMode.Create))
							using (var writer = new StreamWriter(file))
							using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
							{
								await csv.WriteRecordsAsync(session.Requests.Select(r => new PingRequestExportModel(r)));
							}
						}
					}
				});
		}
	}
}
