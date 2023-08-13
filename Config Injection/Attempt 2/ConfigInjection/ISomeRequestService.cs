namespace ConfigInjection;

/// <summary>
/// Interface for a value requesting service. This service doesn't
/// need to run all the time, it's simply a injected class that
/// provides a value based on a query.
/// </summary>
public interface ISomeRequestService
{
    public string SomeValue(string query);
}
