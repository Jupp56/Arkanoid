using System.Collections;
using System.Collections.Generic;
using static System.Enum;
using static System.Math;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int durability = 1;

    public GameObject powerUpPrefab;
    public GameObject powerUpParent;

    public GameObject scoreboard;
    private Score score;

    SpriteRenderer m_SpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        //Fetch the SpriteRenderer from the GameObject
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        //Set the GameObject's Color quickly to a set Color (blue)
        m_SpriteRenderer.color = GetColorByDurability(durability);
        score = scoreboard.GetComponent<Score>();
    }

    // Update is called once per frame
    void Update()
    {
        /*GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log(GetComponent<SpriteRenderer>().color);*/
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (durability > 1)
            {
                durability -= 1;
                m_SpriteRenderer.color = GetColorByDurability(durability);
            }
            else
            {
                DestroyThis();
            }
        }
        else if (collision.gameObject.CompareTag("LaserShot"))
        {
            if (durability <= 1)
                DestroyThis();
            else
            {
                durability -= 1;
                m_SpriteRenderer.color = GetColorByDurability(durability);
            }
            Destroy(collision.gameObject);
        }
    }

    // Destroys the brick while potentially dropping a powerup.
    private void DestroyThis()
    {
        Destroy(gameObject);
        score.ScoreValue += 10;
        if (Random.value > 0/*.95*/)
        {
            GameObject powerUp = Instantiate(powerUpPrefab, gameObject.transform.position, Quaternion.identity, powerUpParent.transform);
            var powerUps = GetValues(typeof(PowerUp.PowerUpType));
            powerUp.GetComponent<PowerUp>().powerUpType = (PowerUp.PowerUpType)powerUps.GetValue((int)(Floor(Random.value * (double)powerUps.Length)));
            powerUp.GetComponent<PowerUp>().UpdateColor();
        }

    }

    

    Color GetColorByDurability(int durability)
    {
        switch (durability)
        {
            case 10: return new Color(1.0f, 0.0f, 0.0f, 1.0f);
            case 9: return new Color(1.0f, 51f / 255f, 0.0f, 1.0f);
            case 8: return new Color(1.0f, 106f / 255f, 0.0f, 1.0f);
            case 7: return new Color(1.0f, 158f / 255f, 0.0f, 1.0f);
            case 6: return new Color(1.0f, 216f / 255f, 0.0f, 1.0f);
            case 5: return new Color(239f / 255f, 1f, 0.0f, 1.0f);
            case 4: return new Color(181f / 255f, 1f, 0.0f, 1.0f);
            case 3: return new Color(132f / 255f, 1f, 0.0f, 1.0f);
            case 2: return new Color(77f / 255f, 1f, 0.0f, 1.0f);
            case 1: return new Color(0.0f, 1.0f, 0.0f, 1.0f);
            default: return new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }
}
