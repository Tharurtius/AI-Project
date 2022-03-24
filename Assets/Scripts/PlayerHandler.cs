using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public float speed = 3f;
    [SerializeField] protected ScoreKeeper _scoreKeeper;
    public int needed;
    public int health;
    public float invincibilityTimer;
    // Start is called before the first frame update
    void Start()//initialization
    {
        needed = 5;
        _scoreKeeper.UpdateScore(needed);
        health = 3;
        invincibilityTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        invincibilityTimer -= Time.deltaTime;
    }
    #region Player Functions
    void Movement()//movement code
    {
        Vector2 moveDirection = Vector2.zero;
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");
        moveDirection = moveDirection * speed * Time.deltaTime;
        transform.position += (Vector3)moveDirection;
    }
    private void TakeDamage()//damage code
    {
        health--;
        invincibilityTimer = 2;
        Destroy(_scoreKeeper.hearts.transform.GetChild(0).gameObject); //remove heart from ui
        //cannot have negative health without errors
    }
    #endregion
    #region Collision stuff
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("player collision");
        string cTag = collision.gameObject.tag;
        if (cTag == "Energy") //if collectable
        {
            _scoreKeeper.energyLoc.Remove(collision.gameObject.transform);
            Destroy(collision.gameObject);
            needed--;
            if (needed < 0)
            {
                needed = 0;
            }
            _scoreKeeper.UpdateScore(needed);
        }
        else if (cTag == "Mines") //if mines
        {
            Destroy(collision.gameObject);
            if (invincibilityTimer <= 0)
            {
                TakeDamage();
            }
        }
        else if (cTag == "NPC") //if npc
        {
            if (invincibilityTimer <= 0)
            {
                TakeDamage();
            }
        }
        else if (cTag == "Spawner") //if enough energy was collected, destroy spawner
        {
            //if (needed == 0)
            //{
                _scoreKeeper.spawnerPosition.Remove(collision.gameObject);
                Destroy(collision.gameObject);
                needed = 5;
            //}
        }
        else if (cTag == "Run Away") //tag for AI that is running away
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("IsDead");//play ai death anim
            Time.timeScale = 0;
            _scoreKeeper.gameText.text = "You Win!";
            _scoreKeeper.gameText.gameObject.SetActive(true);
            _scoreKeeper.scoreText.text = "Congratulations!";
        }
        if (health == 0) //if player dies
        {
            gameObject.GetComponent<Animator>().SetTrigger("IsDead");//play death anim
            Time.timeScale = 0;
            _scoreKeeper.gameText.text = "GAME OVER";
            _scoreKeeper.gameText.gameObject.SetActive(true);
        }
    }
    #endregion
}
