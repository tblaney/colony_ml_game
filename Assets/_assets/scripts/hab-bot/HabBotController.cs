using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotController : MonoBehaviour
{
    HabBot _bot;
    [SerializeField] private HabBotState _state;
    [SerializeField] private MaterialController _materialController;
    [SerializeField] private UIHabBotWorld _uiHabBot;
    [SerializeField] private List<HabBotAddonObject> _addons;
    [SerializeField] private InventoryObject _inventoryObject;
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
        SetupBot();
        ClearAddons();

        _inventoryObject.Initialize(_bot._inventoryIndex, true);
    }
    void SetupState()
    {
        _state.Initialize();
        _state.StartState();
    }
    void SetupBot()
    {
        _materialController.SetColor(HabitationHandler.Instance.GetBotColor(_bot._traits._colorIndex), 2);
        _uiHabBot.Initialize(_bot);
    }
    //----------------------------------------------------------//
    // state-switcher
    void Update()
    {
        if (_state != null)
            _state.UpdateState();

        RestCheck();
        _animator.SetFloat("Speed", _nav.GetVelocity().magnitude);
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
    void RestCheck()
    {
        if (_bot.GetVitality("energy")._val < 1) // on scale 1-100 so, will automatically rest when this occurs no matter the state that is persisting
        {
            SetState(HabBot.State.Rest);
        }
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
    void ClearAddons()
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