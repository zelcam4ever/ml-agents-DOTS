
using System.Collections.Generic;
using System.Linq;
using Zelcam4.MLAgents.CommunicatorObjects;
using System.Runtime.CompilerServices;
using Unity.Entities;


[assembly: InternalsVisibleTo("Zelcam4.ML-Agents.Editor")]
[assembly: InternalsVisibleTo("Zelcam4.ML-Agents.Editor.Tests")]
[assembly: InternalsVisibleTo("Zelcam4.ML-Agents.Runtime.Utils.Tests")]

namespace Zelcam4.MLAgents
{
    internal static class GrpcExtensions
    {
        #region AgentEcs
        
        public static AgentInfoProto ToAgentInfoProto(this AgentEcs ai)
        {
            var agentInfoProto = new AgentInfoProto
            {
                Reward = ai.Reward,
                GroupReward = ai.GroupReward,
                MaxStepReached = ai.MaxStepReached,
                Done = ai.Done,
                Id = ai.EpisodeId,
                GroupId = ai.GroupId,
            };

            return agentInfoProto;
        }

        #endregion

        #region BrainParameters
        /// <summary>
        /// Converts an ActionSpec into to a Protobuf BrainInfoProto so it can be sent.
        /// </summary>
        /// <returns>The BrainInfoProto generated.</returns>
        /// <param name="actionStructure"> Description of the actions for the Agent.</param>
        /// <param name="name">The name of the brain.</param>
        /// <param name="isTraining">Whether or not the Brain is training.</param>
        public static BrainParametersProto ToBrainParametersProto(this ActionsStructure actionStructure, string name, bool isTraining)
        {
            var brainParametersProto = new BrainParametersProto
            {
                BrainName = name,
                IsTraining = isTraining,
                ActionSpec = ToActionSpecProto(actionStructure),
            };

            /*var supportHybrid = Academy.Instance.TrainerCapabilities == null || Academy.Instance.TrainerCapabilities.HybridActions;
            if (!supportHybrid)
            {
                actionSpec.CheckAllContinuousOrDiscrete();
                if (actionSpec.NumContinuousActions > 0)
                {
                    brainParametersProto.VectorActionSizeDeprecated.Add(actionSpec.NumContinuousActions);
                    brainParametersProto.VectorActionSpaceTypeDeprecated = SpaceTypeProto.Continuous;
                }
                else if (actionSpec.NumDiscreteActions > 0)
                {
                    brainParametersProto.VectorActionSizeDeprecated.AddRange(actionSpec.BranchSizes);
                    brainParametersProto.VectorActionSpaceTypeDeprecated = SpaceTypeProto.Discrete;
                }
            }*/

            // TODO handle ActionDescriptions?
            return brainParametersProto;
        }


        /// <summary>
        /// Convert a ActionSpec struct to a ActionSpecProto.
        /// </summary>
        /// <param name="actionStructure">An instance of an action spec struct.</param>
        /// <returns>An ActionSpecProto.</returns>
        public static ActionSpecProto ToActionSpecProto(this ActionsStructure actionStructure)
        {
            var actionSpecProto = new ActionSpecProto
            {
                NumContinuousActions = actionStructure.NumContinuousActions,
                NumDiscreteActions = actionStructure.NumDiscreteActions,
            };
            
            var branches = actionStructure.DiscreteBranchSizes;
            // Loop through the FixedList and add each element one by one.
            foreach (var t in branches)
            {
                actionSpecProto.DiscreteBranchSizes.Add(t);
            }
            return actionSpecProto;
        }

        #endregion

        

        public static UnityRLInitParameters ToUnityRlInitParameters(this UnityRLInitializationInputProto inputProto)
        {
            return new UnityRLInitParameters
            {
                seed = inputProto.Seed,
                numAreas = inputProto.NumAreas,
                pythonLibraryVersion = inputProto.PackageVersion,
                pythonCommunicationVersion = inputProto.CommunicationVersion,
                TrainerCapabilities = inputProto.Capabilities.ToRLCapabilities()
            };
        }

        #region AgentAction
        
        public static void ToAgentAction(this AgentActionProto proto, ref AgentAction agentActionToFill)
        {
            agentActionToFill.ContinuousActions.Clear();
            agentActionToFill.DiscreteActions.Clear();
    
            // Convert Continuous Actions without creating an array
            if (proto.ContinuousActions.Count <= agentActionToFill.ContinuousActions.Capacity)
            {
                // Loop directly over the proto's collection
                foreach (var action in proto.ContinuousActions)
                {
                    agentActionToFill.ContinuousActions.Add(action);
                }
            }
    
            // Convert Discrete Actions without creating an array
            if (proto.DiscreteActions.Count <= agentActionToFill.DiscreteActions.Capacity)
            {
                foreach (var action in proto.DiscreteActions)
                {
                    agentActionToFill.DiscreteActions.Add(action);
                }
            }
        }
        #endregion

        #region Observations
        /// <summary>
        /// Static flag to make sure that we only fire the warning once.
        /// </summary>
        private static bool s_HaveWarnedTrainerCapabilitiesMultiPng;
        private static bool s_HaveWarnedTrainerCapabilitiesMapping;

       
        /// <summary>
        /// Converts a DOTS DynamicBuffer of observation values directly into an
        /// ML-Agents ObservationProto object.
        /// </summary>
        public static ObservationProto GetObservationProto(this DynamicBuffer<ObservationValue> observations)
        {
            const string name = "VectorSensor_DOTS";

            var floatDataProto = new ObservationProto.Types.FloatData
            {
                Data =
                {
                    Capacity = observations.Length
                }
            };

            foreach (var obsValue in observations)
            {
                floatDataProto.Data.Add(obsValue.Value);
            }

            var observationProto = new ObservationProto
            {
                FloatData = floatDataProto,
                Name = name, 
                ObservationType = ObservationTypeProto.Default,
                CompressionType = CompressionTypeProto.None
            };

            // Flat vector of N floats
            observationProto.Shape.Add(observations.Length);

            return observationProto;
        }

        #endregion

        public static UnityRLCapabilities ToRLCapabilities(this UnityRLCapabilitiesProto proto)
        {
            return new UnityRLCapabilities
            {
                BaseRLCapabilities = proto.BaseRLCapabilities,
                ConcatenatedPngObservations = proto.ConcatenatedPngObservations,
                CompressedChannelMapping = proto.CompressedChannelMapping,
                HybridActions = proto.HybridActions,
                TrainingAnalytics = proto.TrainingAnalytics,
                VariableLengthObservation = proto.VariableLengthObservation,
                MultiAgentGroups = proto.MultiAgentGroups,
            };
        }

        public static UnityRLCapabilitiesProto ToProto(this UnityRLCapabilities rlCaps)
        {
            return new UnityRLCapabilitiesProto
            {
                BaseRLCapabilities = rlCaps.BaseRLCapabilities,
                ConcatenatedPngObservations = rlCaps.ConcatenatedPngObservations,
                CompressedChannelMapping = rlCaps.CompressedChannelMapping,
                HybridActions = rlCaps.HybridActions,
                TrainingAnalytics = rlCaps.TrainingAnalytics,
                VariableLengthObservation = rlCaps.VariableLengthObservation,
                MultiAgentGroups = rlCaps.MultiAgentGroups,
            };
        }
    }
}
