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
            tileObject.ChangeColor(Color.yellow);
            if (tileObject is Road)
            {
                if (((Road)tileObject).main) tileObject.ChangeColor(Color.red);
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
                if (((Road)tileObject).main)
                {
                    TooltipSystem.Instance.Show("", "You can't bulldoze a main road.", Color.red, 1.5f);
                    return false;
                }
            }
            tileObject.tile.TerminateTileObject();
            Return();
            CardManager.Instance.RemoveCard(this);
            CardManager.Instance.AddCard(Random.Range(0, 4));
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
