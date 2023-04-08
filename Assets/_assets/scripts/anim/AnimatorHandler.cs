using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField] private List<AnimatorObject> animatorObjects;

    [SerializeField] private List<UnityEvent> animatorEvents;


    public Action<int> OnWeaponEquipFunc;
    public Action OnBikeJumpFunc;

    public Action OnNextUpdateFunc;


    public List<AnimatorEvent> _animatorEventList;
    private List<AnimatorEvent> _animatorEventCache;


    [SerializeField] private bool _debug;

    public float _time;

    // mono
    void Awake()
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            if (animatorObject.layerIndexList.Contains(1))
                animatorObject.animator.SetLayerWeight(1, 0f);
        }
    }

    void LateUpdate()
    {
        if (OnNextUpdateFunc != null)
        {
            OnNextUpdateFunc();
            OnNextUpdateFunc = null;
        }

        List<AnimatorEvent> removalList = new List<AnimatorEvent>();
        if (_debug)
            Debug.Log("Animator Events: " + _animatorEventList.Count);
        foreach (AnimatorEvent animatorEvent in _animatorEventList) 
        {   
            if (animatorEvent.remove)
            {
                removalList.Add(animatorEvent);
                continue;
            }

            AnimatorStateInfo info = GetCurrentAnimatorStateInfo(animatorEvent.index);
            //Debug.Log("Animator Handler Time: " + info.normalizedTime);
            _time = info.normalizedTime;
            if (_time > 1f)
                _time = info.normalizedTime - Mathf.FloorToInt(info.normalizedTime);
                
            if (info.IsName(animatorEvent.name))
            {  
                if (_time < 0.05f)
                {
                    if (animatorEvent.executed && animatorEvent.looping)
                    {   
                        animatorEvent.executed = false;
                    }
                }
                if (!IsInTransition(0))
                {    
                    if (_time >= animatorEvent.timeThreshold)
                    {
                        if (!animatorEvent.executed)
                        {
                            if (animatorEvent.OnEventFunc != null)
                                animatorEvent.OnEventFunc();
                            
                            animatorEvent.executed = true;
                        }

                        if (!animatorEvent.looping)
                            removalList.Add(animatorEvent);
                    } 
                }
                
            } else 
            {
                //if (info.normalizedTime >= animatorEvent.timeThreshold && !IsInTransition(0))
                //{
                //    removalList.Add(animatorEvent);
                //}
            }
        }
        if (_animatorEventCache != null) 
        {
            foreach (AnimatorEvent animatorEvent in _animatorEventCache) 
            {
                _animatorEventList.Add(animatorEvent);
            }

            _animatorEventCache.Clear();
        }

        foreach (AnimatorEvent animatorEvent in removalList) 
        {
            _animatorEventList.Remove(animatorEvent);
        }
    }

    void Update()
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            if (!animatorObject.isWeightPlaying)
                continue;

            Animator _animator = animatorObject.animator;
            if (_animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.95f && !_animator.IsInTransition(1))
            {
                if (animatorObject.weightCoroutine != null)
                    StopCoroutine(animatorObject.weightCoroutine);

                animatorObject.weightCoroutine = StartCoroutine(AnimatorLayerWeightChange(1f, 0f, 1f, _animator, 1));
                animatorObject.isWeightPlaying = false;
            }
        }
        
        
    }

    // animator methods yo chill:
    public void CrossFadeInFixedTime(string name, float time)
    {
        Debug.Log("Animator Handler Cross Fade To: " + name);

        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            foreach (int index in animatorObject.layerIndexList)
            {
                if (animatorObject.animator.HasState(index, Animator.StringToHash(name)))
                {

                    Animator _animator = animatorObject.animator;
                    if (index != 0)
                    {
                        if (!animatorObject.isWeightPlaying)
                        {
                            if (animatorObject.weightCoroutine != null)
                                StopCoroutine(animatorObject.weightCoroutine);

                            animatorObject.weightCoroutine = StartCoroutine(AnimatorLayerWeightChange(0f, 1f, 0.1f, _animator, index));
                            animatorObject.isWeightPlaying = true;
                        }
                    }
                    _animator.CrossFadeInFixedTime(name, time, index);
                }
            }
        }
    }

    public Animator GetAnimator()
    {
        return animatorObjects[0].animator;
    }

    public void SetAnimationState(string stateName, float transitionDuration = 0.1f)
    {
        Debug.Log("Animator Handler Set Animation To: " + stateName);

        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            bool hasBaseLayer = false;
            foreach (int index in animatorObject.layerIndexList)
            {
                Animator _animator = animatorObject.animator;

                if (_animator.HasState(index, Animator.StringToHash(stateName)))
                {
                    if (index != 0)
                    {
                        if (!hasBaseLayer)
                        {
                            if (!animatorObject.isWeightPlaying)
                            {
                                if (animatorObject.weightCoroutine != null)
                                    StopCoroutine(animatorObject.weightCoroutine);

                                animatorObject.weightCoroutine = StartCoroutine(AnimatorLayerWeightChange(0f, 1f, 0.1f, _animator, index));
                                animatorObject.isWeightPlaying = true;
                            }
                        }
                    }
                    else
                    {
                        hasBaseLayer = true;
                    }

                    _animator.CrossFadeInFixedTime(stateName, transitionDuration, index);
                }
                else
                {
                    _animator.CrossFadeInFixedTime("Grounded", transitionDuration, index);
                }
            }
        }
    }

    public void SetAnimationState(string stateName, float transitionDuration, int index)
    {
        Debug.Log("Animator Handler Set Animation To: " + stateName);

        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            Animator _animator = animatorObject.animator;

            if (_animator.HasState(index, Animator.StringToHash(stateName)))
            {
                if (index != 0)
                {
                    if (!animatorObject.isWeightPlaying)
                    {
                        //_animator.SetLayerWeight(index, 1f);
                        if (animatorObject.weightCoroutine != null)
                            StopCoroutine(animatorObject.weightCoroutine);

                        animatorObject.weightCoroutine = StartCoroutine(AnimatorLayerWeightChange(0f, 1f, 0.3f, _animator, index));
                        animatorObject.isWeightPlaying = true;
                    }
                }

                _animator.CrossFadeInFixedTime(stateName, transitionDuration, index);
            }
            else
            {
                _animator.CrossFadeInFixedTime("Grounded", transitionDuration, index);
            }
        }
    }

    public void SetAnimationStateObject(string stateName, float transitionDuration, int index)
    {
        Animator _animator = animatorObjects[index].animator;

        if (_animator.HasState(index, Animator.StringToHash(stateName)))
        {
            _animator.CrossFadeInFixedTime(stateName, transitionDuration, index);
        }
        else
        {
            _animator.CrossFadeInFixedTime("Grounded", transitionDuration, index);
        }
    }

    public AnimatorStateInfo GetCurrentAnimatorStateInfo(int index)
    {
        return animatorObjects[0].animator.GetCurrentAnimatorStateInfo(index);
    }

    public bool IsInTransition(int index)
    {
        return animatorObjects[0].animator.IsInTransition(index);
    }

    public bool IsMatchingTarget()
    {
        return animatorObjects[0].animator.isMatchingTarget;
    }

    public void MatchTarget(Vector3 _matchTargetPosition, Quaternion _matchTargetRotation, AvatarTarget avatarTarget, MatchTargetWeightMask weightMask, float startTime, float targetTime)
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            Animator _animator = animatorObject.animator;

            _animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0f), 0.4f, 0.9f);
        }
    }

    public void SetBool(string name, bool value)
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            Animator _animator = animatorObject.animator;

            _animator.SetBool(name, value);
        }
    }

    public void SetFloat(string name, float value, float dampTime, float deltaTime)
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            Animator _animator = animatorObject.animator;

            _animator.SetFloat(name, value, dampTime, deltaTime);


            Debug.Log("Animator Set Float, for: " + animatorObject.animator.gameObject.ToString() + ", value: " + name + " (" + value.ToString() + ")");
        }
    }

    public bool GetBool(string name)
    {
        return animatorObjects[0].animator.GetBool(name);
    }

    public void SetFloat(int id, float value, float dampTime, float deltaTime)
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            Animator _animator = animatorObject.animator;

            _animator.SetFloat(id, value, dampTime, deltaTime);
        }
    }

    public void SetFloat(string name, float value)
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            Animator _animator = animatorObject.animator;

            _animator.SetFloat(name, value);

            Debug.Log("Animator Set Float, for: " + animatorObject.animator.gameObject.ToString() + ", value: " + name + " (" + value.ToString() + ")");

        }
    }

    public void SetInt(string name, int value)
    {
        foreach (AnimatorObject animatorObject in animatorObjects)
        {
            Animator _animator = animatorObject.animator;
            _animator.SetInteger(name, value);
        }
    }

    public float GetFloat(string name)
    {
        return animatorObjects[0].animator.GetFloat(name);
    }

    public Animator GetMainAnimator()
    {
        return animatorObjects[0].animator;
    }

    public void RemoveAnimatorObjects(List<AnimatorObject> objects)
    {
        foreach (AnimatorObject _object in objects)
        {
            if (animatorObjects.Contains(_object))
            {
                animatorObjects.Remove(_object);
            }
        }
    }

    public void AddAnimatorObjects(List<AnimatorObject> objects)
    {
        foreach (AnimatorObject _object in objects)
        {
            if (!animatorObjects.Contains(_object))
            {
                animatorObjects.Add(_object);
            }
        }
    }

    public void AddAnimatorEvent(AnimatorEvent eventIn)
    {
        if (_debug)
            Debug.Log("Animator Event Add: " + eventIn);

        if (_animatorEventCache == null) {
            _animatorEventCache = new List<AnimatorEvent>();
        }
        _animatorEventCache.Add(eventIn);
        //_animatorEventList.Add(eventIn);
    }

    public void RemoveAnimatorEvent(AnimatorEvent eventIn)
    {
        if (_animatorEventCache.Contains(eventIn))
            _animatorEventCache.Remove(eventIn);
    }

    //Animator Event Listeners:
    public void WeaponEquip(int index)
    {
        //OnWeaponEquip?.Invoke(this, EventArgs.Empty);
        if (OnWeaponEquipFunc != null)
        {
            OnWeaponEquipFunc(index);
        }

    }

    public void BikeJump()
    {
        if (OnBikeJumpFunc != null)
        {
            OnBikeJumpFunc();
        }
    }

    public void AnimatorEvent(int index)
    {
        //animatorEvents[index].Invoke();
    }

    private IEnumerator AnimatorLayerWeightChange(float weightIn, float weightOut, float time, Animator animator, int index)
    {
        float t = 0f;

        while (t <= 1.0f)
        {

            float weight = Mathf.Lerp(weightIn, weightOut, t);
            animator.SetLayerWeight(index, weight);


            t += Time.deltaTime / time;

            yield return null;
        }
    }


}



[Serializable]
public class AnimatorObject
{
    public Animator animator;
    public List<int> layerIndexList;


    public bool isWeightPlaying;
    public Coroutine weightCoroutine;
}


[Serializable]
public class AnimatorEvent 
{
    public Action OnEventFunc;
    public float timeThreshold;
    public string name;
    public int index;

    public bool looping = false;
    public bool remove = false;
    public bool executed = false;

    public AnimatorEvent(string name, Action OnEventFunc, float timeThreshold, int layer = 0)
    {
        this.name = name;
        this.OnEventFunc = OnEventFunc;
        this.timeThreshold = timeThreshold;
        this.index = layer;
    }
}
