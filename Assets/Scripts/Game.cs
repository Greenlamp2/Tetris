using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game : MonoBehaviour {
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static int startingLevel = 0;
    
    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public bool paused = false;

    public float fallSpeed = 1.0f;

    public static int currentScore;
    public Text hud_score;
    public Text hud_level;
    public Text hud_lines;
    public Canvas paused_ui;
    public Canvas hud;

    public static bool gameOver = false;

    private int numberOfRowsThisTurn = 0;

    public GameObject[] tetrominos;

    private int randomTetromino;
    private GameObject nextTetromino;

    void Start() {
        currentScore = 0;
        randomTetromino = Random.Range(0, tetrominos.Length);
        SpawnNextTetromino();
        paused_ui.enabled = false;
    }

    void Update() {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
        UpdatePause();
    }

    void UpdatePause() {
        if (Input.GetKeyUp(KeyCode.P)) {
            if (paused) {
                paused = false;
                hud.enabled = true;
                paused_ui.enabled = false;
                GetComponent<AudioSource>().Play();
                Time.timeScale = 1;
            } else {
                paused = true;
                paused_ui.enabled = true;
                hud.enabled = false;
                GetComponent<AudioSource>().Pause();
                Time.timeScale = 0;
            }
        }
    }

    void UpdateLevel() {
        currentLevel = startingLevel + (numLinesCleared / 10);
    }

    void UpdateSpeed() {
        fallSpeed = 1.0f - ((float) currentLevel * 0.1f);
    }

    void UpdateUI() {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
    }

    public void UpdateScore() {
        if (numberOfRowsThisTurn == 0) return;
        PlayLineClearedAudio();
        switch (numberOfRowsThisTurn) {
            case 1:
                currentScore += scoreOneLine + (currentLevel * 10);
                numLinesCleared += 1;
                break;
            case 2:
                currentScore += scoreTwoLine + (currentLevel * 30);
                numLinesCleared += 2;
                break;
            case 3:
                currentScore += scoreThreeLine + (currentLevel * 100);
                numLinesCleared += 3;
                break;
            case 4:
                currentScore += scoreFourLine + (currentLevel * 400);
                numLinesCleared += 4;
                break;
        }

        numberOfRowsThisTurn = 0;
    }

    public bool CheckIsAboveGrid(Tetromino tetromino) {
        for (int x = 0; x < gridWidth; x++) {
            foreach (Transform mino in tetromino.transform) {
                Vector2 pos = Round(mino.position);
                int y = (int) pos.y;

                if (y > gridHeight - 1) {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsFullRowAt(int y) {
        for (int x = 0; x < gridWidth; x++) {
            if (grid[x, y] == null) return false;
        }

        numberOfRowsThisTurn += 1;

        return true;
    }

    public void DeleteMinoAt(int y) {
        for (int x = 0; x < gridWidth; x++) {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y) {
        for (int x = 0; x < gridWidth; x++) {
            if (grid[x, y] != null) {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y) {
        for (int i = y; i < gridHeight; i++) {
            MoveRowDown(i);
        }
    }

    public void DeleteRow() {
        for (int y = 0; y < gridHeight; y++) {
            if (IsFullRowAt(y)) {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                y--;
            }
        }
    }

    public bool CheckIsInsideGrid(Vector2 pos) {
        int x = (int) pos.x;
        int y = (int) pos.y;

        return (x >= 0 && x < gridWidth && y >= 0);
    }

    public void displayPreview() {
        if (nextTetromino) Destroy(nextTetromino.gameObject);
        nextTetromino = (GameObject) Instantiate(tetrominos[randomTetromino], new Vector2(14f, 16f), Quaternion.identity);
        nextTetromino.GetComponent<Tetromino>().enabled = false;
    }

    public Vector2 Round(Vector2 pos) {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public void SpawnNextTetromino() {
        GameObject tetromino = (GameObject) Instantiate(tetrominos[randomTetromino], new Vector2(5.0f, 20.0f), Quaternion.identity);
        randomTetromino = Random.Range(0, tetrominos.Length);
        displayPreview();
    }

    public void UpdateGrid(Tetromino tetromino) {
        for (int y = 0; y < gridHeight; y++) {
            for (int x = 0; x < gridWidth; x++) {
                if (grid[x, y] != null && grid[x, y].parent == tetromino.transform) {
                    grid[x, y] = null;
                }
            }
        }

        foreach (Transform mino in tetromino.transform) {
            Vector2 pos = Round(mino.position);
            int x = (int) pos.x;
            int y = (int) pos.y;
            if (y < gridHeight) {
                grid[x, y] = mino;
            }
        }
    }

    public void PrintGrid() {
        Utils.ClearLogConsole();
        Debug.Log("============");
        for (int y = gridHeight - 1; y >= 0; y--) {
            string line = "";
            for (int x = 0; x < gridWidth; x++) {
                line += grid[x, y] == null ? "x" : "o";
            }

            Debug.Log(line);
        }

        Debug.Log("============");
    }

    public Transform GetTransformAtGridPosition(Vector2 pos) {
        int x = (int) pos.x;
        int y = (int) pos.y;
        if (y > gridHeight - 1) return null;

        return grid[x, y];
    }

    public void GameOver() {
        gameOver = true;
        SceneManager.LoadScene("GameOver");
    }

    void PlayLineClearedAudio() {
        FindObjectOfType<SoundManager>().audioSource.PlayOneShot(FindObjectOfType<SoundManager>().lineClearedSound);
    }
}