namespace IoCTestDomain.Reporting
{
	/// <summary>
	/// The report sender.
	/// </summary>
	public interface IReportSender
	{
		/// <summary>
		/// Sends the specified report.
		/// </summary>
		/// <param name="report">The report.</param>
		void Send(Report report);
	}
}