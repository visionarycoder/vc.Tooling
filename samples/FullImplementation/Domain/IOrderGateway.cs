using VisionaryCoder.Architecture;

namespace FullImplementation.Domain;

/// <summary>Access vault contract for order persistence.</summary>
[Component(role: ComponentRole.Access)]
public interface IOrderGateway
{
    Task PersistAsync(Order order, CancellationToken cancellationToken = default);
}