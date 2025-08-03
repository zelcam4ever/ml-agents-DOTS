using Unity.Collections;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    /// <summary>
    /// A component that stores the specs of the action space.
    /// This data is baked once and is used to interpret the AgentAction output.
    /// </summary>
    public struct ActionsStructure : IComponentData
    {
        public int NumContinuousActions;

        // This stores the size of each discrete branch, e.g., {2, 3, 4}.
        public FixedList64Bytes<int> DiscreteBranchSizes;
        
        public int NumDiscreteActions => DiscreteBranchSizes.Length;
    }
    
    /// <summary>
    /// A component that stores the action decided by the policy.
    /// This data changes every decision step.
    /// </summary>
    public struct AgentAction : IComponentData
    {
        public FixedList64Bytes<float> ContinuousActions;

        // This will store the chosen action for EACH discrete branch.
        // Its length will be equal to the number of branches.
        public FixedList64Bytes<int> DiscreteActions; 
    }
}