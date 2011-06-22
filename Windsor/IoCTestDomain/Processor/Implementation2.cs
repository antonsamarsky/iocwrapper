using System;
using System.Collections.Generic;
using System.Linq;
using Diagnostics;
using IoCTestDomain.Logger;
using IoCTestDomain.Reporting;

namespace IoCTestDomain.Processor
{
	public class Implementation2 : IEntityProcessor
	{
		public ILogger Logger { get; set; }

		public Implementation2(string constructorVariable)
		{
			Assert.ArgumentNotNullOrEmpty(constructorVariable, "constructorVariable");
			Console.WriteLine(constructorVariable);
		}

		public Implementation2(string constructorVariable, ILogger logger)
		{
			Assert.ArgumentNotNullOrEmpty(constructorVariable, "constructorVariable");
			Assert.ArgumentNotNull(logger, "logger");

			this.Logger = logger;
			Console.WriteLine(constructorVariable);
		}

		public string ClassName
		{
			get { return "Implementation2"; }
		}

		public void Do(string someData)
		{
			Console.WriteLine(ClassName + ": " + someData);
		}

		public void Do<T>(T someEntity) where T : Report
		{
			Console.WriteLine(ClassName + ": ID: " + someEntity.Id + "; Name" + someEntity.Name);
		}

		public IEnumerable<T> Collect<T>(int numberOfElements) where T : Report
		{
			var list = new List<T>(numberOfElements);
			return list.Select((elem, index) =>
			{
				elem.Id = index;
				elem.Name = this.ClassName;
				return elem;
			});
		}
	}
}
