using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIController : MonoBehaviour
{
    [Tooltip("any modifications to do with an image, including material changes")]
    [SerializeField] private List<UIBehaviourImage> _behavioursImage;
    [Tooltip("any modifications to do with a rect transform")]
    [SerializeField] private List<UIBehaviourRect> _behavioursRect;
    [Tooltip("any modifications to do with a text object")]
    [SerializeField] private List<UIBehaviourText> _behavioursText;
    [Tooltip("any modifications to do with activating/deactivating gameobjects")]
    [SerializeField] private List<UIBehaviourObject> _behavioursObject;
    [Tooltip("any modifications to do with fading a canvas group")]
    [SerializeField] private List<UIBehaviourFader> _behavioursFader;
    [Tooltip("any other added modifications")]
    [SerializeField] private List<UIBehaviourMisc> _behavioursMisc;

    private List<UIBehaviour> _behaviours;

    public bool _debug = false;

    void Awake()
    {
        FormList();
    }
    public void FormList()
    {
        if (_behaviours != null)
            return;
        _behaviours = new List<UIBehaviour>();
        _behaviours.AddRange(_behavioursObject);
        _behaviours.AddRange(_behavioursImage);
        _behaviours.AddRange(_behavioursRect);
        _behaviours.AddRange(_behavioursText);
        _behaviours.AddRange(_behavioursFader);
        _behaviours.AddRange(_behavioursMisc);

        foreach (UIBehaviour behaviour in _behaviours)
        {
            behaviour.OnChainFunc = ActivateBehaviourChain;
            behaviour.SetCallbackRoutineTimed(ActivateCallbackRoutineTimed);
            behaviour.SetCallbackRoutineConditional(ActivateCallbackRoutineConditional);
        }
    }

    public UIBehaviour GetBehaviour(int index)
    {
        if (_behaviours == null)
        {
            FormList();
        }

        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
                return behaviour;
        }

        return null;
    }

    public UIBehaviour GetBehaviour(string name)
    {
        if (_behaviours == null)
        {
            FormList();
        }

        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._name == name)
                return behaviour;
        }

        return null;
    }

    //-----------------------------
    public void ActivateBehaviour(int index, bool active = true)
    {
        if (_behaviours == null)
            FormList();
        
        Debug.Log("UIController Activate Behaviour: " + index + ", " + active);
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                if (active)
                {
                    behaviour.Run();
                }
                else
                {
                    behaviour.Stop();
                }
            }
        }
    }

    public void ActivateBehaviourChain(int index, List<int> afterIndex, bool active = true)
    {
        if (_debug)
        {
            Debug.Log("UI Controller Activate Behaviour: " + this.gameObject + ", To: " + index);
            if (afterIndex.Count > 0)
            {
                Debug.Log("UI Controller Activate Chain: " + this.gameObject + ", Count: " + afterIndex.Count + ", Target: " + afterIndex[afterIndex.Count - 1]);
            }
        }
        
        
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                if (active)
                {
                    if (afterIndex != null)
                    {
                        if (afterIndex.Count > 0)
                        {
                            behaviour._runIndex = afterIndex;
                            behaviour._reset = true;
                        }
                    }
                    behaviour.Run();
                }
                else
                {
                    if (afterIndex != null)
                    {
                        if (afterIndex.Count > 0)
                        {
                            behaviour._stopIndex = afterIndex;
                            behaviour._reset = true;
                        }
                    }
                    behaviour.Stop();
                }
            }
        }
    }

    public void ActivateBehaviour(string name, bool active = true)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._name == name)
            {
                if (active)
                {
                    behaviour.Run();
                }
                else
                {
                    behaviour.Stop();
                }
            }
        }
        
    }

    public void SetBehaviourAction(int index, Action OnBehaviourFunc)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index != index)
                continue;

            UIBehaviourRect rect = behaviour as UIBehaviourRect;
            if (rect != null)
            {
                rect.SetAction(OnBehaviourFunc);
            }
        }
    }
    public void ResetBehaviour(int index)
    {
        if (_behaviours == null)
            FormList();
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
                behaviour.Reset();
        }
    }

    //-----------------------------
    public Vector2 SetText(string text, int index = 0)
    {
        if (index != 0)
        {
            foreach (UIBehaviour behaviour in _behaviours)
            {
                if (behaviour._index == index)
                {
                    UIBehaviourText textBehaviour = behaviour as UIBehaviourText;
                    if (textBehaviour != null)
                    {
                        textBehaviour.SetText(text);
                        return textBehaviour.RefreshBounds();
                    }
                }
            }
        } else
        {
            foreach (UIBehaviour behaviour in _behaviours)
            {
                UIBehaviourText textBehaviour = behaviour as UIBehaviourText;
                if (textBehaviour != null)
                {
                    textBehaviour.SetText(text);
                    return textBehaviour.RefreshBounds();
                }
            }
        }

        return default(Vector2);
    }
    public void SetText(string text, string name)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._name == name)
            {
                UIBehaviourText textBehaviour = behaviour as UIBehaviourText;
                if (textBehaviour != null)
                {
                    textBehaviour.SetText(text);
                    return;
                }
            }
        }
    }
    public void SetTextColor(Color color, string name)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._name == name)
            {
                UIBehaviourText textBehaviour = behaviour as UIBehaviourText;
                if (textBehaviour != null)
                {
                    textBehaviour.SetColor(color);
                    return;
                }
            }
        }
    }
    public void RevealText(string text, string name, Action callback = null)
    {
        UIBehaviour behaviour = GetBehaviour(name);
        if (behaviour != null)
        {
            UIBehaviourText behaviour_text = behaviour as UIBehaviourText;
            behaviour_text.RevealText(text, callback);
        }
    }


    //-----------------------------
    public void SetImage(Sprite sprite, int index)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                UIBehaviourImage image = behaviour as UIBehaviourImage;
                if (image != null)
                {
                    image._image.sprite = sprite;
                    return;
                }
            }
        }
    }

    public void SetImage(Sprite sprite, string name)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._name == name)
            {
                UIBehaviourImage image = behaviour as UIBehaviourImage;
                if (image != null)
                {
                    image._image.sprite = sprite;
                    return;
                }
            }
        }
    }
    //-----------------------------
    public void SetPosition(Vector2 position, int index)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                UIBehaviourRect rect = behaviour as UIBehaviourRect;
                if (rect != null)
                {
                    rect.SetPosition(position);
                }
            }
        }
    }

    public void SetTransformPoint(UITransformPoint transform_point, int index)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                UIBehaviourRect rect = behaviour as UIBehaviourRect;
                if (rect != null)
                {
                    rect.SetTransformPoint(transform_point);
                }
            }
        }
    }

    public Vector3 GetPosition(int index)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                UIBehaviourRect rect = behaviour as UIBehaviourRect;
                if (rect != null)
                {
                    return rect.GetPosition();
                }
            }
        }

        return default(Vector3);
    }

    public void SetSize(Vector2 size, int index)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                UIBehaviourRect rect = behaviour as UIBehaviourRect;
                if (rect != null)
                {
                    rect.SetSize(size);
                }
            }
        }
    }
    public void SetScale(Vector3 scale, int index, float time)
    {
        foreach (UIBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                UIBehaviourRect rect = behaviour as UIBehaviourRect;
                if (rect != null)
                {
                    rect.SetScale(scale, time);
                }
            }
        }
    }

    public void ActivateCallbackRoutineTimed(float time, Action<float> callback_func)
    {
        StartCoroutine(CallbackRoutineTimed(time, callback_func));
    }

    public void ActivateCallbackRoutineConditional(Func<bool> condition, Action callback_func)
    {
        StartCoroutine(CallbackRoutineConditional(condition, callback_func));
    }

    private IEnumerator CallbackRoutineTimed(float time, Action<float> callback_func)
    {
        float t = 0f;
        while (t <= 1.0f)
        {
            if (callback_func != null)
                callback_func(t);

            t += Time.deltaTime/time;

            yield return null;
        }
    }

    private IEnumerator CallbackRoutineConditional(Func<bool> condition, Action callback_func)
    {
        while (condition())
        {
            callback_func();
            yield return null;
        }
    }


    

}


[Serializable]
public abstract class UIBehaviour
{
    [Header("Main Inputs")]
    public string _name;
    public int _index;

    [Space(15)]

    public Action<int, List<int>, bool> OnChainFunc;

    [Header("Do you want to execute another event when this is done?")]
    public List<int> _runIndex;
    public List<int> _stopIndex;
    public bool _reset = false;

    protected Action<float, Action<float>> _callback_routine_timed;
    protected Action<Func<bool>, Action> _callback_routine_conditional;


    public abstract void Run();

    public abstract void Stop();

    public abstract void Reset();

    public void ResetAfters()
    {
        _runIndex = new List<int>();
        _stopIndex = new List<int>();
    }

    public void SetCallbackRoutineTimed(Action<float, Action<float>> routine)
    {
        _callback_routine_timed = routine;
    }

    public void SetCallbackRoutineConditional(Action<Func<bool>, Action> routine)
    {
        _callback_routine_conditional = routine;
    }

}

[Serializable]
public class UIBehaviourImage : UIBehaviour
{
    public Image _image;

    public Color _colorIn = Color.white;
    public Color _colorOut = Color.white;

    public Sprite _spriteIn;
    public Sprite _spriteOut;
    
    public override void Run()
    {
        _image.color = _colorIn;

        if (_spriteIn != null)
            _image.sprite = _spriteIn;
    }

    public override void Stop()
    {
        _image.color = _colorOut;
        if (_spriteOut != null)
            _image.sprite = _spriteOut;
    }

    public override void Reset()
    {
        Stop();
    }
}

[Serializable]
public class UIBehaviourRect : UIBehaviour
{
    public UIObjectTransformer _transformer;
    public int _runTransformIndex = 1;
    public int _stopTransformIndex = 0;

    Action StoredAction;

    public override void Run()
    {
        Debug.Log("UI Behaviour Rect Move By: " + _transformer.gameObject + ", To: " + _runTransformIndex);
        if (_runIndex != null)
        {
            Debug.Log("UI Behaviour Rect Move After: " + _transformer.gameObject + ", To: " + _runIndex.Count);
        }
        
        _transformer.MoveTo(_runTransformIndex);

        Action newAction = () => 
        {
            if (StoredAction != null)
                StoredAction(); 

            StoredAction = null;
        };
        
        _transformer.OnTransformPointFunc = newAction;


        if (_runIndex != null)
        {
            // series of runs:
            if (_runIndex.Count == 0)
            {
                if (_reset)
                {
                    _runIndex = null;
                    _reset = false;
                    _transformer.OnTransformFunc = null;
                }
                return;
            }

            List<int> newRunIndices = new List<int>();
            int start = _runIndex[0];
            for (int i = 1; i < _runIndex.Count; i++)
            {
                newRunIndices.Add(_runIndex[i]);
            }

            _transformer.OnTransformFunc = () => 
            {
                Debug.Log("UI Behaviour Rect On Transform Point Func By: " + _transformer.gameObject);
                if (OnChainFunc != null)
                {
                    OnChainFunc(start, newRunIndices, true);
                    if (newRunIndices.Count == 0)
                    {
                        _transformer.OnTransformFunc = null;
                    }
                    //_transformer.OnTransformFunc = null;
                }
            };

            if (_reset)
            {
                _runIndex = null;
                _reset = false;
            }
        } else 
        {
            _transformer.OnTransformFunc = null;
        }
    }

    public override void Stop()
    {
        _transformer.MoveTo(_stopTransformIndex);

        if (_stopIndex != null)
        {
            if (_stopIndex.Count == 0)
            {
                if (_reset)
                {
                    _stopIndex = null;
                    _reset = false;
                }
                return;
            }
            _transformer.OnTransformFunc = () => 
            {
                if (OnChainFunc != null)
                {
                    List<int> newRunIndices = new List<int>();
                    for (int i = 1; i < _runIndex.Count; i++)
                    {
                        newRunIndices.Add(_runIndex[i]);
                    }
                    OnChainFunc(_stopIndex[0], newRunIndices, false);
                }
            };

            if (_reset)
            {
                _stopIndex = null;
                _reset = false;
            } 
       }
    }

    public void SetRun()
    {
        _transformer.MoveTo(_runTransformIndex, true);
    }

    public void SetStop()
    {
        _transformer.MoveTo(_stopTransformIndex, true);
    }

    public override void Reset()
    {
        _transformer.Reset();
    }

    public void SetPosition(Vector2 position)
    {
        _transformer.SetPosition(position);
    }

    public void SetTransformPoint(UITransformPoint transform_point)
    {
        List<UITransformPoint> points = new List<UITransformPoint>();
        points.Add(transform_point);

        _transformer.ActivateSeries(points);
    }

    public Vector3 GetPosition()
    {
        return _transformer.GetPosition();
    }

    public void SetScale(Vector3 scale, float time)
    {
        _transformer.SetScale(scale, time);
    }

    public void SetSize(Vector2 size)
    {
        _transformer.SetSize(size);
    }

    public void SetAction(Action OnTransformPointFunc)
    {
        StoredAction = OnTransformPointFunc;
        //_transformer.OnTransformPointFunc = OnTransformPointFunc;
    }
}

[Serializable]
public class UIBehaviourText : UIBehaviour
{
    public TextMeshProUGUI _text;

    public Color color_in = Color.white;
    public Color color_out = Color.white;

    public RectTransform _rectBackground;

    // cache:
    char[] _chars;
    int _idx;
    string _string;
    Action _callback;
    bool _revealing = false;

    public override void Run()
    {
        _text.color = color_in;
    }
    public override void Stop()
    {
        _text.color = color_out;
    }
    public override void Reset()
    {
        
    }
    public void SetText(string text)
    {
        if (_revealing)
            _revealing = false;
        
        _text.text = text;

        if (_rectBackground != null)
        {
            //Debug.Log("Set Text Refresh Box Size");
            _text.ForceMeshUpdate(true, true);
            //Vector2 preferredValues = new Vector2(_text.GetRenderedWidth(), _text.GetRenderedHeight());
            //Vector2 preferredValues = _text.GetPreferredValues();
            Vector2 preferredValues = _text.GetRenderedValues()*1.2f;
            //Vector2 preferredValues = _text.bounds.size;
            Debug.Log("Set Text Refresh Box Size: " + preferredValues + ", " + _text.bounds);
            Vector2 newSize = new Vector2(preferredValues.x, preferredValues.y);
            _rectBackground.sizeDelta = newSize;
        }
    }
    public void SetColor(Color color)
    {
        color_in = color;
        color_out = color;
        Run();
    }
    public Vector2 RefreshBounds()
    {
        _text.ForceMeshUpdate(); 
        return _text.GetPreferredValues();
    }

    public void RevealText(string text, Action callback)
    {
        _chars = text.ToCharArray();
        _idx = 0;
        _text.text = "";
        _callback = callback;
        _revealing = true;

        // call method
        _callback_routine_conditional(RevealTextCheck, UpdateText);
    }

    void UpdateText()
    {
        if (!_revealing)
            return;
        // callback for updatign revealing text
        _string += _chars[_idx];
        _idx++;

        _text.text = _string;
    }   

    bool RevealTextCheck()
    {
        if (!_revealing)
            return false;
        // returning false will stop the routine
        if (_chars == null)
        {
            _revealing = false;
            return false;
        }

        if (_idx >= _chars.Length)
        {
            if (_callback != null)
                _callback();

            _revealing = false;
            return false;
        }
        return true;
    }
}

[Serializable]
public class UIBehaviourMisc : UIBehaviour
{
    public override void Run()
    {

    }

    public override void Stop()
    {

    }

    public override void Reset()
    {
        
    }
}

[Serializable]
public class UIBehaviourObject : UIBehaviour
{
    public GameObject _object;
    public bool _inverse;
    public bool _destroy = false;

    public override void Run()
    {
        if (_destroy)
        {
            UnityEngine.Object.Destroy(_object.gameObject);
            return;
        }

        if (_inverse)
        {
            _object.SetActive(false);
        }
        else
        {
            _object.SetActive(true);
        }
    }

    public override void Stop()
    {
        if (_destroy)
        {
            UnityEngine.Object.Destroy(_object.gameObject);
            return;
        }
        if (_inverse)
        {
            _object.SetActive(true);
        }
        else
        {
            _object.SetActive(false);
        }
    }

    public override void Reset()
    {
        Stop();
    }
}

[Serializable]
public class UIBehaviourFader: UIBehaviour
{
    public UIFader _fader;
    public float _opacityOn;
    public float _opacityOff;
    public float _transitionTime;

    public bool _looping = false;

    public override void Run()
    {
       _fader.Fade(_opacityOn, _transitionTime);

        if (_looping)
        {
            _fader.OnFadeFunc = () => 
            {
                Stop();
            };

            return;
        }

        if (_runIndex != null)
        {    
            // series of runs:
            if (_runIndex.Count == 0)
            {
                if (_reset)
                {
                    _stopIndex = null;
                    _reset = false;
                }
                return;
            }
            _fader.OnFadeFunc = () => 
            {
                if (OnChainFunc != null)
                {
                    List<int> newRunIndices = new List<int>();
                    for (int i = 1; i < _runIndex.Count; i++)
                    {
                        newRunIndices.Add(_runIndex[i]);
                    }
                    OnChainFunc(_runIndex[0], newRunIndices, true);
                }
            };

            if (_reset)
            {
                _runIndex = null;
                _reset = false;
            }

            
       }
    }

    public override void Stop()
    {
       _fader.Fade(_opacityOff, _transitionTime);

        if (_looping)
        {
            _fader.OnFadeFunc = () => 
            {
                Run();
            };

            return;
        }


       if (_stopIndex != null)
       {
            if (_stopIndex.Count == 0)
            {
                if (_reset)
                {
                    _stopIndex = null;
                    _reset = false;
                }
                return;
            }
           _fader.OnFadeFunc = () => 
            {
                if (OnChainFunc != null)
                {
                    List<int> newRunIndices = new List<int>();
                    for (int i = 1; i < _runIndex.Count; i++)
                    {
                        newRunIndices.Add(_runIndex[i]);
                    }
                    OnChainFunc(_stopIndex[0], newRunIndices, false);
                }
            };

            if (_reset)
            {
                _stopIndex = null;
                _reset = false;
            } 
       }
    }

    public override void Reset()
    {
        _fader.SetOpacity(_opacityOff);
    }
    public void SetOpacity(float val)
    {
        _fader.SetOpacity(val);
    }
}
