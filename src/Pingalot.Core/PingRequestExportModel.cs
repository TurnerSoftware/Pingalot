using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingalot
{
	public class PingRequestExportModel 
	{
		public string Address { get; set; }
		public string Status { get; set; }
		public long RoundtripTime { get; set; }
		public int TimeToLive { get; set; }
		public int BufferLength { get; set; }
		public bool HasMatchingBuffer { get; set; }
		public string RequestTime { get; set; }

		public PingRequestExportModel() { }

		public PingRequestExportModel(PingRequest pingRequest)
		{
			Address = pingRequest.Address.ToString();
			Status = pingRequest.Status.ToString();
			RoundtripTime = pingRequest.RoundtripTime;
			TimeToLive = pingRequest.TimeToLive;
			BufferLength = pingRequest.BufferLength;
			HasMatchingBuffer = pingRequest.HasMatchingBuffer;
			RequestTime = pingRequest.RequestTime.ToString("O");
		}
	}
}
