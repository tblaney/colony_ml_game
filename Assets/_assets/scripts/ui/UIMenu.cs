using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIMenu : UIObject
{
    [SerializeField] private UIButton _newGameButton;
    [SerializeField] private UIButton _continueButton;
    Coroutine _routine;
    bool _ready = false;
    public override void Initialize()
    {
        _newGameButton.OnPointerClickFunc = NewGame;
        _continueButton.OnPointerClickFunc = ContinueGame;
        NodeProcessor.OnProcessorReady += NodeProcessor_OnReady;
    }
    public void Setup()
    {
        Refresh();
    }
    void Refresh()
    {
        if (SaveSystem.SaveExists(1))
        {
            _continueButton.ActivateController("activate", true);
        } else
        {
            _continueButton.ActivateController("activate", false);
        }
    }
    void NewGame()
    {
        _ready = false;
        if (SaveSystem.SaveExists(1))
        {
            SaveSystem.DeleteSave(1);
        }
        _routine = StartCoroutine(LoadingRoutine(0));
        _controller.ActivateBehaviour("loading start", true);
    }
    void ContinueGame()
    {
        _ready = false;
        _routine = StartCoroutine(LoadingRoutine(1));
        _controller.ActivateBehaviour("loading start", true);
    }
    void NodeProcessor_OnReady(object sender, EventArgs e)
    {
        _ready = true;
    }

    private IEnumerator LoadingRoutine(int index)
    {
        _controller.SetText("", "loading text");
        yield return new WaitForSeconds(0.25f);
        GameHandler.Instance.Load(index);
        string dots = ".";
        int i = 0;
        float f = 0.2f;
        while (!_ready)
        {
            if (i == 2)
            {
                dots = ".";
                i = 0;
            } else
            {
                dots += ".";
                i++;
            }
            _controller.SetText(dots, "loading text");
            yield return new WaitForSecondsRealtime(f);
        }
        _controller.ActivateBehaviour("background fade", false);
    }
}
