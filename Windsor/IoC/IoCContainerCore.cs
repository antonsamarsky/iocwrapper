using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.Facilities.FactorySupport;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Diagnostics;
using IoC.Lifetime;
using Logging;

namespace IoC
{
	/// <summary>
	/// The actual implementaion of the IoContainer wrapper made availabe to applications built on top of the
	/// MaterialCore stack. This class is intended to be threadsafe.
	/// </summary>
	public class IoCContainerCore : IDisposable
	{
		/// <summary>
		/// The Reader-Writer lock to be protected with a thread synchronization lock during registering/resolving services.
		/// </summary>
		private readonly ReaderWriterLockSlim iocLocker;

		/// <summary>
		/// The Reader-Writer lock to be protected with a thread synchronization lock during processing IoC children list.
		/// </summary>
		private readonly ReaderWriterLockSlim childrenLocker;

		/// <summary>
		/// The container instance.
		/// </summary>
		private IWindsorContainer container;

		/// <summary>
		/// Field that shows whether container is initialized.
		/// </summary>
		private bool initialised;

		/// <summary>
		/// The fields that shows wether container is disposed.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// The parent container instance.
		/// </summary>
		private IoCContainerCore parent;

		/// <summary>
		/// The name of the container.
		/// </summary>
		private string name;

		/// <summary>
		/// The child sontainer.
		/// </summary>
		private Dictionary<string, IoCContainerCore> childContainers;

		/// <summary>
		/// Initializes a new instance of the <see cref="IoCContainerCore"/> class.
		/// </summary>
		public IoCContainerCore()
		{
			this.container = new WindsorContainer();
			this.iocLocker = new ReaderWriterLockSlim();
			this.childrenLocker = new ReaderWriterLockSlim();
			this.childContainers = new Dictionary<string, IoCContainerCore>();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is initialised.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is initialised; otherwise, <c>false</c>.
		/// </value>
		public bool IsInitialised
		{
			get { return this.initialised; }
		}

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>
		/// The parent container.
		/// </value>
		public IoCContainerCore Parent
		{
			get
			{
				return this.parent;
			}

			set
			{
				Assert.ArgumentNotNull(value, "value");

				this.parent = value;

				Log.Info(string.Format("Parent container named: '{0}' has been set.", value.Name), typeof(IoCContainerCore));
			}
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The container's name.
		/// </value>
		public string Name
		{
			get
			{
				return this.name;
			}

			set
			{
				Assert.ArgumentNotNullOrEmpty(value, "value");

				this.name = value;

				Log.Info(string.Format("Container name: '{0}' has been set.", value), typeof(IoCContainerCore));
			}
		}

		/// <summary>
		/// Adds the child container.
		/// </summary>
		/// <param name="childContainerKey">The child container key.</param>
		/// <param name="childContainer">The child container.</param>
		public virtual void AddChildContainer(string childContainerKey, IoCContainerCore childContainer)
		{
			Assert.ArgumentNotNullOrEmpty(childContainerKey, "childContainerKey");
			Assert.ArgumentNotNull(childContainer, "childContainer");

			this.childrenLocker.EnterWriteLock();
			try
			{
				Assert.IsFalse(this.childContainers.ContainsKey(childContainerKey), string.Format("Child container named: '{0}' has already been added.", childContainerKey));

				this.childContainers.Add(childContainerKey, childContainer);

				Log.Info(string.Format("Child container named: '{0}' has been added.", childContainerKey), typeof(IoCContainerCore));
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message, exception);
				throw;
			}
			finally
			{
				this.childrenLocker.ExitWriteLock();
			}
		}

		/// <summary>
		/// Removes the child container.
		/// </summary>
		/// <param name="childContainerKey">The child container key.</param>
		public virtual void RemoveChildContainer(string childContainerKey)
		{
			Assert.ArgumentNotNullOrEmpty(childContainerKey, "childContainerKey");

			this.childrenLocker.EnterWriteLock();
			try
			{
				Assert.IsFalse(this.childContainers.ContainsKey(childContainerKey), string.Format("Child container named: '{0}' has already been added.", childContainerKey));

				this.childContainers.Remove(childContainerKey);

				Log.Info(string.Format("Child container named: '{0}' has been removed.", childContainerKey), typeof(IoCContainerCore));
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message, exception);
				throw;
			}
			finally
			{
				this.childrenLocker.ExitWriteLock();
			}
		}

		/// <summary>
		/// Gets the child container.
		/// </summary>
		/// <param name="childContainerKey">The child container key.</param>
		/// <returns>The child container instance.</returns>
		public virtual IoCContainerCore GetChildContainer(string childContainerKey)
		{
			Assert.ArgumentNotNullOrEmpty(childContainerKey, "childContainerKey");

			this.childrenLocker.EnterReadLock();
			try
			{
				Assert.IsTrue(this.childContainers.ContainsKey(childContainerKey), string.Format("Child container named: '{0}' does not excist.", childContainerKey));

				return this.childContainers[childContainerKey];
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message, exception);
				throw;
			}
			finally
			{
				this.childrenLocker.ExitReadLock();
			}
		}

		/// <summary>
		/// Initializes the container.
		/// </summary>
		public virtual void InitializeContainer()
		{
			if (this.container == null)
			{
				this.container = new WindsorContainer();
				this.container.AddFacility<FactorySupportFacility>();
			}

			this.initialised = true;
		}

		/// <summary>
		/// Initializes the container.
		/// </summary>
		/// <param name="assemblyNames">The assembly names.</param>
		/// <param name="configurations">The configurations.</param>
		public virtual void InitializeContainer(IEnumerable<string> assemblyNames, IEnumerable<string> configurations)
		{
			Assert.ArgumentNotNullOrEmpty(assemblyNames, "assemblyNames");

			this.InitializeContainer(assemblyNames, configurations, true);
		}

		/// <summary>
		/// Initialization method that allows the user to explicitly define the assemblies and configuration mode that
		/// should be used to configure the IoCContainer.  This is intended for unit tests and programs that do not have
		/// a poet configuration
		/// </summary>
		/// <param name="assemblyNames">The assembly names.</param>
		/// <param name="configurations">The configurations.</param>
		/// <param name="includeRelease">if set to <c>true</c> [include release].</param>
		public virtual void InitializeContainer(IEnumerable<string> assemblyNames, IEnumerable<string> configurations, bool includeRelease)
		{
			Assert.ArgumentNotNullOrEmpty(assemblyNames, "assemblyNames");

			var assemblies = assemblyNames.Select(Assembly.Load);

			this.InitializeContainer(assemblies, configurations, includeRelease);
		}

		/// <summary>
		/// Initialization method that allows the user to explicitly define the assemblies and configuration mode that
		/// should be used to configure the IoCContainer.  This is intended for unit tests and programs that do not have
		/// a poet configuration
		/// </summary>
		/// <param name="assemblies">The name of the assmeblies that should be scanned for IoCContainer registration modules</param>
		/// <param name="configurations">The configurations.</param>
		/// <param name="includeRelease">if set to <c>true</c> [include release].</param>
		public virtual void InitializeContainer(IEnumerable<Assembly> assemblies, IEnumerable<string> configurations, bool includeRelease)
		{
			Assert.ArgumentNotNullOrEmpty(assemblies, "assemblies");

			this.container = new WindsorContainer();
			this.container.AddFacility<FactorySupportFacility>();

			var registrationTypes = from assembly in assemblies
															let types = assembly.GetTypes()
															from type in types
															where !type.IsInterface && typeof(IIoCRegistration).IsAssignableFrom(type)
															select type;

			registrationTypes.ToList().ForEach(type => this.InvokeIoCRegistration(type, configurations, includeRelease));

			this.initialised = true;
		}

		/// <summary>
		/// Resets the container.
		/// </summary>
		public virtual void ResetContainer()
		{
			this.InvokeWriteLock(() =>
			{
				this.container = new WindsorContainer();
				this.childContainers = new Dictionary<string, IoCContainerCore>();
				this.initialised = false;
			});
		}

		/// <summary>
		/// Returns a component instance by the service 
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <returns>The component instance.</returns>
		public virtual T Resolve<T>()
		{
			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>();
			});
		}

		/// <summary>
		/// Resolves the specified name.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="key">The component's key.</param>
		/// <returns>The component instance.</returns>
		public virtual T Resolve<T>(string key)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(key);
			});
		}

		/// <summary>
		/// Resolves the specified arguments.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="arguments">The arguments.</param>
		/// <returns>The component instance.</returns>
		public virtual T Resolve<T>(IDictionary arguments)
		{
			Assert.ArgumentNotNull(arguments, "arguments");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(arguments);
			});
		}

		/// <summary>
		/// Resolves the specified arguments as anonymous type.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>The component instance.</returns>
		public virtual T Resolve<T>(object argumentsAsAnonymousType)
		{
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(argumentsAsAnonymousType);
			});
		}

		/// <summary>
		/// Resolves the specified name.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns>The component instance.</returns>
		public virtual object Resolve(Type service)
		{
			Assert.ArgumentNotNull(service, "service");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(service);
			});
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
		public virtual T Resolve<T>(string key, IDictionary arguments)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(arguments, "arguments");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(key, arguments);
			});
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
		public virtual T Resolve<T>(string key, object argumentsAsAnonymousType)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(key, argumentsAsAnonymousType);
			});
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The component's key.</param>
		/// <param name="service">The service.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public virtual object Resolve(string key, Type service)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(service, "service");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(key, service);
			});
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public virtual object Resolve(Type service, IDictionary arguments)
		{
			Assert.ArgumentNotNull(arguments, "arguments");
			Assert.ArgumentNotNull(service, "service");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(service, arguments);
			});
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>
		/// The Component instance.
		/// </returns>
		public virtual object Resolve(Type service, object argumentsAsAnonymousType)
		{
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");
			Assert.ArgumentNotNull(service, "service");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(service, argumentsAsAnonymousType);
			});
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
		public virtual object Resolve(string key, Type service, IDictionary arguments)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(arguments, "arguments");
			Assert.ArgumentNotNull(service, "service");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(key, service, arguments);
			});
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
		public virtual object Resolve(string key, Type service, object argumentsAsAnonymousType)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");
			Assert.ArgumentNotNull(service, "service");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(key, service, argumentsAsAnonymousType);
			});
		}

		/// <summary>
		/// Tries the resolve.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		/// <param name="object">The @object.</param>
		/// <returns>The component instance.</returns>
		public virtual bool TryResolve<T>(out T @object)
		{
			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>();
			},
			out @object);
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
		public virtual bool TryResolve<T>(string key, out T @object)
		{
			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(key);
			},
			out @object);
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
		public virtual bool TryResolve<T>(IDictionary arguments, out T @object)
		{
			Assert.ArgumentNotNull(arguments, "arguments");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(arguments);
			},
			out @object);
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
		public virtual bool TryResolve<T>(object argumentsAsAnonymousType, out T @object)
		{
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(argumentsAsAnonymousType);
			},
			out @object);
		}

		/// <summary>
		/// Resolves the specified name.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="object">The @object.</param>
		/// <returns>
		/// The component instance.
		/// </returns>
		public virtual bool TryResolve(Type service, out object @object)
		{
			Assert.ArgumentNotNull(service, "service");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(service);
			},
			out @object);
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
		public virtual bool TryResolve<T>(string key, IDictionary arguments, out T @object)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(arguments, "arguments");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(key, arguments);
			},
			out @object);
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
		public virtual bool TryResolve<T>(string key, object argumentsAsAnonymousType, out T @object)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve<T>(key, argumentsAsAnonymousType);
			},
			out @object);
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
		public virtual bool TryResolve(string key, Type service, out object @object)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(service, "service");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(key, service);
			},
			out @object);
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
		public virtual bool TryResolve(Type service, IDictionary arguments, out object @object)
		{
			Assert.ArgumentNotNull(service, "service");
			Assert.ArgumentNotNull(arguments, "arguments");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(service, arguments);
			},
			out @object);
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
		public virtual bool TryResolve(Type service, object argumentsAsAnonymousType, out object @object)
		{
			Assert.ArgumentNotNull(service, "service");
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(service, argumentsAsAnonymousType);
			},
			out @object);
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
		public virtual bool TryResolve(string key, Type service, IDictionary arguments, out object @object)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(service, "service");
			Assert.ArgumentNotNull(arguments, "arguments");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(key, service, arguments);
			},
			out @object);
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
		public virtual bool TryResolve(string key, Type service, object argumentsAsAnonymousType, out object @object)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(service, "service");
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.TryInvokeReadLock(
			() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.Resolve(key, service, argumentsAsAnonymousType);
			},
			out @object);
		}

		/// <summary>
		/// Resolve all valid components that match this type. </summary>
		/// <typeparam name="T">The service type.</typeparam>
		/// <returns>The objects</returns>
		public virtual T[] ResolveAll<T>()
		{
			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.ResolveAll<T>();
			});
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <typeparam name="T">The service type.</typeparam>
		/// <param name="arguments">The arguments.</param>
		/// <returns>The component instances.</returns>
		public virtual T[] ResolveAll<T>(IDictionary arguments)
		{
			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.ResolveAll<T>(arguments);
			});
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <typeparam name="T">The service type.</typeparam>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>The component instances.</returns>
		public virtual T[] ResolveAll<T>(object argumentsAsAnonymousType)
		{
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.ResolveAll<T>(argumentsAsAnonymousType);
			});
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns>
		/// The objects
		/// </returns>
		public virtual Array ResolveAll(Type service)
		{
			Assert.ArgumentNotNull(service, "service");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.ResolveAll(service);
			});
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns> The objects.</returns>
		public virtual Array ResolveAll(Type service, IDictionary arguments)
		{
			Assert.ArgumentNotNull(service, "service");
			Assert.ArgumentNotNull(arguments, "arguments");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.ResolveAll(service, arguments);
			});
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>
		/// The objects
		/// </returns>
		public virtual Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			Assert.ArgumentNotNull(service, "service");
			Assert.ArgumentNotNull(argumentsAsAnonymousType, "argumentsAsAnonymousType");

			return this.InvokeReadLock(() =>
			{
				this.EnsureContainerIsInitialsied();
				return this.container.ResolveAll(service, argumentsAsAnonymousType);
			});
		}

		/// <summary>
		/// Builds up.
		/// </summary>
		/// <typeparam name="T">Service type</typeparam>
		/// <param name="object">The object.</param>
		public virtual void BuildUp<T>(T @object) where T : class
		{
			throw new NotImplementedException("Windsor does not support BuildUp yet");
		}

		/// <summary>
		/// Builds up.
		/// </summary>
		/// <param name="type">The component's type.</param>
		/// <param name="value">The component instance.</param>
		public virtual void BuildUp(Type type, object value)
		{
			throw new NotImplementedException("Windsor does not support BuildUp yet");
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="fromInstance">From instance.</param>
		public virtual void RegisterInstance<TFrom>(TFrom fromInstance)
		{
			Assert.ArgumentNotNull(fromInstance, "@from");

			this.InvokeReadLock(() => this.container.Register(Component.For<TFrom>().Instance(fromInstance)));
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="fromInstance">From instance.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void RegisterInstance<TFrom>(TFrom fromInstance, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNull(fromInstance, "fromInstance");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().Instance(fromInstance), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <param name="fromType">From type.</param>
		/// <param name="fromInstance">From instance.</param>
		public virtual void RegisterInstance(Type fromType, object fromInstance)
		{
			Assert.ArgumentNotNull(fromType, "@fromType");
			Assert.ArgumentNotNull(fromInstance, "@fromInstance");

			this.InvokeReadLock(() => this.container.Register(Component.For(fromType).Instance(fromInstance)));
		}

		/// <summary>
		/// Registers the instance.
		/// </summary>
		/// <param name="fromType">From type.</param>
		/// <param name="fromInstance">From instance.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void RegisterInstance(Type fromType, object fromInstance, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNull(fromType, "fromType");
			Assert.ArgumentNotNull(fromInstance, "fromInstance");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For(fromType).Instance(fromInstance), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="factoryMethod">The factory method to create a service.</param>
		public virtual void Register<TFrom>(Func<TFrom> factoryMethod)
		{
			Assert.ArgumentNotNull(factoryMethod, "factoryMethod");

			this.InvokeReadLock(() => this.container.Register(Component.For<TFrom>().UsingFactoryMethod(() => factoryMethod())));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <param name="factoryMethod">The factory method to create a service.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom>(Func<TFrom> factoryMethod, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNull(factoryMethod, "factoryMethod");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().UsingFactoryMethod(() => factoryMethod()), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		public virtual void Register<TFrom, TTo>() where TTo : TFrom
		{
			this.InvokeReadLock(() => this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>()));
		}

		/// <summary>
		/// Registers the specified parameters.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register<TFrom, TTo>(IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));

				return this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>().DependsOn(windsorDepens.ToArray()));
			});
		}

		/// <summary>
		/// Registers the specified parameters.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="setParameters">The set parameters.</param>
		public virtual void Register<TFrom, TTo>(Action<IDictionary> setParameters) where TTo : TFrom
		{
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>().DynamicParameters((k, d) => setParameters(d))));
		}

		/// <summary>
		/// Registers the specified parameters.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register<TFrom, TTo>(Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Assert.ArgumentNotNull(setParameters, "setParameters");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));

				return this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>().DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom, TTo>(ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>(), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom, TTo>(Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>().DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom, TTo>(IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));

				return this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>().DependsOn(windsorDepens.ToArray()), lifeTimeDefinition));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom, TTo>(Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));

				return this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>().DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		public virtual void Register<TFrom, TTo>(string key) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");

			this.InvokeReadLock(() => this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>().Named(key)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		public virtual void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>().Named(key).DynamicParameters((k, d) => setParameters(d))));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register<TFrom, TTo>(string key, IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));

				return this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>().Named(key).DependsOn(windsorDepens.ToArray()));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));

				return this.container.Register(Component.For<TFrom>().ImplementedBy<TTo>().Named(key).DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom, TTo>(string key, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>().Named(key), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>().Named(key).DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TFrom">The type of from.</typeparam>
		/// <typeparam name="TTo">The type of to.</typeparam>
		/// <param name="key">Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register<TFrom, TTo>(string key, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>().Named(key).DependsOn(windsorDepens.ToArray()), lifeTimeDefinition));
			});
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
		public virtual void Register<TFrom, TTo>(string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition) where TTo : TFrom
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(LifeTimeRegistration.GetManager(Component.For<TFrom>().ImplementedBy<TTo>().Named(key).DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		public virtual void Register(Type from, Type to)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");

			this.InvokeReadLock(() => this.container.Register(Component.For(from).ImplementedBy(to)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		public virtual void Register(Type from, Type to, Action<IDictionary> setParameters)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(Component.For(from).ImplementedBy(to).DynamicParameters((k, d) => setParameters(d))));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register(Type from, Type to, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(Component.For(from).ImplementedBy(to).DependsOn(windsorDepens.ToArray()));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register(Type from, Type to, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(Component.For(from).ImplementedBy(to).DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register(Type from, Type to, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register(Type from, Type to, Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to).DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register(Type from, Type to, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to).DependsOn(windsorDepens.ToArray()), lifeTimeDefinition));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The typer from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register(Type from, Type to, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to).DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		public virtual void Register(Type from, Type to, string key)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");

			this.InvokeReadLock(() => this.container.Register(Component.For(from).ImplementedBy(to).Named(key)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		public virtual void Register(Type from, Type to, string key, Action<IDictionary> setParameters)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(Component.For(from).ImplementedBy(to).Named(key).DynamicParameters((k, d) => setParameters(d))));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register(Type from, Type to, string key, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(Component.For(from).ImplementedBy(to).Named(key).DependsOn(windsorDepens.ToArray()));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="dependencies">The dependencies.</param>
		public virtual void Register(Type from, Type to, string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(Component.For(from).ImplementedBy(to).Named(key).DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)));
			});
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register(Type from, Type to, string key, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to).Named(key), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="setParameters">The set parameters.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register(Type from, Type to, string key, Action<IDictionary> setParameters, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() => this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to).Named(key).DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <param name="from">The type from.</param>
		/// <param name="to">The type to.</param>
		/// <param name="key">The Component's key.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="lifeTimeDefinition">The life time definition.</param>
		public virtual void Register(Type from, Type to, string key, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to).Named(key).DependsOn(windsorDepens.ToArray()), lifeTimeDefinition));
			});
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
		public virtual void Register(Type from, Type to, string key, Action<IDictionary> setParameters, IEnumerable<KeyValuePair<string, string>> dependencies, ILifeTimeDefinition lifeTimeDefinition)
		{
			Assert.ArgumentNotNullOrEmpty(key, "key");
			Assert.ArgumentNotNull(to, "to");
			Assert.ArgumentNotNull(from, "from");
			Assert.ArgumentNotNull(lifeTimeDefinition, "lifeTimeDefinition");
			Assert.ArgumentNotNullOrEmpty(dependencies, "dependencies");
			Assert.ArgumentNotNull(setParameters, "setParameters");

			this.InvokeReadLock(() =>
			{
				var windsorDepens = dependencies.Select(parameter => ServiceOverride.ForKey(parameter.Key).Eq(parameter.Value));
				return this.container.Register(LifeTimeRegistration.GetManager(Component.For(from).ImplementedBy(to).Named(key).DependsOn(windsorDepens.ToArray()).DynamicParameters((k, d) => setParameters(d)), lifeTimeDefinition));
			});
		}

		/// <summary>
		/// Removes the specified service from the IoC.
		/// </summary>
		/// <typeparam name="T">The service type.</typeparam>
		public virtual void Remove<T>()
		{
			var type = typeof(T);

			this.InvokeReadLock(() => 
			{
				if (this.container.Kernel.HasComponent(type.FullName))
				{
					return this.container.Kernel.RemoveComponent(type.FullName);
				}

				object service; 
				if (this.TryResolve(type, out service))
				{
					var serviceType = service.GetType();
					if (this.container.Kernel.HasComponent(serviceType.FullName))
					{
						return this.container.Kernel.RemoveComponent(serviceType.FullName);
					}
				}

				return false;
			});
		}

		/// <summary>
		/// Removes the specified service from the IoC.
		/// </summary>
		/// <param name="type">The service  type.</param>
		public virtual void Remove(Type type)
		{
			Assert.ArgumentNotNull(type, "type");

			this.InvokeReadLock(() =>
			{
				if (this.container.Kernel.HasComponent(type.FullName))
				{
					return this.container.Kernel.RemoveComponent(type.FullName);
				}

				object service;
				if (this.TryResolve(type, out service))
				{
					var serviceType = service.GetType();
					if (this.container.Kernel.HasComponent(serviceType.FullName))
					{
						return this.container.Kernel.RemoveComponent(serviceType.FullName);
					}
				}

				return false;
			});
		}

		/// <summary>
		/// Determines whether this instance is registered.
		/// </summary>
		/// <typeparam name="T">The type of the service.</typeparam>
		/// <returns>
		/// 	<c>true</c> if this instance is registered; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsRegistered<T>()
		{
			var type = typeof(T);
			return this.InvokeReadLock(() => this.container.Kernel.HasComponent(type));
		}

		/// <summary>
		/// Determines whether the specified key is registered.
		/// </summary>
		/// <typeparam name="T">The service type. </typeparam>
		/// <param name="key">The service key.</param>
		/// <returns>
		/// 	<c>true</c> if the specified key is registered; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsRegistered<T>(string key)
		{
			return this.InvokeReadLock(() => this.container.Kernel.GetAssignableHandlers(typeof(T)).Any(c => string.Equals(key, c.ComponentModel.Name)));
		}

		/// <summary>
		/// Determines whether the specified type is registered.
		/// </summary>
		/// <param name="type">The type of the service.</param>
		/// <returns>
		/// <c>true</c> if the specified type is registered; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsRegistered(Type type)
		{
			Assert.ArgumentNotNull(type, "type");

			return this.InvokeReadLock(() => this.container.Kernel.HasComponent(type));
		}

		/// <summary>
		/// Determines whether the specified key is registered.
		/// </summary>
		/// <param name="type">The service type.</param>
		/// <param name="key">The service key.</param>
		/// <returns>
		/// <c>true</c> if the specified key is registered; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsRegistered(Type type, string key)
		{
			return this.InvokeReadLock(() => this.container.Kernel.GetAssignableHandlers(type).Any(c => string.Equals(key, c.ComponentModel.Name)));
		}

		/// <summary>
		/// Registers the type.
		/// </summary>
		/// <typeparam name="TRegistrator">The type of the registrator.</typeparam>
		/// <param name="registrations">The registrations.</param>
		public virtual void Configure<TRegistrator>(params TRegistrator[] registrations)
		{
			if (typeof(IRegistration).IsAssignableFrom(typeof(TRegistrator)))
			{
				this.InvokeWriteLock(() =>
				{
					foreach (var registration in registrations)
					{
						container.Register((IRegistration)registration);
					}
				});
			}

			if (typeof(IWindsorInstaller).IsAssignableFrom(typeof(TRegistrator)))
			{
				this.InvokeWriteLock(() =>
				{
					foreach (var registration in registrations)
					{
						container.Install((IWindsorInstaller)registration);
					}
				});
			}

			throw new NotImplementedException("Currently only IRegistration and IWindsorInstaller are supported");
		}

		/// <summary>
		/// Invokes the IoC registration fixture.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		public virtual void InvokeIoCRegistrationFixture(string typeName)
		{
			Assert.ArgumentNotNullOrEmpty(typeName, "typeName");

			Type type = Type.GetType(typeName);

			this.InvokeIoCRegistrationFixture(type);
		}

		/// <summary>
		/// Invokes the IoC registration fixture.
		/// </summary>
		/// <param name="type">The component's type.</param>
		/// <exception cref="InvalidOperationException">"Unable to create an instance of '{0}' when configuring the IoC Container (does it have a default constructor).</exception>
		public virtual void InvokeIoCRegistrationFixture(Type type)
		{
			Assert.ArgumentNotNull(type, "type");

			IIoCRegistration registration;
			if (!ReflectionUtils.TryCreateTypeFromDefaultConstructor(type, out registration))
			{
				string error = string.Format(CultureInfo.CurrentCulture, "Unable to create an instance of '{0}' when configuring the IoC Container (does it have a default constructor).", type);
				throw new InvalidOperationException(error);
			}

			registration.BuildContainer(this);
		}

		/// <summary>
		/// Cleans up.
		/// </summary>
		/// <param name="value">The value.</param>
		public virtual void Release(object value)
		{
			Assert.ArgumentNotNull(value, "value");

			this.container.Release(value);
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					// NOTE: Dispose managed resources.
					if (this.container != null)
					{
						this.container.Dispose();
					}
				}
			}

			this.disposed = true;
		}

		#endregion

		/// <summary>
		/// Ensures that at the very least the default IoCContainer initialisation has been performed
		/// </summary>
		private void EnsureContainerIsInitialsied()
		{
			if (!this.initialised)
			{
				this.InitializeContainer();
			}
		}

		/// <summary>
		/// Invokes the io C registration.
		/// </summary>
		/// <param name="type">The component's type.</param>
		/// <param name="configurations">The configurations.</param>
		/// <param name="includeRelease">if set to <c>true</c> [include release].</param>
		private void InvokeIoCRegistration(Type type, IEnumerable<string> configurations, bool includeRelease)
		{
			Assert.ArgumentNotNull(type, "type");

			IoCRegistrationAttribute attribute = Attribute.GetCustomAttribute(type, typeof(IoCRegistrationAttribute)) as IoCRegistrationAttribute;

			if (attribute == null)
			{
				return;
			}

			if ((attribute.Mode == IoCRegistrationMode.Release && includeRelease) ||
					(configurations != null && configurations.Any() && (attribute.Mode == IoCRegistrationMode.Custom && configurations.Contains(attribute.CustomMode))))
			{
				this.InvokeIoCRegistrationFixture(type);
			}
		}

		/// <summary>
		/// Invokes the specified action.
		/// </summary>
		/// <param name="action">The delegate to run.</param>
		private void InvokeWriteLock(Action action)
		{
			this.iocLocker.EnterWriteLock();

			try
			{
				action();
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message, exception, typeof(IoCContainerCore));
				throw new InvalidOperationException(exception.Message, exception);
			}
			finally
			{
				this.iocLocker.ExitWriteLock();
			}
		}

		/// <summary>
		/// Invokes the specified delegate.
		/// </summary>
		/// <typeparam name="T">The delegate return type.</typeparam>
		/// <param name="func">The delegate to run.</param>
		/// <returns>The function result.</returns>
		private T InvokeReadLock<T>(Func<T> func)
		{
			if (!this.iocLocker.IsReadLockHeld && !this.iocLocker.IsWriteLockHeld)
			{
				this.iocLocker.EnterReadLock();
			}

			try
			{
				return func();
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message, exception, typeof(IoCContainerCore));
				throw new InvalidOperationException(exception.Message, exception);
			}
			finally
			{
				if (this.iocLocker.IsReadLockHeld)
				{
					this.iocLocker.ExitReadLock();
				}
			}
		}

		/// <summary>
		/// Invokes the specified delegate.
		/// </summary>
		/// <param name="func">The delegate to run</param>
		/// <returns>The function result.</returns>
		private object InvokeReadLock(Func<object> func)
		{
			if (!this.iocLocker.IsReadLockHeld && !this.iocLocker.IsWriteLockHeld)
			{
				this.iocLocker.EnterReadLock();
			}

			try
			{
				return func();
			}
			catch (Exception exception)
			{
				Log.Error(exception.Message, exception, typeof(IoCContainerCore));
				throw new InvalidOperationException(exception.Message, exception);
			}
			finally
			{
				if (this.iocLocker.IsReadLockHeld)
				{
					this.iocLocker.ExitReadLock();
				}
			}
		}

		/// <summary>
		/// Tries the invoke.
		/// </summary>
		/// <typeparam name="T">The type of the result.</typeparam>
		/// <param name="func">The delegate.</param>
		/// <param name="object">The @object.</param>
		/// <returns>The result of invocation.</returns>
		private bool TryInvokeReadLock<T>(Func<T> func, out T @object)
		{
			if (!this.iocLocker.IsReadLockHeld && !this.iocLocker.IsWriteLockHeld)
			{
				this.iocLocker.EnterReadLock();
			}

			bool retval = false;

			try
			{
				@object = func();
				retval = true;
			}
			catch (Exception exception)
			{
				Log.Warn(exception.Message, exception, typeof(IoCContainerCore));
				@object = default(T);
			}
			finally
			{
				if (this.iocLocker.IsReadLockHeld)
				{
					this.iocLocker.ExitReadLock();
				}
			}

			return retval;
		}

		/// <summary>
		/// Tries the invoke.
		/// </summary>
		/// <param name="func">The delegate to run.</param>
		/// <param name="object">The @object.</param>
		/// <returns>The result of invocation.</returns>
		private bool TryInvokeReadLock(Func<object> func, out object @object)
		{
			bool retval = false;
			if (!this.iocLocker.IsReadLockHeld && !this.iocLocker.IsWriteLockHeld)
			{
				this.iocLocker.EnterReadLock();
			}

			try
			{
				@object = func();
				retval = true;
			}
			catch (Exception exception)
			{
				Log.Warn(exception.Message, exception, typeof(IoCContainerCore));
				@object = default(object);
			}
			finally
			{
				if (this.iocLocker.IsReadLockHeld)
				{
					this.iocLocker.ExitReadLock();
				}
			}

			return retval;
		}
	}
}
