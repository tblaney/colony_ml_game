using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UIHabitation : UIObject
{
    [Header("Inputs:")]
    [SerializeField] private string _buttonDefaultPrefabName;
    [SerializeField] private UIItemInventory _uiItemInventory;
    [Header("Debug:")]
    [SerializeField] private List<UIHabBot> _uiBots;
    Habitation _habitation;
    UIHabBot _uiBotCurrent;

    public override void Initialize()
    {

    }
    public void Setup(Habitation habitation)
    {
        _habitation = habitation;
        _uiItemInventory.Setup(habitation);
        RefreshBots();
    }
    void RefreshBots()
    {
        ClearBots();
        SetupBots();
    }
    void ClearBots()
    {
        if (_uiBots == null)
        {
            _uiBots = new List<UIHabBot>();
            return;
        }
        foreach (UIHabBot bot in _uiBots)
        {
            bot.DestroyUI();
        }
        _uiBots.Clear();
    }
    void SetupBots()
    {
        _uiBots = new List<UIHabBot>();
        GameObject prefab = PrefabHandler.Instance.GetPrefab(_buttonDefaultPrefabName);
        foreach (HabBot bot in _habitation._bots)
        {
            GameObject obj = Instantiate(prefab, this.transform);
            UIHabBot uiHabBot = obj.GetComponent<UIHabBot>();
            uiHabBot.Setup(bot, BotClickFunc);
            _uiBots.Add(uiHabBot);
        }
        CenterUIHabBots();
    }
    void BotClickFunc(UIHabBot uiHabBot)
    {
        if (_uiBotCurrent != null)
        {
            if (_uiBotCurrent == uiHabBot)
            {
                _uiBotCurrent.Activate(false);
                _uiBotCurrent = null;
                return;
            }
            _uiBotCurrent.Activate(false);
        }
        _uiBotCurrent = uiHabBot;
        _uiBotCurrent.Activate(true);
    }   
    void CenterUIHabBots()
    {
        // get start x position
        int count = _uiBots.Count;
        int count_half = Mathf.FloorToInt(count / 2);
        float x = 0f;
        if (count % 2 == 0)
        {
            x = -(count_half*120f) - 60f;
        } else
        {
            x = -(count_half*120f);
        }
        Vector2 position = _uiBots[0].GetPosition();
        Vector2 positionStart = new Vector2(x, position.y);
        int i = 0;
        foreach (UIHabBot uiBot in _uiBots)
        {
            uiBot.SetPostion(positionStart + new Vector2((120f*i), 0f));
            i++;
        }
    }
}
