# Group Learning for Deterministic Agents in a Colony Survival Game

![alt text](https://github.com/tblaney/colony_ml_game/blob/main/images/title_image.png?raw=true)

Final Presentation: https://www.canva.com/design/DAGF_tO4IQw/sHbLdANv4G2ntozuqKfv-A/edit

To play - clone this repo and run colony.exe in Builds folder.

This work integrates Reinforcement Learning (RL) into a colony survival simulation game using the ML-Agents Unity library, to create interesting and emergent non-playable and playable character behaviors. In past works, the difficulty of group learning in game scenarios where agents drop in and out of the group as they learn has been illustrated. In response the MA-POCA network was created. However, even recent group learning examples in ML-Agents fail to capitalize on MA-POCA’s ability to adapt to a variable number of learning agents. This is unfortunate as game scenarios where agents die or spawn at variable times are very common, especially for more complex scenarios. Complexity in this case is defined as the amount of unique states, and therefore behaviors, an individual agent can occupy. Furthermore, all recent examples focus on agents which simultaneously learn individually in a physics-heavy setting, and as a group. This is not a common scenario in games, which usually use algorithmic agents that behave according to different states. This work applies MA-POCA to this novel training scenario, and determines effective reward structures for cases where agents are learning as a group and individually. As a proof of concept, this new model is applied in a completed strategy survival game built from the ground up in Unity.

Training Branch --> "colony_rl_training"
