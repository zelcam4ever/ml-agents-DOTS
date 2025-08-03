using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    [UpdateAfter(typeof(ExternalCommunicatorSystem))]
    public partial class DecideActionSystem : SystemBase
    {
        private AgentAction _jobDataCache = new AgentAction();
        protected override void OnUpdate()
        {
            var uniqueBrainNames = new NativeHashSet<FixedString32Bytes>(16, Allocator.Temp);
            
            // We search for the N brains once, and then we iterate over each one. Doing so we avoid doing string lookups for each agent
            foreach (var (brain, agent) in 
                     SystemAPI.Query<RefRO<BrainSimple>, RefRW<AgentEcs>>().WithAll<RequestDecisionTag>())
            {
                uniqueBrainNames.Add(brain.ValueRO.FullyQualifiedBehaviorName.Value);
            }

            if (uniqueBrainNames.Count == 0) return;
            
            CommunicatorManager.DecideAction();
            
            foreach (var brainName in uniqueBrainNames)
            {
                // Expensive lookup that is performed once per brain, instead of once per agent
                var actionsForThisBrain = CommunicatorManager.GetActionsForBrain(brainName);
                if (actionsForThisBrain == null || actionsForThisBrain.Count == 0) continue;
                
                var nativeActions = new NativeHashMap<int, AgentAction>(
                    actionsForThisBrain.Count,
                    Allocator.TempJob
                );

                foreach (var (agentId, actionBuffer) in actionsForThisBrain)
                {
                    foreach (var val in actionBuffer.ContinuousActions) { _jobDataCache.ContinuousActions.Add(val); }
                    foreach (var val in actionBuffer.DiscreteActions) { _jobDataCache.DiscreteActions.Add(val); }
                    
                    nativeActions.Add(agentId, _jobDataCache);
                    _jobDataCache.ContinuousActions.Clear();
                    _jobDataCache.DiscreteActions.Clear();
                }
                
                var job = new DistributeActionsJob
                {
                    ActionsToDistribute = nativeActions
                };
                
                var brainFilter = new BrainSimple { FullyQualifiedBehaviorName = brainName };
                
                // Chain the disposal of the nativeActions to the job's dependency handle
                Dependency = nativeActions.Dispose(job.ScheduleParallel(
                    GetEntityQuery(typeof(AgentEcs),
                        ComponentType.ReadWrite<AgentAction>(),
                        ComponentType.ReadOnly(brainFilter.GetType())),
                        Dependency));
            }
        }
    }
    
    [BurstCompile]
    public partial struct DistributeActionsJob : IJobEntity
    {
        [ReadOnly]
        public NativeHashMap<int, AgentAction> ActionsToDistribute;
        
        public void Execute(in AgentEcs agent, RefRW<AgentAction> actionComponent)
        {
            if (!ActionsToDistribute.TryGetValue(agent.EpisodeId, out var actionData)) return;
            
            actionComponent.ValueRW.ContinuousActions = actionData.ContinuousActions;
            actionComponent.ValueRW.DiscreteActions = actionData.DiscreteActions;
        }
    }
}