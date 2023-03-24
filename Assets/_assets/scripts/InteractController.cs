using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    CameraCaster _caster;
    public Interactable _interactable;

    void Awake()
    {
        _caster = GetComponent<CameraCaster>();
    }

    void Update()
    {
        Interactable interactable = _caster.GetInteractable();
        if (interactable != null)
        {
            Debug.Log("Interactable: " + interactable.gameObject.name);
            if (_interactable != null)
            {
                if (_interactable != interactable)
                {
                    _interactable.InteractHover(false);
                    _interactable = interactable;
                    _interactable.InteractHover(true);
                }
            } else
            {
                _interactable = interactable;
                _interactable.InteractHover(true);
            }
        } else
        {
            if (_interactable != null)
            {
                _interactable.InteractHover(false);
                _interactable = null;
            }
        }
    }

}
