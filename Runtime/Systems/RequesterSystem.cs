using Unity.Entities;
using static Unity.Entities.SystemAPI;

namespace Zelcam4.MLAgents
{
    /// <summary>
    /// Equivalent to DecisionRequester. Iterates over all Agents and check whether they request Decision or Actions
    /// </summary>
    [UpdateAfter(typeof(InitializeEnvironmentSystem))]
    public partial struct RequesterSystem: ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AcademyTraining>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            var academyStepCount = GetSingleton<AcademyTraining>().StepCount;


            foreach (var (decisionRequest, entity) in Query<RefRO<DecisionRequest>>().WithEntityAccess())
            {
                if (academyStepCount % decisionRequest.ValueRO.DecisionPeriod == decisionRequest.ValueRO.DecisionStep)
                {
                    ecb.AddComponent<RequestDecisionTag>(entity);
                    ecb.AddComponent<RequestActionTag>(entity);
                }
                else if (decisionRequest.ValueRO.TakeActionsBetweenDecisions)
                {
                    ecb.AddComponent<RequestActionTag>(entity);
                }
            }
        }
    }
}