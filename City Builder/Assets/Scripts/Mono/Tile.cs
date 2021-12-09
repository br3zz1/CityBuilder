using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public TileObject tileObject { get; private set; }

    public int x { get; private set; }
    public int y { get; private set; }

    public bool untouchable;

    void Start()
    {
        
    }

    public void SetCoords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void BuildTileObject(TileObject prefab, bool init = true, bool smoke = true)
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
        if(init) t.Init(this);
        if(GameManager.Instance != null && smoke)
        {
            GameObject g = Instantiate(GameManager.Instance.SmokeEffect, transform.position, Quaternion.identity);
            Destroy(g, 1.5f);
        }
        OnMouseExit();
    }

    public void TerminateTileObject(bool smoke = true)
    {
        if (tileObject == null) return;
        TileObject tObj = tileObject;
        tileObject = null;
        tObj.Terminate();
        if(smoke)
        {
            GameObject g = Instantiate(GameManager.Instance.SmokeEffect, transform.position, Quaternion.identity);
            Destroy(g, 1.5f);
        }
        Destroy(tObj.gameObject);
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void OnMouseEnter()
    {
        
        if (untouchable) return;
        ToolController.Instance.hoveringOver = this;
    }

    public void OnMouseExit()
    {
        if ((object)ToolController.Instance.hoveringOver == this) ToolController.Instance.hoveringOver = null;
    }
}
