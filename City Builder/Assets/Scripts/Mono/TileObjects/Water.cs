using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : TileObject
{

    [SerializeField]
    private Color waterColor;

    [SerializeField]
    private Color shoreColor;

    [SerializeField]
    private GameObject[] shores;

    [SerializeField]
    private GameObject water;

    private Dictionary<string, Tile> neighbours;

    public override void Init(Tile tile)
    {
        base.Init(tile);
        neighbours = WorldManager.Instance.GetNeighboursNSEW(new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.z));
        if (!preview)
        {
            tile.GetComponent<MeshRenderer>().enabled = false;
            UpdateShores();
            UpdateNeighboursShores();
        }
        UpdateColors();
    }

    private void UpdateColors()
    {
        water.GetComponent<MeshRenderer>().material.color = waterColor;
        foreach (GameObject shore in shores)
        {
            shore.GetComponent<MeshRenderer>().material.color = shoreColor;
        }
    }

    public override void Terminate()
    {
        base.Terminate();

        UpdateNeighboursShores();
    }

    public override int CalculateScore()
    {
        int score = 0;
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            if (n.Value.tileObject is Water) score += 20;
        }
        return score;
    }

    private void UpdateNeighboursShores()
    {
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            if (n.Value.tileObject is Water) ((Water)n.Value.tileObject).UpdateShores();
        }
    }

    public void UpdateShores()
    {
        foreach (KeyValuePair<string, Tile> n in neighbours)
        {
            if (n.Key == "N")
            {
                if (n.Value.tileObject is Water) shores[0].SetActive(false); else shores[0].SetActive(true);
            }
            if (n.Key == "S")
            {
                if (n.Value.tileObject is Water) shores[1].SetActive(false); else shores[1].SetActive(true);
            }
            if (n.Key == "E")
            {
                if (n.Value.tileObject is Water) shores[2].SetActive(false); else shores[2].SetActive(true);
            }
            if (n.Key == "W")
            {
                if (n.Value.tileObject is Water) shores[3].SetActive(false); else shores[3].SetActive(true);
            }
        }
    }
}