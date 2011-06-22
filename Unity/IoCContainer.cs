using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace IoC
{
    /// <summary>
    /// Singleton access to the IoC container
    /// </summary>
    [SuppressMessage(
        "Microsoft.Naming", 
        "CA1709:IdentifiersShouldBeCasedCorrectly", 
        Justification = "IOCContainer just looks horrible and will cause far to much code churn.")]
    public static class IoCContainer
    {
        #region Constructors

        /// <summary>
        /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        /// </summary>
        [SuppressMessage(
        "Microsoft.Performance",
        "CA1810:InitializeReferenceTypeStaticFieldsInline",
        Justification = "Static constructor required for BeforeFieldInit flag.")]
        static IoCContainer()
        {
        }

        #endregion

        #region Public Methods

        public static bool TryResolve<T>(out T obj)
        {
            Debug.Assert(null != _container);

            bool retval = false;
            obj = default(T);
            
            retval = _container.TryResolve<T>(out obj);            
            
            return retval;
        }

        public static bool TryResolve<T>(string name, out T obj)
        {
            Debug.Assert(null != _container);

            bool retval = false;
            obj = default(T);

            retval = _container.TryResolve<T>(name, out obj);
            
            return retval;
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Matches the underlying unity methods signature.")]
        public static T Resolve<T>()
        {
            Debug.Assert(null != _container);
            return _container.Resolve<T>();
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Matches the underlying unity methods signature.")]
        public static T Resolve<T>(string name)
        {
            Debug.Assert(null != _container);
            return _container.Resolve<T>(name);
        }

        [SuppressMessage(
            "Microsoft.Naming",
            "CA1702:CompoundWordsShouldBeCasedCorrectly",
            Justification = "Matches the underlying unity methods name.")]
        public static void BuildUp<T>(T obj) where T : class
        {
            Debug.Assert(null != _container);
            _container.BuildUp<T>(obj);
        }

        [SuppressMessage(
    "Microsoft.Naming",
    "CA1702:CompoundWordsShouldBeCasedCorrectly",
    Justification = "Matches the underlying unity methods name.")]
        public static void BuildUp(Type type, Object value)
        {
            Debug.Assert(null != _container);
            _container.BuildUp(type, value);
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Matches the underlying unity methods signature.")]
        public static void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            Debug.Assert(null != _container);
            _container.RegisterType<TFrom, TTo>();
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Matches the underlying unity methods signature.")]
        public static void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            Debug.Assert(null != _container);
            _container.RegisterType<TFrom, TTo>(name);
        }

        public static void RegisterType(Type from, Type to)
        {
            Debug.Assert(null != _container);
            _container.RegisterType(from, to);
        }

        public static void RegisterType(Type from, Type to, string name)
        {
            Debug.Assert(null != _container);
            _container.RegisterType(from, to, name);
        }

        /// <summary>
        /// Perform custom container initialisation using only the IoCRegistration points that match one of the specified modes
        /// (This is intended for unit tests that want to uses a custom set of type mappings for moc'ing)
        /// </summary>
        /// <param name="configs"></param>
        public static void InitialiseContainer(string[] assemblies, string[] configurations)
        {
            Debug.Assert(null != _container);
            List<string> configAssemblies = GetConfigAssemblies();

            if(null != assemblies) // check to see if we have anything to add
                configAssemblies.AddRange(assemblies);

            _container.InitialiseContainer(configAssemblies.ToArray(), configurations);
        }

        /// <summary>
        /// Perform custom container initialisation using only the IoCRegistration points that match one of the specified modes
        /// (This is intended for unit tests that want to uses a custom set of type mappings for moc'ing)
        /// 
        /// This method will try to relosve assemblies via configuration file
        /// </summary>
        /// <param name="configs"></param>
        public static void InitialiseContainer()
        {
            Debug.Assert(null != _container);
            List<string> configAssemblies = GetConfigAssemblies();
            _container.InitialiseContainer(configAssemblies.ToArray(), null);     
        }

        private static List<string> GetConfigAssemblies()
        {
            var section =
                ConfigurationManager.GetSection(IoCAssemblyRegistration.ConfigurationSectionName) as
                IoCAssemblyRegistration;

            var configAssemblies = new List<string>();
            if (section != null)
            {
                foreach (AssemblyElement element in section.Assemblies)
                    configAssemblies.Add(element.Assembly);
            }

            return configAssemblies;
        }

        /// <summary>
        /// Resets the container (really only useful for unit tests)
        /// </summary>
        public static void ResetContainer()
        {
            Debug.Assert(null != _container);
            _container.ResetContainer();
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Marked as obsolete and will be removed.")]
        [Obsolete("Use the version 'InvokeRegistrationMethod(Type type)'")]
        public static void InvokeRegistrationMethod<T>() where T : IIoCRegistration
        {
            Debug.Assert(null != _container);
            _container.InvokeIoCRegistrationFixture<T>();
        }

        public static void InvokeRegistrationMethod(Type type)
        {
            Debug.Assert(null != _container);
            _container.InvokeIoCRegistrationFixture(type);
        }

        public static void InvokeRegistrationMethod(string type)
        {
            Debug.Assert(null != _container);
            _container.InvokeIoCRegistrationFixture(type);
        }
        #endregion

        #region Member Variables          
        private static IoCContainerCore _container = new IoCContainerCore();
        #endregion
    }
}
