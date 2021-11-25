using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : TileObject
{
    private Dictionary<string, Tile> neighbours;

    public override void Init(Tile tile)
    {
        base.Init(tile);
        neighbours = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
    }

    public override int CalculateScore()
    {
        int score = 0;
        List<RoadDistance> objects = new List<RoadDistance>();
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            if (n.Value.tileObject is Road) WorldManager.Instance.RecursiveRoadSearch((Road)n.Value.tileObject, 1, 10, ref objects);
        }

        foreach (RoadDistance rd in objects)
        {
            if (rd.obj == this) continue;
            if(rd.obj is House)
            {
                score += 10 / rd.distance;
            }
        }
        return score;
    }
}
