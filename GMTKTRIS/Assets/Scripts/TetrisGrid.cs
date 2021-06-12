using System;
using System.Collections.Generic;
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
                tiles[x, y] = Instantiate(squarePrefab, new Vector3(x, y), Quaternion.identity, transform);
            }
        }
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") != 0) inputDirection = Input.GetAxis("Horizontal") > 0 ? Vector2Int.right : Vector2Int.left;
        if (Input.GetAxis("Vertical") < 0) realTickLength = originalTickLenght * 0.5f;
        else realTickLength = originalTickLenght;

        if (passedTickTime >= realTickLength)
        {
            if (CanMove(fallingShape, Vector2Int.down))
            {
                Move(fallingShape, Vector2Int.down + inputDirection);
                DrawFallingShape();
            }
            else
            {
                ShapeHasLanded(fallingShape);
            }

            // UpdateTiles();

            passedTickTime = 0;
        }

        inputDirection = Vector2Int.zero;
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

        fallingShape.data.Position = new Vector2Int(5, 18);
        fallingShape.transform.position = new Vector3(fallingShape.data.Position.x, fallingShape.data.Position.y);
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