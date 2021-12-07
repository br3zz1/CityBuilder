using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulldozeCard : Card
{

    public override void Tick()
    {
        if (ToolController.Instance.hoveringOver is TileObject)
        {
            TileObject tileObject = (TileObject)ToolController.Instance.hoveringOver;

            bool bulldozeOk = true;
            if (tileObject is Road)
            {
                bulldozeOk = false;
            }
            if(bulldozeOk)
            {
                tileObject.ChangeColor(Color.yellow);
            }
            else
            {
                tileObject.ChangeColor(Color.red);
            }
        }
    }

    public override bool Use()
    {
        if (ToolController.Instance.hoveringOver is TileObject)
        {
            TileObject tileObject = (TileObject)ToolController.Instance.hoveringOver;
            if (tileObject is Road)
            {
                TooltipSystem.Instance.Show("", "You can't bulldoze roads.", Color.red, 1.5f);
                return false;
            }
            GameManager.Instance.UpdateScore(GameManager.Instance.score - tileObject.AddedValue());

            tileObject.tile.TerminateTileObject();
            Return();
            CardManager.Instance.RemoveCard(this);
            CardManager.Instance.AddCard(Random.Range(0, 3));
            return true;
        }
        return false;
    }

    public override void Return()
    {
        base.Return();
        if (ToolController.Instance.hoveringOver is TileObject)
        {
            TileObject tileObject = (TileObject)ToolController.Instance.hoveringOver;
            tileObject.DefaultColor();
        }
    }
}
