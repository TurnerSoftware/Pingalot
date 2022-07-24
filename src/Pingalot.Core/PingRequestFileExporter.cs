using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pingalot
{
	public class PingRequestFileExporter
	{

		public void onPingCompleted(object sender, PingCompletedEventArgs e)
		{
			Console.WriteLine("ExportFIle Event works!");

		}

	}
}
