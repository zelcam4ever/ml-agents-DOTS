using Unity.Entities;


namespace Zelcam4.MLAgents
{
    [UpdateAfter(typeof(ActionSystemGroup))]
    public partial class RewardGroup : ComponentSystemGroup {}
    
}