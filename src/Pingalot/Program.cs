using CommandLine;
using Pingalot.Layouts;
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


					PingStats session = null;

					if (pingArgs.Layout == Layout.Traditional)
					{
						session = await TraditionalPing.StartAsync(options);
					}
					else if (pingArgs.Layout == Layout.Modern)
					{
						session = await ModernPing.StartAsync(options);
					}

				});
		}
	}
}
