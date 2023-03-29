using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    /*
        class that handles all UI objects, and links between different behaviours
    */
    
    // main inputs:
    [Header("Main Inputs:")]
    public int _index;
    public bool _clickable = true;
    public bool Clickable
    {
        get {return _clickable;}
        set {_clickable = value;}
    }

    [Header("Style Behaviour:")]
    [SerializeField] private UIController _controller;
    [SerializeField] private List<UIButtonBehaviour> _behaviours;
    

    [Header("Text:")]
    public TextMeshProUGUI _text;
    public GameObject _tooltip;
    public Image _image;

    //[Header("Audio:")]

    // actions:
    public Action OnPointerEnterFunc;
    public Action OnPointerExitFunc;
    public Action OnPointerDownFunc;
    public Action OnPointerUpFunc;
    public Action OnPointerClickFunc;

    // cache:
    RectTransform _rect;
    public static UIButton _selectedClick;
    private Coroutine _routine;


    // mono i mono:
    void Awake()
    {
        _rect = GetComponent<RectTransform>();

        if (_tooltip != null)
        {
            ActivateTooltip(false);
        }
    }

    void Start()
    {
        ResetBehaviours();
    }

    void OnEnable()
    {
        ResetBehaviours();
    }

    void OnDisable()
    {
        if (_selectedClick == this)
            ClearClick();
    }

    public void Activate(bool active)
    {
        this.gameObject.SetActive(active);
    }


    // click toggle methods:
    public void ClearClick()
    {
        _selectedClick = null;

        ActivateBehaviours(UIButtonBehaviour.Style.Click, false);
    }
    
    public void SelectClick()
    {
        _selectedClick = this;

        ActivateBehaviours(UIButtonBehaviour.Style.Click, true);
    }

    public void ClickCheck()
    {
        // handle toggling:
        if (_selectedClick != null)
        {
            _selectedClick.ClearClick();
        }

        SelectClick();
    }



    // behaviour methods:
    void ActivateBehaviours(UIButtonBehaviour.Style style, bool active)
    {
        foreach (UIButtonBehaviour behaviour in _behaviours)
        {
            if (behaviour._style != style)
                continue;

            if (active) 
            {
                _controller.ActivateBehaviour(behaviour._controllerIndex, true);
                if (behaviour._tooltip != null && behaviour._tooltip != "")
                {
                    UIHandler.Instance.ActivateTooltip(true, behaviour._tooltip);
                }
            } 
            else 
            {
                _controller.ActivateBehaviour(behaviour._controllerIndex, false);
                if (behaviour._tooltip != null && behaviour._tooltip != "")
                {
                    UIHandler.Instance.ActivateTooltip(false);
                }
            }
        }
    }

    public void ResetBehaviours()
    {
        foreach (UIButtonBehaviour behaviour in _behaviours)
        {
            _controller.ResetBehaviour(behaviour._controllerIndex);
            Debug.Log("UI Button Reset Behaviour; " + this.gameObject.name);
        }
    }



    // pointer methods
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!_clickable)
            return;
        
        ActivateBehaviours(UIButtonBehaviour.Style.Hover, true);

        if (OnPointerEnterFunc != null)
            OnPointerEnterFunc();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!_clickable)
            return;

        ActivateBehaviours(UIButtonBehaviour.Style.Hover, false);

        if (OnPointerExitFunc != null)
            OnPointerExitFunc();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!_clickable)
            return;

        ClickCheck();

        if (OnPointerClickFunc != null)
            OnPointerClickFunc();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_clickable)
            return;

        if (OnPointerDownFunc != null)
            OnPointerDownFunc();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_clickable)
            return;

        if (OnPointerUpFunc != null)
            OnPointerUpFunc();
    }   


    // external methods
    public void SetText(string text)
    {
        if (text == null)
            return;

        _text.text = text;
        _text.ForceMeshUpdate(); 
    }

    public void SetImage(Sprite sprite)
    {
        if (_image != null)
            _image.sprite = sprite;
    }

    public void RevealText(string text, float time = 0.1f, Action OnRevealFunc = null)
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(RevealTextRoutine(text, time, OnRevealFunc));
    }

    public void ActivateTooltip(bool active)
    {
        Debug.Log("Button Activate Tooltip: " + this.gameObject.ToString() + ", " + active);
        _tooltip.SetActive(active);
    }

    public void ActivateImage(bool active = true)
    {
        if (_image != null)
            _image.gameObject.SetActive(active);
    }

    public void SetPosition(Vector2 position)
    {
        _rect.anchoredPosition = position;
    }

    public Vector2 GetPosition()
    {
        return _rect.anchoredPosition;
    }
    public RectTransform GetRect()
    {
        return _rect;
    }
    public void ActivateController(int index, bool active)
    {
        if (_controller != null)
            _controller.ActivateBehaviour(index, active);
    }
    public void ActivateController(string name, bool active)
    {
        if (_controller != null)
            _controller.ActivateBehaviour(name, active);
    }
    public UIController GetController()
    {
        return _controller;
    }
    private IEnumerator RevealTextRoutine(string text, float timeInteval, Action OnRevealFunc)
    {
        char[] textChars = text.ToCharArray();
        string currentText = "";
        int index = 0;

        for (int i = 0; i < textChars.Length; i++)
        {
            currentText += textChars[i];
            _text.text = currentText;

            yield return new WaitForSeconds(timeInteval);
        }

        if (OnRevealFunc != null)
            OnRevealFunc();
    }

}

[Serializable]
public class UIButtonBehaviour
{
    public int _controllerIndex;
    public string _tooltip = null;
    
    public enum Style
    {
        Hover,
        Click,
    }
    public Style _style;
}