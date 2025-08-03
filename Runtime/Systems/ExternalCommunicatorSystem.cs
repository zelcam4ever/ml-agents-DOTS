using Unity.Entities;

using static Unity.Entities.SystemAPI;

namespace Zelcam4.MLAgents
{
    [UpdateAfter(typeof(ObservationCollectionGroup))]
    public partial class ExternalCommunicatorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
            foreach (var (agent, policy, actionsStructure, observations) in
                     Query<RefRW<AgentEcs>, RefRO<BrainSimple>, RefRO<ActionsStructure>, DynamicBuffer<ObservationValue>>()
                         .WithAll<RemotePolicy,RequestDecisionTag>())
            {
                // Will try to subscribe the same brain multiple times, but we can assure all agents try at least once 
                if (!agent.ValueRO.Initialized)
                {
                    CommunicatorManager.SubscribeBrain(policy.ValueRO.FullyQualifiedBehaviorName.Value, actionsStructure.ValueRO);
                    agent.ValueRW.Initialized = true;
                }
                
                CommunicatorManager.PutObservation(policy.ValueRO.FullyQualifiedBehaviorName.Value, agent.ValueRO, observations);
                
                agent.ValueRW.Reward = 0f;
                agent.ValueRW.GroupReward = 0f;
                
            }
        }
    }
}