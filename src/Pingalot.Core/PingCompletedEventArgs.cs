namespace Pingalot
{
	public delegate void PingCompletedEventHandler(object sender, PingCompletedEventArgs e);

	public class PingCompletedEventArgs
	{
		public PingRequest CompletedPing { get; init; }
        public PingStats PingStatsSession { get; init; }
	}
}