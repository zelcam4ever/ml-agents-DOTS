using Unity.Entities;

namespace Zelcam4.MLAgents
{
    public struct AgentEcs : IComponentData
    {
        public float Reward;
        public float GroupReward;
        public float CumulativeReward;
        public int MaxStep;
        public int StepCount;
        public int CompletedEpisodes;
        public int EpisodeId;
        public bool Initialized;

        public int GroupId;
        public bool IsEnabled;      

        //Adding From AgentInfo
        public bool MaxStepReached;
        public bool Done;
        public bool StartingEpisode;
    }
    
    public struct EndEpisodeTag : IComponentData {}
    public struct EpisodeBeginTag : IComponentData {}
}