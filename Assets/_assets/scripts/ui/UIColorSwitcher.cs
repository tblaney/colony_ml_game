using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIColorSwitcher : UIObject
{
    [SerializeField] private List<UISlider> _sliders;
    [SerializeField] private Image _image;
    public Action<Color> OnColorChangeFunc;
    public Color _color;
    public override void Initialize()
    {
        _color.a = 1f;
        foreach (UISlider slider in _sliders)
        {
            slider._slider.onValueChanged.AddListener((float val) => SliderCallback(slider));
            SliderCallback(slider);
        }
    }
    void SliderCallback(UISlider slider)
    {
        Debug.Log("UI Color Switcher: " + slider._slider.value + ", " + slider._index);
        float val = slider._slider.value;
        switch (slider._index)
        {
            case 1:
                _color.r = val;
                break;
            case 2:
                _color.g = val;
                break;
            case 3:
                _color.b = val;
                break;
        }
        if (OnColorChangeFunc != null)
            OnColorChangeFunc(_color);  

        _image.color = _color;
    }
    public void SetSliderValue(int index, float val)
    {
        foreach (UISlider slider in _sliders)
        {
            if (slider._index == index)
            {
                slider._slider.value = val;
                SliderCallback(slider);
                return;
            }
        }
    }
}

[Serializable]
public struct UISlider
{
    public Slider _slider;
    public int _index;
}
