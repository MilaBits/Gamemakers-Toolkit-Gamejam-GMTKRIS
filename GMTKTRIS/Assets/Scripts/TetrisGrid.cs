using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class TetrisGrid : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(10, 20);

    [SerializeField]
    private Tile squarePrefab;

    public Tile[,] tiles;

    public List<Tetromino> shapes;

    private Shape fallingShape;
    public Shape shapePrefab;

    private int[,] cells;


    private bool rotate;
    private bool clockwise;
    public Vector2Int inputDirection;

    public float originalTickLenght;
    private float realTickLength;
    private float passedTickTime;

    private void Awake()
    {
        cells = new int[size.x, size.y];
        tiles = new Tile[size.x, size.y];
    }

    private void Start()
    {
        realTickLength = originalTickLenght;
        InitGrid();
        NewShape();
    }

    [ContextMenu("Draw")]
    public void InitGrid()
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                tiles[x, y] = Instantiate(squarePrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
            }
        }
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") < 0) inputDirection = Vector2Int.left;
        if (Input.GetAxis("Horizontal") > 0) inputDirection = Vector2Int.right;
        if (Input.GetAxis("Vertical") < 0) realTickLength = originalTickLenght * 0.5f;
        else realTickLength = originalTickLenght;

        if (Input.GetKey(KeyCode.Q))
        {
            rotate = true;
            clockwise = false;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotate = true;
            clockwise = false;
        }

        if (passedTickTime >= realTickLength)
        {
            if (rotate && CanRotate(fallingShape))
            {
                Rotate(fallingShape, false);
                rotate = false;
            }

            if (CanMove(fallingShape, inputDirection))
            {
                Move(fallingShape, inputDirection);
                inputDirection = Vector2Int.zero;
            }

            if (CanMove(fallingShape, Vector2Int.down))
            {
                Move(fallingShape, Vector2Int.down);
                DrawFallingShape();
            }
            else
            {
                ShapeHasLanded(fallingShape);
            }

            passedTickTime = 0;
        }

        passedTickTime += Time.deltaTime;
    }

    private void UpdateTiles()
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                tiles[x, y].State = cells[x, y];
                tiles[x, y].UpdateVisual();
            }
        }
    }

    private void ShapeHasLanded(Shape shape)
    {
        Debug.Log("landed");
        SolidifyShape(shape);
        NewShape();
    }

    private void NewShape()
    {
        fallingShape = Instantiate(shapePrefab);
        fallingShape.data = shapes[Random.Range(0, shapes.Count)];
        for (int i = 0; i < fallingShape.data.Shape.Length; i++)
        {
            fallingShape.transform.GetChild(i).localPosition += new Vector3(fallingShape.data.Shape[i].x, fallingShape.data.Shape[i].y);
            fallingShape.transform.GetChild(i).GetComponent<SpriteRenderer>().color = fallingShape.data.Color;
        }

        fallingShape.data.Position = new Vector2Int(Random.Range(3, 8), 17);
        fallingShape.transform.position = new Vector3(fallingShape.data.Position.x, fallingShape.data.Position.y);
    }

    private bool CanRotate(Shape shape)
    {
        for (int i = 0; i < shape.data.Shape.ToList().Count; i++)
        {
            var newpos = shape.data.Shape[i].Rotate(clockwise ? 90 : -90);
            newpos += new Vector2Int((int) shape.transform.position.x, (int) shape.transform.position.y);
            if (newpos.x < 0 || newpos.x > cells.GetLength(0) - 1 || newpos.y < 0 || tiles[newpos.x, newpos.y].State != 0) return false;
        }

        return true;
    }

    private void Rotate(Shape shape, bool clockwise)
    {
        var newShape = new Vector2Int[4];
        for (int i = 0; i < shape.data.Shape.ToList().Count; i++)
        {
            newShape[i] = shape.data.Shape[i].Rotate(clockwise ? 90 : -90);
            shape.transform.GetChild(i).transform.position = shape.transform.position + new Vector3(newShape[i].x, newShape[i].y);
        }

        shape.data.Shape = newShape;
    }

    private void SolidifyShape(Shape shape)
    {
        foreach (var pos in shape.data.Shape)
        {
            var x = shape.data.Position.x + pos.x;
            var y = shape.data.Position.y + pos.y;
            if (x < cells.GetLength(0) && y < cells.GetLength(1) && y > 0)
            {
                tiles[x, y].State = 1;
                tiles[x, y].SetColor(shape.data.Color);
                Destroy(fallingShape);
            }
        }
    }

    private bool CanMove(Shape shape, Vector2Int direction)
    {
        foreach (var pos in shape.data.Shape)
        {
            var x = shape.data.Position.x + pos.x + direction.x;
            var y = shape.data.Position.y + pos.y + direction.y;
            if (x < 0 || x > cells.GetLength(0) - 1 || y < 0 || tiles[x, y].State != 0) return false;
        }

        return true;
    }

    private void DrawFallingShape()
    {
        for (int i = 0; i < fallingShape.data.Shape.Length; i++)
        {
            var x = fallingShape.data.Position.x + fallingShape.data.Shape[i].x;
            var y = fallingShape.data.Position.y + fallingShape.data.Shape[i].y;
            // tiles[x, y].SetColor(fallingShape.data.Color);
        }
    }

    private void Move(Shape shape, Vector2Int direction)
    {
        shape.data.Position += direction;
        shape.transform.position = new Vector3(shape.data.Position.x, shape.data.Position.y);
    }
}