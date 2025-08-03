using Unity.Entities;
using UnityEngine;
using static Unity.Entities.SystemAPI;

namespace Zelcam4.MLAgents
{
    [UpdateAfter(typeof(AgentIsDoneSystem))]
    public partial class PreAgentResetSystemGroup : ComponentSystemGroup {}
    
    [UpdateInGroup(typeof(PreAgentResetSystemGroup))]
    public partial class AgentOnCompletedEpidoseObservationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (agent, policy, observations) in
                     Query<RefRW<AgentEcs>, RefRO<BrainSimple>, DynamicBuffer<ObservationValue>>()
                         .WithAll<RemotePolicy,EndEpisodeTag>())
            {
                CommunicatorManager.PutObservation(policy.ValueRO.FullyQualifiedBehaviorName.Value, agent.ValueRO, observations);
            }
        }
    }
}