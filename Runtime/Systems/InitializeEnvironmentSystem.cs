using Unity.Entities;

namespace Zelcam4.MLAgents
{
    [UpdateBefore(typeof(RequesterSystem))]
    public partial struct InitializeEnvironmentSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            CommunicatorManager.AwakeCalled();
        }
    }
}
    

