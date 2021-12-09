using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    public static WorldManager Instance { get; private set; }

    public bool testMode;

    [Header("Tree Generation Settings")]
    [SerializeField]
    private float treeNoiseScale;
    [SerializeField]
    private float treeProbability;
    [SerializeField]
    private float treeThreshold;

    private int worldSeed;

    private int worldSize = 100;

    private Tile[,] world;

    private GameObject tilePrefab;
    private Road roadPrefab;

    private RoadType[,] roadLayout;

    public List<TileObject> tileObjects;

    public int calculatedScore;
    
    void Awake()
    {
        Instance = this;
        tileObjects = new List<TileObject>();
        world = new Tile[worldSize, worldSize];
        tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");
        roadPrefab = Resources.Load<Road>("Prefabs/TileObjects/Road");
        LoopCoordinates(SpawnTile, new Vector2Int(worldSize, worldSize));
    }

    void Start()
    {
        if(testMode)
        {
            GenerateWorld();
            return;
        }
        if (!SaveSystem.Load())
        {
            Debug.Log("Generating new level");
            worldSeed = UnityEngine.Random.Range(0, 2000000);
            RoadGenerator.townRadius = 7;
            roadLayout = RoadGenerator.GenerateRoadLayout(worldSeed);
            GenerateWorld();
        }
        else
        {
            Debug.Log("Loading level");
        }
    }

    /*public void CalculateScore()
    {
        calculatedScore = 0;
        LoopCoordinates(Calculate, new Vector2Int(worldSize, worldSize));
    }

    void Calculate(Vector2Int coord)
    {
        TileObject tObj = world[coord.x, coord.y].tileObject;
        if(tObj != null)
        {
            calculatedScore += tObj.CalculateScore();
        }
    }*/

    void GenerateWorld()
    {
        UnityEngine.Random.InitState(worldSeed);
        LoopCoordinates(Generation, new Vector2Int(worldSize, worldSize));
    }

    public void Regenerate()
    {
        TileObject[] tempArr = tileObjects.ToArray();
        foreach(TileObject t in tempArr)
        {
            t.tile.TerminateTileObject(false);
        }
        GenerateWorld();
    }

    public Tile TileAt(int x, int y)
    {
        return world[x, y];
    }

    void SpawnTile(Vector2Int coord)
    {
        GameObject tileObj = Instantiate(tilePrefab, new Vector3(coord.x, 0, coord.y), Quaternion.Euler(90, 0, 0));
        world[coord.x, coord.y] = tileObj.GetComponent<Tile>();
        tileObj.name = "Tile [" + coord.x + "," + coord.y + "]";
        tileObj.transform.parent = transform;
        Tile tile = tileObj.GetComponent<Tile>();
        if (coord.x < 37 || coord.x > 63 || coord.y < 37 || coord.y > 63)
        {
            tile.untouchable = true;
            //m = tileUntouchableMaterial;
        }
        tile.SetCoords(coord.x, coord.y);
    }

    void Generation(Vector2Int coord)
    {
        if(testMode)
        {
            TreeGeneration(coord);
            return;
        }
        if (roadLayout[coord.x, coord.y] != RoadType.None)
        {
            if(roadLayout[coord.x, coord.y] == RoadType.Main)
            {
                world[coord.x, coord.y].BuildTileObject(roadPrefab, false, false);
                ((Road)world[coord.x, coord.y].tileObject).main = true;
                world[coord.x, coord.y].tileObject.Init(world[coord.x, coord.y]);
            }
            else if(coord.x > 38 && coord.x < 62 && coord.y > 38 && coord.y < 62)
            {
                world[coord.x, coord.y].BuildTileObject(roadPrefab, smoke: false);
            }
        }
        else
        {
            TreeGeneration(coord);
        }
    }

    void TreeGeneration(Vector2Int coord)
    {
        //if (UnityEngine.Random.Range(0, 20) < 1) world[coord.x, coord.y].BuildTileObject(GameManager.Instance.namedPrefabs["Tree"], smoke: false);
        float pX = coord.x * treeNoiseScale * Mathf.PI;
        float pY = coord.y * treeNoiseScale * Mathf.PI;
        float value = Mathf.Clamp01(Mathf.PerlinNoise(pX, pY));
        if(value > treeThreshold)
        {
            float p = Mathf.Pow(UnityEngine.Random.Range(0f,1f), treeProbability);
            if(value > p) world[coord.x, coord.y].BuildTileObject(GameManager.Instance.namedPrefabs["Tree"], smoke: false);
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

    public void RecursiveRoadSearch(Road road, int maxDistance, ref List<ObjectDistance> objects, ref List<Road> visited, int distance = 1)
    {
        if (distance > maxDistance) return;

        visited.Add(road);

        foreach (KeyValuePair<string, Tile> n in road.neighbours)
        {
            if (n.Value.tileObject is Road)
            {
                Road r = (Road)n.Value.tileObject;
                if (visited.Contains(r)) continue;
                RecursiveRoadSearch(r, maxDistance, ref objects, ref visited, distance + 1);
            }
            else if (n.Value.tileObject != null)
            {
                bool newDist = true;
                foreach(ObjectDistance rd in objects)
                {
                    if(rd.obj == n.Value.tileObject)
                    {
                        newDist = false;
                        rd.distance = Mathf.Min(rd.distance, distance);
                    }
                }
                if (newDist) objects.Add(new ObjectDistance() { distance = distance, obj = n.Value.tileObject });
            }
        }
    }
    
}

public class ObjectDistance
{
    public float distance;
    public TileObject obj;
}
