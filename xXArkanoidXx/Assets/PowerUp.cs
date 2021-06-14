using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    const float speed = 1;

    public enum PowerUpType
    {
        Laser,
        Widen,
        Multiply,
        Stick,
        BonusBall
    }

    public PowerUpType powerUpType = PowerUpType.Stick;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(0, -1) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        switch (powerUpType)
        {
            case PowerUpType.Laser:
                sr.color = Color.red;
                break;
            case PowerUpType.Widen:
                sr.color = Color.yellow;
                break;
            case PowerUpType.Multiply:
                sr.color = Color.blue;
                break;
            case PowerUpType.Stick:
                sr.color = Color.magenta;
                break;
            case PowerUpType.BonusBall:
                sr.color = Color.green;
                break;
        }
    }
}
