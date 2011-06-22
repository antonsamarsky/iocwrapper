using System;
using Castle.MicroKernel.Lifestyle;

namespace IoC.Lifetime
{
	/// <summary>
	/// The transient
	/// </summary>
	public class Transient : ILifeTimeDefinition
	{
		public Type LifetimeManager
		{
			get { return typeof(TransientLifestyleManager); }
		}
	}
}