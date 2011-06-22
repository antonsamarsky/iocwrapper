using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Web.Compilation;
using Logging.Common;
using Microsoft.Practices.Unity;
using System.Threading;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Unity.Configuration;

namespace IoC
{
    /// <summary>
    /// The actual implementaion of the IoContainer wrapper made availabe to applications built on top of the
    /// MaterialCore stack. This class is intended to be threadsafe.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "IOCContainer just looks horrible and will cause far to much code churn")]
    public class IoCContainerCore : IDisposable
    {
        private IUnityContainer _container;
        private bool _initialised;
        private ReaderWriterLock _rwl = new ReaderWriterLock();
        private bool _disposed;

        /// <summary>
        /// Default constructor
        /// </summary>
        public IoCContainerCore()
        {
            _container = new UnityContainer();
        }

        /// <summary>
        /// Initialised the IoC Container using the poet configuration (this is performed implicitly if the container is
        /// used prior to being explicitly initialised)
        /// </summary>
        public void InitialiseContainer()
        {
            Debug.Assert(null != _container);

            _container = new UnityContainer();

            UnityConfigurationSection section
                = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            if (null != section && null != section.Containers && null != section.Containers.Default) // did they suply some *.config initialisation?
                section.Containers.Default.Configure(_container);

            _initialised = true;

        }

        public void InitialiseContainer(IList<string> assemblies, string[] configurations)
        {
            InitialiseContainer(assemblies, configurations, true);
        }

        /// <summary>
        /// Initialisation method that allows the user to explicitly define the assemblies and configuration mode that
        /// should be used to configure the IoCContainer.  This is intended for unit tests and programs that do not have
        /// a poet configuration
        /// </summary>
        /// <param name="assemblies">The name of the assmeblies that should be scanned for IoCContainer registration modules</param>
        /// <param name="configs">The name of the modes that should be used when determining the IoCContainer registration modules
        /// that should be invoked.</param>
        public void InitialiseContainer(IList<string> assemblies, string[] configurations, bool includeRelease)
        {
            Debug.Assert(null != _container);

            try
            {
                _rwl.AcquireReaderLock(1000);

                IList<Assembly> a = new List<Assembly>();

                if (null != assemblies)
                {
                    foreach (string assembly in assemblies)
                    {
                        a.Add(Assembly.Load(assembly));
                    }
                }

                InitialiseContainer(a, configurations, includeRelease);

            }
            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform default IoC Container initialisation", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        public void InitialiseContainer(IList<Assembly> assemblies, string[] configurations)
        {
            InitialiseContainer(assemblies, configurations, true);
        }

        /// <summary>
        /// Initialisation method that allows the user to explicitly define the assemblies and configuration mode that
        /// should be used to configure the IoCContainer.  This is intended for unit tests and programs that do not have
        /// a poet configuration
        /// </summary>
        /// <param name="assemblies">The name of the assmeblies that should be scanned for IoCContainer registration modules</param>
        /// <param name="configs">The name of the modes that should be used when determining the IoCContainer registration modules
        /// that should be invoked.</param>
        public void InitialiseContainer(IList<Assembly> assemblies, string[] configurations, bool includeRelease)
        {
            Debug.Assert(null != _container);

            try
            {
                _rwl.AcquireReaderLock(1000);

                /* Logger.LogMessage(
                     LoggerLevel.Info,
                     "+++ IoCContainerImpl::InitialiseContainer(assemblies, configs) - Starting container initialisation",
                     null);*/

                _container = new UnityContainer();

                UnityConfigurationSection section
                    = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                
                if (null != section && null != section.Containers.Default) // did they suply some *.config initialisation?
                    section.Containers.Default.Configure(_container);

                foreach (Assembly a in assemblies)
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (!t.IsInterface && typeof(IIoCRegistration).IsAssignableFrom(t))
                        {
                            InvokeIoCRegistration(t, configurations, includeRelease);
                            break;
                        }
                    }
                }

                _initialised = true;

                /*Logger.LogMessage(
                    LoggerLevel.Info,
                    "+++ IoCContainerImpl::InitialiseContainer(assemblies, configs) - Finished container initialisation",
                    null);*/
            }
            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform custom IoC Container initialisation", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Resets the container.
        /// </summary>
        public void ResetContainer()
        {
            try
            {
                _rwl.AcquireWriterLock(1000);

                _container = new UnityContainer();
                _initialised = false;

                /* Logger.LogMessage(
                     LoggerLevel.Info,
                     "+++ IoCContainerImpl::ResetContainer() - IoC Container has been reset",
                     null);*/
            }
            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to reset the IoC Container initialisation", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsWriterLockHeld)
                    _rwl.ReleaseWriterLock();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Matches underlying container syntax.")]
        public T Resolve<T>()
        {
            Debug.Assert(null != _container);
            Debug.Assert(_initialised);

            try
            {
                _rwl.AcquireReaderLock(1000);

                EnsureContainerIsInitialsied();
                return _container.Resolve<T>();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Unable to perform resolve on the IoC Container", ex,Area.IoCioCContainerCore);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Matches underlying container syntax.")]
        public T Resolve<T>(string name)
        {
            Debug.Assert(null != _container);
            Debug.Assert(_initialised);

            if (null == name)
                throw new ArgumentNullException("name");

            try
            {
                _rwl.AcquireReaderLock(1000);

                EnsureContainerIsInitialsied();
                return _container.Resolve<T>(name);
            }
            catch (Exception ex)
            {
                // Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform resolve on the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "No clear documentation available for what unity will throw.")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            Justification = "Pattern matches existing Try methods.")]
        public bool TryResolve<T>(out T obj)
        {
            Debug.Assert(null != _container);
            //Debug.Assert(_initialised);

            bool retval = false;

            try
            {
                _rwl.AcquireReaderLock(1000);

                EnsureContainerIsInitialsied();

                obj = _container.Resolve<T>();
                retval = true;
            }
            catch (Exception ex)
            {
                // Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform resolve on the IoC Container", ex);
                obj = default(T);
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }

            return retval;
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "No clear documentation available for what unity will throw.")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            Justification = "Pattern matches existing Try methods.")]
        public bool TryResolve<T>(string name, out T obj)
        {
            Debug.Assert(null != _container);
            //Debug.Assert(_initialised);

            if (null == name)
                throw new ArgumentNullException("name");

            bool retval = false;

            try
            {
                _rwl.AcquireReaderLock(1000);

                EnsureContainerIsInitialsied();

                obj = _container.Resolve<T>(name);
                retval = true;
            }
            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform resolve on the IoC Container", ex);
                obj = default(T);
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }

            return retval;
        }

        [SuppressMessage(
            "Microsoft.Naming",
            "CA1702:CompoundWordsShouldBeCasedCorrectly",
            Justification = "Matches the underlying unity methods signature.")]
        public void BuildUp<T>(T obj) where T : class
        {
            Debug.Assert(null != _container);
            Debug.Assert(_initialised);

            try
            {
                _rwl.AcquireReaderLock(1000);

                EnsureContainerIsInitialsied();
                _container.BuildUp<T>(obj);
            }
            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform BuildUp using the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }


        [SuppressMessage(
    "Microsoft.Naming",
    "CA1702:CompoundWordsShouldBeCasedCorrectly",
    Justification = "Matches the underlying unity methods signature.")]
        public void BuildUp(Type type, Object value)
        {
            Debug.Assert(null != _container);
            Debug.Assert(_initialised);

            try
            {
                _rwl.AcquireReaderLock(1000);

                EnsureContainerIsInitialsied();
                _container.BuildUp(type, value);
            }
            catch (Exception ex)
            {
                // Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform BuildUp using the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Matches underlying container syntax.")]
        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            Debug.Assert(null != _container);

            try
            {
                _rwl.AcquireReaderLock(1000);

                _container.RegisterType<TFrom, TTo>();

                /* Logger.LogMessage(
                     LoggerLevel.Info,
                     "+++ IoCContainerImpl::RegisterType<TFrom, TTo>() - registered mapping from {0} to {1}",
                     null,
                     typeof(TFrom).AssemblyQualifiedName.ToString(),
                     typeof(TTo).AssemblyQualifiedName.ToString());*/
            }
            catch (Exception ex)
            {
                // Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform register type using the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Matches underlying container syntax.")]
        public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            Debug.Assert(null != _container);

            if (null == name)
                throw new ArgumentNullException("name");

            try
            {
                _rwl.AcquireReaderLock(1000);

                _container.RegisterType<TFrom, TTo>(name);

                /*Logger.LogMessage(
                    LoggerLevel.Info,
                    "+++ IoCContainerImpl::RegisterType<TFrom, TTo>() - registered mapping from {0} to {1}",
                    null,
                    typeof(TFrom).AssemblyQualifiedName.ToString(),
                    typeof(TTo).AssemblyQualifiedName.ToString());*/
            }
            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform register type using the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        public void RegisterType(Type from, Type to)
        {
            Debug.Assert(null != _container);

            try
            {
                _rwl.AcquireReaderLock(1000);

                _container.RegisterType(from, to);

                /* Logger.LogMessage(
                     LoggerLevel.Info,
                     "+++ IoCContainerImpl::RegisterType<TFrom, TTo>() - registered mapping from {0} to {1}",
                     null,
                     from.AssemblyQualifiedName.ToString(),
                     to.AssemblyQualifiedName.ToString());*/
            }
            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform register type using the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        public void RegisterType(Type from, Type to, string name)
        {
            Debug.Assert(null != _container);

            if (null == name)
                throw new ArgumentNullException("name");

            try
            {
                _rwl.AcquireReaderLock(1000);

                _container.RegisterType(from, to, name);

                /* Logger.LogMessage(
                     LoggerLevel.Info,
                     "+++ IoCContainerImpl::RegisterType<TFrom, TTo>() - registered mapping from {0} to {1}",
                     null,
                     from.AssemblyQualifiedName.ToString(),
                     to.AssemblyQualifiedName.ToString());*/
            }
            catch (Exception ex)
            {
                // Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform register type using the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        public bool IsInitialised
        {
            get { return _initialised; }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Marked as obsolete and will be removed.")]
        [Obsolete("Use the version 'InvokeRegistrationMethod(Type type)'")]
        public void InvokeIoCRegistrationFixture<T>() where T : IIoCRegistration
        {
            InvokeIoCRegistrationFixture(typeof(T));
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "IOCContainer just looks horrible and will cause far to much code churn")]
        public void InvokeIoCRegistrationFixture(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentException("No type specified.", "typeName");

            Type type = Type.GetType(typeName);
            System.Diagnostics.Debug.Assert(null != type);

            InvokeIoCRegistrationFixture(type);
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "IOCContainer just looks horrible and will cause far to much code churn")]
        public void InvokeIoCRegistrationFixture(Type type)
        {
            if (null == type)
                throw new ArgumentNullException("type");

            try
            {
                _rwl.AcquireReaderLock(1000);

                IIoCRegistration reg = null;

                if (!ReflectionHelper.TryCreateTypeFromDefaultConstructor<IIoCRegistration>(type, out reg))
                {
                    string error = string.Format(
                        CultureInfo.CurrentCulture,
                        "Unable to create an instance of '{0}' when configuring the IoC Container (does it have a default constructor).",
                        type.ToString());

                    /*  Logger.LogMessage(
                          LoggerLevel.Warn,
                          "*** IoCContainer::InvokeIoCRegistrationFixture(type) - Class '{0}' unable create instance (does it have a default constructor?).",
                          null,
                          type.ToString());*/

                    throw new Exception(error);
                }

                Debug.Assert(null != reg);

                reg.BuildContainer(this);

            }

            catch (Exception ex)
            {
                //Logger.LogMessage(LoggerLevel.Fatal, "Unable to perform register type using the IoC Container", ex);
                throw;
            }
            finally
            {
                if (_rwl.IsReaderLockHeld)
                    _rwl.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Ensures that at the very least the default IoCContainer initialisation has been performed
        /// </summary>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "No clear documentation available for what unity will throw.")]
        private void EnsureContainerIsInitialsied()
        {
            Debug.Assert(_rwl.IsReaderLockHeld);

            LockCookie lc = new LockCookie();

            try
            {
                lc = _rwl.UpgradeToWriterLock(1000); // block everyone

                if (!IsInitialised)
                    InitialiseContainer();
            }
            catch (Exception ex)
            {
                // Logger.LogMessage(LoggerLevel.Error, "Unable to ensure the IoC Container was initialised.", ex);
            }
            finally
            {
                if (_rwl.IsWriterLockHeld)
                    _rwl.DowngradeFromWriterLock(ref lc);
            }
        }

        private void InvokeIoCRegistration(Type type, string[] configurations, bool includeRelease)
        {
            Debug.Assert(null != type);

            //type.GetMethod("BuildContainer").Invoke(ReflectionHelper.CreateTypeFromDefaultConstructor(type, false, ""), new object[] { _container });
            IoCRegistrationAttribute attr =
                Attribute.GetCustomAttribute(type, typeof(IoCRegistrationAttribute)) as IoCRegistrationAttribute;

            if (null == attr)
            {
                /*Logger.LogMessage(
                    LoggerLevel.Warn,
                    "+++ IoCContainerImpl::InvokeIoCRegistration(type, configurations, includeRelease) - unable to find expected IoCRegistration attribute on '{0}'.",
                    null,
                    type.ToString());*/
            }
            else
            {
                if ((attr.Mode == IoCRegistrationMode.Release && includeRelease) ||
                    (null != configurations && (attr.Mode == IoCRegistrationMode.Custom && configurations.Contains(attr.CustomMode)))) // a little more verbose than is maybe necessary
                {
                    InvokeIoCRegistrationFixture(type);
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose managed resources.
                    if (null != _container)
                        _container.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
