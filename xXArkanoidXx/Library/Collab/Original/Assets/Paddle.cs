using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    const float speed = 5;

    private ContactPoint2D[] contacts = new ContactPoint2D[10];
    private float originalWidth;
    private double paddleWidth;

    public PowerUp.PowerUpType? currentlyHeldPowerUp;
    public GameObject laserShot;

    public bool ballSticks = true;

    public GameObject ball;

    private float timeLeftWide = 0f;

    public GameObject ballPrefab;
    public Transform gameArea;

    public void setBallSticksFalse()
    {
        this.ballSticks = false;
    }

    void Start()
    {
        originalWidth = transform.localScale.x;
        CalculatePaddleWidth();
    }

    private void CalculatePaddleWidth()
    {
        paddleWidth = this.transform.lossyScale.x * GetComponent<SpriteRenderer>().size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        Rigidbody2D body = GetComponent<Rigidbody2D>();

        // checks, if bounds of screen are reached, calculated based on the dimensions of the paddle.
        if ((body.position.x + (paddleWidth / 2) > 3 && hAxis > 0) || (body.position.x - (paddleWidth / 2) < -3 && hAxis < 0))
        {
            body.velocity = new Vector2(0, 0);
            if (ballSticks)
            {
                ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            return;
        }

        if (ballSticks)
        {
            // ball takeoff
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ballSticks = false;
                ball.GetComponent<Ball>().Takeoff();
                if (currentlyHeldPowerUp == PowerUp.PowerUpType.Stick) currentlyHeldPowerUp = null;
            }
            // ball keeps sticking, move ball the same way as paddle
            else
            {
                ball.GetComponent<Rigidbody2D>().velocity = new Vector2(hAxis * speed, 0);
            }
        }

        body.velocity = new Vector2(hAxis * speed, 0);

        if (currentlyHeldPowerUp != null)
        {
            switch (currentlyHeldPowerUp)
            {
                case PowerUp.PowerUpType.Laser:
                    GetComponent<SpriteRenderer>().color = Color.red;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        GameObject laser = Instantiate(laserShot, this.transform.position, Quaternion.identity);
                        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 15f);
                        currentlyHeldPowerUp = null;
                    }
                    break;
                // takeoff handled above, just delete currentlyHeldPowerUp if necessary
                case PowerUp.PowerUpType.Stick:
                    GetComponent<SpriteRenderer>().color = Color.magenta;
                    if (ballSticks)
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            currentlyHeldPowerUp = null;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }

        if (timeLeftWide > 0f)
        {
            timeLeftWide -= Time.deltaTime;
            if (timeLeftWide > 0f)
            {
                transform.localScale = transform.localScale.WithX(1.5f * originalWidth);
                CalculatePaddleWidth();
            }
            else
            {
                transform.localScale = transform.localScale.WithX(originalWidth);
                CalculatePaddleWidth();
            }
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            var contactpoints = collision.GetContacts(contacts);

            var contactPosition = contacts[0].point;
            var ownPosition = this.transform.position;
            Rigidbody2D ballRigidBody = collision.gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;

            // "catch" ball when respective powerUp is enabled
            if (currentlyHeldPowerUp == PowerUp.PowerUpType.Stick && ballSticks == false)
            {
                ballRigidBody.velocity = new Vector2(0, 0);
                collision.gameObject.transform.position = ownPosition;
                this.ballSticks = true;
                return;
            }

            float relative_x_offset = contactPosition.x - ownPosition.x;
            float normalized_offset = (float)(relative_x_offset / (paddleWidth / 2));
            float speed = Ball.speed;
            Vector2 newVector = new Vector2(normalized_offset, 1f) * speed;

            ballRigidBody.velocity = newVector;
        }
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            var effect = collision.gameObject.GetComponent<PowerUp>().powerUpType;
            if (currentlyHeldPowerUp != null && currentlyHeldPowerUp == PowerUp.PowerUpType.Stick && effect == PowerUp.PowerUpType.Stick)
            {
                Destroy(collision.gameObject);
                return;
            }
            Destroy(collision.gameObject);
            if (effect == PowerUp.PowerUpType.Widen)
            {
                timeLeftWide = 10f;
            }
            else if (effect == PowerUp.PowerUpType.Multiply)
            {
                GameObject ball = gameArea.FirstChild(x => x.gameObject.CompareTag("Ball")).gameObject;
                GameObject ball1 = Instantiate(ballPrefab, gameArea);
                GameObject ball2 = Instantiate(ballPrefab, gameArea);
                ball1.transform.position = ball.transform.position;
                ball2.transform.position = ball.transform.position;
                Vector2 dir1 = new Vector2(-1, 1) * Ball.speed;
                Vector2 dir2 = new Vector2(1, 1) * Ball.speed;
                ball1.GetComponent<Rigidbody2D>().velocity = dir1;
                ball2.GetComponent<Rigidbody2D>().velocity = dir2;
            }
            else if (effect == PowerUp.PowerUpType.BonusBall)
            {
                gameArea.gameObject.GetComponent<GameArea>().spareBalls++;
                gameArea.gameObject.GetComponent<GameArea>().UpdateSpareBallsText();
            }
            else
            {
                currentlyHeldPowerUp = effect;
            }
        }
    }

}
