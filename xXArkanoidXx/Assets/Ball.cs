using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public const float speed = 3;
    private const float minVerticalVelocity = 0.4f;

    public void Takeoff()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(1, 1) * speed;
    }

    private void Update()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        if (body.velocity.y != 0 && Math.Abs(body.velocity.y) < minVerticalVelocity)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y < 0 ? -minVerticalVelocity : minVerticalVelocity);
        }
    }
}
