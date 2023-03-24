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
    [SerializeField] private GameObject _hoverObject;

    // cache
    private Vector2 _maskSize;
    private Vector2 _defaultContentPosition;
    private Vector2 _defaultContentSize;
    private bool _canScroll;
    private List<RectTransform> _slotList;
    private Vector2 _slotSize;

    //-----------------------------------------------------

    void Awake()
    {
        Initialize();
        if (_hoverObject == null)
            _hoverObject = this.gameObject;
    }
    void Update()
    {
        if (_canScroll)
        {
            List<GameObject> objs = UIHandler.Instance.GetGameObjectsUnderMouse();
            if (!objs.Contains(_hoverObject))
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
    public void Initialize()
    {
        _defaultContentPosition = _rectContents.anchoredPosition;

        _defaultContentSize = _rectContents.sizeDelta;

        _maskSize = _mask.sizeDelta;
        _slotList = new List<RectTransform>();

        _scrollbar.onValueChanged.AddListener((float val) => ScrollbarCallback(val));
        Refresh();
    }
    public void Refresh()
    {
        if (_rectContents.sizeDelta.y > _maskSize.y)
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
        //_rectContents.anchoredPosition -= new Vector2(0f, slotSize.y / 2f);

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
            float difference = contentSize.y - _maskSize.y;
            difference -= _slotSize.y;

            float distance = difference * value;
            _rectContents.anchoredPosition = _defaultContentPosition + new Vector2(0f, distance);

        }
    }
}
