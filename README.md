# Colony Game
Colony Machine Learning Game in Unity

## Training Setup for Developers
1. Create a trainer_config.yaml file in the colony_game project
2. Install anaconda
3. Create new anaconda environment and activate it:
```
conda create -n ml-agents-env
conda activate ml-agents-env
```
4. Install the following libraries to correspond with our usage of ml-agents 1.0.8
```
pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html
pip3 install mlagents==0.26.0
```
5. Test that installation is complete by running 
```
mlagents-learn -h
```
This will let you view options for executing training, including setting pytorch to use gpu.
6. Assuming unity environment is correct and complete for training with ml-agents, begin a training run:
```
mlagents-learn ./trainer_config.yaml --torch-device cuda --run-id test_run_01
```
7. ml-agents will prompt you to start your scene in the unity environment. Do so to begin training. 
8. Open a different anaconda window and view your training progress with tensorboard (summaries are logged in the same folder where trainer_config.yaml is stored):
```
tensorboard --logdir summaries
```