namespace FleetControl.Domain.Shared;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
