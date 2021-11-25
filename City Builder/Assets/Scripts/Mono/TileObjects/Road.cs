using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : TileObject
{
    [SerializeField]
    private Color roadColor;

    [SerializeField]
    private Color mainRoadColor;

    [SerializeField]
    private Color sidewalkColor;

    [SerializeField]
    private Color mainSidewalkColor;

    [SerializeField]
    private GameObject[] sidewalks;

    [SerializeField]
    private GameObject road;

    public bool main;

    public Dictionary<string, Tile> neighbours;

    public override void Init(Tile tile)
    {
        base.Init(tile);
        neighbours = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
        if (!preview)
        {
            UpdateSidewalks();
            UpdateNeighboursSidewalks();
        }
        UpdateColors();
    }

    private void UpdateColors()
    {
        if(main) road.GetComponent<MeshRenderer>().material.color = mainRoadColor;
        else road.GetComponent<MeshRenderer>().material.color = roadColor;
        foreach (GameObject sidewalk in sidewalks)
        {
            if (main) sidewalk.GetComponent<MeshRenderer>().material.color = mainSidewalkColor;
            else sidewalk.GetComponent<MeshRenderer>().material.color = sidewalkColor;
        }
    }

    public override void Terminate()
    {
        base.Terminate();

        UpdateNeighboursSidewalks();
    }

    private void UpdateNeighboursSidewalks()
    {
        
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            if (n.Value.tileObject is Road) ((Road)n.Value.tileObject).UpdateSidewalks();
        }
    }

    public void UpdateSidewalks()
    {
        foreach(KeyValuePair<string, Tile> n in neighbours)
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
