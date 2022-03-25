using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    #region Variables
    //list of energy locations
    public List<Transform> energyLoc;
    //list of spawner positions
    public List<GameObject> spawnerPosition;
    public int spawnIndex;
    //prefabs used for spawning
    public GameObject energyPrefab;
    public GameObject minePrefab;
    public Text gameText, scoreText;
    public HorizontalLayoutGroup hearts;
    #endregion
    #region Functions
    //find closest energy for any object
    public int LowestEnergyDistance(Transform origin)//find energy with lowest distance
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
    public void NewObject(int item)//0 = spawn energy, 1 = spawn mine
    {
        if (spawnerPosition.Count == 0)//if all spawners destroyed
        {
            return;
        }
        //randomize position
        float x = Random.Range(-4, 4);
        float y = Random.Range(-4, 4);
        int spawnIndex = Random.Range(0, spawnerPosition.Count);//randomize which spawner to use
        x += spawnerPosition[spawnIndex].transform.position.x;
        y += spawnerPosition[spawnIndex].transform.position.y;
        if (item == 1)//if mine
        {
            Instantiate(minePrefab, new Vector2(x, y), Quaternion.identity);
        }
        else//if energy
        {
            GameObject newPoint = Instantiate(energyPrefab, new Vector2(x, y), Quaternion.identity);
            energyLoc.Add(newPoint.transform);
        }
    }
    public void ToggleDangerZone()//turn on and off the mine spawning zone
    {
        if (spawnerPosition[0].transform.GetChild(0).gameObject.activeInHierarchy == true)//if dangerzone in first spawner is active
        {
            for (int i = 0; i < spawnerPosition.Count; i++)//find all spawners and set dangerzone to false
            {
                spawnerPosition[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < spawnerPosition.Count; i++)//find all spawners and set dangerzone to true
            {
                spawnerPosition[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    public void UpdateScore(int score) //change in-game text
    {
        if (score == 0)
        {
            scoreText.text = "Destroy the Spawners!";
        }
        else
        {
            scoreText.text = "Collect " + score + " more!";
        }
    }
    #endregion
}