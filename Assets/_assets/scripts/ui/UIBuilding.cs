using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuilding : UIObject
{
    public static int _currentBuildingIndex = 0;

    [Header("Inputs:")]
    public string _prefabNameBuilding;
    public GameObject _controllerContainer;
    public float _controllerSpacing = 40f;

    [Space(10)]
    List<UIController> _controllers;
    bool _active;

    public override void Initialize()
    {
        _controllers = new List<UIController>();
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
        foreach (Building building in BuildingHandler.Instance.GetBuildings())
        {
            GameObject prefab = PrefabHandler.Instance.GetPrefab(_prefabNameBuilding);
            GameObject obj = Instantiate(prefab, _controllerContainer.transform);
            UIController controller = obj.GetComponent<UIController>();
            //controller.SetText(parameter._name, "name");
            //controller.SetText(parameter._description, "description");
            //controller.SetText(building._amount.ToString(), "amount");
            //controller.SetText("(" + building._type.ToString() + ")", "type");
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
