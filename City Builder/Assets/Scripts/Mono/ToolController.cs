using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public static ToolController Instance;

    public Tool tool;

    public object selected;

    public List<NamedTileObject> namedTileObjects;

    public TileObject tileObjectBuildPrefab;

    public Material hoverMaterial;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        tool = Tool.Build;
        tileObjectBuildPrefab = namedTileObjects[0].tileObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(tool == Tool.Bulldoze)
            {
                if(selected is TileObject)
                {
                    ((TileObject)selected).tile.TerminateTileObject();
                }
            }
            if(tool == Tool.Build)
            {
                Debug.Log("p");
                if(selected is Tile)
                {
                    Debug.Log("pp");
                    ((Tile)selected).BuildTileObject(tileObjectBuildPrefab);
                }
            }
        }
    }
}

[Serializable]
public class NamedTileObject {
    public string name;
    public TileObject tileObject;
}

public enum Tool
{
    Bulldoze,
    Build,
    Select,
    None
}
