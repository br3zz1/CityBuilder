using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolController : MonoBehaviour
{
    public static ToolController Instance;

    public object hoveringOver;

    public IUseable Useable
    {
        get
        {
            return useable;
        }
        set
        {
            if (useable != null && value != null) useable.Return();
            useable = value;
        }
    }

    private IUseable useable;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        useable?.Tick();
        if(Input.GetMouseButtonDown(0))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                useable?.Use();
            }
        }
    }
}

public interface IUseable
{
    public bool Use();
    public void Tick();
    public void Return();
}
