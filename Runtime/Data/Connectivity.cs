using Unity.Entities;

namespace Zelcam4.MLAgents
{
    /// <summary>
    /// A tag component added to a singleton entity (e.g., the Academy)
    /// to signal that the gRPC communication has been successfully initialized.
    /// </summary>
    public struct Connected : IComponentData {}
    public struct Testing : IComponentData {}
}