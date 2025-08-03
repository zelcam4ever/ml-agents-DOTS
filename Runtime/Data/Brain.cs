using Unity.Collections;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    public struct BrainSimple: IComponentData
    {
        public FixedString32Bytes FullyQualifiedBehaviorName;
    }
    
    public struct RemotePolicy : IComponentData { }
    public struct InferencePolicy : IComponentData { }
    public struct HeuristicPolicy : IComponentData { }
    
}
