namespace Logging
{
	/// <summary>
	/// The level of log information. 
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// No logging. 
		/// </summary>
		None = 0,

		/// <summary>
		/// Debug level. 
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Information level. 
		/// </summary>
		Info = 2,

		/// <summary>
		/// Warning level. 
		/// </summary>
		Warning = 3,

		/// <summary>
		/// Error level. 
		/// </summary>
		Error = 4,

		/// <summary>
		/// Fatal error level. 
		/// </summary>
		Fatal = 5
	}
}