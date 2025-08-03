using Unity.Entities;

namespace Zelcam4.MLAgents
{
    // Automatic decision and action requests
    public struct DecisionRequest : IComponentData
    {
        public int DecisionPeriod;
        public int DecisionStep;
        public bool TakeActionsBetweenDecisions;
    }
    
    // Tags to define the agent behavior
    public struct RequestDecisionTag : IComponentData {}
    public struct RequestActionTag : IComponentData {}
}