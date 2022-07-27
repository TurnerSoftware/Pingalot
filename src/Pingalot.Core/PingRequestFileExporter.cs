using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace Pingalot
{
	public class PingRequestFileExporter 
	{
		private FileStream stream;
		private StreamWriter writer;
		private CsvWriter csv;

		public PingRequestFileExporter(string ExportFileFullPath)
		{

			stream = File.Open(ExportFileFullPath, FileMode.Append,FileAccess.Write, FileShare.Read);
			writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
			csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
			csv.WriteHeader<PingRequestExportModel>();
			csv.NextRecord();

		}

		public void onPingCompletedExportResultToFile(object sender, PingCompletedEventArgs pingCompletedEventArgs)
		{
			// write a single pingrequest record to export file
			var singleExportablePingResult = new PingRequestExportModel(pingCompletedEventArgs.CompletedPing);
			csv.WriteRecord(singleExportablePingResult);
			csv.Flush();
			csv.NextRecord();
		}

		//public void Dispose()
		//{
		//	csv.Dispose();
		//	writer.Dispose();
		//	stream.Dispose();
		//}

		//~PingRequestFileExporter()
		//{
		//	this.Dispose();
		//}
	}
}
