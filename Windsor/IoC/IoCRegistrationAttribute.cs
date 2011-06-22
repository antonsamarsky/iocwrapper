using System;
using System.Diagnostics.CodeAnalysis;

namespace IoC
{
	/// <summary>
	/// The IoC registration mode.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "IOCContainer just looks horrible and will cause far to much code churn")]
	public enum IoCRegistrationMode
	{
		/// <summary>
		/// The release mode
		/// </summary>
		Release,

		/// <summary>
		/// The custom mode.
		/// </summary>
		Custom
	}

	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "IOCContainer just looks horrible and will cause far to much code churn")]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class IoCRegistrationAttribute : Attribute
	{
		public IoCRegistrationMode Mode
		{
			get { return _mode; }
			set { _mode = value; }
		}

		public string CustomMode
		{
			get { return _customMode; }
			set { _customMode = value; }
		}

		private IoCRegistrationMode _mode = IoCRegistrationMode.Release;
		private string _customMode;
	}
}
