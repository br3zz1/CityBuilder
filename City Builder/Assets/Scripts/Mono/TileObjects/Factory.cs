using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : TileObject
{

    public override void Init(Tile tile)
    {
        base.Init(tile);
    }

    public override int AddedValue(bool negative = false)
    {
        base.AddedValue(negative);
        BeginAddValue();
        score = 0;

        ForeachAirDistanceDo((ObjectDistance od) => {
            if (od.distance < 5f && od.obj is House) AddValue(od.obj, -7);
            else if (od.distance < 4f && od.obj is TreeObject) AddValue(od.obj, -1);
            else if (od.distance < 6f && od.obj is Coalmine) AddValue(od.obj, 8);
        });

        ForeachRoadDistanceDo((ObjectDistance od) => {
            if (od.distance < 8f && od.obj is Warehouse) AddValue(od.obj, 2);
        });

        ForeachNeighbourDo((TileObject t) => {
            if (t is Factory) AddValue(t, 1);
        });

        EndAddValue();
        return score;
    }
}
