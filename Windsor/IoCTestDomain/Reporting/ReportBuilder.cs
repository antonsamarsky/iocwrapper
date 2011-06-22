using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IoCTestDomain.Reporting
{
	/// <summary>
	/// The report builder implementation.
	/// </summary>
	public class ReportBuilder : IReportBuilder
	{
		/// <summary>
		/// Creates the reports.
		/// </summary>
		/// <returns>
		/// The collection of reports.
		/// </returns>
		public IEnumerable<Report> CreateReports()
		{
			Trace.WriteLine("ReportBuilder: CreateReports");

			return Enumerable.Range(0, 10).Select(i => new Report { Id = i, Name = "Name_" + i });
		}
	}
}