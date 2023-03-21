using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIHabBot : UIObject
{
    [Header("Inputs:")]
    public UIButton _button;
    public UIController _controllerInfo;
    public UIButton _buttonStateSelector;
    public List<UIButton> _buttonStates;
    public List<UIVitality> _vitalities;
    public List<Color> _colors;
    public TMP_InputField _inputFieldName;
    public UICustomScrollbar _scrollbar;

    // cache:
    HabBot _bot;
    bool _stateOptionsActive;
    bool _active;

    //------------------------------------//
    public override void Initialize()
    {
        int i = 0;
        foreach (UIButton button in _buttonStates)
        {
            UIController controller = button.GetController();
            controller.FormList();
            HabBot.State state = (HabBot.State)i;
            controller.SetText(state.ToString(), 1);
            button.OnPointerClickFunc = () =>
            {
                if (_bot != null)
                    _bot.SetState(state);

                ActivateStateOptions(false);
            };
            i++;
        }
        _buttonStateSelector.OnPointerClickFunc = () =>
        {
            ActivateStateOptions(!_stateOptionsActive);
        };
        _stateOptionsActive = false;
    }
    public void Setup(HabBot bot, Action<UIHabBot> buttonClickFunc)
    {
        _bot = bot;
        Initialize();
        _inputFieldName.text = _bot._name;
        int i = 0;
        foreach (UIVitality vitality in _vitalities)
        {
            vitality.Setup(_bot._vitalities[i]);
            i++;
        }
        _button.OnPointerClickFunc = () => { buttonClickFunc(this); };
        _bot.OnStateChange += Bot_StateChange;
        _bot._traits.OnTraitsChange += Bot_TraitsChange;
        _inputFieldName.onValueChanged.AddListener(delegate{InputNameChange();});
        Refresh();
        Activate(false);
    }
    public void Activate(bool active = true)
    {
        ActivateStateOptions(false);
        if (active)
        {
            _button.GetController().ActivateBehaviour(1, true);
            _controllerInfo.ActivateBehaviour(2, true);
            _controllerInfo.ActivateBehaviour(1, true);
        } else
        {
            _button.GetController().ActivateBehaviour(1, false);
            _controllerInfo.ActivateBehaviour(1, false);
        }
        _active = active;
    }
    void ActivateStateOptions(bool active)
    {
        if (active)
        {
            _controller.ActivateBehaviour(2, true);
            _controller.ActivateBehaviour(1, true);
        } else
        {
            _controller.ActivateBehaviour(1, false);
        }
        _stateOptionsActive = active;
    }
    public void InputNameChange()
    {
        _bot._name = _inputFieldName.text;
    }
    public void Bot_StateChange(object sender, EventArgs e)
    {
        RefreshState();
    }
    public void Bot_TraitsChange(object sender, EventArgs e)
    {
        Refresh();
    }
    void Refresh()
    {
        _button.GetController().SetText(_bot._name, "bot name");
        HabBotTraits traits = _bot._traits;
        foreach (HabBotTrait trait in traits._traits)
        {
            _controller.SetText(trait._val.ToString("F1"), trait._type.ToString());
            _controller.SetTextColor(GetColor(trait._val), trait._type.ToString());
        }
        RefreshState();
    }
    void RefreshState()
    {
        UIController controller = _buttonStateSelector.GetController();
        controller.FormList();
        controller.SetText(_bot._state.ToString(), 1);
    }
    Color GetColor(float val)
    {
        if (val < 0.33f)
        {
            return _colors[0];
        } else if (val >= 0.33f && val < 0.66f)
        {
            return _colors[1];
        } else
        {
            return _colors[2];
        }
    }
    public void SetPostion(Vector2 position)
    {
        _controller.SetPosition(position, 20);
    }
    public Vector2 GetPosition()
    {
        return GetComponent<RectTransform>().anchoredPosition;
    }
}
