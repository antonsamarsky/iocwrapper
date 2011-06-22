using System.Collections.Generic;

namespace IoCTestDomain.Reporting
{
	/// <summary>
	/// The report builder interface.
	/// </summary>
	public interface IReportBuilder
	{
		/// <summary>
		/// Creates the reports.
		/// </summary>
		/// <returns>The collection of reports.</returns>
		IEnumerable<Report> CreateReports();
	}
}