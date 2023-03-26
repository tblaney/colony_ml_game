using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCaster : MonoBehaviour
{
    [SerializeField] LayerMask _maskBlockers;
    BoxCollider _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }
    public bool BlockCast()
    {
        RaycastHit[] hits = Physics.BoxCastAll(this.transform.TransformPoint(_collider.center), _collider.size/2f, Vector3.down, Quaternion.identity, Mathf.Infinity, _maskBlockers);
        List<Collider> colliders = new List<Collider>();
        Debug.Log("Block Cast: " + hits.Length);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != _collider)
                colliders.Add(hit.collider);
        }
        return colliders.Count < 1;
    }
}
