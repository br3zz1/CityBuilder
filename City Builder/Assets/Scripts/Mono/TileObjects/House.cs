using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : TileObject // D�d� t��du (komponentu) TileObject
{

    // P�eps�n� metody pro p�id�n� sk�re
    public override int AddedValue(bool negative = false)
    {
        // Vol�n� metody ze z�kladn� t��dy (TileObject)
        base.AddedValue(negative);

        // Za��tek po��t�n� sk�re
        BeginAddValue();
        score = 0;

        // Pro ka�d� objekt spolu se silni�n� vzd�lenost� zavol� metodu RoadDistance
        ForeachRoadDistanceDo(RoadDistance);

        // Zde vyu��v�m lambda v�raz pro zkr�cen�. Pro ka�d� objekt spolu s p��mou vzd�lenost� provede k�d v lambda v�razu
        ForeachAirDistanceDo((ObjectDistance od) => {
            // Pokud vzd�lenost je men�� ne� 3 a typ je TreeObject, p�id� hodnotu 2
            if (od.distance < 3f && od.obj is TreeObject) AddValue(od.obj, 2);
        });

        ForeachNeighbourDo((TileObject t) =>
        {
            if (t is Water) AddValue(t, -10);
        });

        // Konec po��t�n� sk�re
        EndAddValue();

        return score;
    }

    private void RoadDistance(ObjectDistance rd)
    {
        // Pokud typ je House a vzd�lenost je men�� ne� 5, p�id� hodnotu 1
        if(rd.obj is House && rd.distance < 5f)
        {
            AddValue(rd.obj, 1);
        }
        // Pokud typ je Commercial a vzd�lenost je men�� ne� 5, p�id� hodnotu 2
        else if (rd.obj is Commercial && rd.distance < 5f)
        {
            AddValue(rd.obj, 2);
        }
    }
}

/*

*/
