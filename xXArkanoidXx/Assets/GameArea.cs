using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Assets.Level;

public class GameArea : MonoBehaviour
{
    public int cols;
    public int rows;
    public GameObject brickPrefab;
    public GameObject paddle;
    private int currentLevel = 0;
    public int spareBalls = 3;
    public Text spareBallsText;
    public GameObject ballPrefab;
    public GameObject gameOverScreen;
    public GameObject gameCompletedScreen;
    private bool gameInitialized = false;
    public GameObject powerUpParent;
    public GameObject enemyPrefab;
    public GameObject enemyParent;
    public GameObject scoreboard;
    public List<EnemySpawn> enemySpawns;
    public GameObject highScore;

    public int numDestroyedBricks = 0;

    /*
    public void Start()
    {
        LevelGenerator.SaveLevelToPath("D://Dokumente//testlevel.json", new Level());
    }*/



    public void InitializeArea()
    {

        Level level;
        if (SceneManager.GetActiveScene().name == "Predefined")
        {
            try
            {
                Debug.Log("opening level " + currentLevel);
                level = LevelGenerator.LoadLevelFromPath("assets/level" + currentLevel + ".json");
            } catch (Exception)
            {
                GameCompleted();
                return;
            }

            enemySpawns = level.enemies;
            var zeroSpawns = enemySpawns.FindAll(x => x.blockCount == 0);

            if (zeroSpawns.Count != 0)
            {
                foreach (EnemySpawn spawn in zeroSpawns)
                {
                    SpawnEnemy(spawn);
                    enemySpawns.Remove(spawn);
                }
            }


        }
        else
        {
            level = LevelGenerator.GenerateLevel(currentLevel);
        }

        BrickType[,] bricks = level.bricks;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.rect.width;
        float colWidth = width / cols;
        float height = rectTransform.rect.height;
        float rowHeight = height / rows;


        for (int x = 0; x < bricks.GetLength(0); x++)
        {
            for (int y = 0; y < bricks.GetLength(1); y++)
            {
                if (bricks[x, y] != BrickType.empty)
                {
                    float brickX = (y + 0.5f) * colWidth - width / 2;
                    float brickY = (bricks.GetLength(0) - x - 0.5f) * rowHeight - height / 2;
                    GameObject brick = Instantiate(brickPrefab, (Vector2)this.transform.position + new Vector2(brickX, brickY), Quaternion.identity, this.transform);
                    // used upon dropping a powerUp. Needs to be set here as a prefab cannot get assigned "normal" objects in the editor
                    brick.GetComponent<Brick>().powerUpParent = powerUpParent;
                    Brick br = brick.GetComponent(typeof(Brick)) as Brick;
                    br.brickType = bricks[x, y];
                    br.scoreboard = scoreboard;
                    br.gameArea = this;
                    brick.transform.localScale = new Vector2(colWidth / 0.4f, rowHeight / 0.2f);
                }
            }
        }
        gameInitialized = true;
    }

    public void SpawnEnemy(EnemySpawn enemySpawn)
    {
        float spawnx;
        float spawny = 4.5f;
        switch (enemySpawn.spawnDirection)
        {
            case EnemySpawn.SpawnDirection.left:
                spawnx = -2;
                break;
            case EnemySpawn.SpawnDirection.right:
                spawnx = 2;
                break;
            default:
                spawnx = -2;
                break;
        }
        GameObject enemyGO = Instantiate(enemyPrefab, new Vector3(spawnx, spawny, 0f), Quaternion.identity, enemyParent.transform);
        enemyGO.GetComponent<Enemy>().score = scoreboard.GetComponent<Score>();
    }

    private void Update()
    {
        if (gameOverScreen.activeSelf ||gameCompletedScreen.activeSelf) return;

        // spawn enemies if appropriate
        if (enemySpawns != null)
        {
            List<EnemySpawn> deleteList = new List<EnemySpawn>();
            foreach (EnemySpawn enemySpawn in enemySpawns)
            {
                if (enemySpawn.blockCount == numDestroyedBricks)
                {

                    SpawnEnemy(enemySpawn);
                    deleteList.Add(enemySpawn);
                }
            }
            foreach (EnemySpawn enemySpawn in deleteList)
            {
                enemySpawns.Remove(enemySpawn);
            }
        }

        var paddleController = paddle.GetComponent<Paddle>();
        if (transform.CountChildren(x => x.gameObject.CompareTag("Brick")) <= 0)
        {
            numDestroyedBricks = 0;
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
            UpdateSpareBallsText();
            if (spareBalls < 1)
            {
                GameOver();
                return;
            }
            GameObject ball = Instantiate(ballPrefab, transform);
            ball.transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y + 0.2f, 0);
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

    public void UpdateSpareBallsText()
    {
        spareBallsText.text = spareBalls.ToString();
    }

    private void GameOver()
    {
        highScore.GetComponent<HighScore>().SaveScore();
        gameOverScreen.SetActive(true);
    }

    private void GameCompleted()
    {
        highScore.GetComponent<HighScore>().SaveScore();
        gameCompletedScreen.SetActive(true);
    }
}
