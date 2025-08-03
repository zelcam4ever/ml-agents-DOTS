using System;
using Unity.Entities;
using UnityEngine;

namespace Zelcam4.MLAgents
{
    public class AgentAuthoring: MonoBehaviour
    {
        public string behaviourName = "a";
        public int maxStep;
        
        
        [Header("Actions")]
        [Tooltip("The number of continuous actions the agent can take.")]
        [Min(0)]
        public int numContinuousActions = 0;

        [Tooltip("An array defining the size of each discrete action branch.")]
        public int[] discreteBranchSizes = Array.Empty<int>();


        public bool decisionRequester = true;
        // These fields will only appear if the bool decisionRequester true
        
        /// <summary>
        /// The frequency with which the agent requests a decision. A DecisionPeriod of 5 means
        /// that the Agent will request a decision every 5 Academy steps. /// </summary>
        public int decisionPeriod = 2;
        
        /// <summary>
        /// By changing this value, the timing of decision
        /// can be shifted even among agents with the same decision period. The value can be
        /// from 0 to DecisionPeriod - 1.
        /// </summary>
        public int decisionStep = 0;
        
        /// <summary>
        /// Indicates whether the agent will take an action during the Academy steps where
        /// it does not request a decision. Has no effect when DecisionPeriod is set to 1.
        /// </summary>
        public bool takeActionsBetweenDecisions = true;
        
        
        private class Baker : Baker<AgentAuthoring>
        {
            public override void Bake(AgentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                // Agent baking
                AddComponent(entity, new AgentEcs() 
                {
                    MaxStep = authoring.maxStep,
                    EpisodeId = EpisodeIdCounter.GetEpisodeId(),
                });
                
                // Policy baking
                AddComponent(entity, new RemotePolicy());

                // Brain baking
                AddComponent(entity, new BrainSimple
                {
                    FullyQualifiedBehaviorName = authoring.behaviourName
                });
                
                // Actions runtime baking
                var actionComponent = new AgentAction();
                
                for (int i = 0; i < authoring.numContinuousActions; i++)
                {
                    actionComponent.ContinuousActions.Add(0f);
                }
                
                int numDiscreteBranches = authoring.discreteBranchSizes.Length;
                for (int i = 0; i < numDiscreteBranches; i++)
                {
                    actionComponent.DiscreteActions.Add(0);
                }
                AddComponent(entity, actionComponent);
        
                // Actions Spec baking
                var specComponent = new ActionsStructure
                {
                    NumContinuousActions = authoring.numContinuousActions
                };
                
                foreach (var branchSize in authoring.discreteBranchSizes)
                {
                    specComponent.DiscreteBranchSizes.Add(branchSize);
                }
                AddComponent(entity, specComponent);
                
                // Decision baking
                if (authoring.decisionRequester)
                {
                    AddComponent(entity, new DecisionRequest()
                    {
                        DecisionPeriod = authoring.decisionPeriod,
                        DecisionStep = authoring.decisionStep,
                        TakeActionsBetweenDecisions = authoring.takeActionsBetweenDecisions
                    });
                }
            }
        }
    }

    
}