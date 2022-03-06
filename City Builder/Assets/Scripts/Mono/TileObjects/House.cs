using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : TileObject // Dìdí tøídu (komponentu) TileObject
{

    // Pøepsání metody pro pøidání skóre
    public override int AddedValue(bool negative = false)
    {
        // Volání metody ze základní tøídy (TileObject)
        base.AddedValue(negative);

        // Zaèátek poèítání skóre
        BeginAddValue();
        score = 0;

        // Pro každý objekt spolu se silnièní vzdáleností zavolá metodu RoadDistance
        ForeachRoadDistanceDo(RoadDistance);

        // Zde využívám lambda výraz pro zkrácení. Pro každý objekt spolu s pøímou vzdáleností provede kód v lambda výrazu
        ForeachAirDistanceDo((ObjectDistance od) => {
            // Pokud vzdálenost je menší než 3 a typ je TreeObject, pøidá hodnotu 2
            if (od.distance < 3f && od.obj is TreeObject) AddValue(od.obj, 2);
        });

        ForeachNeighbourDo((TileObject t) =>
        {
            if (t is Water) AddValue(t, -10);
        });

        // Konec poèítání skóre
        EndAddValue();

        return score;
    }

    private void RoadDistance(ObjectDistance rd)
    {
        // Pokud typ je House a vzdálenost je menší než 5, pøidá hodnotu 1
        if(rd.obj is House && rd.distance < 5f)
        {
            AddValue(rd.obj, 1);
        }
        // Pokud typ je Commercial a vzdálenost je menší než 5, pøidá hodnotu 2
        else if (rd.obj is Commercial && rd.distance < 5f)
        {
            AddValue(rd.obj, 2);
        }
    }
}

/*

*/
