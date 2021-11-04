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

    public override void Tick()
    {
        if (ToolController.Instance.hoveringOver is Tile)
        {
            Tile tile = (Tile)ToolController.Instance.hoveringOver;
            HoverOverTile(tile);
        }
        else if (ToolController.Instance.hoveringOver is TileObject)
        {
            Tile tile = ((TileObject)ToolController.Instance.hoveringOver).tile;
            HoverOverTileObject(tile);
        }
        if(buildingPreview != null)
        {
            buildingPreview.transform.position = Vector3.Lerp(buildingPreview.transform.position, previewDesiredPosition, Time.deltaTime * 10f);
        }
    }

    private void HoverOverTile(Tile tile)
    {
        if(tile.tileObject != null)
        {
            HoverOverTileObject(tile);
            return;
        }
        if (buildingPreview == null)
        {
            SpawnBuildingPreview(tile);
        }
        buildingPreview.DefaultColor();
        previewDesiredPosition = tile.transform.position + Vector3.up * 0.3f;
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
                TooltipSystem.Instance.Show("", "This space is occupied.", Color.red, 2f);
                return false;
            }
            tile.BuildTileObject(buildingPrefab);

            if (buildingPreview != null)
            {
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
        CardManager.Instance.AddCard(Random.Range(0,4));
        return true;
    }

    public override void Return()
    {
        if(buildingPreview != null)
        {
            Destroy(buildingPreview.gameObject);
        }
        buildingPreview = null;
        base.Return();
    }
}
