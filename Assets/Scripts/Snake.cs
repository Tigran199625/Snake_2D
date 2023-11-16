using CodeMonkey;
using CodeMonkey.Utils;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private enum Direction {
        Left,
        Right,
        Up,
        Down
    }

    private enum State {
        Alive,
        Dead
    }

    private State state;
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;

    bool visualUpdated = true;

    // Contain amount of time between the moves
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;

    // Snake growing logic part 

    // When value is 0, snake have only head
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;

    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
    }

    private void Awake() {
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;

        // Situation where snake have only his head.
        snakeBodySize = 0;
        snakeMovePositionList = new List<SnakeMovePosition>();

        snakeBodyPartList = new List<SnakeBodyPart>();

        state = State.Alive;
    }

    // Update is called once per frame
    private void Update()
    {
        switch(state) {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
    }

    private void HandleInput() {

        if (Input.GetKeyDown(KeyCode.UpArrow) && visualUpdated) {
            if (gridMoveDirection != Direction.Down) {
                gridMoveDirection = Direction.Up;
                visualUpdated = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && visualUpdated) {
            if (gridMoveDirection != Direction.Up) {
                gridMoveDirection = Direction.Down;
                visualUpdated = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && visualUpdated) {
            if (gridMoveDirection != Direction.Right) {
                gridMoveDirection = Direction.Left;
                visualUpdated = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && visualUpdated) {
            if (gridMoveDirection != Direction.Left) {
                gridMoveDirection = Direction.Right;
                visualUpdated = false;
            }
        }

    }

    private void HandleGridMovement() {

        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax) {
            gridMoveTimer -= gridMoveTimerMax;

            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);

            SnakeMovePosition previosSnakeMovePosition = null;
            if(snakeMovePositionList.Count > 0) {
                previosSnakeMovePosition = snakeMovePositionList[0];
            }


            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previosSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch(gridMoveDirection) {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                case Direction.Left:  gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up:    gridMoveDirectionVector = new Vector2Int(0, +1); break;
                case Direction.Down:  gridMoveDirectionVector = new Vector2Int(0, -1); break;

            }

            gridPosition += gridMoveDirectionVector;

            gridPosition = levelGrid.ValidateGridPosition(gridPosition);


            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood) {
                // Snake ate food, grow body
                snakeBodySize++;
                CreateSnakeBodyPart();
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }

            // To avoid the "no need" extension of the list.
            if (snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyParts();

            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList) {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if(gridPosition == snakeBodyPartGridPosition) {
                    // GAME OVER
                    state = State.Dead;
                    GameHandler.SnakeDied();
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                }
            }


            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

            visualUpdated = true;

        }
    }

    private void CreateSnakeBodyPart() {

        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));

    }
    private void UpdateSnakeBodyParts() {
        for (int i = 0; i < snakeBodyPartList.Count; i++) {
            snakeBodyPartList[i].SetGridPosition(snakeMovePositionList[i]);
        }
    }
    private float GetAngleFromVector(Vector2Int dir) {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition() {
        return gridPosition;
    }


    // Return the full list of positions occupies by the snake: Head + Body
    public List<Vector2Int> GetFullSnakeGridPositionList() {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList) {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private class SnakeBodyPart {

        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        public SnakeBodyPart(int bodyIndex) {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetGridPosition(SnakeMovePosition snakeMovePosition) {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            switch (snakeMovePosition.GetDirection()) {
                default:
                    // CHECKED
                case Direction.Up: // Currently going Up 
                    switch(snakeMovePosition.GetPreviousDirevtion()) {
                        default:
                            angle = 0; break;
                        case Direction.Left: // Previously was going Left
                            angle = 0 + 45;
                            transform.position = new Vector3(transform.position.x + .2f, (transform.position.y + .2f));
                            break;
                        case Direction.Right: // Previously was going Right
                            angle = 0 - 45;
                            transform.position = new Vector3(transform.position.x - .2f, (transform.position.y + .2f));
                            break;
                    }
                    break;

                    // 
                case Direction.Down: // Currently going Down
                    switch (snakeMovePosition.GetPreviousDirevtion()) {
                        default:
                            angle = 180; break;
                        case Direction.Left: // Previously was going Left
                            angle = 180 - 45;
                            transform.position = new Vector3(transform.position.x + .2f, (transform.position.y - .2f));
                            break;
                        case Direction.Right: // Previously was going Right
                            angle = 180 + 45;
                            transform.position = new Vector3(transform.position.x - .2f, (transform.position.y - .2f));
                            break;
                    }
                    break;

                case Direction.Left: // Currently going to the Left
                    switch (snakeMovePosition.GetPreviousDirevtion()) {
                        default:
                            angle = -90; break;
                        case Direction.Down: // Previously was going Down
                            angle = -45;
                            transform.position = new Vector3(transform.position.x - .2f, (transform.position.y + .2f));
                            break;
                        case Direction.Up: // Previously was going Up
                            angle = 45;
                            transform.position = new Vector3(transform.position.x - .2f, (transform.position.y - .2f));
                            break;
                    }
                    break;


                case Direction.Right: // Currently going Right
                    switch (snakeMovePosition.GetPreviousDirevtion()) {
                        default:
                            angle = 90; break;
                        case Direction.Down: // Previously was going Down
                            angle = 45;
                            transform.position = new Vector3(transform.position.x + .2f, (transform.position.y + .2f));
                            break;
                        case Direction.Up: // Previously was going Up
                            angle = -45;
                            transform.position = new Vector3(transform.position.x + .2f, (transform.position.y - .2f));
                            break;
                    }
                    break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition() {
            return snakeMovePosition.GetGridPosition();
        }
    }

    // Class handles one Move Position from the Snake
    private class SnakeMovePosition {

        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction) {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;

        }
        public Vector2Int GetGridPosition() {
            return gridPosition;
        }
        public Direction GetDirection() {
            return direction;
        }
        public Direction GetPreviousDirevtion() {
            if(previousSnakeMovePosition == null) {
                return Direction.Right;
            } else {
                return previousSnakeMovePosition.direction;
            }
        }

    }
}
