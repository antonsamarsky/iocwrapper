namespace IoC
{
	/// <summary>
	/// The IoCRegistration interface.
	/// </summary>
	public interface IIoCRegistration
	{
		/// <summary>
		/// Builds the IoC container.
		/// </summary>
		/// <param name="container">
		/// The container.
		/// </param>
		void BuildContainer(IoCContainerCore container);
	}
}
