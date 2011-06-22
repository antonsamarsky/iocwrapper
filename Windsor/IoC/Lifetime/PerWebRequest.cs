using System;
using Castle.MicroKernel.Lifestyle;

namespace IoC.Lifetime
{
	/// <summary>
	/// The PerWebRequest lifetime manager
	/// </summary>
	public class PerWebRequest : ILifeTimeDefinition
	{
		public Type LifetimeManager
		{
			get { return typeof(PerWebRequestLifestyleManager); }
		}
	}
}