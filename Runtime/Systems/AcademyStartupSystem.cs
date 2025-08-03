using Unity.Entities;

namespace Zelcam4.MLAgents
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct AcademyStartupSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;

            // Check if an entity with the AcademyTraining already exists
            var query = SystemAPI.QueryBuilder().WithAll<AcademyTraining>().Build();
            if (!query.IsEmpty)
            {
                return;
            }
            
            var academyEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<AcademyTraining>(academyEntity);
        }
    }
}

