using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int movementCounter = 0;
    public Score score;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movementCounter++;
        if (movementCounter == 200)
        {
            movementCounter = 0;
            bool rightLeft = Random.value > 0.5f;

            // slightly higher chance to go down than up, so it eventually reaches the paddle or floor and despawns
            bool upDown = Random.value > 0.6f;

            float x = rightLeft ? 1 : -1;
            float y = upDown ? 1 : -1;

            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ball") || collision.gameObject.name == "Paddle")
        {
            Debug.Log("Enemy destroyed correctly, points should be added");
            Destroy(gameObject);
        } else if(collision.gameObject.name == "Floor")
        {
            score.ScoreValue += 10;
            Destroy(gameObject);
        }
    }

}
