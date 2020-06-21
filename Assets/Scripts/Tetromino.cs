using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour {
    private float fall = 0f;
    float fallSpeed;

    public bool allowRotation = true;
    public bool limitRotation = false;

    public int individualScore = 100;
    private float individualScoreTime = 0;

    private float continuousVerticalSpeed = 0.05f;
    private float continuousHorizontalSpeed = 0.1f;
    private float buttonDownWaitMax = 0.2f;
    private float buttonDownWaitTimer = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    private float verticalTimer = 0;
    private float horizontalTimer = 0;

    private bool drop = false;

    void Start() {
        fallSpeed = GameObject.FindObjectOfType<Game>().fallSpeed;
    }

    void Update() {
        CheckUserInput();
        UpdateIndividualScore();
    }

    void UpdateIndividualScore() {
        if (individualScoreTime < 1) {
            individualScoreTime += Time.deltaTime;
        } else {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    void CheckUserInput() {
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.UpArrow)) {
            movedImmediateHorizontal = false;
            movedImmediateVertical = false;
            horizontalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimer = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            if (movedImmediateHorizontal && buttonDownWaitTimer < buttonDownWaitMax) {
                buttonDownWaitTimer += Time.deltaTime;
                return;
            }

            if (movedImmediateHorizontal && horizontalTimer < continuousHorizontalSpeed) {
                horizontalTimer += Time.deltaTime;
                return;
            }

            if (!movedImmediateHorizontal) movedImmediateHorizontal = true;

            horizontalTimer = 0;
            RightArrowAction();
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (movedImmediateHorizontal && buttonDownWaitTimer < buttonDownWaitMax) {
                buttonDownWaitTimer += Time.deltaTime;
                return;
            }

            if (movedImmediateHorizontal && horizontalTimer < continuousHorizontalSpeed) {
                horizontalTimer += Time.deltaTime;
                return;
            }

            if (!movedImmediateHorizontal) movedImmediateHorizontal = true;

            horizontalTimer = 0;
            LeftArrowAction();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            UpArrowAction();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            fallSpeed = 0.0000001f;
            drop = true;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) {
            if (movedImmediateVertical && !drop && buttonDownWaitTimer < buttonDownWaitMax) {
                buttonDownWaitTimer += Time.deltaTime;
                return;
            }

            if (movedImmediateVertical && !drop && verticalTimer < continuousVerticalSpeed) {
                verticalTimer += Time.deltaTime;
                return;
            }

            if (!movedImmediateVertical) movedImmediateVertical = true;

            verticalTimer = 0;
            DownArrowAction();
        }
    }

    void RightArrowAction() {
        transform.position += new Vector3(1, 0, 0);
        if (!CheckIsValidPosition()) {
            transform.position += new Vector3(-1, 0, 0);
        } else {
            PlayMoveAudio();
            FindObjectOfType<Game>().UpdateGrid(this);
        }
    }

    void LeftArrowAction() {
        transform.position += new Vector3(-1, 0, 0);
        if (!CheckIsValidPosition()) {
            transform.position += new Vector3(1, 0, 0);
        } else {
            PlayMoveAudio();
            FindObjectOfType<Game>().UpdateGrid(this);
        }
    }

    void UpArrowAction() {
        if (!allowRotation) return;

        if (transform.rotation.eulerAngles.z < 90 && limitRotation) {
            transform.Rotate(0, 0, 90);
        } else {
            transform.Rotate(0, 0, -90);
        }

        if (!CheckIsValidPosition()) {
            if (transform.rotation.eulerAngles.z >= 90 && limitRotation) transform.Rotate(0, 0, -90);
            else transform.Rotate(0, 0, 90);
        } else {
            PlayRotateAudio();
            FindObjectOfType<Game>().UpdateGrid(this);
        }
    }

    void DownArrowAction() {
        transform.position += new Vector3(0, -1, 0);
        if (!CheckIsValidPosition()) {
            transform.position += new Vector3(0, 1, 0);
            enabled = false;
            FindObjectOfType<Game>().DeleteRow();

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this)) {
                FindObjectOfType<Game>().GameOver();
            }

            PlayLandAudio();
            drop = false;
            FindObjectOfType<Game>().SpawnNextTetromino();
            Game.currentScore += individualScore;
        } else {
            if (Input.GetKey(KeyCode.DownArrow)) PlayMoveAudio();
            FindObjectOfType<Game>().UpdateGrid(this);
        }

        fall = Time.time;
    }

    bool CheckIsValidPosition() {
        foreach (Transform mino in transform) {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            if (!FindObjectOfType<Game>().CheckIsInsideGrid(pos)) {
                return false;
            }

            Transform collision = FindObjectOfType<Game>().GetTransformAtGridPosition(pos);
            if (collision != null && collision.parent != transform) {
                return false;
            }
        }

        return true;
    }

    void PlayMoveAudio() {
        FindObjectOfType<SoundManager>().audioSource.PlayOneShot(FindObjectOfType<SoundManager>().moveSound);
    }

    void PlayRotateAudio() {
        FindObjectOfType<SoundManager>().audioSource.PlayOneShot(FindObjectOfType<SoundManager>().rotateSound);
    }

    void PlayLandAudio() {
        FindObjectOfType<SoundManager>().audioSource.PlayOneShot(FindObjectOfType<SoundManager>().landSound);
    }
}