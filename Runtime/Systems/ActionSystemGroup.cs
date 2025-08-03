using Unity.Entities;

namespace Zelcam4.MLAgents
{
    [UpdateAfter(typeof(DecideActionSystem))]
    public partial class ActionSystemGroup : ComponentSystemGroup {}
}