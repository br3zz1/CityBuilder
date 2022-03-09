using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coalmine : TileObject
{

    public override void Init(Tile tile)
    {
        base.Init(tile);

        allowedRotations.Clear();
        AllowRotations("Coal");
    }

    public override int AddedValue(bool negative = false)
    {
        base.AddedValue(negative);
        BeginAddValue();
        score = 0;

        ForeachAirDistanceDo((ObjectDistance od) => {
            if (od.distance < 5f && od.obj is House) AddValue(od.obj, -7);
            else if (od.distance < 4f && od.obj is TreeObject) AddValue(od.obj, -1);
            else if (od.distance < 6f && od.obj is Factory) AddValue(od.obj, 8);
        });

        ForeachNeighbourDo((TileObject t) => {
            if (t.ObjectName == "Coal") AddValue(t, 10);
        });

        EndAddValue();
        return score;
    }

}
