using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    public static WorldManager Instance { get; private set; }

    [SerializeField]
    private int worldSize;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Road roadPrefab;

    private Tile[,] world;

    private bool[,] roadLayout;

    [SerializeField]
    private Material tileMaterial;
    [SerializeField]
    private Material tileUntouchableMaterial;
    
    void Start()
    {
        Instance = this;
        world = new Tile[worldSize, worldSize];
        RoadGenerator.townRadius = 7;
        roadLayout = RoadGenerator.GenerateRoadLayout(5);
        GenerateWorld();
    }

    void Update()
    {
        
    }

    void GenerateWorld()
    {
        LoopCoordinates(SpawnTile, new Vector2Int(worldSize, worldSize));
        LoopCoordinates(Generation, new Vector2Int(worldSize, worldSize));
    }

    void SpawnTile(Vector2Int coord)
    {
        GameObject tileObj = Instantiate(tilePrefab, new Vector3(coord.x, 0, coord.y), Quaternion.Euler(90, 0, 0));
        world[coord.x, coord.y] = tileObj.GetComponent<Tile>();
        tileObj.name = "Tile [" + coord.x + "," + coord.y + "]";
        tileObj.transform.parent = transform;
        Tile tile = tileObj.GetComponent<Tile>();
        Material m = tileMaterial;
        if (coord.x < 37 || coord.x > 63 || coord.y < 37 || coord.y > 63)
        {
            tile.untouchable = true;
            m = tileUntouchableMaterial;
        }
        tile.SetCoords(coord.x, coord.y, m);
    }

    void Generation(Vector2Int coord)
    {
        if (roadLayout[coord.x, coord.y])
        {
            world[coord.x, coord.y].BuildTileObject(roadPrefab);
        }
    }

    public Dictionary<string,Tile> GetNeighboursNSEW(Vector2Int coord)
    {
        Dictionary<string, Tile> tileDic = new Dictionary<string, Tile>();

        if(coord.y < 99)
        {
            tileDic.Add("N", world[coord.x, coord.y + 1]);
        }
        if (coord.y > 0)
        {
            tileDic.Add("S", world[coord.x, coord.y - 1]);
        }
        if (coord.x < 99)
        {
            tileDic.Add("E", world[coord.x + 1, coord.y]);
        }
        if (coord.x > 0)
        {
            tileDic.Add("W", world[coord.x - 1, coord.y]);
        }

        return tileDic;
    }

    public void Cleartile(int x, int y)
    {
        world[x, y].TerminateTileObject();
    }

    void LoopCoordinates(Action<Vector2Int> action, Vector2Int size, Vector2Int? offset = null)
    {
        if (!offset.HasValue) offset = new Vector2Int(0, 0);
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                action?.Invoke(new Vector2Int(x,y) + offset.Value);
            }
        }
    }
}
