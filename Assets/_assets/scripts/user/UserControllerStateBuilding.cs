using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerStateBuilding : UserControllerState
{
    public Building _building;
    BuildingNodeObject _node;
    public int _chain;
    Vector3Int _positionStart;
    
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnStartState()
    {
        //_cam.SetManual();
        _building = BuildingHandler.Instance.GetBuilding(UIBuildingMenu._currentBuildingIndex);
        Debug.Log("User Controller State Building: " + _building._name);
        // spawn in prefab
        Vector3 spawnPosition = CameraHandler.Instance.GetBuildingPosition();
        if (spawnPosition == default(Vector3))
        {
            StopState();
            return;
        }
        if (_node != null)
        {
            Destroy(_node.gameObject);
        }
        _node = HabitationHandler.Instance.SpawnNodeUnassigned(_building.GetNodeBuilding(Utils.Tools.VectorToInt(spawnPosition))) as BuildingNodeObject;
        _node.RemoveFromQueue();
        _controller.ActivateSelector(true);
        ShaderHandler.Instance.SetGridOpacity(0.2f);
    }

    public override void OnStopState()
    {
        if (_node != null)
        {
            Destroy(_node.gameObject);
            _node = null;
        }
        _chain = 0;
        _controller.ActivateSelector(false);
        ShaderHandler.Instance.SetGridOpacity(0f);
    }

    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopState();
            return;
        }
        Vector3 position = CameraHandler.Instance.GetBuildingPosition();
        position.y = 30f;
        Vector3Int positionInt = Utils.Tools.VectorToInt(position);
        if (_chain > 0)
        {
            // already holding down something, so want to make sure it only travels on axes
            Vector3Int difference = positionInt - _positionStart;
            difference = new Vector3Int(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z));
            if (difference.x > difference.z)
            {
                positionInt = new Vector3Int(positionInt.x, positionInt.y, _positionStart.z);
            } else if (difference.x < difference.z)
            {
                positionInt = new Vector3Int(_positionStart.x, positionInt.y, positionInt.z);
            } else 
            {
                positionInt = _positionStart;
            }
        }
        _node.UpdatePosition(positionInt);
        _controller.UpdateSelector(positionInt, SelectorController.State.Default);
        if (Input.GetMouseButton(0))
        {
            if (!UIHandler.Instance.IsMouseOverUI() && _node.CanPlace())
                Place(positionInt);
        } else
        {
            _chain = 0;
        }
    }
    void Place(Vector3Int position)
    {
        if (_chain == 0)
        {
            _positionStart = position;
        } else if (_chain == 1)
        {
            // second node
            
        }
        HabitationHandler.Instance.NewNode(_building.GetNodeBuilding(position));
        _chain++;
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
