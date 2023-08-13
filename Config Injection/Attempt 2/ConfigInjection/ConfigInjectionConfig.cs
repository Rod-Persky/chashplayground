namespace ConfigInjection;

/// <summary>
/// The configuration that will be injected into the
/// service. It's pretty basic.
/// </summary>
public record ConfigInjectionConfig
{
    public bool SomeParameter { get; init; }
}
