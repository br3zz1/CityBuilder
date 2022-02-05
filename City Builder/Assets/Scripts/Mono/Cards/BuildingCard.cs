using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCard : Card
{

    [Header("Building Card")]
    [SerializeField]
    private TileObject buildingPrefab;

    private TileObject buildingPreview;
    private Vector3 previewDesiredPosition;
    private float actualRotation = 0f;
    private float previewDesiredRotation;

    private Tile lastTile;

    
    private List<int> allowedRotations;

    public override void Tick()
    {
        if (allowedRotations == null) allowedRotations = new List<int>();
        if (ToolController.Instance.hoveringOver is Tile)
        {
            Tile tile = (Tile)ToolController.Instance.hoveringOver;
            if(tile != lastTile || Input.GetKeyDown(KeyCode.R))
            {
                lastTile = tile;
                GameManager.Instance.addedScoreText.text = "";
                HoverOverTile(tile);
            }
        }
        else if (ToolController.Instance.hoveringOver is TileObject)
        {
            Tile tile = ((TileObject)ToolController.Instance.hoveringOver).tile;
            if(tile != lastTile)
            {
                lastTile = tile;
                GameManager.Instance.addedScoreText.text = "";
                HoverOverTileObject(tile);
            }        
        }
        if(buildingPreview != null)
        {
            buildingPreview.transform.position = Vector3.Lerp(buildingPreview.transform.position, previewDesiredPosition, Time.deltaTime * 10f);
            float target = previewDesiredRotation;
            if (previewDesiredRotation - actualRotation > 180f) target -= 360;
            if (actualRotation - previewDesiredRotation > 180f) target += 360;
            actualRotation = Mathf.Lerp(actualRotation, target, Time.deltaTime * 20f);
            if (actualRotation > 360f) actualRotation -= 360f;
            if (actualRotation < 0f) actualRotation += 360f;
            buildingPreview.transform.rotation = Quaternion.Euler(0, actualRotation, 0);
        }
    }

    private void HoverOverTile(Tile tile)
    {
        if(tile.tileObject != null)
        {
            if(!tile.tileObject.preview)
            {
                HoverOverTileObject(tile);
                return;
            }
        }
        if (buildingPreview == null)
        {
            SpawnBuildingPreview(tile);
        }

        buildingPreview.DefaultColor();

        buildingPreview.Init(tile);

        // ROTATIONS
        allowedRotations.Clear();
        foreach (int r in buildingPreview.allowedRotations)
        {
            allowedRotations.Add(r);
        }

        if (allowedRotations.Count == 0)
        {
            HoverOverTileObject(tile);
            return;
        }
        if (!allowedRotations.Contains((int)previewDesiredRotation)) previewDesiredRotation = allowedRotations[0];
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (allowedRotations.IndexOf((int)previewDesiredRotation) == allowedRotations.Count - 1) previewDesiredRotation = allowedRotations[0];
            else previewDesiredRotation = allowedRotations[allowedRotations.IndexOf((int)previewDesiredRotation) + 1];
        }

        //buildingPreview.transform.rotation = Quaternion.Euler(0, previewDesiredRotation, 0);

        previewDesiredPosition = tile.transform.position + Vector3.up / 4f;

        int v = buildingPreview.AddedValue();
        GameManager.Instance.addedScoreText.text = (v < 0 ? "" : "+") + v;
    }

    private void HoverOverTileObject(Tile tile)
    {
        if (buildingPreview == null)
        {
            SpawnBuildingPreview(tile);
        }
        buildingPreview.ChangeColor(Color.red);
        previewDesiredPosition = tile.transform.position + Vector3.up;
    }

    private void SpawnBuildingPreview(Tile tile)
    {
        GameObject obj = Instantiate(buildingPrefab.gameObject, tile.transform.position + Vector3.up, Quaternion.identity);
        obj.layer = 2;
        TileObject t = obj.GetComponent<TileObject>();
        buildingPreview = t;
        t.preview = true;
        t.Init(tile);
    }

    public override bool Use()
    {
        if (ToolController.Instance.hoveringOver is Tile)
        {
            Tile tile = (Tile)ToolController.Instance.hoveringOver;
            if (tile.tileObject != null)
            {
                if(!tile.tileObject.preview)
                {
                    TooltipSystem.Instance.Show("", "This space is occupied.", Color.red, 2f);
                    return false;
                }
            }
            if(allowedRotations.Count == 0)
            {
                TooltipSystem.Instance.Show("", "This building must be placed roadside.", Color.red, 2f);
                return false;
            }
            tile.BuildTileObject(buildingPrefab);
            tile.tileObject.transform.rotation = Quaternion.Euler(0, previewDesiredRotation, 0);
            GameManager.Instance.UpdateScore(GameManager.Instance.score + tile.tileObject.AddedValue());
            SaveSystem.Save();

            if (buildingPreview != null)
            {
                buildingPreview.Terminate();
                Destroy(buildingPreview.gameObject);
                buildingPreview = null;
            }
        }
        else
        {
            TooltipSystem.Instance.Show("", "This space is occupied.", Color.red, 2f);
            return false;
        }
        Return();
        CardManager.Instance.RemoveCard(this);
        CardManager.Instance.AddCard(Random.Range(0,3));
        return true;
    }

    public override void Return()
    {
        //if (lastTile != null) lastTile.SetPreviewTileObject(null);
        if (buildingPreview != null)
        {
            buildingPreview.Terminate();
            Destroy(buildingPreview.gameObject);
        }
        buildingPreview = null;
        base.Return();
    }
}
