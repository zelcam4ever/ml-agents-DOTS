using Unity.Entities;

namespace Zelcam4.MLAgents
{
    [UpdateAfter(typeof(EpisodeCompletedGroup))]
    public partial class TagResetGroup : ComponentSystemGroup {}
    
    // Reset EpisodeCompletedTag
    [UpdateInGroup(typeof(TagResetGroup))]
    public partial struct EpisodeCompletedResetSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (agent, entity) in 
                     SystemAPI.Query<RefRO<AgentEcs>>().WithEntityAccess()
                         .WithAny<EndEpisodeTag>())
            {
                ecb.RemoveComponent<EndEpisodeTag>(entity);
            }
        }
    }
    
    // Reset Decision and Action Request Tags
    [UpdateInGroup(typeof(TagResetGroup))]
    public partial struct BehaviourResetSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (agent, entity) in 
                     SystemAPI.Query<RefRO<AgentEcs>>().WithEntityAccess()
                         .WithAny<RequestDecisionTag, RequestActionTag>())
            {
                ecb.RemoveComponent<RequestDecisionTag>(entity);
                ecb.RemoveComponent<RequestActionTag>(entity);
            }
        }
    }
}