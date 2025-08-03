using Unity.Burst;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    // Needed to set the done flag for PyTorch training
    [BurstCompile]
    [UpdateAfter(typeof(EpisodeCompletedGroup))]
    public partial struct AgentIsDoneSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new SetDoneFlagJob();
            
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct SetDoneFlagJob : IJobEntity
    {
        private void Execute(ref AgentEcs agent, in EndEpisodeTag tag)
        {
            agent.Done = true;
        }
    }
}