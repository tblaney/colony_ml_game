using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UITime : UIObject
{
    [Header("Inputs:")]
    [SerializeField] private UIButton _button;
    [SerializeField] private UIButton _buttonPausePlay;
    [SerializeField] private List<RectTransform> _landmarkRects;
    [SerializeField] private RectTransform _rectLine;
    RectTransform _rect;
    RectTransform _rectButton;
    Dictionary<Vector2, float> _timeDic;
    public bool _following = false;
    Vector2 _currentPosition;
    
    public override void Initialize()
    {
        _rect = GetComponent<RectTransform>();
        _rectButton = _button.GetComponent<RectTransform>();
        _timeDic = new Dictionary<Vector2, float>();
        float val = 0f;
        int i = 0;
        foreach (RectTransform rect in _landmarkRects)
        {
            float valTemp = val;
            if (i == 0)
                valTemp = 1f;
            _timeDic.Add(rect.anchoredPosition, valTemp);
            val += 1f;
            i++;
        }
        _button.OnPointerDownFunc = () =>
        {
            if (!_following)
                _following = true;
        };
        _button.OnPointerUpFunc = () =>
        {
            if (_following)
                _following = false;
        };
        _buttonPausePlay.OnPointerClickFunc = () =>
        {
            if (GameHandler._paused)
            {
                GameHandler.Instance.Resume();
                _controller.ActivateBehaviour("pause", true);
                _controller.ActivateBehaviour("play", false);
            } else
            {
                GameHandler.Instance.Pause();
                _controller.ActivateBehaviour("play", true);
                _controller.ActivateBehaviour("pause", false);
            }
        };
        Vector2 point = new Vector2(-100f, 0f);
        Vector2 positionNew = GetCorrectedPosition(point);
        _controller.SetPosition(positionNew, 1);
        _controller.SetText(_timeDic[positionNew].ToString(), "text time");
        //TimeHandler.Instance.SetTimeScale(_timeDic[_currentPosition]);
    }
    void Update()
    {
        if (_following)
        {
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectLine, Input.mousePosition, null, out point);
            point.y = _rectButton.anchoredPosition.y;
            if (point.x < -100f)
                point.x = -100f;
            if (point.x > 100f)
                point.x = 100f;
            Vector2 positionNew = GetCorrectedPosition(point);
            Debug.Log("UI Time: " + point);

            _controller.SetPosition(positionNew, 1);
            _controller.SetText(_timeDic[positionNew].ToString(), "text time");

            TimeHandler.Instance.SetTimeScale(_timeDic[_currentPosition]);
        }

        _controller.SetText(TimeHandler._timeWorld._date, "text date");
        _controller.SetText("Day: " + TimeHandler._timeWorld._day.ToString(), "text day");
    }
    Vector2 GetCorrectedPosition(Vector2 position)
    {
        float distanceMin = 10000f;
        Vector2 positionOut = new Vector2();
        foreach (Vector2 pos in _timeDic.Keys)
        {
            float distance = Vector2.Distance(pos, position);
            if (distance < distanceMin)
            {   
                positionOut = pos;
                distanceMin = distance;
            }
        }
        _currentPosition = positionOut;
        return positionOut;
    }
}
