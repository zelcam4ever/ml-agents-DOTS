using Unity.Entities;
using static Unity.Entities.SystemAPI;

namespace Zelcam4.MLAgents
{
    /// <summary>
    /// Handles the episode steps, substituting the Academy event.
    /// </summary>
    [UpdateAfter(typeof(RequesterSystem))]
    public partial struct IncrementStepSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            GetSingletonRW<AcademyTraining>().ValueRW.StepCount+=1;
            GetSingletonRW<AcademyTraining>().ValueRW.TotalStepCount+=1;
            
            foreach (var agent in Query<RefRW<AgentEcs>>())
            {
                agent.ValueRW.StepCount += 1;
            }
        }
        
    }
}