using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    //list of energy locations
    public List<Transform> energyLoc;
    //find closest energy for any object
    public int LowestEnergyDistance(Transform origin)
    {
        float lowestDistance = float.PositiveInfinity;
        int lowestIndex = 0;
        float distance;
        for (int i = 0; i < energyLoc.Count; i++)
        {
            distance = Vector2.Distance(origin.position, energyLoc[i].transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                lowestIndex = i;
            }
        }
        return lowestIndex;
    }
}
