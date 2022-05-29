using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingalot
{
	public class PingRequestOptions
	{
		public string Address { get; init; }
		public TimeSpan PingTimeout { get; init; }
		public int BufferSize { get; init; }
		public int TimeTolive { get; init; }
		public TimeSpan DelayBetweenPings { get; init; }
		public int NumberOfPings { get; init; }
		public string ExportFile { get; init; }
	}
}
