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
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.paused == false && useable != null)
        {
            /*
            useable?.Tick();
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    useable?.Use();
                }
            }*/

            if (Input.GetMouseButton(0))
            {
                useable?.Tick();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    useable?.Use();
                    useable?.Return();
                }
                else
                {
                    useable?.Return();
                }
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
