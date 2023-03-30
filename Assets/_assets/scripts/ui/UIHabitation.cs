using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UIHabitation : UIObject
{
    [Header("Inputs:")]
    [SerializeField] private string _buttonDefaultPrefabName;
    [SerializeField] private UIItemMenu _uiItemMenu;
    [SerializeField] private UIBuildingMenu _uiBuildingMenu;
    [SerializeField] private UIHabitationQueue _uiQueue;
    [SerializeField] private UINode _uiNode;
    [SerializeField] private List<UIBuilding> _uiBuildings;
    [SerializeField] private UIEnemy _uiEnemy;

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
        _uiItemMenu.Setup(habitation);
        RefreshBots();
        _habitation.OnBotAmountChange += Habitation_BotChange;
    }
    private void Habitation_BotChange(object sender, EventArgs e)
    {
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
        _uiNode.ActivateUI(false);
        foreach (UIBuilding uIBuilding in _uiBuildings)
        {
            uIBuilding.ActivateUI(false);
        }
        _uiEnemy.Activate(false);
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
            x = -(count_half*120f) + 60f;
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
    public void ActivateBot(HabBot bot)
    {
        UIHabBot uiBot = GetUIHabBot(bot);
        if (uiBot != null)
        {
            BotClickFunc(uiBot);
        }
    }
    public void ActivateNode(Node node, Building building = null)
    {
        ClearBottom();
        if (building != null)
        {
            UIBuilding uiBuilding = GetUIBuilding(building._index);
            uiBuilding.Setup(node, building);
        } else
        {
            _uiNode.Setup(node);
        }
    }
    public void ActivateEnemy(IEnemy enemy)
    {
        ClearBottom();
        _uiEnemy.Setup(enemy);
    }
    void ClearBottom()
    {
        _uiNode.ActivateUI(false);
        if (_uiBotCurrent != null)
        {
            BotClickFunc(_uiBotCurrent);    
        }
        foreach (UIBuilding uIBuilding in _uiBuildings)
        {
            uIBuilding.ActivateUI(false);
        }
        _uiEnemy.ActivateUI(false);
    }
    public UIBuilding GetUIBuilding(int index)
    {
        foreach (UIBuilding build in _uiBuildings)
        {
            if (build._indexBuilding == index)
                return build;
        }
        return null;
    }
    public UIHabBot GetUIHabBot(HabBot bot)
    {
        foreach (UIHabBot uiBot in _uiBots)
        {
            if (uiBot.GetBot() == bot)
                return uiBot;
        }
        return null;
    }
}
