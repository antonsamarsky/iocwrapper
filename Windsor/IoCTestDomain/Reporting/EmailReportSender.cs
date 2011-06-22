using System;

namespace IoCTestDomain.Reporting
{
	/// <summary>
	/// The e-mail report sender.
	/// </summary>
	public class EmailReportSender : IReportSender
	{
		/// <summary>
		/// Sends the specified report by e-mail.
		/// </summary>
		/// <param name="report">The report.</param>
		public void Send(Report report)
		{
			Console.WriteLine("EmailReportSender: Report Id: " + report.Id  + "; Name: " + report.Name);
		}
	}
}