using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulldozeCard : Card
{

    TileObject lastTileObj;
    private int rValue;

    public override void Tick()
    {
        if (ToolController.Instance.hoveringOver is TileObject)
        {
            TileObject tileObject = (TileObject)ToolController.Instance.hoveringOver;
            if (lastTileObj == tileObject) return;
            GameManager.Instance.addedScoreText.text = "";
            if (lastTileObj != null) lastTileObj.ClearValueTexts();
            lastTileObj = tileObject;
            rValue = tileObject.AddedValue(true);
            GameManager.Instance.addedScoreText.text = (rValue > 0 ? "+" : "") + rValue;

            bool bulldozeOk = true;
            if (tileObject is Road || GameManager.Instance.score + rValue < 0)
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
        else
        {
            if (lastTileObj != null) lastTileObj.ClearValueTexts();
            GameManager.Instance.addedScoreText.text = "";
            lastTileObj = null;
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
            else if (GameManager.Instance.score + rValue < 0)
            {
                TooltipSystem.Instance.Show("", "You need at least " + -rValue + " score to bulldoze this.", Color.red, 1.5f);
                return false;
            }
            GameManager.Instance.UpdateScore(GameManager.Instance.score - tileObject.AddedValue());

            tileObject.tile.TerminateTileObject();
            Return();
            CardManager.Instance.RemoveCard(this);
            CardManager.Instance.AddCard(Random.Range(0, 3));
            return true;
        }
        else if (ToolController.Instance.hoveringOver == null)
        {
            TooltipSystem.Instance.Show("", "Out of bounds.", Color.red, 1.5f);
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
            tileObject.ClearValueTexts();
        }
    }
}
