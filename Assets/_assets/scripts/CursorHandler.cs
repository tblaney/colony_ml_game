using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour, IHandler
{
    public static CursorHandler Instance;
    public Texture2D _cursor_tex;
    private CursorMode _cursor_mode = CursorMode.Auto;
    private Vector2 _hot_spot = new Vector2(0.5f, 0.5f);
    public void Initialize()
    {
        Instance = this;
        Setup();
    }
    public void Setup()
    {
        _hot_spot.x = _cursor_tex.width/2;
        _hot_spot.y = _cursor_tex.height/2;
        Cursor.SetCursor(_cursor_tex, _hot_spot, _cursor_mode);
    }
}
