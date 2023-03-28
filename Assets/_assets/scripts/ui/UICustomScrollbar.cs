using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICustomScrollbar : MonoBehaviour
{
    [Header("Inputs:")]
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private RectTransform _rectContents; 
    [SerializeField] private RectTransform _mask;
    [SerializeField] private List<GameObject> _hoverObjects;
    public enum Type
    {
        Vertical,
        Horizontal
    }
    public Type _type;

    // cache
    private Vector2 _maskSize;
    private Vector2 _defaultContentPosition;
    private Vector2 _defaultContentSize;
    private bool _canScroll;
    private List<RectTransform> _slotList;
    private Vector2 _slotSize;
    bool _active;
    //-----------------------------------------------------

    void Awake()
    {
        Initialize();
        if (_hoverObjects == null)
        {
            _hoverObjects = new List<GameObject>() {this.gameObject};
        }
    }
    void Update()
    {
        RectCheck();
        if (!_active)
            return;
        if (_canScroll)
        {
            List<GameObject> objs = UIHandler.Instance.GetGameObjectsUnderMouse();
            bool contains = false;
            foreach (GameObject obj in objs)
            {
                if (_hoverObjects.Contains(obj))
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
                return;
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                _scrollbar.value -= scroll;
                float newValue = Mathf.Clamp(_scrollbar.value, 0f, 1f);
                Debug.Log(newValue);

                _scrollbar.value = newValue;
            }
        }
    }
    void RectCheck()
    {
        if (_rectContents.sizeDelta.y <= _mask.sizeDelta.y)
        {
            if (_active)
                _active = false;
        } else
        {
            if (!_active)
                _active = true;
        }
    }
    void OnDisable()
    {
        Debug.Log("Custom Scrollbar Disabled: " + this.gameObject);
    }
    public void Initialize()
    {
        _defaultContentPosition = _rectContents.anchoredPosition;
        _defaultContentSize = _rectContents.sizeDelta;
        _maskSize = _mask.sizeDelta;
        _slotList = new List<RectTransform>();
        _scrollbar.onValueChanged.AddListener((float val) => ScrollbarCallback(val));
        _active = true;
        Refresh();
    }
    public void Refresh()
    {
        bool condition = _rectContents.sizeDelta.y > _maskSize.y;
        if (_type == Type.Horizontal)
        {
            condition = _rectContents.sizeDelta.x > _maskSize.x;
        }
        if (condition)
        {
            _scrollbar.gameObject.SetActive(true);
            _canScroll = true;
        }
        else
        {
            _scrollbar.gameObject.SetActive(false);
            _canScroll = false;
        }
        _defaultContentPosition = _rectContents.anchoredPosition;
    }
    public RectTransform AddSlot(RectTransform slot, Vector2 slotSize)
    {
        RectTransform newSlot = Instantiate(slot, _rectContents).GetComponent<RectTransform>();
        newSlot.anchoredPosition = slot.anchoredPosition - new Vector2(0f, slotSize.y * _slotList.Count);
        _rectContents.sizeDelta += new Vector2(0f, slotSize.y);
        newSlot.gameObject.SetActive(true);
        _slotList.Add(newSlot);
        Refresh();
        return newSlot;
    }
    public void ClearSlots()
    {
        foreach (RectTransform rect in _slotList)
        {
            Destroy(rect.gameObject);
        }
        _slotList.Clear();
        _rectContents.sizeDelta = _defaultContentSize;
    }

    void ScrollbarCallback(float value)
    {
        if (_canScroll)
        {
            Vector2 contentSize = _rectContents.sizeDelta;
            float difference = 0f;
            switch (_type)
            {
                case Type.Vertical:
                    difference = contentSize.y - _maskSize.y;
                    difference -= _slotSize.y;
                    break;
                case Type.Horizontal:
                    difference = contentSize.x - _maskSize.x;
                    difference -= _slotSize.x;
                    break;
            }
            float distance = difference * value;
            switch (_type)
            {
                case Type.Vertical:
                    _rectContents.anchoredPosition = _defaultContentPosition + new Vector2(0f, distance);
                    break;
                case Type.Horizontal:
                    _rectContents.anchoredPosition = _defaultContentPosition - new Vector2(distance, 0f);
                    break;
            }

        }
    }
}
