using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemMenu : UIObject
{
    [Header("Inputs:")]
    public string _prefabNameItem;
    public GameObject _controllerContainer;
    public float _controllerSpacing = 40f;

    [Space(10)]
    List<UIController> _controllers;
    Habitation _habitation;
    bool _active;
    public override void Initialize()
    {
        _controllers = new List<UIController>();
    }
    public void Setup(Habitation habitation)
    {
        _habitation = habitation;
        Refresh();
        Activate(false);
    }
    public override void Activate(bool active = true)
    {
        _active = active;
        if (active)
        {
            Refresh();
        }
    }
    void Refresh()
    {
        ClearAll();
        int y = 0;
        foreach (Item item in _habitation.GetAllItems())
        {
            if (item._amount == 0)
                continue;
            
            GameObject prefab = PrefabHandler.Instance.GetPrefab(_prefabNameItem);
            GameObject obj = Instantiate(prefab, _controllerContainer.transform);
            UIController controller = obj.GetComponent<UIController>();
            controller.SetText(item._name, "name");
            controller.SetText(item._options._description, "description");
            controller.SetText(item._amount.ToString(), "amount");
            controller.SetText("(" + item._options._type.ToString() + ")", "type");
            _controllers.Add(controller);
            y++;
        }
        // need to refresh size of contents
        float height = y*_controllerSpacing;
        _controller.SetSize(new Vector2(300f, height + 20f), 10);
    }
    void ClearAll()
    {
        foreach (UIController controller in _controllers)
        {
            Destroy(controller.gameObject);
        }
        _controllers.Clear();
    }
}
