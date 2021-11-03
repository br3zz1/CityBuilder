using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileObject : MonoBehaviour
{

    public Tile tile { get; protected set; }

    public Dictionary<string, string> meta;
    
    public virtual void Init(Tile tile)
    {
        this.tile = tile;
    }

    public virtual void OnMouseEnter()
    {
        ToolController.Instance.hoveringOver = this;
    }

    public virtual void OnMouseExit()
    {
        if ((object)ToolController.Instance.hoveringOver == this) ToolController.Instance.hoveringOver = null;
    }

    public virtual void Terminate()
    {

    }

}
