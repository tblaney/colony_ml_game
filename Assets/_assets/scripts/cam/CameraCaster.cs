using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCaster : MonoBehaviour
{
    Camera _cam;
    public LayerMask _maskTerrain;
    public LayerMask _maskInteractable;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }
    public Vector3 GetCenterTerrainPosition()
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, 200f, _maskTerrain);
        if (isHit)
        {
            return hit.point;
        } else
        {
            return default(Vector3);
        }
    }
    public Vector3 GetMouseTerrainPosition()
    {
        Vector2 mousePos = Input.mousePosition;
        Ray ray = _cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, Mathf.Infinity, _maskTerrain);
        if (isHit)
        {
            return hit.point;
        }
        return default(Vector3);
    }
    public Interactable GetInteractable()
    {
        Vector2 mousePos = Input.mousePosition;
        Ray ray = _cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, Mathf.Infinity, _maskInteractable);
        if (isHit)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                return interactable;
            } else
            {
                interactable = hit.collider.transform.parent.GetComponent<Interactable>();
                if (interactable != null)
                    return interactable;
            }
        }
        return null;
    }
}
