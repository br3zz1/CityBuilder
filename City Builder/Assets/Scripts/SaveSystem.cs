using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    
    public static void Save()
    {
        JsonData data = new JsonData();
        List<ObjectData> objects = new List<ObjectData>();
        foreach(TileObject to in WorldManager.Instance.tileObjects)
        {
            ObjectData od = new ObjectData();
            od.coord = new int[2];
            od.coord[0] = Mathf.RoundToInt(to.transform.position.x);
            od.coord[1] = Mathf.RoundToInt(to.transform.position.z);
            //if (to is House) Debug.Log(to.transform.localRotation.eulerAngles.y);
            od.rotation = Mathf.RoundToInt(to.transform.localRotation.eulerAngles.y);
            //if (to is House) Debug.Log(od.rotation);
            od.name = to.ObjectName;
            if (to is Road) od.main = ((Road)to).main;
            objects.Add(od);
        }
        data.objects = objects.ToArray();
        data.score = GameManager.Instance.score;

        string json = JsonUtility.ToJson(data);
        string path = Application.persistentDataPath + "/save.json";

        File.WriteAllText(path, json);
    }

    public static bool Load()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (!File.Exists(path)) return false;
        string json = File.ReadAllText(path);
        JsonData data = JsonUtility.FromJson<JsonData>(json);
        GameManager.Instance.UpdateScore(data.score);
        foreach(ObjectData od in data.objects)
        {
            WorldManager.Instance.loadQueue.Enqueue(() =>
            {
                Tile tile = WorldManager.Instance.TileAt(od.coord[0], od.coord[1]);
                if (tile == null) return;
                if (od.main)
                {
                    tile.BuildTileObject(GameManager.Instance.namedPrefabs[od.name], false, false);
                    (tile.tileObject as Road).main = true;
                    tile.tileObject.Init(tile);
                }
                else
                {
                    tile.BuildTileObject(GameManager.Instance.namedPrefabs[od.name], smoke: false);
                    tile.tileObject.transform.rotation = Quaternion.Euler(0, od.rotation, 0);
                }
            });
        }
        return true;
    }
}

[Serializable]
public class JsonData
{
    public int score;
    public ObjectData[] objects;
}

[Serializable]
public class ObjectData
{
    public int[] coord;
    public int rotation;
    public string name;
    public bool main;
}