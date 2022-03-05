using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public List<int> allowedRotations;
    public bool roadSide;
    public bool nonRotable;

    public string ObjectName { get { return objectName; } }
    [SerializeField]
    private string objectName;

    public string ObjectDesc { get { return objectDesc; } }
    [SerializeField, TextArea]
    private string objectDesc;

    private float tooltipCooldown;
    private bool tooltip;
    
    public virtual void Init(Tile tile)
    {
        if (!preview)
        {
            WorldManager.Instance.tileObjects.Add(this);
            //if(!WorldManager.Instance.testMode) SaveSystem.Save();
        }
        if(valueTexts == null) valueTexts = new Dictionary<TileObject, ValueTextUsed>();
        this.tile = tile;
        neighbours = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
        defaultColors = new List<Color>();
        if(preview)
        {
            allowedRotations = new List<int>();
            if (nonRotable) allowedRotations.Add(0);
            else if (roadSide) AllowRoadsideRotations();
            else allowedRotations.AddRange(new int[] { 0, 90, 180, 270 });
        }
    }

    private void Update()
    {
        if((object)ToolController.Instance.hoveringOver == this && !EventSystem.current.IsPointerOverGameObject())
        {
            if (!tooltip && ToolController.Instance.Useable == null)
            {
                tooltipCooldown -= Time.deltaTime;
                if (tooltipCooldown < 0)
                {
                    tooltip = true;
                    TooltipSystem.Instance.Show(objectName, objectDesc, Color.grey);
                }
            }
        }
        
    }

    public virtual void OnMouseEnter()
    {
        if (tile.untouchable) return;
        ToolController.Instance.hoveringOver = this;
        tooltipCooldown = 1f;
    }

    public virtual void OnMouseExit()
    {
        if ((object)ToolController.Instance.hoveringOver == this) ToolController.Instance.hoveringOver = null;
        if(defaultColors.Count > 0) DefaultColor();
        if(tooltip)
        {
            tooltip = false;
            TooltipSystem.Instance.Hide();
        }
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
                if (def)
                {
                    foreach(Material mat in child.GetComponent<MeshRenderer>().materials)
                    {
                        defaultColors.Add(mat.color);
                        mat.color = color;
                    }
                }
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
                foreach(Material mat in child.GetComponent<MeshRenderer>().materials)
                {
                    mat.color = defaultColors[i];
                    i++;
                }
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
                valueTexts[obj].obj.color = value > 0 ? GameManager.Instance.positiveValueTextColor : GameManager.Instance.negativeValueTextColor;
                valueTexts[obj].obj.text = (value > 0 ? "+": "") + value;
            }
            else
            {
                GameObject val = Instantiate(WorldManager.Instance.valueTextPrefab, WorldManager.Instance.valueTextContainer.transform);
                Text txt = val.GetComponent<Text>();
                txt.color = value > 0 ? GameManager.Instance.positiveValueTextColor : GameManager.Instance.negativeValueTextColor;
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

    private void AllowRoadsideRotations()
    {
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            if (n.Key == "N")
            {
                if (n.Value.tileObject is Road) allowedRotations.Add(180);
            }
            if (n.Key == "S")
            {
                if (n.Value.tileObject is Road) allowedRotations.Add(0);
            }
            if (n.Key == "E")
            {
                if (n.Value.tileObject is Road) allowedRotations.Add(270);
            }
            if (n.Key == "W")
            {
                if (n.Value.tileObject is Road) allowedRotations.Add(90);
            }
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
