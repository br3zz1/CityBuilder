using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileObject : MonoBehaviour
{

    public Tile tile { get; protected set; }

    public Dictionary<string, string> meta;

    public Queue<Material> childMaterials;

    private bool defaultMaterial = true;
    
    public virtual void Init(Tile tile)
    {
        this.tile = tile;
        TempInit();
    }

    public virtual void TempInit()
    {
        childMaterials = new Queue<Material>();
    }

    public virtual void OnMouseEnter()
    {
        ToolController.Instance.selected = this;
        if (ToolController.Instance.tool == Tool.Bulldoze)
        {
            ApplyHoverMaterial(new Color(1,0,0,0.5f));
        }
    }

    public void ApplyHoverMaterial(Color c)
    {
        if(defaultMaterial == false)
        {
            ApplyDefaultMaterial();
        }
        MeshRenderer[] allChildren = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in allChildren)
        {
            childMaterials.Enqueue(mr.material);
            Material hm = ToolController.Instance.hoverMaterial;
            hm.color = c;
            mr.material = hm;
        }
        defaultMaterial = false;
    }

    public void ApplyDefaultMaterial()
    {
        if (defaultMaterial == true) return;
        MeshRenderer[] allChildren = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in allChildren)
        {
            try
            {
                mr.material = childMaterials.Dequeue();
            }
            catch
            {

            }
        }
    }

    public virtual void OnMouseExit()
    {
        if (!defaultMaterial)
        {
            ApplyDefaultMaterial();
        }
    }

    public virtual void Terminate()
    {

    }

}
