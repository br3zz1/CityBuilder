using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : TileObject
{
    public override int AddedValue(bool negative = false)
    {
        base.AddedValue(negative);
        BeginAddValue();
        score = 0;

        ForeachAirDistanceDo((ObjectDistance od) => {
            if (od.distance < 3f && od.obj is Water) AddValue(od.obj, 4);
            else if (od.distance < 6f && od.obj is House) AddValue(od.obj, 6);
        });

        ForeachNeighbourDo((TileObject t) => {
            if (t is TreeObject) AddValue(t, 1);
        });

        EndAddValue();
        return score;
    }
}
