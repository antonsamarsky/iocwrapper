using System;
using System.Collections.Generic;
using System.Linq;
using IoC;
using Moq;
using NUnit.Framework;

namespace IoCTestDomain.Reporting.Tests
{
	/// <summary>
	/// The reporter tests.
	/// </summary>
	[TestFixture]
	public class ReporterTests
	{
		/// <summary>
		/// Fixtures the set up.
		/// </summary>
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			IoCContainer.InitializeContainer();
			IoCContainer.Register<IReportBuilder, ReportBuilder>();
			IoCContainer.Register<IReportSender, EmailReportSender>();
			IoCContainer.Register<Reporter, Reporter>();
		}

		[Test]
		public void SendReportsOnRealObjectsTest()
		{
			var reporter = IoCContainer.Resolve<Reporter>();
			reporter.SendReports();
		}

		[Test]
		public void SendReportsHowToTest()
		{
			// What can I check here?
			// var reporter = new Reporter();
			// reporter.SendReports();
		}

		[Test]
		public void SendReportsTest()
		{
			var report = new Mock<Report>();

			var builder = new Mock<IReportBuilder>();
			builder.Setup(m => m.CreateReports()).Returns(new List<Report> { report.Object });

			var sender = new Mock<IReportSender>();

			var reporter = new Reporter(builder.Object, sender.Object);
			reporter.SendReports();

			sender.Verify(rs => rs.Send(report.Object), Times.Once());
		}

		[TestCase(1000)]
		public void SendManyReportsTest(int numberOfReports)
		{
			var reports = Enumerable.Range(1, numberOfReports).Select(index => new Report { Id = index, Name = "report_name_" + index }).ToList();

			var builder = new Mock<IReportBuilder>();
			builder.Setup(m => m.CreateReports()).Returns(reports);

			var sender = new Mock<IReportSender>();

			var reporter = new Reporter(builder.Object, sender.Object);
			reporter.SendReports();

			reports.ForEach(report => sender.Verify(rs => rs.Send(report), Times.Once()));
		}

		[Test]
		public void SendReportsTestNotReports()
		{
			var builder = new Mock<IReportBuilder>();
			builder.Setup(m => m.CreateReports()).Returns(new List<Report>());

			var sender = new Mock<IReportSender>();

			var reporter = new Reporter(builder.Object, sender.Object);

			Assert.Throws<NoReportsException>(reporter.SendReports);
		}
	}
}
