using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class BuildGrid : MonoBehaviour
    {
        [SerializeField]
        private Tile squarePrefab;

        public Tile[,] tiles;

        private Vector2Int[] lastValidPositions;
        private Shape movingShape;
        public Shape shapePrefab;

        public Vector2Int size = new Vector2Int(10, 10);

        public List<Tetromino> shapes = new List<Tetromino>();

        public int nextShapeCount = 4;
        public List<Shape> nextShapes = new List<Shape>();
        public List<Shape> placedShapes = new List<Shape>();

        private Stack<Tetromino> bag = new Stack<Tetromino>();

        public bool paused;

        private void Awake()
        {
            tiles = new Tile[size.x, size.y];
        }

        private void Start()
        {
            InitGrid();

            MakeNextShapes();

            ShowNextShapes();
            NextShape(0);
        }

        private void MakeNextShapes()
        {
            for (int i = 0; i < nextShapeCount; i++)
            {
                var shape = Instantiate(shapePrefab, transform, false);
                shape.tetromino = NewShape();
                for (int j = 0; j < shape.tetromino.Shape.Length; j++)
                {
                    shape.transform.GetChild(j).localPosition = new Vector2(shape.tetromino.Shape[j].x, shape.tetromino.Shape[j].y);
                    shape.transform.GetChild(j).GetComponent<SpriteRenderer>().color = shape.tetromino.Color;
                }

                shape.renderer.Rotate(shape.tetromino.Rotation * 90);
                shape.renderer.id = shape.tetromino.id;

                nextShapes.Add(shape);
            }
        }

        private void ShowNextShapes()
        {
            for (int i = 0; i < nextShapes.Count; i++)
            {
                nextShapes[i].transform.localPosition = new Vector3(1 + i * 4, -4);
            }
        }

        private void Update()
        {
            if (paused) return;
            if (Input.GetKeyDown(KeyCode.W)) Move(movingShape, Vector2Int.up);
            if (Input.GetKeyDown(KeyCode.A)) Move(movingShape, Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.S)) Move(movingShape, Vector2Int.down);
            if (Input.GetKeyDown(KeyCode.D)) Move(movingShape, Vector2Int.right);
            if (Input.GetKeyDown(KeyCode.Q)) Rotate(movingShape, false);
            if (Input.GetKeyDown(KeyCode.E)) Rotate(movingShape, true);
            if (Input.GetKeyDown(KeyCode.Space)) Place(movingShape);
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchShape(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchShape(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchShape(2);
        }

        private void Place(Shape shape)
        {
            if (IsValidToPlace(shape, true))
            {
                for (int i = 0; i < shape.tetromino.Shape.Length; i++)
                {
                    var pos = shape.tetromino.Shape[i];
                    int x = (int) shape.transform.localPosition.x + pos.x;
                    int y = (int) shape.transform.localPosition.y + pos.y;
                    if (x < size.x && y < size.y && y >= 0)
                    {
                        tiles[x, y].State = 1;
                        tiles[x, y].SetColor(shape.tetromino.Color);
                        tiles[x, y].SetSprite(shape.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite);
                        tiles[x, y].transform.rotation = shape.transform.GetChild(i).rotation;
                    }
                }

                if (nextShapes.Count > 0)
                {
                    NextShape(0);
                }
                else
                {
                    ClearTiles();

                    var tetrisGrid = FindObjectOfType<TetrisGrid>();
                    tetrisGrid.AddFallingShapes(placedShapes);
                    placedShapes = new List<Shape>();
                    movingShape = null;

                    paused = true;
                }
            }
        }

        private void ClearTiles()
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    tiles[x, y].State = 0;
                    tiles[x, y].SetColor(new Color());
                }
            }
        }

        private bool IsValidToPlace(Shape shape, bool checkForState)
        {
            for (int i = 0; i < shape.tetromino.Shape.Length; i++)
            {
                int x = (int) shape.transform.localPosition.x + shape.tetromino.Shape[i].x;
                int y = (int) shape.transform.localPosition.y + shape.tetromino.Shape[i].y;

                if (x < 0 || x > size.x - 1 ||
                    y < 0 || y > size.y - 1) return false;
                if (checkForState && tiles[x, y].State != 0) return false;
            }

            return true;
        }

        private void Move(Shape shape, Vector2Int direction)
        {
            Vector2Int newPos = new Vector2Int((int) shape.transform.localPosition.x + direction.x, (int) shape.transform.localPosition.y + direction.y);

            if (CanMove(shape, direction, false))
            {
                shape.transform.localPosition = new Vector3(newPos.x, newPos.y);
                if (CanMove(shape, direction, true)) lastValidPositions = shape.tetromino.Shape;
            }
        }

        private bool CanMove(Shape shape, Vector2Int direction, bool checkForState)
        {
            foreach (var pos in shape.tetromino.Shape)
            {
                int x = (int) shape.transform.localPosition.x + pos.x + direction.x;
                int y = (int) shape.transform.localPosition.y + pos.y + direction.y;

                if (x < 0 || x > size.x - 1 ||
                    y < 0 || y > size.y - 1) return false;
                if (checkForState && tiles[x, y].State != 0) return false;
            }

            return true;
        }

        private bool CanRotate(Shape shape, bool clockwise, bool checkForState)
        {
            for (int i = 0; i < shape.tetromino.Shape.ToList().Count; i++)
            {
                var newpos = shape.tetromino.Shape[i].Rotate(clockwise ? 90 : -90);
                newpos += new Vector2Int((int) shape.transform.localPosition.x, (int) shape.transform.localPosition.y);

                if (newpos.x < 0 || newpos.x > size.x - 1 ||
                    newpos.y < 0 || newpos.y > size.y - 1) return false;
                if (checkForState && tiles[newpos.x, newpos.y].State != 0) return false;
            }

            return true;
        }

        private void Rotate(Shape shape, bool clockwise)
        {
            var newShape = new Vector2Int[4];
            for (int i = 0; i < shape.tetromino.Shape.ToList().Count; i++)
            {
                newShape[i] = shape.tetromino.Shape[i].Rotate(clockwise ? 90 : -90);
            }

            if (CanRotate(shape, clockwise, false))
            {
                shape.tetromino.Shape = newShape;
                for (int i = 0; i < newShape.Length; i++) shape.transform.GetChild(i).localPosition = new Vector2(shape.tetromino.Shape[i].x, shape.tetromino.Shape[i].y);

                shape.tetromino.Rotation += clockwise ? 1 : -1;
                shape.tetromino.Rotation = shape.tetromino.Rotation % 4;
                shape.renderer.Rotate(clockwise ? 90 : -90);

                if (CanRotate(shape, clockwise, true)) lastValidPositions = newShape;
            }
        }

        private Shape NextShape(int i)
        {
            movingShape = nextShapes.ToList()[i];
            nextShapes.RemoveAt(i);

            placedShapes.Add(movingShape);

            movingShape.transform.localPosition += new Vector3(0, 5);

            ShowNextShapes();
            return movingShape;
        }

        private void SwitchShape(int i)
        {
            if (nextShapes.Count >= i)
            {
                var pos = movingShape.transform.localPosition;
                nextShapes.Add(movingShape);
                placedShapes.Remove(movingShape);

                NextShape(i);
                movingShape.transform.localPosition = pos;
                ShowNextShapes();
            }
        }

        private Tetromino NewShape()
        {
            if (bag.Count == 0)
            {
                var items = shapes.ToList();
                items.Shuffle();
                bag = new Stack<Tetromino>(items);
            }

            return new Tetromino(bag.Pop());
        }

        public void InitGrid()
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    tiles[x, y] = Instantiate(squarePrefab, transform, false);
                    tiles[x, y].transform.localPosition = new Vector3(x, y, 1);
                    tiles[x, y].SetColor(new Color());
                }
            }
        }

        public void Go()
        {
            MakeNextShapes();
            ShowNextShapes();
            NextShape(0);
            paused = false;
        }
    }
}