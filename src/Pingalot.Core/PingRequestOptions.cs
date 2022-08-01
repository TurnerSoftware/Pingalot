using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

		public string? ExportFileFullPath { get; set; }

		public static string? SetExportFile(string ExportFileFullPath, bool UseExportFileDefault)
		{
			// if provided an export path then lets use it - it takes preference even if -e is set
			// error check file path here...
			if (ExportFileFullPath != null)
			{
				return ExportFileFullPath;
			}

			// no export path was provided, is -e set?
			if (UseExportFileDefault)
			{
				var fileNameDateTime = DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss");
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\results_" + fileNameDateTime + ".csv";
			}

			// --export not specified and -e not set,  this will return null - we dont want to export at all
			return ExportFileFullPath;
		}
	}
}
