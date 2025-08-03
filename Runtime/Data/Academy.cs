using Unity.Entities;

namespace Zelcam4.MLAgents
{
    /// <summary>
    /// Stores the global data, episodes, steps and areas.
    /// </summary>
    public struct AcademyTraining: IComponentData
    {
        public bool IsInitialized;
        public bool IsCommunicatorOn;
        public int EpisodeCount;
        public int StepCount;
        public int TotalStepCount;
        public bool HadFirstReset;
        public int NumAreas;
    }
    
    /// <summary>
    /// Tag to enable automatic stepping each FixedUpdate loop
    /// </summary>
    public struct AcademyAutomaticStepper : IComponentData, IEnableableComponent { }
}