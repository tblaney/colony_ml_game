using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIFader : MonoBehaviour
{
    [Header("Inputs:")]
    [SerializeField] private CanvasGroup _canvas;

    // cache:
    Coroutine _coroutine;
    public Action OnFadeFunc;

    bool _performing;
    float _opacityCache;



    public void Fade(float outOpacity, float time, Action onFade = null)
    {
        _opacityCache = outOpacity;

        if (!gameObject.activeInHierarchy)
        {
            SetOpacity(outOpacity);
            if (onFade != null)
                onFade();

            return;
        }

        if (_coroutine != null) 
        {
            StopCoroutine(_coroutine);
            OnFadeFunc = null;
        }

        _coroutine = StartCoroutine(FadeCoroutine(outOpacity, time));

        if (onFade != null) {
            OnFadeFunc = onFade;
        }
    }


    public void SetOpacity(float opacity)
    {
        _canvas.alpha = opacity;
    }

    void OnDisable()
    {
        if (_performing)
        {
            SetOpacity(_opacityCache);
            /*
            if (OnFadeFunc != null)
            {
                OnFadeFunc();
            }
            */
            //OnFadeFunc = null;
            _performing = false;

            if (_coroutine != null) 
            {
                StopCoroutine(_coroutine);
            }
        }
    }

    void OnEnable()
    {
        OnFadeFunc = null;
        _performing = false;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }



    private IEnumerator FadeCoroutine(float outOpacity, float time) 
    {
        _performing = true;

        float t = 0;

        float startOpacity = _canvas.alpha;

        while (t <= 1.0f) {
            
            float opacity = Mathf.Lerp(startOpacity, outOpacity, t);
            _canvas.alpha = opacity;

            t += Time.deltaTime / time;

            yield return null;
        }

        _canvas.alpha = outOpacity;

        Action OnFade = OnFadeFunc;
        
        if (OnFadeFunc != null) 
        {
            //OnFadeFunc();
            OnFadeFunc = null;
            OnFade();
        }



        _performing = false;

    }
}
