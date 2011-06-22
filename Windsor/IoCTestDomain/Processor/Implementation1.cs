using System;
using System.Collections.Generic;
using System.Linq;
using IoCTestDomain.Logger;
using IoCTestDomain.Reporting;

namespace IoCTestDomain.Processor
{
	public class Implementation1 : IEntityProcessor
	{
		public Implementation1(ILogger logger)
		{
			this.Logger = logger;
		}

		public ILogger Logger { get; set; }

		public virtual string ClassName
		{
			get
			{
				this.Logger.Log("LOG ClassName: ");
				return "Implementation1";
			}
		}

		public virtual void Do(string someData)
		{
			this.Logger.Log("LOG Do: ");
			Console.WriteLine(ClassName + ": " + someData);
		}

		public virtual void Do<T>(T someEntity) where T : Report
		{
			this.Logger.Log("LOG Do: ");
			Console.WriteLine(ClassName + ": ID: " + someEntity.Id + "; Name" + someEntity.Name);
		}

		public virtual IEnumerable<T> Collect<T>(int numberOfElements) where T : Report
		{
			this.Logger.Log("LOG Collect: ");
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