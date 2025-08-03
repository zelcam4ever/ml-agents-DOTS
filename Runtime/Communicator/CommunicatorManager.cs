using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Zelcam4.MLAgents;

using UnityEngine;
using Zelcam4.MLAgents.CommunicatorObjects;

namespace Zelcam4.MLAgents
{ 
public static class CommunicatorManager 
{
    /// Pointer to the communicator currently in use by the Academy.
    internal static ICommunicator Communicator;
    
    static bool m_Initialized;
    
    /// <summary>
    /// Unity package version of com.Zelcam4.ML-Agents.
    /// This must match the version string in package.json and is checked in a unit test.
    /// </summary>
    internal const string k_PackageVersion = "3.0.0";

    const int k_EditorTrainingPort = 5004;

    const string k_PortCommandLineFlag = "--mlagents-port";
    
    const string k_ApiVersion = "1.5.0";
    
    // Random seed used for inference.
    static int m_InferenceSeed;

    /// <summary>
    /// Set the random seed used for inference. This should be set before any Agents are added
    /// to the scene. The seed is passed to the ModelRunner constructor, and incremented each
    /// time a new ModelRunner is created.
    /// </summary>
    public static int InferenceSeed
    {
        set { m_InferenceSeed = value; }
    }
    
    static int m_NumAreas;

    /// <summary>
    /// Number of training areas to instantiate.
    /// </summary>
    public static int NumAreas => m_NumAreas;
    
    /// <summary>
    /// Returns the RLCapabilities of the python client that the unity process is connected to.
    /// </summary>
    internal static UnityRLCapabilities TrainerCapabilities { get; set; }
    
    public static void AwakeCalled()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!CommunicatorFactory.CommunicatorRegistered)
        {
            Debug.Log("Registered Communicator in Agent.");
            CommunicatorFactory.Register<ICommunicator>(RpcCommunicator.Create);
        }
#endif

        LazyInitialize();
    }

    internal static void LazyInitialize()
    {
        if (!m_Initialized)
        {
            InitializingAcademy();
            m_Initialized = true;
        }
    }
    /// <summary>
    /// Initializes the environment, configures it and initializes the Academy.
    /// </summary>
    static void InitializingAcademy()
    {
        //var port = ReadPortFromArgs();
        var port = 5004;
        if (port > 0)
        {
            Communicator = CommunicatorFactory.Create();
        }

        if (Communicator == null && CommunicatorFactory.Enabled && port > 0)
        {
            Debug.Log("Communicator failed to start!");
        }

        // We try to exchange the first message with Python. If this fails, it means
        // no Python Process is ready to train the environment. In this case, the
        // environment must use Inference.
        bool initSuccessful = false;
        var communicatorInitParams = new CommunicatorInitParameters
        {
            port = port,
            unityCommunicationVersion = k_ApiVersion,
            unityPackageVersion = k_PackageVersion,
            name = "AcademySingleton",
            CSharpCapabilities = new UnityRLCapabilities()
        };

        try
        {
            initSuccessful = Communicator.Initialize(
                communicatorInitParams,
                out var unityRlInitParameters
            );
            if (initSuccessful)
            {
                UnityEngine.Random.InitState(unityRlInitParameters.seed);
                // We might have inference-only Agents, so set the seed for them too.
                m_InferenceSeed = unityRlInitParameters.seed;
                m_NumAreas = unityRlInitParameters.numAreas;
                TrainerCapabilities = unityRlInitParameters.TrainerCapabilities;
                TrainerCapabilities.WarnOnPythonMissingBaseRLCapabilities();
            }
            else
            {
                Debug.Log($"Couldn't connect to trainer on port {port} using API version {k_ApiVersion}. Will perform inference instead.");
                Communicator = null; 
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Unexpected exception when trying to initialize communication: {ex}\nWill perform inference instead.");
            Communicator = null;
        }
    }
    
    public static void PutObservation(string behaviorName, AgentEcs info, DynamicBuffer<ObservationValue> observations)
    {
        Communicator.PutObservations(behaviorName, info, observations);
    }

    public static void SubscribeBrain(string name, ActionsStructure actionsStructure)
    {
        Communicator.SubscribeBrain(name, actionsStructure);
    }
    
    public static void DecideAction()
    {
        Communicator.DecideBatch();
    }
    
    public static Dictionary<int, AgentAction> GetActionsForBrain(FixedString32Bytes brainName)
    {
        var dicActions = Communicator?.GetActionsForBrain(brainName.ToString());
        return dicActions ?? new Dictionary<int, AgentAction>();
    }
}
}

