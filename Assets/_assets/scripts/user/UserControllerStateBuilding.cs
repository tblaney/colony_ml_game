using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerStateBuilding : UserControllerState
{
    public Building _building;
    BuildingNodeObject _node;
    
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnStartState()
    {
        //_cam.SetManual();
        _building = BuildingHandler.Instance.GetBuilding(UIBuilding._currentBuildingIndex);
        Debug.Log("User Controller State Building: " + _building._name);
        // spawn in prefab
        Vector3 spawnPosition = CameraHandler.Instance.GetBuildingPosition();
        if (spawnPosition == default(Vector3))
        {
            StopState();
            return;
        }
        _node = HabitationHandler.Instance.SpawnNodeUnassigned(_building.GetNodeBuilding(Utils.Tools.VectorToInt(spawnPosition))) as BuildingNodeObject;
        _controller.ActivateSelector(true);
    }

    public override void OnStopState()
    {
        if (_node != null)
        {
            Destroy(_node.gameObject);
        }
        _controller.ActivateSelector(false);
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopState();
        }
        Vector3 position = CameraHandler.Instance.GetBuildingPosition();
        position.y = 30f;
        Vector3Int positionInt = Utils.Tools.VectorToInt(position);
        _node.transform.position = positionInt;
        _controller.UpdateSelector(positionInt, SelectorController.State.Default);
        if (Input.GetMouseButton(0) && !UIHandler.Instance.IsMouseOverUI() && _node.CanPlace())
        {
            Place(positionInt);
        }
    }
    void Place(Vector3Int position)
    {
        HabitationHandler.Instance.NewNode(_building.GetNodeBuilding(position));
        if (Input.GetMouseButton(0) && !UIHandler.Instance.IsMouseOverUI())
        {
            StartState();
        } else
        {
            StopState();
        }
        //StopState();
    }
}
