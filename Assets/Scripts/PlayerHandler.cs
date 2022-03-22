using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public float speed = 3f;
    [SerializeField] protected ScoreKeeper _scoreKeeper;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    void Movement()
    {
        Vector2 moveDirection = Vector2.zero;
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.y = Input.GetAxis("Vertical");
        moveDirection = moveDirection * speed * Time.deltaTime;
        transform.position += (Vector3)moveDirection;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("player collision");
        if (collision.gameObject.tag == "Energy")
        {
            _scoreKeeper.energyLoc.Remove(collision.gameObject.transform);
            Destroy(collision.gameObject);
        }
    }
}
