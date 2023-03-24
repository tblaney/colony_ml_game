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
    public UIButton _buttonStateDefault;
    public List<UIVitality> _vitalities;
    public List<Color> _colors;
    public TMP_InputField _inputFieldName;
    public UICustomScrollbar _scrollbar;
    public UIController _controllerInventoryItem;
    public UIController _controllerAddonItem;
    List<UIController> _controllersItems;
    List<UIController> _controllsAddons;

    // cache:
    HabBot _bot;
    bool _stateOptionsActive;
    bool _active;

    //------------------------------------//
    public override void Initialize()
    {
        _buttonStates = new List<UIButton>();
        _buttonStateSelector.OnPointerClickFunc = () =>
        {
            ActivateStateOptions(!_stateOptionsActive);
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
        _button.GetController().SetText(_bot._name, "bot name");
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
        RefreshStates();
        RefreshInventoryAddons();

        _button.GetController().SetText(_bot._name, "bot name");
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
            };
            _buttonStates.Add(button);
            i++;
        }
        _controller.SetSize(new Vector2(124f, 41f*i), 31);
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
