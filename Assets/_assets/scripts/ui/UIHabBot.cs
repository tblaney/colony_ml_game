using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIHabBot : UIObject
{
    [Header("Inputs:")]
    public UIButton _buttonMain;
    public UIButton _buttonStateSelector;
    public UIButton _buttonStateDefault;
    public UIButton _buttonExpansion;
    List<UIButton> _buttonStates;
    public UIController _controllerInfo;
    public UIController _controllerInventoryItem;
    public UIController _controllerAddonItem;
    List<UIController> _controllersItems;
    List<UIController> _controllsAddons;
    public List<UIVitality> _vitalities;
    public TMP_InputField _inputFieldName;
    public UICustomScrollbar _scrollbar;
    public UIColorSwitcher _colorSwitcher;
    public List<Color> _colors;
    // static:
    public static HabBot _activeHabBotFollow;

    // cache:
    HabBot _bot;
    bool _stateOptionsActive;
    bool _active;
    bool _expansion;

    //------------------------------------//
    public override void Initialize()
    {
        _buttonStates = new List<UIButton>();
        _buttonStateSelector.OnPointerClickFunc = () =>
        {
            if (_bot._stateCooldown | _bot._stateLock)
                return;
            
            ActivateStateOptions(!_stateOptionsActive);
        };
        _buttonExpansion.OnPointerClickFunc = () =>
        {
            Expand(!_expansion);
        };
        _stateOptionsActive = false;
        _controllersItems = new List<UIController>();
        _controllsAddons = new List<UIController>();
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
        _buttonMain.OnPointerClickFunc = () => { buttonClickFunc(this); };
        _bot.OnStateChange += Bot_StateChange;
        _bot._traits.OnTraitsChange += Bot_TraitsChange;
        _bot.OnStateAccessChange += Bot_StateAccessChange;
        _inputFieldName.onValueChanged.AddListener(delegate{InputNameChange();});

        _colorSwitcher.SetSliderValue(1, _bot._traits._color.r);
        _colorSwitcher.SetSliderValue(2, _bot._traits._color.g);
        _colorSwitcher.SetSliderValue(3, _bot._traits._color.b);
        _colorSwitcher.OnColorChangeFunc = ColorSwitcherChangeFunc;

        Refresh();
        Activate(false);
        Expand(false);
    }
    public void Activate(bool active = true)
    {
        Expand(false);
        ActivateStateOptions(false);
        if (active)
        {
            _activeHabBotFollow = this._bot;
            UserHandler.Instance.SetUserState(UserController.State.Following);
            _buttonMain.GetController().ActivateBehaviour(1, true);
            _controllerInfo.ActivateBehaviour(2, true);
            _controllerInfo.ActivateBehaviour(1, true);
        } else
        {
            _activeHabBotFollow = null;
            UserHandler.Instance.SetUserState(UserController.State.Viewing);
            _buttonMain.GetController().ActivateBehaviour(1, false);
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
    void Expand(bool active)
    {
        if (active)
            _controllerInfo.ActivateBehaviour("expansion activate", true);

        _controllerInfo.ActivateBehaviour("expansion", active);
        _expansion = active;
    }
    public void InputNameChange()
    {
        _bot._name = _inputFieldName.text;
        _buttonMain.GetController().SetText(_bot._name, "bot name");
    }
    public void Bot_StateChange(object sender, EventArgs e)
    {
        //RefreshState();
        Refresh();
    }
    public void Bot_TraitsChange(object sender, EventArgs e)
    {
        Refresh();
    }
    public void Bot_StateAccessChange(object sender, EventArgs e)
    {
        Refresh();
    }
    void ColorSwitcherChangeFunc(Color color)
    {
        _bot.SetColor(color);
    }
    void Refresh()
    {
        RefreshStates();
        RefreshInventoryAddons();

        if (_bot._stateLock)
        {
            _buttonStateSelector.ActivateController(52, true);
        } else if (_bot._stateCooldown)
        {
            _buttonStateSelector.ActivateController(50, true);
        } else
        {
            _buttonStateSelector.ActivateController(51, true);
        }

        _buttonMain.GetController().SetText(_bot._name, "bot name");
        HabBotTraits traits = _bot._traits;
        foreach (HabBotTrait trait in traits._traits)
        {
            _controller.SetText(trait._val.ToString("F1"), trait._type.ToString());
            _controller.SetTextColor(GetColor(trait._val), trait._type.ToString());
        }

        RefreshState();
    }
    void ClearStates()
    {
        foreach (UIButton button in _buttonStates)
        {
            Destroy(button.gameObject);
        }
        _buttonStates.Clear();
    }
    void RefreshStates()
    {
        ClearStates();
        List<HabBot.State> states = _bot.GetAvailableStates();
        int i = 0;
        foreach (HabBot.State state in states)
        {
            UIButton button = Instantiate(_buttonStateDefault, _buttonStateDefault.transform.parent);
            button.Activate(true);
            UIController controller = button.GetController();
            controller.FormList();
            controller.SetText(state.ToString(), 1);
            button.OnPointerClickFunc = () =>
            {
                if (_bot != null)
                    _bot.SetState(state);

                ActivateStateOptions(false);
                Refresh();
            };
            _buttonStates.Add(button);
            i++;
        }
        _controller.SetSize(new Vector2(95f*i, 35f), 31);
    }
    void RefreshState()
    {
        UIController controller = _buttonStateSelector.GetController();
        controller.FormList();
        controller.SetText(_bot._state.ToString(), 1);
    }
    void ClearInventoryAddons()
    {
        foreach (UIController controller in _controllersItems)
        {
            Destroy(controller.gameObject);
        }
        foreach (UIController controller in _controllsAddons)
        {
            Destroy(controller.gameObject);
        }
        _controllersItems.Clear();
        _controllsAddons.Clear();
    }
    void RefreshInventoryAddons()
    {
        ClearInventoryAddons();
        List<Item> items = ItemHandler.Instance.GetItemInventory(_bot._inventoryIndex)._items;
        List<HabBotAddon> addons = _bot._addons;
        foreach (Item item in items)
        {
            if (item._amount <= 0)
                continue;
            
            UIController controllerItem = Instantiate(_controllerInventoryItem, _controllerInventoryItem.transform.parent);
            controllerItem.gameObject.SetActive(true);
            controllerItem.FormList();
            controllerItem.SetText(item._name, "name");
            controllerItem.SetText(item._amount.ToString(), "amount");
            _controllersItems.Add(controllerItem);
        }
        foreach (HabBotAddon addon in addons)
        {
            UIController controllerAddon = Instantiate(_controllerAddonItem, _controllerAddonItem.transform.parent);
            controllerAddon.gameObject.SetActive(true);
            controllerAddon.FormList();
            controllerAddon.SetText(addon._type.ToString(), "name");
            _controllersItems.Add(controllerAddon);
        }
    }
    public HabBot GetBot()
    {
        return _bot;
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
