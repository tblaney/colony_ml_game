using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorController : MonoBehaviour
{
    public enum State
    {
        Default,
        Blocked,
    }
    public State _state;
    public List<Color> _colors;
    public MaterialController _materialController;
    
    void Awake()
    {
        Activate(false);
    }
    public void UpdateSelector(Vector3Int position, State state)
    {
        transform.position = new Vector3(position.x, 30f, position.z);
        _materialController.SetColor(_colors[(int)state], 1);
    }
    public void Activate(bool active = true)
    {
        this.gameObject.SetActive(active);
    }
}
