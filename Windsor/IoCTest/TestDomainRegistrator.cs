using IoC;
using IoCTestDomain.Logger;
using IoCTestDomain.Processor;
using IoCTestDomain.Reporting;

namespace IoCTest
{
	[IoCRegistration]
	public class TestDomainRegistrator : IIoCRegistration
	{
		public void BuildContainer(IoCContainerCore container)
		{
			container.Register<IEntityProcessor, Implementation1>("Implementation1");
			container.Register<IEntityProcessor, Implementation1Child>("Implementation1Child");
			container.Register(typeof(IEntityProcessor), typeof(Implementation2), "Implementation2");
			container.Register<ILogger, Logger>();
			container.Register<ILogger, LoggerExtended>("LoggerExtended");

			container.Register<IReportBuilder, ReportBuilder>();
			container.Register<IReportSender, EmailReportSender>();
			container.Register<Reporter, Reporter>();
		}
	}
}