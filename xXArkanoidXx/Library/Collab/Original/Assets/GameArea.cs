using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Level;

public class GameArea : MonoBehaviour
{
    public int cols;
    public int rows;
    public GameObject brickPrefab;
    public GameObject paddle;
    private int currentLevel = 1;
    private int spareBalls = 3;
    public Text spareBallsText;
    public GameObject ballPrefab;
    public GameObject gameOverScreen;
    private bool gameInitialized = false;
    public GameObject powerUpParent;
    public GameObject enemyParent;
    public GameObject scoreboard;

    
    public void Start()
    {
        //LevelGenerator.SaveLevelToPath("D://Dokumente//testlevel.json", new Level());
    }

    public void InitializeArea()
    {
        //BrickType[,] level = LevelGenerator.GenerateLevel(currentLevel, cols, rows);
        BrickType[,] level = LevelGenerator.LoadLevelFromPath("level1.json").bricks;
        //test level generation with every brick active
        /*int[,] level = new int[13, 18];

        for(int i = 0; i<13; i++)
        {
            for (int j =0; j<18; j++)
            {
                level[i, j] = 1;
            }
        }*/

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.rect.width;
        float colWidth = width / cols;
        float height = rectTransform.rect.height;
        float rowHeight = height / rows;
        Debug.Log(width);
        Debug.Log(colWidth);
        Debug.Log(height);
        Debug.Log(rowHeight);
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                if (level[x, y] != BrickType.empty)
                {
                    float brickX = (y + 0.5f) * colWidth - width / 2;
                    float brickY = (level.GetLength(0) - x - 0.5f) * rowHeight - height / 2;
                    GameObject brick = Instantiate(brickPrefab, (Vector2)this.transform.position + new Vector2(brickX, brickY), Quaternion.identity, this.transform);
                    // used upon dropping a powerUp. Needs to be set here as a prefab cannot get assigned "normal" objects in the editor
                    brick.GetComponent<Brick>().powerUpParent = powerUpParent;
                    Brick br = brick.GetComponent(typeof(Brick)) as Brick;
                    br.brickType = level[x, y];
                    //br.durability = 1;
                    br.scoreboard = scoreboard;

                    brick.transform.localScale = new Vector2(colWidth / 0.4f, rowHeight / 0.2f);
                }
            }
        }
        gameInitialized = true;
    }

    private void Update()
    {
        if (gameOverScreen.activeSelf) return;
        var paddleController = paddle.GetComponent<Paddle>();
        if (transform.CountChildren(x => x.gameObject.CompareTag("Brick")) <= 0)
        {
            if (gameInitialized)
            {
                paddleController.ballSticks = true;
                paddleController.currentlyHeldPowerUp = null;
                foreach (Transform child in transform)
                {
                    if (child.gameObject.CompareTag("Ball"))
                    {
                        Destroy(child.gameObject);
                    }
                }

                foreach (Transform child in powerUpParent.transform)
                {
                    Destroy(child.gameObject);
                }
                GameObject ball = Instantiate(ballPrefab, paddle.transform.position + new Vector3(0, 0.15f, 0), Quaternion.identity, transform);
                ball.GetComponent<Rigidbody2D>().velocity *= 0;
                paddleController.ball = ball;
            }

            currentLevel++;
            InitializeArea();
            return;
        }

        // ball lost
        if (transform.CountChildren(x => x.gameObject.CompareTag("Ball")) <= 0)
        {
            spareBalls--;
            spareBallsText.text = spareBalls.ToString();
            if (spareBalls < 1)
            {
                GameOver();
                return;
            }
            GameObject ball = Instantiate(ballPrefab, transform);
            ball.transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y, 0);
            // ball.transform.position = ball.transform.position.WithX(paddle.transform.position.x);
            ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            // let ball stick to paddle, set ball as current ball of paddle
            paddleController.ballSticks = true;
            paddleController.ball = ball;
            paddleController.currentlyHeldPowerUp = null;

            // Destroy all collectable powerups
            foreach (Transform t in powerUpParent.transform)
            {
                Destroy(t.gameObject);
            }

            foreach (Transform t in enemyParent.transform)
            {
                Destroy(t.gameObject);
            }

        }
    }

    private void GameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
