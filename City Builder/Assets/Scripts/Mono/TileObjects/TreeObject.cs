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
            else if (od.distance < 3f && od.obj is House) AddValue(od.obj, 2);
            else if (od.distance < 7f && od.obj is Coalmine) AddValue(od.obj, -1);
            else if (od.distance < 4f && od.obj is Factory) AddValue(od.obj, -1);
        });

        ForeachNeighbourDo((TileObject t) => {
            if (t is TreeObject) AddValue(t, 1);
        });

        EndAddValue();
        return score;
    }
}
