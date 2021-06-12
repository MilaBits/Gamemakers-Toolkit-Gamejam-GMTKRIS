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

    // private int[,] cells;


    public Vector2Int inputDirection;

    public float originalTickLenght;
    private float realTickLength;
    private float passedTickTime;

    private void Awake()
    {
        // cells = new int[size.x, size.y];
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
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tiles[x, y] = Instantiate(squarePrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
            }
        }
    }

    private void Update()
    {
        if (Input.GetAxis("Vertical") < 0) realTickLength = originalTickLenght * 0.5f;
        else realTickLength = originalTickLenght;

        if (Input.GetKeyDown(KeyCode.Q) && CanRotate(fallingShape, false)) Rotate(fallingShape, false);
        if (Input.GetKeyDown(KeyCode.E) && CanRotate(fallingShape, true)) Rotate(fallingShape, true);

        if (Input.GetAxis("Horizontal") != 0)
        {
            if (Input.GetKeyDown(KeyCode.D) && CanMove(fallingShape, Vector2Int.right)) Move(fallingShape, Vector2Int.right);
            if (Input.GetKeyDown(KeyCode.A) && CanMove(fallingShape, Vector2Int.left)) Move(fallingShape, Vector2Int.left);
        }

        if (passedTickTime >= realTickLength)
        {
            if (CanMove(fallingShape, Vector2Int.down))
            {
                Move(fallingShape, Vector2Int.down);
            }
            else
            {
                ShapeHasLanded(fallingShape);
            }

            passedTickTime = 0;
        }

        passedTickTime += Time.deltaTime;
    }

    private void ShapeHasLanded(Shape shape)
    {
        Debug.Log("landed");
        SolidifyShape(shape);
        RemoveLines();

        NewShape();
    }

    private int RemoveLines()
    {
        int fullRows = 0;
        for (int y = 0; y < size.y; y++)
        {
            int filledCount = 0;
            for (int x = 0; x < size.x; x++)
            {
                if (tiles[x, y].State != 0) filledCount++;
            }

            if (filledCount == size.x)
            {
                fullRows++;
            }
        }

        for (int i = fullRows; i > 0; i--)
        {
            RemoveLine(i-1);
        }

        return fullRows;
    }

    private void RemoveLine(int row)
    {
        for (int i = 0; i < size.x - 1; i++)
        {
            tiles[i, row].State = 0;
            tiles[i, row].SetColor(Color.white);
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y - 1; y++)
            {
                tiles[x, y] = tiles[x, y + 1];
                tiles[x, y].SetColor(tiles[x, y + 1].GetColor());
            }
        }
    }

    private void NewShape()
    {
        const int id = 4;
        fallingShape = Instantiate(shapePrefab);
        fallingShape.data = shapes[id];
        for (int i = 0; i < fallingShape.data.Shape.Length; i++)
        {
            fallingShape.transform.GetChild(i).localPosition += new Vector3(fallingShape.data.Shape[i].x, fallingShape.data.Shape[i].y);
            fallingShape.transform.GetChild(i).GetComponent<SpriteRenderer>().color = fallingShape.data.Color;
        }

        fallingShape.data.Position = new Vector2Int(Random.Range(3, 8), 17);
        fallingShape.transform.position = new Vector3(fallingShape.data.Position.x, fallingShape.data.Position.y);
        fallingShape.renderer.Rotate(fallingShape.data.Rotation * 90);
        fallingShape.renderer.id = id;
    }

    private bool CanRotate(Shape shape, bool clockwise)
    {
        for (int i = 0; i < shape.data.Shape.ToList().Count; i++)
        {
            var newpos = shape.data.Shape[i].Rotate(clockwise ? 90 : -90);
            newpos += new Vector2Int((int) shape.transform.position.x, (int) shape.transform.position.y);
            if (newpos.x < 0 || newpos.x > size.x - 1 || newpos.y < 0 || tiles[newpos.x, newpos.y].State != 0) return false;
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

        shape.data.Rotation += clockwise ? 1 : -1;
        shape.data.Rotation = shape.data.Rotation % 4;
        shape.data.Shape = newShape;
        shape.renderer.Rotate(clockwise ? 90 : -90);
    }

    private void SolidifyShape(Shape shape)
    {
        foreach (var pos in shape.data.Shape)
        {
            var x = shape.data.Position.x + pos.x;
            var y = shape.data.Position.y + pos.y;
            if (x < size.x && y < size.y && y >= 0)
            {
                tiles[x, y].State = 1;
                tiles[x, y].SetColor(shape.data.Color);
            }
        }

        Destroy(fallingShape.gameObject);
    }

    private bool CanMove(Shape shape, Vector2Int direction)
    {
        foreach (var pos in shape.data.Shape)
        {
            var x = shape.data.Position.x + pos.x + direction.x;
            var y = shape.data.Position.y + pos.y + direction.y;
            if (x < 0 || x > size.x - 1 || y < 0 || tiles[x, y].State != 0) return false;
        }

        return true;
    }

    private void Move(Shape shape, Vector2Int direction)
    {
        shape.data.Position += direction;
        shape.transform.position = new Vector3(shape.data.Position.x, shape.data.Position.y);
    }
}