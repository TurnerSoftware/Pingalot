using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Pingalot
{
	public class PingRequest
	{
		public IPAddress Address { get; init; }
		public IPStatus Status { get; init; }
		public long RoundtripTime { get; init; }
		public int TimeToLive { get; init; }
		public int BufferLength { get; init; }
		public bool HasMatchingBuffer { get; init; }
		public DateTime RequestTime { get; init; }
	}
}
