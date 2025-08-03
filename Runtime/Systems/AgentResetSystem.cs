using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    [BurstCompile]
    [UpdateAfter(typeof(PreAgentResetSystemGroup))]
    public partial struct AgentResetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new SetAgentResetJob();
            
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct SetAgentResetJob : IJobEntity
    {
        private void Execute(ref AgentEcs agent, in EndEpisodeTag tag)
        {
            agent.CompletedEpisodes += 1;
            agent.Reward = 0f;
            agent.GroupReward = 0f;
            agent.CumulativeReward = 0f;
            agent.StepCount = 0;
            agent.MaxStepReached = false;
            agent.Done = false;
            agent.StartingEpisode = true;
        }
    }
}