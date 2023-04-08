using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialController : MonoBehaviour
{
    [SerializeField] private List<MaterialBehaviourFloat> _behavioursFloat;
    [SerializeField] private List<MaterialBehaviourColor> _behavioursColor;

    private List<MaterialBehaviour> _behaviours;

    public int _materialIndex = -1;

    Renderer _renderer;

    List<CustomBlock> _blocks;


    void Awake()
    {
        RefreshRenderer();

        Setup();
    }

    public void RefreshRenderer()
    {
        _renderer = this.GetComponent<MeshRenderer>();
        if (_renderer == null)
            _renderer = this.GetComponent<SkinnedMeshRenderer>();
        _blocks = new List<CustomBlock>();
        
        for (int i = 0; i < _renderer.sharedMaterials.Length; i++)
        {
            if (_materialIndex != -1 && _materialIndex != i)
                continue;
            
            CustomBlock customBlock = new CustomBlock() {};

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block, i);

            Texture2D tex = _renderer.sharedMaterials[i].GetTexture("_MainTex") as Texture2D;

            if (tex != null)
                block.SetTexture("_MainTex", tex);

            customBlock._block = block;
            if (tex != null)
                customBlock._tex = tex;

            _blocks.Add(customBlock);
        }

        Refresh();
    }

    void Setup()
    {
        _behaviours = new List<MaterialBehaviour>();

        if (_behavioursFloat != null)
        {
            foreach (MaterialBehaviourFloat mat in _behavioursFloat)
            {
                _behaviours.Add(mat);

                mat.Setup(_renderer, _blocks, Refresh);
            }
        }

        if (_behavioursColor != null)
        {
            foreach (MaterialBehaviourColor mat in _behavioursColor)
            {
                _behaviours.Add(mat);

                mat.Setup(_renderer, _blocks, Refresh);
            }
        }
    }
    
    
    void Update()
    {
        if (_behaviours == null)
            return;

        foreach (MaterialBehaviour behaviour in _behaviours)
        {
            if (behaviour._active)
            {
                behaviour.UpdateBlock();
            }
        }
    }


    public void SetMaterial(Material material, int index = 0)
    {
        _renderer.sharedMaterials[index] = material;

        RefreshRenderer();

        Setup();
    }

    public void SetMaterials(Material[] materials)
    {
        _renderer.sharedMaterials = materials;

        RefreshRenderer();

        Setup();
    }
    public void ActivateBehaviour(int index, bool active = true, Action OnStopFunc = null)
    {
        foreach (MaterialBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
            {
                if (active)
                    behaviour.Run(OnStopFunc);
                else
                    behaviour.Stop();
            }
        }
    }
    public void ActivateBehaviours(List<int> indices, bool active = true, Action OnStopFunc = null)
    {
        foreach (MaterialBehaviour behaviour in _behaviours)
        {
            if (indices.Contains(behaviour._index))
            {
                if (active)
                    behaviour.Run(OnStopFunc);
                else
                    behaviour.Stop();
            }
        }
    }
    public void SetColor(Color color, int index)
    {
        MaterialBehaviour behaviour = GetBehaviour(index);
        if (behaviour != null)
        {
            MaterialBehaviourColor behaviourColor = behaviour as MaterialBehaviourColor;
            behaviourColor.SetColor(color);
        }
    }
    public void SetColor(Color color, string name)
    {
        MaterialBehaviour behaviour = GetBehaviour(name);
        if (behaviour != null)
        {
            MaterialBehaviourColor behaviourColor = behaviour as MaterialBehaviourColor;
            behaviourColor.SetColor(color);
        }
    }
        public void SetFloat(float val, int index)
    {
        MaterialBehaviour behaviour = GetBehaviour(index);
        if (behaviour != null)
        {
            MaterialBehaviourFloat behaviourFloat = behaviour as MaterialBehaviourFloat;
            behaviourFloat.SetFloat(val);
        }
    }
    public void SetFloat(float val, string name)
    {
        MaterialBehaviour behaviour = GetBehaviour(name);
        if (behaviour != null)
        {
            MaterialBehaviourFloat behaviourFloat = behaviour as MaterialBehaviourFloat;
            behaviourFloat.SetFloat(val);
        }
    }

    public void AddBehaviour(MaterialBehaviour behaviour)
    {
        if (_behaviours == null)
            _behaviours = new List<MaterialBehaviour>();

        if (_behavioursFloat == null)
            _behavioursFloat = new List<MaterialBehaviourFloat>();

        if (_behavioursColor == null)
            _behavioursColor = new List<MaterialBehaviourColor>();

        behaviour.Setup(_renderer, _blocks, Refresh);
        _behaviours.Add(behaviour);
    }

    void Refresh()
    {
        for (int i = 0; i < _renderer.sharedMaterials.Length; i++)
        {
            if (_materialIndex != -1 && _materialIndex != i)
                continue;
            
            _renderer.SetPropertyBlock(_blocks[i]._block, i);
        }
    }

    public MaterialBehaviour GetBehaviour(int index)
    {
        foreach (MaterialBehaviour behaviour in _behaviours)
        {
            if (behaviour._index == index)
                return behaviour;
        }
        return null;
    }
    public MaterialBehaviour GetBehaviour(string index)
    {
        foreach (MaterialBehaviour behaviour in _behaviours)
        {
            if (behaviour._name == index)
                return behaviour;
        }
        return null;
    }
}


[Serializable]
public class CustomBlock
{
    public MaterialPropertyBlock _block;
    public Texture2D _tex;
}


[Serializable]
public abstract class MaterialBehaviour
{
    [Header("Inputs:")]
    public string _name;
    public int _index;
    private Renderer _renderer;

    public Action OnStopFunc;
    public Action OnRefreshFunc;

    [Space(10)]
    [Header("Do Not Set:")]
    public bool _active;
    public List<CustomBlock> _blocks;

    protected void Activate(bool active = true)
    {
        _active = active;
    }

    public void Setup(Renderer renderer, List<CustomBlock> blocks, Action OnRefresh)
    {
        _renderer = renderer;
        _blocks = blocks;

        OnRefreshFunc = OnRefresh;

        Refresh();
    }
    

    protected void Clear()
    {
        if (_blocks == null)
        {
            return;
        }
        
        foreach (CustomBlock block in _blocks)
        {
            block._block.Clear();

            if (block._tex != null)
                block._block.SetTexture("_MainTex", block._tex);
        }

        Refresh();
    }

    protected void Refresh()
    {
        if (OnRefreshFunc != null)
        {
            OnRefreshFunc();
        }
        //_renderer.SetPropertyBlock(_block, _materialIndex);
    }

    public abstract void Run(Action OnStopFunc = null);

    public abstract void UpdateBlock();

    public abstract void Stop();

}

[Serializable]
public class MaterialBehaviourFloat : MaterialBehaviour
{
    [Space(10)]
    [Header("Custom Float Properties:")]

    [SerializeField] private float _in;
    [SerializeField] private float _out;
    [SerializeField] private float _time = 1f;
    [SerializeField] private bool _hardIn = true;
    float _t;
    
    public override void Run(Action OnStopFunc = null)
    {   
        Activate();

        this.OnStopFunc = OnStopFunc;
        //_in = _blocks[0]._block.GetFloat(_name);
        if (!_hardIn)
        {
            _in = _blocks[0]._block.GetFloat(_name);
        }
        _t = 0f;
    }

    public override void UpdateBlock()
    {
        if (!_active)
            return;

        float value = Mathf.Lerp(_in, _out, _t);
        _t += Time.deltaTime / _time;

        Debug.Log("Material Behaviour Float: " + value);

        foreach (CustomBlock block in _blocks)
        {
            block._block.SetFloat(_name, value);
        }
        
        Refresh();
        //_renderer.SetPropertyBlock(_block);

        if (_t >= 1.0f)
        {
            Stop();
        }
    }

    public override void Stop()
    {
        Activate(false);

        if (OnStopFunc != null)
        {
            OnStopFunc();
        }
    }
    public void SetFloat(float val)
    {
        foreach (CustomBlock block in _blocks)
        {
            block._block.SetFloat(_name, val);
        }
        Refresh();
    }
}

[Serializable]
public class MaterialBehaviourColor : MaterialBehaviour
{
    [Space(10)]
    [Header("Custom Float Properties:")]

    [SerializeField] private Color _in;
    [SerializeField] private Color _out;

    float _t;
    
    public override void Run(Action OnStopFunc = null)
    {   
        this.OnStopFunc = OnStopFunc;

        foreach (CustomBlock block in _blocks)
        {
            block._block.SetColor(_name, _in);
        }
        Refresh();
    }

    public override void UpdateBlock()
    {

    }

    public override void Stop()
    {
        foreach (CustomBlock block in _blocks)
        {
            block._block.SetColor(_name, _out);
        }
        Refresh();

        if (OnStopFunc != null)
        {
            OnStopFunc();
        }
    }

    public void SetColor(Color color)
    {
        foreach (CustomBlock block in _blocks)
        {
            block._block.SetColor(_name, color);
        }
        Refresh();
    }
}

