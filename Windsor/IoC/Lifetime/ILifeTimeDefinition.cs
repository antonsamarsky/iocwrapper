using System;

namespace IoC.Lifetime
{
	/// <summary>
	/// The lifetime definition.
	/// </summary>
	public interface ILifeTimeDefinition
	{
		Type LifetimeManager { get; }
	}
}