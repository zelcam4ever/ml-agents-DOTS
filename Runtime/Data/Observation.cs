using Unity.Entities;

namespace Zelcam4.MLAgents
{
    public struct ObservationValue : IBufferElementData
    {
        public float Value;
        
        // Convenience
        public static implicit operator float(ObservationValue e) { return e.Value; }
        public static implicit operator ObservationValue(float f) { return new ObservationValue { Value = f }; }
    }
    
    /// <summary>
    /// A generic request to observe a value from a component of type T.
    /// </summary>
    public struct ObservationRequest<T> : IBufferElementData where T : IComponentData
    {
        // ID to define which value to get from the component T
        public byte SourceSubId;

        // The index in the final observation buffer where this value should be written
        public int TargetIndex;
    }
    

    /// <summary>
    /// Interface for a struct that knows how to extract a float value
    /// from a component of type T based on a sub-ID.
    /// </summary>
    public interface IObservationExtractor<T> where T : IComponentData
    {
        float Extract(in T component, byte sourceSubId);
    }
}