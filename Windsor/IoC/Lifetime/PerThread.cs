using System;
using Castle.MicroKernel.Lifestyle;

namespace IoC.Lifetime
{
	/// <summary>
	/// The per thread.
	/// </summary>
	public class PerThread : ILifeTimeDefinition
	{
		public Type LifetimeManager
		{
			get { return typeof(PerThreadLifestyleManager); }
		}
	}
}