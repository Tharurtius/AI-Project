using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] protected ScoreKeeper _scoreKeeper;
    [SerializeField] protected int waypoint;
    [SerializeField] private AIState currentState;
    public float speed, chaseDistance;
    public GameObject player;

    private enum AIState
    {
        Gather,
        Attack,
        Defence,
    }
    // Start is called before the first frame update
    void Start()
    {
        currentState = AIState.Gather;
        NextState();
    }
    private void NextState()
    {
        switch (currentState)
        {
            case AIState.Attack:
                //StartCoroutine(AttackState());//chase player
                break;
            case AIState.Defence:
                //StartCoroutine(DefenceState());//spawn energy and mine
                break;
            case AIState.Gather:
                StartCoroutine(GatherState());//gather energy
                break;
            default:
                Debug.Log("Error!");
                break;
        }
    }
    IEnumerator AttackState()//chase player
    {
        Debug.Log("Attack: Enter");
        while (currentState == AIState.Attack)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > chaseDistance)
            {
                currentState = AIState.Gather;
            }
            else
            {
                Debug.Log("Currently Attacking");
                AIMoveTowards(player.transform);
            }
             yield return null;
        }
        Debug.Log("Attack: Exit");
        NextState();
    }
    IEnumerator DefenceState()//spawn mine and energy
    {
        Debug.Log("Defense");
        yield return null;
    }
    IEnumerator GatherState()//gather energy
    {
        Debug.Log("Gather: Enter");
        waypoint = _scoreKeeper.LowestEnergyDistance(transform);
        while (currentState == AIState.Gather)
        {
            Debug.Log("Currently Gathering");
            if (_scoreKeeper.energyLoc.Count > 0)
            {
                AIMoveTowards(_scoreKeeper.energyLoc[waypoint]);
            }
            else
            {
                currentState = AIState.Defence;
            }
            if (Vector2.Distance(player.transform.position, transform.position) < chaseDistance)
            {
                currentState = AIState.Attack;
            }
            yield return null;
        }
        Debug.Log("Gather: Exit");
        NextState();
    }
    public void AIMoveTowards(Transform goal)
    {
        Vector2 directionToGoal = (goal.transform.position - transform.position);//direction to goal
        directionToGoal.Normalize();
        transform.position += (Vector3)directionToGoal * speed * Time.deltaTime;//move to goal
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision");
        if (collision.gameObject.tag == "Energy")
        {
            _scoreKeeper.energyLoc.Remove(collision.gameObject.transform);
            Destroy(collision.gameObject);
        }
    }
}
