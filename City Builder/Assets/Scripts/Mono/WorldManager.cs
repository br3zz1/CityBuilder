using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    public static WorldManager Instance { get; private set; }

    [Header("Noise Generation Settings")]
    [SerializeField]
    private float treeNoiseScale;
    [SerializeField]
    private float treeProbability;
    [SerializeField]
    private float treeThreshold;
    [SerializeField]
    private float waterNoiseScale;
    [SerializeField]
    private float waterThreshold;

    [Header("Vein Generation Settings")]
    [SerializeField]
    private float coalProbability;

    private Vector2 noiseOffset;

    private int worldSeed;

    private int worldSize = 100;

    private Tile[,] world;

    private GameObject tilePrefab;
    private Road roadPrefab;

    private RoadType[,] roadLayout;

    public GameObject valueTextPrefab;
    public GameObject valueTextContainer;

    public List<TileObject> tileObjects;

    public int calculatedScore;

    public Queue<Action> loadQueue;
    private bool loadComplete;
    private int totalJobs;

    private Mesh tileMesh;

    void Awake()
    {
        Instance = this;
        tileObjects = new List<TileObject>();
        loadQueue = new Queue<Action>();
        world = new Tile[worldSize, worldSize];
        tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");
        roadPrefab = Resources.Load<Road>("Prefabs/TileObjects/Road");
        LoopCoordinates(SpawnTile, new Vector2Int(60, 60), new Vector2Int(20, 20));
    }

    void Start()
    {
        if (!SaveSystem.Load())
        {
            GameManager.Instance.lastMilestone = 0;
            GameManager.Instance.nextMilestone = 50;
            GameManager.Instance.UpdateScore(0);

            CardManager.Instance.AddCard("House");
            CardManager.Instance.AddCard("House");
            CardManager.Instance.AddCard("House");

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
        loadQueue.Enqueue(() =>
        {
            GenerateTileMesh();
        });
        totalJobs = loadQueue.Count;
    }

    void Update()
    {
        if(!loadComplete)
        {
            float stop = Time.realtimeSinceStartup + 0.01f;
            while (Time.realtimeSinceStartup < stop)
            {
                if (loadQueue.Count > 0)
                {
                    loadQueue.Dequeue().Invoke();
                }
                else
                {
                    loadComplete = true;
                    SaveSystem.Save();
                    SceneFade.Instance.Loaded();
                    break;
                }
            }
            float done = 1f - (float)loadQueue.Count / totalJobs;
            SceneFade.Instance.percentSlider.value = done;
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
        noiseOffset = new Vector2(UnityEngine.Random.Range(-100000f, 100000f), UnityEngine.Random.Range(-100000f, 100000f));
        LoopCoordinates(Generation, new Vector2Int(60, 60), new Vector2Int(20,20));
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

    public Tile TileAt(Vector2Int coord)
    {
        return world[coord.x, coord.y];
    }

    void SpawnTile(Vector2Int coord)
    {
        loadQueue.Enqueue(() =>
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
        });
    }

    void Generation(Vector2Int coord)
    {
        loadQueue.Enqueue(() =>
        {
            if (roadLayout[coord.x, coord.y] != RoadType.None)
            {
                if (roadLayout[coord.x, coord.y] == RoadType.Main)
                {
                    world[coord.x, coord.y].BuildTileObject(roadPrefab, false, false);
                    ((Road)world[coord.x, coord.y].tileObject).main = true;
                    world[coord.x, coord.y].tileObject.Init(world[coord.x, coord.y]);
                }
                else if (coord.x > 38 && coord.x < 62 && coord.y > 38 && coord.y < 62)
                {
                    world[coord.x, coord.y].BuildTileObject(roadPrefab, smoke: false);
                }
            }
            else
            {
                if(!NoiseGeneration(coord, noiseOffset, "Water", waterNoiseScale, waterThreshold)) {
                    if (RandomGeneration(coord, "Coal", coalProbability)) return;
                    NoiseGeneration(coord, noiseOffset, "Tree", treeNoiseScale, treeThreshold, treeProbability);
                }
            }
        });
    }

    bool NoiseGeneration(Vector2Int coord, Vector2 offset, string prefabName, float noiseScale, float threshold, float probability=100)
    {
        //if (UnityEngine.Random.Range(0, 20) < 1) world[coord.x, coord.y].BuildTileObject(GameManager.Instance.namedPrefabs["Tree"], smoke: false);
        float pX = coord.x * noiseScale * Mathf.PI + offset.x;
        float pY = coord.y * noiseScale * Mathf.PI + offset.y;
        float value = Mathf.Clamp01(Mathf.PerlinNoise(pX, pY));
        if(value > threshold)
        {
            float p = Mathf.Pow(UnityEngine.Random.Range(0f,1f), probability);
            if(value > p) world[coord.x, coord.y].BuildTileObject(GameManager.Instance.namedPrefabs[prefabName], smoke: false);
            return true;
        }
        return false;
    }

    bool RandomGeneration(Vector2Int coord, string prefabName, float probability)
    {
        float value = UnityEngine.Random.Range(0f, 1f);
        if(value < probability)
        {
            world[coord.x, coord.y].BuildTileObject(GameManager.Instance.namedPrefabs[prefabName], smoke: false);
            return true;
        }
        return false;
    }

    public Dictionary<string,Tile> GetNeighboursNSEW(Vector2Int coord)
    {
        Dictionary<string, Tile> tileDic = new Dictionary<string, Tile>();

        if(coord.y < 99)
        {
            if(world[coord.x, coord.y + 1] != null) tileDic.Add("N", world[coord.x, coord.y + 1]);
        }
        if (coord.y > 0)
        {
            if (world[coord.x, coord.y - 1] != null) tileDic.Add("S", world[coord.x, coord.y - 1]);
        }
        if (coord.x < 99)
        {
            if (world[coord.x + 1, coord.y] != null) tileDic.Add("E", world[coord.x + 1, coord.y]);
        }
        if (coord.x > 0)
        {
            if (world[coord.x - 1, coord.y] != null) tileDic.Add("W", world[coord.x - 1, coord.y]);
        }

        return tileDic;
    }

    void GenerateTileMesh()
    {
        Debug.Log("Generating mesh");
        tileMesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        LoopCoordinates((Vector2Int coord) => { Vert(coord, ref verts, ref indices, ref uvs); }, Vector2Int.one * 60, Vector2Int.one * 20);
        tileMesh.vertices = verts.ToArray();
        tileMesh.triangles = indices.ToArray();
        tileMesh.uv = uvs.ToArray();
        tileMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = tileMesh;
    }

    void Vert(Vector2Int coord, ref List<Vector3> verts, ref List<int> indices, ref List<Vector2> uvs)
    {
        
        Tile t = TileAt(coord);
        if (t == null) return;
        if (t.tileObject is Water) return;
        Vector3 vert = (Vector2)coord;
        int i = verts.Count - 1;
        verts.Add(new Vector3(coord.x - 0.5f, 0, coord.y - 0.5f));
        uvs.Add(new Vector2(0,0));
        verts.Add(new Vector3(coord.x + 0.5f, 0, coord.y - 0.5f));
        uvs.Add(new Vector2(1, 0));
        verts.Add(new Vector3(coord.x - 0.5f, 0, coord.y + 0.5f));
        uvs.Add(new Vector2(0, 1));
        verts.Add(new Vector3(coord.x + 0.5f, 0, coord.y + 0.5f));
        uvs.Add(new Vector2(1, 1));
        indices.AddRange( new int[]{ i + 1, i + 3, i + 2, i + 4, i + 2, i + 3 } );
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
