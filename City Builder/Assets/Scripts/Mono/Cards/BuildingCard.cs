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
            if(buildingPreview == null)
            {
                GameObject obj = Instantiate(buildingPrefab.gameObject, tile.transform.position + Vector3.up, Quaternion.identity);
                obj.layer = 2;
                TileObject t = obj.GetComponent<TileObject>();
                buildingPreview = t;
                t.preview = true;
                t.Init(tile);
            }
            previewDesiredPosition = tile.transform.position + Vector3.up * 0.3f;
        }
        else if (ToolController.Instance.hoveringOver is TileObject)
        {
            Tile tile = ((TileObject)ToolController.Instance.hoveringOver).tile;
            previewDesiredPosition = tile.transform.position + Vector3.up;
        }
        if(buildingPreview != null)
        {
            buildingPreview.transform.position = Vector3.Lerp(buildingPreview.transform.position, previewDesiredPosition, Time.deltaTime * 10f);
        }
    }

    public override bool Use()
    {
        if (ToolController.Instance.hoveringOver is Tile)
        {
            Tile tile = (Tile)ToolController.Instance.hoveringOver;
            tile.BuildTileObject(buildingPrefab);

            if (buildingPreview != null)
            {
                Destroy(buildingPreview.gameObject);
                buildingPreview = null;
            }
        }
        else
        {
            return false;
        }
        Return();
        CardManager.Instance.RemoveCard(this);
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
