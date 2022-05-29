using Spectre.Console;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Pingalot.Layouts
{
	public static class ModernPing
	{
		public static async Task<PingSession> StartAsync(PingRequestOptions options)
		{
			var pingRequestAgent = new PingRequestAgent(options);
			var cancellationTokenSource = new CancellationTokenSource();

			Console.CancelKeyPress += (sender, e) =>
			{
				e.Cancel = true;
				cancellationTokenSource.Cancel();
			};

			PingSession results = null;

			if (options.NumberOfPings != -1)
			{
				await AnsiConsole.Progress()
					.Columns(new ProgressColumn[]
					{
						new TaskDescriptionColumn(),
						new ProgressBarColumn(),
						new PercentageColumn(),
						new RemainingTimeColumn()
					})
					.StartAsync(async ctx =>
					{
						var requestsRemaining = ctx.AddTask($"Sending {options.NumberOfPings} pings to [yellow]{options.Address}[/]", new ProgressTaskSettings
						{
							MaxValue = options.NumberOfPings
						});
						pingRequestAgent.PingCompleted += (sender, e) =>
						{
							requestsRemaining.Increment(1);
						};

						results = await pingRequestAgent.StartAsync(cancellationTokenSource.Token);
					});
			}
			else
			{
				await AnsiConsole.Status()
					.Spinner(Spinner.Known.Dots8Bit)
					.StartAsync($"Pinging {options.Address}...", async ctx =>
					{
						pingRequestAgent.PingCompleted += (sender, e) =>
						{
							if (e.CompletedPing.Status != IPStatus.Success)
							{
								AnsiConsole.MarkupLine("[grey54]{0:yyyy-MM-ddTHH:mm:ss}: {1}[/]", e.CompletedPing.RequestTime, e.CompletedPing.Status);
							}

							var packetsLostColour = "grey54";
							if (e.Session.PacketsLostPercentage > 5)
							{
								packetsLostColour = "red";
							}
							else if (Math.Round(e.Session.PacketsLostPercentage, 2) > 0)
							{
								packetsLostColour = "maroon";
							}

							ctx.Status($"Continuously pinging [yellow]{options.Address}[/] [grey54]({e.Session.PacketsSent} sent, [{packetsLostColour}]{e.Session.PacketsLostPercentage:0.00}% lost[/], {e.Session.AverageRoundtrip}ms average, {(int)e.Session.Elapsed.TotalMinutes}:{e.Session.Elapsed.Seconds:00} elapsed)[/]");
						};

						results = await pingRequestAgent.StartAsync(cancellationTokenSource.Token);
					});
			}
			

			if (results != null && results.PacketsSent > 0)
			{
				AnsiConsole.WriteLine();
				AnsiConsole.Render(new Rule($"[white]Ping results for [yellow]{options.Address}[/][/]").RuleStyle("grey54"));
				AnsiConsole.WriteLine();

				var table = new Table()
					.Centered()
					.AddColumns(
						new TableColumn("Packets (Sent/Received/Lost)").Centered(), 
						new TableColumn("Minimum Roundtrip").Centered(), 
						new TableColumn("Maximum Roundtrip").Centered(), 
						new TableColumn("Average Roundtrip").Centered(), 
						new TableColumn("Elapsed Time").Centered()
					)
					.SimpleBorder();

				table.AddRow(
					$"{results.PacketsSent} / {results.PacketsReceived} / {results.PacketsLost}", 
					results.MinimumRoundtrip.ToString("0ms"), 
					results.MaximumRoundtrip.ToString("0ms"), 
					results.AverageRoundtrip.ToString("0ms"), 
					$"{(int)results.Elapsed.TotalMinutes}:{results.Elapsed.Seconds:00}"
				);

				AnsiConsole.Render(table);
			}
			else
			{
				AnsiConsole.WriteLine("No results available.");
			}

			AnsiConsole.WriteLine();

			return results;
		}
	}
}
