using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : TileObject
{
    [SerializeField]
    private Color roadColor;

    [SerializeField]
    private Color sidewalkColor;

    [SerializeField]
    private GameObject[] sidewalks;

    [SerializeField]
    private GameObject road;

    public override void Init(Tile tile)
    {
        base.Init(tile);

        UpdateColors(roadColor, sidewalkColor);

        UpdateSidewalks();
        UpdateNeighboursSidewalks();
    }

    private void UpdateColors(Color roadC, Color sidewalkC)
    {
        road.GetComponent<MeshRenderer>().material.color = roadC;
        foreach (GameObject sidewalk in sidewalks)
        {
            sidewalk.GetComponent<MeshRenderer>().material.color = sidewalkC;
        }
    }

    public override void Terminate()
    {
        base.Terminate();

        UpdateNeighboursSidewalks();
    }

    private void UpdateNeighboursSidewalks()
    {
        Dictionary<string, Tile> neigh = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
        foreach (KeyValuePair<string, Tile> n in neigh)
        {
            if (n.Value.tileObject is Road) ((Road)n.Value.tileObject).UpdateSidewalks();
        }
    }

    public void UpdateSidewalks()
    {
        Dictionary<string, Tile> neigh = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
        foreach(KeyValuePair<string, Tile> n in neigh)
        {
            if (n.Key == "N") {
                if (n.Value.tileObject is Road) sidewalks[0].SetActive(false); else sidewalks[0].SetActive(true);
            }
            if (n.Key == "S") {
                if (n.Value.tileObject is Road) sidewalks[1].SetActive(false); else sidewalks[1].SetActive(true);
            }
            if (n.Key == "E") {
                if (n.Value.tileObject is Road) sidewalks[2].SetActive(false); else sidewalks[2].SetActive(true);
            }
            if (n.Key == "W") {
                if (n.Value.tileObject is Road) sidewalks[3].SetActive(false); else sidewalks[3].SetActive(true);
            }
        }
    }
}
