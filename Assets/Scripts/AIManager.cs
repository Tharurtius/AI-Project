using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] protected ScoreKeeper _scoreKeeper;
    [SerializeField] protected int waypointIndex;
    [SerializeField] protected Transform defenceWaypoint;
    [SerializeField] private AIState currentState;
    public float speed, chaseDistance;
    public GameObject player;

    private enum AIState
    {
        Gather,
        Attack,
        Defence,
        RunAway,
    }
    #endregion
    // Start is called before the first frame update
    void Start()//initializations
    {
        currentState = AIState.Gather;
        NextState();
    }
    #region State Machine
    private void NextState() //state machine
    {
        switch (currentState)
        {
            case AIState.Attack:
                StartCoroutine(AttackState());//chase player
                break;
            case AIState.Defence:
                StartCoroutine(DefenceState());//spawn energy and mine
                break;
            case AIState.Gather:
                StartCoroutine(GatherState());//gather energy
                break;
            case AIState.RunAway:
                StartCoroutine(RunState());
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
            CheckSpawner();
            if (Vector2.Distance(player.transform.position, transform.position) > chaseDistance)
            {
                currentState = AIState.Gather;
            }
            else//if player runs to safe distance
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
        Debug.Log("Defense: Enter");
        _scoreKeeper.ToggleDangerZone();
        while (Vector2.Distance(transform.position, defenceWaypoint.position) > 0.2)//while not in center of screen
        {
            CheckSpawner();
            AIMoveTowards(defenceWaypoint);//move towards center of screen
            yield return null;
        }
        Debug.Log("Defense: Spawning mines");
        int x = Random.Range(1, 4);//how many mines are spawned
        for (int i = 0; i < x; i++)
        {
            _scoreKeeper.NewObject(1);//spawn mines
        }
        _scoreKeeper.ToggleDangerZone();
        gameObject.GetComponent<Animator>().SetBool("Spawning", true);//turn on spawning anim
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<Animator>().SetBool("Spawning", false);//turn off spawning anim
        CheckSpawner();
        Debug.Log("Defense: Spawning energy");
        x = Random.Range(1, 9);//how many energy spawned is randomised
        for (int i = 0; i < x; i++)
        {
            _scoreKeeper.NewObject(0);//spawn energy
        }
        currentState = AIState.Gather;
        Debug.Log("Defense: Exit");
        NextState();
    }
    IEnumerator GatherState()//gather energy
    {
        Debug.Log("Gather: Enter");
        while (currentState == AIState.Gather)
        {
            waypointIndex = _scoreKeeper.LowestEnergyDistance(transform);
            Debug.Log("Currently Gathering");
            if (_scoreKeeper.energyLoc.Count > 0)
            {
                AIMoveTowards(_scoreKeeper.energyLoc[waypointIndex]);
            }
            else
            {
                currentState = AIState.Defence;
            }
            if (Vector2.Distance(player.transform.position, transform.position) < chaseDistance)
            {
                currentState = AIState.Attack;
            }
            CheckSpawner();
            yield return null;
        }
        Debug.Log("Gather: Exit");
        NextState();
    }
    IEnumerator RunState()//run away
    {
        while (true)
        {
            Debug.Log("Running away!");
            AIMoveAway(player.transform);
            yield return null;
        }
    }
    #endregion
    #region Functions
    public void AIMoveTowards(Transform goal)
    {
        Vector2 directionToGoal = (goal.transform.position - transform.position);//direction to goal
        directionToGoal.Normalize();
        transform.position += (Vector3)directionToGoal * speed * Time.deltaTime;//move to goal
    }
    public void AIMoveAway(Transform goal)
    {
        Vector2 directionToGoal = (goal.transform.position - transform.position);//direction to player
        directionToGoal.Normalize();
        transform.position -= (Vector3)directionToGoal * speed * Time.deltaTime;//move away from player
    }
    public void CheckSpawner()//to swap to run away when spawners are down
    {
        if (_scoreKeeper.spawnerPosition.Count == 0)
        {
            currentState = AIState.RunAway;
            tag = "Run Away";//to make ai vulnerable to player
            _scoreKeeper.scoreText.text = "Finish it off!";
        }
    }
    #endregion
    #region Collisions
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Debug.Log("collision");
        if (collision.gameObject.tag == "Energy")//natural ai behaviour, chasing down energy
        {
            _scoreKeeper.energyLoc.Remove(collision.gameObject.transform);
            Destroy(collision.gameObject);
            waypointIndex = _scoreKeeper.LowestEnergyDistance(transform);
        }
        else if (collision.gameObject.tag == "Mines")//ai is immune to mine damage
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Spawner")//if ai gets stuck on spawner
        {
            Vector3 direction = Vector3.zero;
            direction.x = Random.Range(-2, 3);
            direction.y = Random.Range(-2, 3);
            transform.position += direction * speed * Time.deltaTime;//as a bonus, makes ai look angry
        }
    }
    #endregion 
}
