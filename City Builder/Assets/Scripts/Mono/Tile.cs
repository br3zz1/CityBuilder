using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField]
    private Color color;

    public TileObject tileObject { get; private set; }

    private TileObject temporaryObject;

    void Start()
    {
        GetComponent<MeshRenderer>().material.color = color;
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
        ToolController.Instance.selected = this;
        if(ToolController.Instance.tool == Tool.Build)
        {
            if(tileObject == null && ToolController.Instance.tileObjectBuildPrefab != null)
            {
                ToolController.Instance.tileObjectBuildPrefab.gameObject.layer = 2;
                GameObject obj = Instantiate(ToolController.Instance.tileObjectBuildPrefab.gameObject, transform.position, Quaternion.identity);
                ToolController.Instance.tileObjectBuildPrefab.gameObject.layer = 0;
                obj.transform.parent = transform;
                temporaryObject = obj.GetComponent<TileObject>();
                temporaryObject.TempInit();
                temporaryObject.ApplyHoverMaterial(new Color(0.7f,0.7f,1,0.5f));
            }
        }
    }

    public void OnMouseExit()
    {
        if(temporaryObject != null)
        {
            Destroy(temporaryObject.gameObject);
            temporaryObject = null;
        }
    }

    void Update()
    {
        
    }
}
