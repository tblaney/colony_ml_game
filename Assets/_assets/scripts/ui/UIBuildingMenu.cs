using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuildingMenu : UIObject
{
    public static int _currentBuildingIndex = 0;

    [Header("Inputs:")]
    public string _prefabNameBuilding;
    public GameObject _buttonContainer;
    public float _controllerSpacing = 40f;
    public UIButton _button;

    [Space(10)]
    List<UIButton> _buttons;
    bool _active;
    
    public override void Initialize()
    {
        _buttons = new List<UIButton>();
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
            GameObject obj = Instantiate(prefab, _buttonContainer.transform);
            UIButton button = obj.GetComponent<UIButton>();
            button.OnPointerClickFunc = () =>
            {
                ButtonClickCallback(building);
            };
            UIController controller = button.GetController();
            controller.SetText(building._name, "name");
            controller.SetText(building._description, "description");
            string items = "";
            foreach (ItemInput itemInput in building._itemRequirements)
            {
                items += itemInput._name + ", " + itemInput._amount.ToString() + " " + "\n";
            }
            controller.SetText(items, "items");
            _buttons.Add(button);
            y++;
        }
        // need to refresh size of contents
        float height = y*_controllerSpacing;
        _controller.SetSize(new Vector2(300f, height + 20f), 10);
    }
    void ButtonClickCallback(Building building)
    {
        Debug.Log("UIBuilding Button Click Callback");
        _currentBuildingIndex = building._index;
        //ActivateUI(false);
        UserHandler.Instance.SetUserState(UserController.State.Building);
        _button.OnPointerClickFunc();
    }
    void ClearAll()
    {
        foreach (UIButton button in _buttons)
        {
            Destroy(button.gameObject);
        }
        _buttons.Clear();
    }
}
