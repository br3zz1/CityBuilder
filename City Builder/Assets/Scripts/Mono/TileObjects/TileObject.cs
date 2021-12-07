using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{

    public Tile tile { get; protected set; }

    public Dictionary<string, Tile> neighbours;

    [HideInInspector]
    public bool preview;

    private List<Color> defaultColors;


    public string ObjectName { get { return objectName; } }
    [SerializeField]
    private string objectName;
    
    public virtual void Init(Tile tile)
    {
        if (!preview)
        {
            WorldManager.Instance.tileObjects.Add(this);
            SaveSystem.Save();
        }
        this.tile = tile;
        neighbours = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
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

    public virtual int AddedValue()
    {
        return 0;
    }

    public virtual void Terminate()
    {
        WorldManager.Instance.tileObjects.Remove(this);
        SaveSystem.Save();
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

    protected void ForeachRoadDistanceDo(Action<ObjectDistance> action)
    {
        if (preview) neighbours = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
        List<ObjectDistance> objects = new List<ObjectDistance>();
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            List<Road> visited = new List<Road>();
            if (n.Value.tileObject is Road) WorldManager.Instance.RecursiveRoadSearch((Road)n.Value.tileObject, 10, ref objects, ref visited);
        }
        List<TileObject> calculatedTileObjects = new List<TileObject>();
        foreach (ObjectDistance rd in objects)
        {
            if (rd.obj == this) continue;
            if (calculatedTileObjects.Contains(rd.obj)) continue;
            calculatedTileObjects.Add(rd.obj);
            action(rd);
        }
    }

    protected void ForeachNeighbourDo(Action<TileObject> action)
    {
        if (preview) neighbours = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            if (n.Value.tileObject != null) action(n.Value.tileObject);
        }
    }

    protected void ForeachAirDistanceDo(Action<ObjectDistance> action)
    {
        foreach(TileObject t in WorldManager.Instance.tileObjects)
        {
            if (t == this) continue;
            if (t == null) continue;
            float dist = Vector3.Distance(t.tile.transform.position, tile.transform.position);
            action(new ObjectDistance() { distance = dist, obj = t });
        }
    }
}
