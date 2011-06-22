using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IoC.Lifetime;

namespace IoC
{
	/// <summary>
	/// Singleton access to the IoC container
	/// </summary>
	public static class IoCContainer
	{
		/// <summary>
		/// The container.
		/// </summary>
		private static readonly IoCContainerCore Container;

		/// <summary>
		/// Initializes static members of the <see cref="IoCContainer"/> class.
		/// Explicit static constructor to tell C# compiler not to mark type as before field init.
		/// </summary>
		static IoCContainer()
		{
			Container = new IoCContainerCore();
		}

		/// <summary>
		/// Gets Container.
		/// </summary>
		public static IoCContainerCore ContainerCore
		{
			get { return Container; }
		}

		/// <summary>
		/// Returns a component instance by the service 
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <returns>The component instance.</returns>
		public static T Resolve<T>()
		{
			return Container.Resolve<T>();
		}

		/// <summary>
		/// Resolves the specified name.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="key">The component's key.</param>
		/// <returns>The component instance.</returns>
		public static T Resolve<T>(string key)
		{
			return Container.Resolve<T>(key);
		}

		/// <summary>
		/// Resolves the specified arguments.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="arguments">The arguments.</param>
		/// <returns>The component instance.</returns>
		public static T Resolve<T>(IDictionary arguments)
		{
			return Container.Resolve<T>(arguments);
		}

		/// <summary>
		/// Resolves the specified arguments as anonymous type.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>The component instance.</returns>
		public static T Resolve<T>(object argumentsAsAnonymousType)
		{
			return Container.Resolve<T>(argumentsAsAnonymousType);
		}

		/// <summary>
		/// Resolves the specified name.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns>The component instance.</returns>
		public static object Resolve(Type service)
		{
			return Container.Resolve(service);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="key">The component's key.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static T Resolve<T>(string key, IDictionary arguments)
		{
			return Container.Resolve<T>(key, arguments);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="key">The component's key.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static T Resolve<T>(string key, object argumentsAsAnonymousType)
		{
			return Container.Resolve<T>(key, argumentsAsAnonymousType);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The component's key.</param>
		/// <param name="service">The service.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static object Resolve(string key, Type service)
		{
			return Container.Resolve(key, service);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static object Resolve(Type service, IDictionary arguments)
		{
			return Container.Resolve(service, arguments);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static object Resolve(Type service, object argumentsAsAnonymousType)
		{
			return Container.Resolve(service, argumentsAsAnonymousType);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The component's key.</param>
		/// <param name="service">The service.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static object Resolve(string key, Type service, IDictionary arguments)
		{
			return Container.Resolve(key, service, arguments);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The component's key.</param>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static object Resolve(string key, Type service, object argumentsAsAnonymousType)
		{
			return Container.Resolve(key, service, argumentsAsAnonymousType);
		}

		/// <summary>
		/// Tries the resolve.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="object">The @object.</param>
		/// <returns>The component instance.</returns>
		public static bool TryResolve<T>(out T @object)
		{
			return Container.TryResolve(out @object);
		}

		/// <summary>
		/// Tries the resolve.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The component instance.
		/// </returns>
		public static bool TryResolve<T>(string key, out T @object)
		{
			return Container.TryResolve(key, out @object);
		}

		/// <summary>
		/// Tries the resolve.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="arguments">The arguments.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The component instance.
		/// </returns>
		public static bool TryResolve<T>(IDictionary arguments, out T @object)
		{
			return Container.TryResolve(arguments, out @object);
		}

		/// <summary>
		/// Resolves the specified arguments as anonymous type.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The component instance.
		/// </returns>
		public static bool TryResolve<T>(object argumentsAsAnonymousType, out T @object)
		{
			return Container.TryResolve(argumentsAsAnonymousType, out @object);
		}

		/// <summary>
		/// Resolves the specified name.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The component instance.
		/// </returns>
		public static bool TryResolve(Type service, out object @object)
		{
			return Container.TryResolve(service, out @object);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="key">The component's key.</param>
		/// <param name="arguments">The arguments.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static bool TryResolve<T>(string key, IDictionary arguments, out T @object)
		{
			return Container.TryResolve(key, arguments, out @object);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="key">The component's key.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static bool TryResolve<T>(string key, object argumentsAsAnonymousType, out T @object)
		{
			return Container.TryResolve(key, argumentsAsAnonymousType, out @object);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The component's key.</param>
		/// <param name="service">The service.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static bool TryResolve(string key, Type service, out object @object)
		{
			return Container.TryResolve(key, service, out @object);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="arguments">The arguments.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static bool TryResolve(Type service, IDictionary arguments, out object @object)
		{
			return Container.TryResolve(service, arguments, out @object);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static bool TryResolve(Type service, object argumentsAsAnonymousType, out object @object)
		{
			return Container.TryResolve(service, argumentsAsAnonymousType, out @object);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The component's key.</param>
		/// <param name="service">The service.</param>
		/// <param name="arguments">The arguments.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public static bool TryResolve(string key, Type service, IDictionary arguments, out object @object)
		{
			return Container.TryResolve(key, service, arguments, out @object);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The component's key.</param>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The component instance.
		/// </returns>
		public static bool TryResolve(string key, Type service, object argumentsAsAnonymousType, out object @object)
		{
			return Container.TryResolve(key, service, argumentsAsAnonymousType, out @object);
		}

		/// <summary>
		/// Resolve all valid components that match this type. </summary>
		/// <typeparam name="T">The service type.</typeparam>
		/// <returns>The objects</returns>
		public static T[] ResolveAll<T>()
		{
			return Container.ResolveAll<T>();
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <typeparam name="T">The service type.</typeparam>
		/// <param name="arguments">The arguments.</param>
		/// <returns>The component instances.</returns>
		public static T[] ResolveAll<T>(IDictionary arguments)
		{
			return Container.ResolveAll<T>(arguments);
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <typeparam name="T">The service type.</typeparam>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>The component instances.</returns>
		public static T[] ResolveAll<T>(object argumentsAsAnonymousType)
		{
			return Container.ResolveAll<T>(argumentsAsAnonymousType);
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns>
		/// The objects
		/// </returns>
		public static Array ResolveAll(Type service)
		{
			return Container.ResolveAll(service);
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns> The objects.</returns>
		public static Array ResolveAll(Type service, IDictionary arguments)
		{
			return Container.ResolveAll(service, arguments);
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>
		/// The objects
		/// </returns>
		public static Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			return Container.ResolveAll(service, argumentsAsAnonymousType);
		}

		/// <summary>
		/// Builds up.
		/// </summary>
		/// <typeparam name="T">Service type</typeparam>
		/// <param name="object">The object.</param>
		public static void BuildUp<T>(T @object) where T : class
		{
			Container.BuildUp(@object);
		}

		/// <summary>
		/// Builds up.
		/// </summary>
		/// <param name="type">The component's type.</param>
		/// <param name="value">The component instance.</param>
		public static void BuildUp(Type type, object value)
		{
			Container.BuildUp(type, value);
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="fromInstance">From instance.</param>
		public static void RegisterInstance<TFrom>(TFrom fromInstance)
		{
			Container.RegisterInstance(fromInstance);
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="fromInstance">From instance.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void RegisterInstance<TFrom>(TFrom fromInstance, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.RegisterInstance(fromInstance, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <param name="fromType">From type.</param>
		/// <param name="fromInstance">From instance.</param>
		public static void RegisterInstance(Type fromType, object fromInstance)
		{
			Container.RegisterInstance(fromType, fromInstance);
		}


		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <param name="fromType">From type.</param>
		/// <param name="fromInstance">From instance.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void RegisterInstance(Type fromType, object fromInstance, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.RegisterInstance(fromType, fromInstance, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the specified factory method.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="factoryMethod">The factory method to create a service instance.</param>
		public static void Register<TFrom>(Func<TFrom> factoryMethod)
		{
			Container.Register(factoryMethod);
		}

		/// <summary>
		/// Registers the specified factory method.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="factoryMethod">The factory method to create a service instance.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom>(Func<TFrom> factoryMethod, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(factoryMethod, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		public static void Register<TFrom, TTo>() where TTo : TFrom
		{
			Container.Register<TFrom, TTo>();
		}

		/// <summary>
		/// Registers the specified parameters.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register<TFrom, TTo>(IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(dependencies);
		}

		/// <summary>
		/// Registers the specified set parameters.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="setParameters">The set parameters.</param>
		public static void Register<TFrom, TTo>(Action<IDictionary> setParameters) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(setParameters);
		}

		/// <summary>
		/// Registers the specified set parameters.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register<TFrom, TTo>(Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(setParameters, dependencies);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		/// <param name="setParameters">The set parameters.</param>
		public static void Register<TFrom, TTo>(Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(setParameters, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom, TTo>(IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom, TTo>(Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(setParameters, dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		public static void Register<TFrom, TTo>(string key) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		public static void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key, setParameters);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register<TFrom, TTo>(string key, IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key, dependencies);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key, setParameters, dependencies);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key, setParameters, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom, TTo>(string key, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key, dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key, setParameters, dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		public static void Register(Type from, Type to)
		{
			Container.Register(from, to);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		public static void Register(Type from, Type to, Action<IDictionary> setParameters)
		{
			Container.Register(from, to, setParameters);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register(Type from, Type to, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Container.Register(from, to, dependencies);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register(Type from, Type to, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Container.Register(from, to, setParameters, dependencies);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, setParameters, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, setParameters, dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		public static void Register(Type from, Type to, string key)
		{
			Container.Register(from, to, key);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		public static void Register(Type from, Type to, string key, Action<IDictionary> setParameters)
		{
			Container.Register(from, to, key, setParameters);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register(Type from, Type to, string key, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Container.Register(from, to, key, dependencies);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public static void Register(Type from, Type to, string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Container.Register(from, to, key, setParameters, dependencies);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom, TTo>(ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register<TFrom, TTo>(string key, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(key, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, string key, Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, key, setParameters, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, string key, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, key, dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, key, setParameters, dependencies, lifeTimeDefinition);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public static void Register(Type from, Type to, string key, ILifeTimeDefinition lifeTimeDefinition)
		{
			Container.Register(from, to, key, lifeTimeDefinition);
		}

		/// <summary>
		/// Removes the specified service from the IoC.
		/// </summary>
		/// <typeparam name="T">The service type.</typeparam>
		public static void Remove<T>()
		{
			Container.Remove<T>();
		}

		/// <summary>
		/// Removes the specified service from the IoC.
		/// </summary>
		/// <param name="type">The service  type.</param>
		public static void Remove(Type type)
		{
			Container.Remove(type);
		}

		/// <summary>
		/// Determines whether this instance is registered.
		/// </summary>
		/// <typeparam name="T">The type of the service.</typeparam>
		/// <returns>
		/// 	<c>true</c> if this instance is registered; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsRegistered<T>()
		{
			return Container.IsRegistered<T>();
		}

		/// <summary>
		/// Determines whether this instance is registered.
		/// </summary>
		/// <typeparam name="T">The type of the service.</typeparam>
		/// <param name="key">The service key.</param>
		/// <returns>
		/// <c>true</c> if this instance is registered; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsRegistered<T>(string key)
		{
			return Container.IsRegistered<T>(key);
		}

		/// <summary>
		/// Determines whether the specified type is registered.
		/// </summary>
		/// <param name="type">The type of the service.</param>
		/// <returns>
		/// <c>true</c> if the specified type is registered; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsRegistered(Type type)
		{
			return Container.IsRegistered(type);
		}

		/// <summary>
		/// Determines whether the specified type is registered.
		/// </summary>
		/// <param name="type">The type of the service.</param>
		/// <param name="key">The service key.</param>
		/// <returns>
		/// <c>true</c> if the specified type is registered; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsRegistered(Type type, string key)
		{
			return Container.IsRegistered(type, key);
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TRegistrator">The type of the registrator.</typeparam>
		/// <param name="registrations">The registrations.</param>
		public static void Configure<TRegistrator>(params TRegistrator[] registrations)
		{
			Container.Configure(registrations);
		}

		/// <summary>
		/// Cleans up.
		/// </summary>
		/// <param name="value">The value.</param>
		public static void Release(object value)
		{
			Container.Release(value);
		}

		/// <summary>
		/// Perform custom container initialization using only the IoCRegistration points that match one of the specified modes
		/// (This is intended for unit tests that want to uses a custom set of type mappings for mocking)
		/// </summary>
		/// <param name="assemblies">The assemblies.</param>
		/// <param name="configurations">The configurations.</param>
		public static void InitializeContainer(IEnumerable<string> assemblies, IEnumerable<string> configurations)
		{
			Container.InitializeContainer(assemblies, configurations);
		}

		/// <summary>
		/// Perform custom container initialization using only the IoCRegistration points that match one of the specified modes
		/// (This is intended for unit tests that want to uses a custom set of type mappings for moc'ing)
		/// This method will try to resolve assemblies via configuration file
		/// </summary>
		public static void InitializeContainer()
		{
			if (Container.IsInitialised)
			{
				return;
			}

			IEnumerable<string> configAssemblies = GetConfigAssemblies();
			if (configAssemblies.Any())
			{
				Container.InitializeContainer(configAssemblies, null);
			}
			else
			{
				Container.InitializeContainer();
			}
		}

		/// <summary>
		/// Resets the container (really only useful for unit tests)
		/// </summary>
		public static void ResetContainer()
		{
			Container.ResetContainer();
		}

		/// <summary>
		/// The invoke registration method.
		/// </summary>
		/// <param name="type">The type of the registration.</param>
		public static void InvokeRegistrationMethod(Type type)
		{
			Container.InvokeIoCRegistrationFixture(type);
		}

		/// <summary>
		/// The invoke registration method.
		/// </summary>
		/// <param name="type">The type of the registration.</param>
		public static void InvokeRegistrationMethod(string type)
		{
			Container.InvokeIoCRegistrationFixture(type);
		}

		/// <summary>
		/// Gets the config assemblies.
		/// </summary>
		/// <returns>The list of assemblies.</returns>
		private static IEnumerable<string> GetConfigAssemblies()
		{
			var section = ConfigurationManager.GetSection(IoCAssemblyRegistration.ConfigurationSectionName) as IoCAssemblyRegistration;
			if (section == null)
			{
				// throw new ConfigurationErrorsException("Configuration section was not found");
				yield break;
			}

			foreach (AssemblyElement element in section.Assemblies)
			{
				yield return element.Assembly;
			}

		}
	}
}
