using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotController : MonoBehaviour
{
    HabBot _bot;
    [SerializeField] private HabBotState _state;
    [SerializeField] private MaterialController _materialController;
    [SerializeField] private List<HabBotAddonObject> _addons;
    [SerializeField] private InventoryObject _inventoryObject;
    [SerializeField] private Interactable _interactable;
    AnimatorHandler _animator;
    NavigationController _nav;
    Action<HabBotController> OnDeathFunc;
    
    //----------------------------------------------------------//
    // setup
    public void Initialize(HabBot bot, Action<HabBotController> OnDeathFunc)
    {
        Debug.Log("HabBotController: " + bot._name);
        this.OnDeathFunc = OnDeathFunc;
        _animator = GetComponent<AnimatorHandler>();
        _nav = GetComponent<NavigationController>();

        this._bot = bot;
        SetupState();
        //_uiHabBot.Initialize(_bot);
        ClearAddons();

        _inventoryObject.Initialize(_bot._inventoryIndex, true);

        _bot.OnColorChange += Bot_ColorChange;
        RefreshColor();

        (_interactable as InteractableHabBot).Setup(_bot);
    }
    void OnDisable()
    {
        if (_bot != null)
            _bot.OnColorChange -= Bot_ColorChange;
    }
    void SetupState()
    {
        _state.SetupState(_bot);
        _state.StartState();
    }
    void RefreshColor()
    {
        _materialController.SetColor(_bot._traits._color, 2);
    }
    //----------------------------------------------------------//
    // state-switcher
    void Update()
    {
        if (_state != null)
            _state.UpdateState();

        StockpileCheck();
        RestCheck();
        float magnitude = _nav.GetVelocity().magnitude;
        if (magnitude < 0.2f)
            magnitude = 0f;
        _animator.SetFloat("Speed", magnitude);
        _bot._position = transform.position;
        _bot._velocity = _nav.GetVelocity();
    }
    public void SetState(HabBot.State state)
    {
        _bot.SetState(state);
    }
    public void MakeStateDecision()
    {
        SetState(_bot._stateDefault);
    }
    //----------------------------------------------------------//
    // checks
    bool RestCheck()
    {
        if (_bot._state != HabBot.State.Rest && _bot.GetVitality("energy")._val < 5) // on scale 1-100 so, will automatically rest when this occurs no matter the state that is persisting
        {
            _bot.SetState(HabBot.State.Rest, true);
            return true;
        }
        return false;
    }
    bool StockpileCheck()
    {
        ItemInventory inventory = ItemHandler.Instance.GetItemInventory(_bot._inventoryIndex);
        if (_bot._state != HabBot.State.Stockpile && !inventory.CapacityCheck())
        {
            SetState(HabBot.State.Stockpile);
            return true;
        }
        return false;
    }
    bool EatCheck()
    {
        if (_bot._state != HabBot.State.Eat && _bot.GetVitality("hunger")._val < 5)
        {
            _bot.SetState(HabBot.State.Eat, true);
            return true;
        }
        return false;
    }
    void Bot_ColorChange(object sender, EventArgs e)
    {
        RefreshColor();
    }
    // gets
    public HabBot GetBot()
    {
        return _bot;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    // end of this state
    public void DestroyBot()
    {
        if (_state != null)
            _state.StopState();
        if (OnDeathFunc != null)
            OnDeathFunc(this);
        Destroy(this.gameObject);
    }
    public void ActivateAddon(HabBotAddon.Type type)
    {
        ClearAddons();
        foreach (HabBotAddonObject obj in _addons)
        {
            if (obj._type == type)
            {
                obj._obj.SetActive(true);
            }
        }
    }
    public void ClearAddons()
    {
        foreach (HabBotAddonObject obj in _addons)
        {
            obj._obj.SetActive(false);
        }
    }
}

[Serializable]
public struct HabBotAddonObject
{
    public HabBotAddon.Type _type;
    public GameObject _obj;
}