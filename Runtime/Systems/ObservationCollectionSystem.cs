using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace Zelcam4.MLAgents
{
    
    //Partial class defined to allow for generic type arguments in attributes (UpdateAfter)
    [UpdateAfter(typeof(IncrementStepSystem))]
    public partial class ObservationCollectionGroup : ComponentSystemGroup {}

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