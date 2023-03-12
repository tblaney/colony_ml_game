# Colony Machine Learning Game in Unity
## Description
Intelligent brain trained to manipulate individual colonist states based on observations of the environment.
Goal: Optimize PPO network to learn most efficient set of states for all colonists when subjected to a wide range of scenarios.
Stretch Goal: Develop and train a countering brain agent to spawn in unique threats to the colony (enemies, weather events (maybe?)...etc...)

## Scripts
#### ColonyHandler.cs
Every script is executed from this script (EnvironmentReset method, or Initialize method). This handles all the different training areas and communication from agent to area.
##### ColonyParameters (class)
Contains the hyperparameters for the game environment
##### RewardWeight (struct)
Contains the reward values and weights for the reward structure of the game. Basically an agent, or colonist area will communicate with ColonyHandler to get the weights when it wants to add a reward - this way we can just change these values here for easy model-tuning.
#### ColonyArea.cs
Performs spawning/despawning of agents and enemies, and receives commands from ColonyHandler. 
##### Colony (class)
Contains information on the colony, including its colonists.
#### ColonyProcessor.cs
Linked to ColonyArea and performs spawning/despawning of food/rock objects around the map based on colonist interaction.
#### ColonistAgent.cs
Handles any form of communication with the model, and reacts to its output. Agent behaviour is dependent on this class, as it is essentially a state-switcher.
##### Colonist (class)
Class storing any colonist information that will eventually be passed to the model.
States:
- Idle
  --> Agent stays still, no actions
- Rest (TODO)
  --> Agent moves to a rest location, and stays there to regenerate lost energy
- Mine (TODO - same as collect pretty much)
  --> Agent moves to nearest (and not busy) mineral object and begins mining process
- Patrol
  --> Agent wanders in random motion until an enemy enters its viewing radius, at which point it begins to attack
- Heal (TODO)
  --> Agent finds other agents with less-than-perfect health, and travels to them to administer a health regen
- Collect
  --> Agent moves to nearest (and not busy) food object and collects it by breaking it down
##### ColonistTraits (class)
Class storing specific and uneditable traits of a colonist, which alters the effectiveness of a colonist performing a specific task.
##### ColonistBehaviour.cs
Abstract class that acts as the state behaviour. Should therefore have override classes for each state.

#### EnemyAgent.cs
(Not an ML Agent currently) Controls the movement and behaviour of enemy entities, who chase down nearest colonists and melee attack.
#### EnemyBehaviour.cs
Equivalent basically to the ColonistBehaviour class, except it offers room to grow and add other enemy functionality besides simple chase and attack.

#### NavigationController.cs
Link between colonist/enemy movement and NavMeshSurface (which is Unity's in-house pathfinding) with NavMeshAgent (movement control).



