using Unity.Core;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    [UpdateBefore(typeof(RequesterSystem))]
    public partial struct InitializeEnvironmentSystem : ISystem
    {
        private bool m_IsInitialized;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AcademyTraining>();
            m_IsInitialized = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            // Exit if we have already initialized.
            if (m_IsInitialized)
            {
                // Disable this system so it doesn't run again.
                state.Enabled = false;
                return;
            }

            // Get the elapsed time from the start of the application.
            var elapsedTime = state.World.Time.ElapsedTime;

            // Wait for 10 seconds before attempting to initialize.
            if (elapsedTime > 5.0f)
            {
                // Call your existing initialization method.
                CommunicatorManager.AwakeCalled();

                // Find your singleton Academy entity.
                var academyEntity = SystemAPI.GetSingletonEntity<AcademyTraining>();

                // Add the 'Connected' tag to the Academy entity.
                state.EntityManager.AddComponent<Connected>(academyEntity);
                state.EntityManager.AddComponent<Testing>(academyEntity);

                // Mark as initialized so this logic doesn't run again.
                m_IsInitialized = true;
            }
        }
    }
}
    

