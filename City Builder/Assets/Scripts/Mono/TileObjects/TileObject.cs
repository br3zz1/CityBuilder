using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileObject : MonoBehaviour
{

    public Tile tile { get; protected set; }

    public Dictionary<string, Tile> neighbours;

    [HideInInspector]
    public bool preview;

    private List<Color> defaultColors;
    private Dictionary<TileObject, ValueTextUsed> valueTexts;

    protected int score;

    private bool negative;

    public string ObjectName { get { return objectName; } }
    [SerializeField]
    private string objectName;
    
    public virtual void Init(Tile tile)
    {
        if (!preview)
        {
            WorldManager.Instance.tileObjects.Add(this);
            if(!WorldManager.Instance.testMode) SaveSystem.Save();
        }
        if(valueTexts == null) valueTexts = new Dictionary<TileObject, ValueTextUsed>();
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

    public virtual int AddedValue(bool negative = false)
    {
        this.negative = negative;
        return 0;
    }

    public virtual void Terminate()
    {
        GameManager.Instance.addedScoreText.text = "";
        if (!preview)
        {
            WorldManager.Instance.tileObjects.Remove(this);
            SaveSystem.Save();
        }
        else
        {
            foreach(KeyValuePair<TileObject, ValueTextUsed> v in valueTexts)
            {
                Destroy(v.Value.obj.gameObject);
            }
        }
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

    protected void BeginAddValue()
    {
        foreach (KeyValuePair<TileObject, ValueTextUsed> t in valueTexts)
        {
            t.Value.used = false;
        }
    }

    protected void AddValue(TileObject obj, int value)
    {
        if (obj == null) Debug.LogError("help");
        if (negative) value = -value;
        score += value;
        if(preview || negative)
        {
            if (valueTexts.ContainsKey(obj))
            {
                valueTexts[obj].used = true;
                valueTexts[obj].obj.text = (value > 0 ? "+": "-") + value;
            }
            else
            {
                GameObject val = Instantiate(WorldManager.Instance.valueTextPrefab, WorldManager.Instance.valueTextContainer.transform);
                Text txt = val.GetComponent<Text>();
                txt.text = (value > 0 ? "+" : "") + value;
                val.GetComponent<ValueText>().parent = obj;
                val.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
                valueTexts.Add(obj, new ValueTextUsed() { obj = txt, used = true });
            }
        }
    }

    protected void EndAddValue()
    {
        negative = false;
        List<TileObject> clear = new List<TileObject>();
        foreach (KeyValuePair<TileObject, ValueTextUsed> t in valueTexts)
        {
            if (!t.Value.used)
            {
                clear.Add(t.Key);
            }
        }
        foreach (TileObject t in clear)
        {
            Destroy(valueTexts[t].obj.gameObject);
            valueTexts.Remove(t);
        }
    }

    public void ClearValueTexts()
    {
        foreach (KeyValuePair<TileObject, ValueTextUsed> v in valueTexts)
        {
            Destroy(v.Value.obj.gameObject);
        }
        valueTexts.Clear();
    }
}

public class ValueTextUsed
{
    public bool used;
    public Text obj;
}
