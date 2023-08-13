namespace ConfigInjection;

/// <summary>
/// Our promise to DI about what functions our
/// service has to offer.
/// </summary>
public interface IConfigInjectedService
{
    public bool GetTheValue();
}
