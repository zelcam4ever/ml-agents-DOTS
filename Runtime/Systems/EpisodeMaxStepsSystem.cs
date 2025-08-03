using Unity.Burst;
using Unity.Entities;
using static Unity.Entities.SystemAPI;


namespace Zelcam4.MLAgents
{
    [UpdateAfter(typeof(RewardGroup))]
    public partial class EpisodeCompletedGroup : ComponentSystemGroup {}
    
    [BurstCompile]
    [UpdateInGroup(typeof(EpisodeCompletedGroup))]
    public partial struct EpisodeMaxStepsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            var job = new TerminateOnMaxStepsJob
            {
                ECB = ecb.AsParallelWriter()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    [BurstCompile]
    public partial struct TerminateOnMaxStepsJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        
        private void Execute([ChunkIndexInQuery] int entityInQueryIndex, Entity entity, ref AgentEcs agent)
        {
            if (agent.MaxStep > 0 && agent.StepCount >= agent.MaxStep)
            {
                agent.MaxStepReached = true;
                ECB.AddComponent<EndEpisodeTag>(entityInQueryIndex, entity);
            }
        }
    }
}