using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{

    public Tile tile { get; protected set; }

    public Dictionary<string, string> meta;

    public bool preview;

    private List<Color> defaultColors;
    
    public virtual void Init(Tile tile)
    {
        this.tile = tile;
        defaultColors = new List<Color>();
    }

    public virtual void OnMouseEnter()
    {
        if (tile.untouchable) return;
        ToolController.Instance.hoveringOver = this;
    }

    public virtual void OnMouseExit()
    {
        if ((object)ToolController.Instance.hoveringOver == this) ToolController.Instance.hoveringOver = null;
        if(defaultColors.Count > 0) DefaultColor();
    }

    public virtual void Terminate()
    {

    }

    public void ChangeColor(Color color)
    {
        bool def = defaultColors.Count < 1;
        if (GetComponent<MeshRenderer>() != null)
        {
            if(def) defaultColors.Add(GetComponent<MeshRenderer>().material.color);
            GetComponent<MeshRenderer>().material.color = color;
        }
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.GetComponent<MeshRenderer>() != null)
            {
                if (def) defaultColors.Add(child.GetComponent<MeshRenderer>().material.color);
                child.GetComponent<MeshRenderer>().material.color = color;
            }
        }
    }

    public void DefaultColor()
    {
        if (defaultColors == null) return;
        if (defaultColors.Count < 1) return;
        int i = 0;
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().material.color = defaultColors[i];
            i++;
        }
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.GetComponent<MeshRenderer>() != null)
            {
                child.GetComponent<MeshRenderer>().material.color = defaultColors[i];
                i++;
            }
        }
        defaultColors.Clear();
    }

}
