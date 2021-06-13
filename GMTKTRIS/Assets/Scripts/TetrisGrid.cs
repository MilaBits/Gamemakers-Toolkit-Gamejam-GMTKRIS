using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TetrisGrid : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(10, 20);

    public List<ShapeScript> ShapeScripts = new List<ShapeScript>();

    public Sprite singleCat;

    public GameObject scriptObject;
    public GameObject shapeScriptObject;

    public AudioClip placeSound;
    public AudioSource audioSource;

    [SerializeField]
    private Tile squarePrefab;

    public Tile[,] tiles;

    public List<Tetromino> shapes;

    private Shape fallingShape;
    public Shape shapePrefab;

    public bool paused;

    public float originalTickLenght;
    private float realTickLength;
    private float passedTickTime;

    public List<Shape> fallingShapes;
    public List<Shape> fallingShapeOffsets;

    public int score;
    public TextMeshProUGUI scoreText;

    public UnityEvent Landed = new UnityEvent();
    public UnityEvent RemovedBlock = new UnityEvent();
    public UnityEvent Lost = new UnityEvent();

    private void Awake()
    {
        tiles = new Tile[size.x, size.y];
    }

    private void Start()
    {
        realTickLength = originalTickLenght;
        InitGrid();
    }

    [ContextMenu("Draw")]
    public void InitGrid()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tiles[x, y] = Instantiate(squarePrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
                tiles[x, y].SetColor(new Color());
            }
        }
    }

    private void Update()
    {
        if (paused) return;

        if (Input.GetAxis("Vertical") < 0) realTickLength = originalTickLenght * 0.5f;
        else realTickLength = originalTickLenght;

        if (Input.GetAxis("Horizontal") != 0)
        {
            if (Input.GetKeyDown(KeyCode.D) && CanMove(fallingShapes, Vector2Int.right)) Move(fallingShapes, Vector2Int.right);
            if (Input.GetKeyDown(KeyCode.A) && CanMove(fallingShapes, Vector2Int.left)) Move(fallingShapes, Vector2Int.left);
        }

        if (passedTickTime >= realTickLength)
        {
            if (CanMove(fallingShapes, Vector2Int.down))
            {
                Move(fallingShapes, Vector2Int.down);
            }
            else
            {
                ShapesHaveLanded(fallingShapes);
            }

            passedTickTime = 0;
        }

        passedTickTime += Time.deltaTime;
    }

    private void ShapeHasLanded(Shape shape)
    {
        SolidifyShape(shape);
        RemoveLines();
    }

    private void ShapesHaveLanded(List<Shape> shapes)
    {
        SolidifyShapes(shapes);
        StartCoroutine(RemoveLines());

        audioSource.PlayOneShot(placeSound);

        Landed.Invoke();

        SwitchToBuildGrid();
    }

    private void SwitchToBuildGrid()
    {
        var buildGrid = FindObjectOfType<BuildGrid>();
        buildGrid.Go();

        paused = true;
        fallingShapes = null;
    }


    private IEnumerator RemoveLines()
    {
        int fullRows = 0;
        for (int y = size.y - 1; y >= 0; y--)
        {
            int filledCount = 0;
            for (int x = 0; x < size.x; x++)
            {
                if (tiles[x, y].Occupied) filledCount++;
            }

            if (filledCount == size.x)
            {
                fullRows++;
                StartCoroutine(RemoveLine(y));
                yield return new WaitForSeconds(.5f);
            }

            filledCount = 0;
        }
    }

    private IEnumerator RemoveLine(int row)
    {
        foreach (var shapeScript in ShapeScripts)
        {
            shapeScript.RemoveComponentsInRow(row, singleCat);
        }

        for (int i = 0; i < size.x - 1; i++)
        {
            tiles[i, row].State = 0;
            tiles[i, row].Occupied = false;
            tiles[i, row].SetColor(new Color());
            RemovedBlock.Invoke();
            yield return new WaitForSeconds(.1f);
        }

        score++;
        scoreText.text = score.ToString();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = row; y < size.y - 1; y++)
            {
                tiles[x, y].State = tiles[x, y + 1].State;
                tiles[x, y].Occupied = tiles[x, y + 1].Occupied;
                tiles[x, y].SetColor(tiles[x, y + 1].GetColor());
                tiles[x, y].SetSprite(tiles[x, y + 1].GetComponent<SpriteRenderer>().sprite);
                tiles[x, y].transform.rotation = tiles[x, y + 1].transform.rotation;
            }
        }
    }

    private void NewShape()
    {
        int id = Random.Range(0, 7);
        fallingShape = Instantiate(shapePrefab);


        fallingShape.tetromino = shapes[id];
        for (int i = 0;
            i < fallingShape.tetromino.Shape.Length;
            i++)
        {
            fallingShape.transform.GetChild(i).localPosition += new Vector3(fallingShape.tetromino.Shape[i].x, fallingShape.tetromino.Shape[i].y);
            fallingShape.transform.GetChild(i).GetComponent<SpriteRenderer>().color = fallingShape.tetromino.Color;
        }

        fallingShape.transform.position = new Vector3(fallingShape.transform.position.x, fallingShape.transform.position.y);
        fallingShape.renderer.Rotate(fallingShape.tetromino.Rotation * 90);
        fallingShape.renderer.id = id;
    }

    private bool CanRotate(Shape shape, bool clockwise)
    {
        for (int i = 0;
            i < shape.tetromino.Shape.ToList().Count;
            i++)
        {
            var newpos = shape.tetromino.Shape[i].Rotate(clockwise ? 90 : -90);
            newpos += new Vector2Int((int) shape.transform.position.x, (int) shape.transform.position.y);
            if (newpos.x < 0 || newpos.x > size.x - 1 || newpos.y < 0 || tiles[newpos.x, newpos.y].Occupied) return false;
        }

        return true;
    }

    private void Rotate(Shape shape, bool clockwise)
    {
        var newShape = new Vector2Int[4];
        for (int i = 0;
            i < shape.tetromino.Shape.ToList().Count;
            i++)
        {
            newShape[i] = shape.tetromino.Shape[i].Rotate(clockwise ? 90 : -90);
            shape.transform.GetChild(i).transform.position = shape.transform.position + new Vector3(newShape[i].x, newShape[i].y);
        }

        shape.tetromino.Rotation += clockwise ? 1 : -1;
        shape.tetromino.Rotation = shape.tetromino.Rotation % 4;
        shape.tetromino.Shape = newShape;
        shape.renderer.Rotate(clockwise ? 90 : -90);
    }

    private void SolidifyShape(Shape shape)
    {
        var thing = shape.ShapeScript;
        for (int i = 0; i < shape.tetromino.Shape.Length; i++)
        {
            var pos = shape.tetromino.Shape[i];
            var x = (int) shape.transform.position.x + pos.x;
            var y = (int) shape.transform.position.y + pos.y;
            if (x < size.x && y < size.y && y >= 0)
            {
                tiles[x, y].State = shape.tetromino.id;
                tiles[x, y].Occupied = true;
                tiles[x, y].SetColor(shape.tetromino.Color);
                tiles[x, y].SetSprite(shape.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite);
                tiles[x, y].transform.rotation = shape.transform.GetChild(i).rotation;
                thing.AddComponent(tiles[x, y].gameObject, y);
            }
        }

        Destroy(shape.gameObject);
    }

    private void SolidifyShapes(List<Shape> shapes)
    {
        foreach (Shape shape in shapes)
        {
            SolidifyShape(shape);
        }
    }

    private bool CanMove(Shape shape, Vector2Int direction)
    {
        foreach (var pos in shape.tetromino.Shape)
        {
            var x = (int) shape.transform.position.x + pos.x + direction.x;
            var y = (int) shape.transform.position.y + pos.y + direction.y;
            if (x < 0 || x > size.x - 1 || y < 0 || tiles[x, y].Occupied) return false;
        }

        return true;
    }

    private bool CanMove(List<Shape> shapes, Vector2Int direction)
    {
        foreach (Shape shape in shapes)
        {
            if (!CanMove(shape, direction)) return false;
        }

        return true;
    }

    private void Move(Shape shape, Vector2Int direction)
    {
        shape.transform.position += new Vector3(direction.x, direction.y);
    }

    private void Move(List<Shape> shapes, Vector2Int direction)
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            Move(shapes[i], direction);
        }
    }

    public void AddFallingShapes(List<Shape> placedShapes)
    {
        fallingShapes = placedShapes.ToList();

        for (int i = 0; i < fallingShapes.Count; i++)
        {
            // make a game objects
            var go = Instantiate(shapeScriptObject, scriptObject.transform);
            var shapeScript = go.GetComponent<ShapeScript>();
            ShapeScripts.Add(shapeScript);

            var pos = placedShapes[i].transform.localPosition + new Vector3(0, 10);
            fallingShapes[i].transform.SetParent(transform);
            fallingShapes[i].transform.position = pos;
            fallingShapes[i].ShapeScript = shapeScript;
        }
        
        if (fallingShapes.Any(x => !IsValidToPlace(x, true)))
        {
            Lost.Invoke();
            return;
        }

        paused = false;
    }

    private bool IsValidToPlace(Shape shape, bool checkForState)
    {
        for (int i = 0; i < shape.tetromino.Shape.Length; i++)
        {
            int x = (int) shape.transform.position.x + shape.tetromino.Shape[i].x;
            int y = (int) shape.transform.position.y + shape.tetromino.Shape[i].y;

            if (x < 0 || x > size.x - 1 ||
                y < 0 || y > size.y - 1) return false;
            if (checkForState && tiles[x, y].Occupied) return false;
        }

        return true;
    }
}