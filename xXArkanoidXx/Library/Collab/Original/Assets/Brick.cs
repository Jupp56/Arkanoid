using System.Collections;
using System.Collections.Generic;
using static System.Enum;
using static System.Math;
using UnityEngine;
using static Assets.Level;

public class Brick : MonoBehaviour
{
    private int durability = 1;

    public GameObject powerUpPrefab;
    public GameObject powerUpParent;

    public GameObject scoreboard;
    private Score score;

    SpriteRenderer m_SpriteRenderer;

    
    // Set this while instantiating!
    public BrickType brickType;

    // Start is called before the first frame update
    void Start()
    {
        if (brickType == BrickType.empty) throw new System.ArgumentException("Instantiating non-existing bricks is a bit pointless, ain't it?");

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.color = GetColorByBrickType(brickType);
        
        if(brickType == BrickType.double_)
        {
            this.durability = 2;
        }

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
        if (brickType == BrickType.indestructible) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            if (durability > 1)
            {
                durability -= 1;
                //m_SpriteRenderer.color = GetColorByDurability(durability);
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
                //m_SpriteRenderer.color = GetColorByDurability(durability);
            }
            Destroy(collision.gameObject);
        }
    }

    // Destroys the brick while potentially dropping a powerup.
    private void DestroyThis()
    {
        Destroy(gameObject);
        score.ScoreValue += 10;
        if (Random.value > 0.95)
        {
            GameObject powerUp = Instantiate(powerUpPrefab, gameObject.transform.position, Quaternion.identity, powerUpParent.transform);
            var powerUps = GetValues(typeof(PowerUp.PowerUpType));
            powerUp.GetComponent<PowerUp>().powerUpType = (PowerUp.PowerUpType)powerUps.GetValue((int)(Floor(Random.value * (double)powerUps.Length)));
        }

    }

    
    Color GetColorByBrickType(BrickType brickType)
    {
        switch(brickType)
        {
            case BrickType.empty: throw new System.ArgumentException("What moron tried to render a nonexisting brick?");
            case BrickType.red: return new Color(1.0f, 0.0f, 0.0f);
            case BrickType.yellow: return new Color(1f, 1f, 0f);
            case BrickType.green: return new Color(0f, 1f, 0f);
            case BrickType.lightBlue: return new Color(0.2f, 0.2f, 1f);
            case BrickType.orange: return new Color(1f, 0.2f, 0f);
            case BrickType.white: return new Color(1f, 1f, 1f);
            case BrickType.blue: return new Color(0f, 0f, 1f);
            case BrickType.purple: return new Color(1f, 0f, 1f);
            case BrickType.double_: return new Color(0.8f, 0.8f, 0.8f);
            case BrickType.indestructible: return new Color(0.8f, 0.8f, 0.1f);
            default: throw new System.ArgumentException("This brick is not implemented in the brick class. You have got a problem.");
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
