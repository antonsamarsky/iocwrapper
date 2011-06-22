using System.Linq;

namespace IoCTestDomain.Reporting
{
	/// <summary>
	/// The reporter implementation class that sends the reports.
	/// </summary>
	public class Reporter
	{
		/// <summary>
		/// The report builder dependency.
		/// </summary>
		private readonly IReportBuilder reportBuilder;

		/// <summary>
		/// The report sender dependency.
		/// </summary>
		private readonly IReportSender reportSender;

		/// <summary>
		/// Initializes a new instance of the <see cref="Reporter"/> class.
		/// </summary>
		/// <param name="reportBuilder">The report builder.</param>
		/// <param name="reportSender">The report sender.</param>
		public Reporter(IReportBuilder reportBuilder, IReportSender reportSender)
		{
			this.reportBuilder = reportBuilder;
			this.reportSender = reportSender;
		}

		/// <summary>
		/// Sends the reports.
		/// </summary>
		public void SendReports()
		{
			var reportsCollection = this.reportBuilder.CreateReports();
			if (!reportsCollection.Any())
			{
				throw new NoReportsException();
			}

			foreach (var report in reportsCollection)
			{
				this.reportSender.Send(report);
			}
		}

		/*
		/// <summary>
		/// Sends the reports.
		/// Asks report builder to create a list of reports and send it by mail.
		/// </summary>
		public void SendReports()
		{
			var reportBuilder = new ReportBuilder();
			var reportsCollection = reportBuilder.CreateReports();

			if (!reportsCollection.Any())
			{
				throw new NoReportsException();
			}

			var reportSender = new EmailReportSender();
			foreach (Report report in reportsCollection)
			{
				reportSender.Send(report);
			}
		}
		*/
	}
}