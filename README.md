# com.zelcam4.ml-agents
This package is a version of the custom Unity package [ML-Agents](https://github.com/Unity-Technologies/ml-agents) framework built entirely on Unity's Data-Oriented Technology Stack (DOTS)

DOTS-Native Training Loop: Core agent lifecycle (Observe, Decide, Act, Reward) implemented with parallel jobs.

ML-Agents is a Unity package that allows users to use state-of-the-art machine learning to create intelligent character behaviors in any Unity environment (games, robotics, film, etc.).

## Installation

Please refer to the [ML-Agents github repo] for installation instructions to start using ML Agents.

Once installed, you substitute the `com.Unity.MLAgents` package in your project with `com.zelcam4.ml-agents` from this repository. You can directly import this package from the Unity package manager.

## How to use
![](\Documentation~\sample.gif)
Please import the BasicSample provided in the package to get started and open the `Sample Sub Scene.scene` sub-scene.

### Agents:
Add the `Agent Authoring` component to any GameObject to create an agent. This component will automatically bake the corresponding entity with the Brain parameters defined in the Inspector.


### Observations:
Add the `Observation Authoring` component to the same GameObject as the `Agent Authoring` to create an observation component. 
This component will automatically bake the corresponding entity with the Sensor parameters defined in the Inspector.

Update the different custom observations with systems under `[UpdateBefore(typeof(ObservationCollectionGroup))]`. Once they have been updated,
schedule different Jobs with `GatherJob<T, TExtractor> : IJobChunk` under `[UpdateInGroup(typeof(ObservationCollectionGroup))]`.

> [!WARNING]  
> In order to take advantage of generics, it is neccessary to register types at assembly level.
`[assembly:RegisterGenericComponentType(typeof(ObservationRequest<CustomObservation>))]`

For more information, please refer to the [code summaries](\Runtime\Data\Observation.cs) in the package and defined systems
in the sample

### Actions:
Actions are configured in the `Agent Authoring` component, following the same structure as the original ML-Agents.

Implement a system under `[UpdateInGroup(typeof(ActionSystemGroup))]` where the user takes usage from the actions
when the entity has the tag `RequestActionTag`.

### Rewards:
Add different rewards with systems under `[UpdateInGroup(typeof(RewardGroup))]`.


## Future Plans

⏳ Inference Integration: Currently inference is not supported, only the training loop

🚀 Sensors: Currently only custom components are supported, being the user responsible for updating them

📚 Documentation & Samples: Create detailed documentation and example scenes to showcase best practices

📈 Performance Optimizations: Further profiling and optimization of the core systems

[ML-Agents github repo]: https://github.com/Unity-Technologies/ml-agents

