using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    
    //Partial class defined to allow for generic type arguments in attributes (UpdateAfter)
    [UpdateAfter(typeof(IncrementStepSystem))]
    public partial class ObservationCollectionGroup : ComponentSystemGroup {}
    
    [UpdateInGroup(typeof(ObservationCollectionGroup))]
    public partial class ObservationCollectionSystem<T, TExtractor> : SystemBase
        where T : unmanaged, IComponentData
        where TExtractor : struct, IObservationExtractor<T>
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<RequestDecisionTag,ObservationValue, ObservationRequest<T>, T>()
                .Build(this);
        }

        protected override void OnUpdate()
        {
            var job = new GatherJob<T, TExtractor>
            {
                FinalObservationBufferHandle = GetBufferTypeHandle<ObservationValue>(false),
                RequestsHandle = GetBufferTypeHandle<ObservationRequest<T>>(true),
                SourceComponentHandle = GetComponentTypeHandle<T>(true),
                Extractor = default
            };
            
            this.Dependency = job.ScheduleParallel(_query, this.Dependency);
        }
    }

    [BurstCompile]
    public struct GatherJob<T, TExtractor> : IJobChunk
        where T : unmanaged, IComponentData
        where TExtractor : struct, IObservationExtractor<T>
    {
        // Type handles are used to get data from the chunk
        public BufferTypeHandle<ObservationValue> FinalObservationBufferHandle;
        [ReadOnly] public BufferTypeHandle<ObservationRequest<T>> RequestsHandle;
        [ReadOnly] public ComponentTypeHandle<T> SourceComponentHandle;

        public TExtractor Extractor;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            // Get accessors for the data in this chunk
            var finalObservationAccessor = chunk.GetBufferAccessor(ref FinalObservationBufferHandle);
            var requestsAccessor = chunk.GetBufferAccessor(ref RequestsHandle);
            var sourceComponentArray = chunk.GetNativeArray(ref SourceComponentHandle);

            // Loop through each entity in the chunk
            for (int i = 0; i < chunk.Count; i++)
            {
                var finalObservationBuffer = finalObservationAccessor[i];
                var requests = requestsAccessor[i];
                var sourceComponent = sourceComponentArray[i];
            
                // The core logic is the same as before
                foreach (var request in requests)
                {
                    float observation = Extractor.Extract(in sourceComponent, request.SourceSubId);
                    finalObservationBuffer[request.TargetIndex] = observation;
                }
            }
        }
    }
    
}