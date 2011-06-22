using System.Diagnostics.CodeAnalysis;

namespace IoC
{
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification="IOCContainer just looks horrible and will cause far to much code churn")]
    public interface IIoCRegistration
    {
        void BuildContainer(IoCContainerCore container);
    }
}
