using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIObjectTransformer : MonoBehaviour
{
    [Header("Inputs:")]
    [SerializeField] List<UITransformPoint> _transforms;
    [SerializeField] private bool _unscaledTime;


    // cache
    private RectTransform _rect;
    [Tooltip("Do not edit")]
    public int _index;

    // actions
    public Action OnTransformFunc;
    public Action OnTransformPointFunc;

    // coroutines
    private Coroutine _routine;
    List<UITransformPoint> _transformsCache;


    private UITransformPoint _defaultPoint;
    bool _performing;
    int _indexCache;


    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        _defaultPoint = new UITransformPoint() 
        {
            position = _rect.anchoredPosition,
            rotation = _rect.localRotation.eulerAngles,
            scale = _rect.localScale,
        };
    }

    void OnDisable()
    {
        if (_performing)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }
            SetTransformPoint(_transforms[_indexCache]);
            //OnTransformPointFunc = null;
            _performing = false;
        }
        
    }

    void OnEnable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
        _performing = false;
        //OnTransformPointFunc = null;
        /*
        OnTransformFunc = null;
        _performing = false;
        */
        //Reset();
    }

    public void Reset()
    {
        if (_defaultPoint == null)
            return;

        SetTransformPoint(_defaultPoint);
    }


    public void ActivateSeries(List<UITransformPoint> transformPoints)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _index = 0;

        this._transformsCache = transformPoints;
        UITransformPoint point = _transformsCache[_index];
        Vector3 anchoredPos = _rect.anchoredPosition;

        if (point.position == anchoredPos && point.rotation == _rect.localEulerAngles && point.scale == _rect.localScale)
        {
            return;
        }

        if (_rect.gameObject.activeInHierarchy)
        {
            _routine = StartCoroutine(TransformCoroutineUI(point));
        }
        else
        {
            SetTransformPoint(point);
        }
    }


    public void MoveTo(int index, bool hardTransform = false, Action OnMoveFunc = null)
    {
        if (index < _transforms.Count)
        {
            if (OnMoveFunc != null) 
            {
                OnTransformFunc = OnMoveFunc;
            }

            if (hardTransform | !gameObject.activeInHierarchy)
            {
                SetTransformPoint(_transforms[index]);
            } else
            {
                List<UITransformPoint> points = new List<UITransformPoint>();
                points.Add(_transforms[index]);
                _indexCache = index;
                ActivateSeries(points);
            }
        }   
    }


    // get methods:
    public Vector2 GetAnchoredPosition()
    {
        return _rect.anchoredPosition;
    }

    public Vector3 GetPosition()
    {
        return _rect.anchoredPosition;
    }
    

    // set methods:
    public void SetRect(RectTransform _rect)
    {
        this._rect = _rect;
    }

    public void SetTransformPoint(UITransformPoint transformPoint)
    {
        if (_rect == null)
            return;
        
        _rect.anchoredPosition = transformPoint.position;
        _rect.localScale = transformPoint.scale;
        Vector3 rotationIn = transformPoint.rotation;
        if (transformPoint.isAdditionalRotation)
        {
            rotationIn +=  _rect.localRotation.eulerAngles;
        }
        Vector3 rotationVector = new Vector3(0f, 0f, rotationIn.z);

        //_rect.localRotation = Quaternion.Euler(transformPoint.rotation);
        /*
        if (OnTransformFunc != null)
        {
            OnTransformFunc();
        }

        if (OnTransformPointFunc != null)
        {
            OnTransformPointFunc();
            OnTransformPointFunc = null;
        }
        */
    }

    public void SetPosition(Vector3 pos)
    {
        _rect.anchoredPosition = pos;
    }

    public void SetScale(Vector3 scale)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _rect.localScale = scale;
    }

    public void SetScale(Vector3 scale, float time)
    {
        UITransformPoint point = new UITransformPoint() 
        {
            position = _rect.anchoredPosition,
            scale = scale,
            rotation = _rect.localRotation.eulerAngles,
            time = time,
        };

        List<UITransformPoint> points = new List<UITransformPoint>();
        points.Add(point);
        ActivateSeries(points);
    }


    public void SetSize(Vector2 size)
    {
        _rect.sizeDelta = size;
    }



    // transformer coroutine:
    private IEnumerator TransformCoroutineUI(UITransformPoint point)
    {
        _performing = true;

        float time = point.time;
        Vector3 positionIn = point.position;
        Vector3 rotationIn = point.rotation;
        if (point.isAdditionalRotation)
        {
            rotationIn +=  _rect.localRotation.eulerAngles;
        }
        Vector3 scaleIn = point.scale;

        float t = 0f;

        Vector3 startPos = _rect.anchoredPosition;
        Vector3 startScale = _rect.localScale;
        Vector3 startRotation = _rect.localRotation.eulerAngles;

        if (point.OnTransformPointFunc != null)
        {
            point.OnTransformPointFunc();
        }

        while (t <= 1.0f)
        {
            Vector3 pos = Vector3.Lerp(startPos, positionIn, t);
            Vector3 scale = Vector3.Lerp(startScale, scaleIn, t);
            float rotation = Mathf.Lerp(startRotation.z, rotationIn.z, t);
            Vector3 rotationVector = new Vector3(0f, 0f, rotation);

            _rect.anchoredPosition = pos;
            _rect.localScale = scale;
            _rect.localRotation = Quaternion.Euler(rotationVector);

            if (_unscaledTime)
            {
                t += Time.unscaledDeltaTime / time;
            }
            else
            {
                t += Time.deltaTime / time;
            }
            yield return null;
        }

        _rect.anchoredPosition = positionIn;
        _rect.localScale = scaleIn;
        _rect.localRotation = Quaternion.Euler(rotationIn);

        if (_index < _transformsCache.Count - 1)
        {
            _index++;
            UITransformPoint pointNext = _transformsCache[_index];
            _routine = StartCoroutine(TransformCoroutineUI(pointNext));
        }

        if (OnTransformPointFunc != null)
        {
            OnTransformPointFunc();
            OnTransformPointFunc = null;
        }

        if (OnTransformFunc != null)
        {
            OnTransformFunc();
        }

        _performing = false;

    }
}



[Serializable]
public class UITransformPoint
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public Action OnTransformPointFunc;
    public float time;

    public bool isAdditionalRotation;

    public UITransformPoint(Vector3 position, Vector3 rotation = default(Vector3), Vector3 scale = default(Vector3), float time = 0.5f, Action OnTransformFunc = null)
    {
        if (scale == default(Vector3))
            scale = Vector3.one;
        
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.time = time;
        this.OnTransformPointFunc = OnTransformFunc;
    }

    public UITransformPoint()
    {
        
    }
}
