using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField]
    private Color color;

    public TileObject tileObject { get; private set; }

    private TileObject temporaryObject;

    public int x { get; private set; }
    public int y { get; private set; }

    void Start()
    {
        GetComponent<MeshRenderer>().material.color = color;
        
    }

    public void SetCoords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void BuildTileObject(TileObject prefab)
    {
        if(prefab.gameObject == null)
        {
            Debug.LogError("prefab gameobject null");
            return;
        }
        GameObject obj = Instantiate(prefab.gameObject, transform.position, Quaternion.identity);
        obj.transform.parent = transform;
        TileObject t = obj.GetComponent<TileObject>();
        tileObject = t;
        t.Init(this);
        OnMouseExit();
    }

    public void TerminateTileObject()
    {
        if (tileObject == null) return;
        TileObject tObj = tileObject;
        tileObject = null;
        tObj.Terminate();
        Destroy(tObj.gameObject);
    }

    public void OnMouseEnter()
    {
        if(x < 37 || x > 63 || y < 37 || y > 63)
        {
            return;
        }
        ToolController.Instance.hoveringOver = this;
    }

    public void OnMouseExit()
    {
        if ((object)ToolController.Instance.hoveringOver == this) ToolController.Instance.hoveringOver = null;
    }
}
