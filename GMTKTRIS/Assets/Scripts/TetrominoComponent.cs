using UnityEngine;

public class TetrominoComponent : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector2Int pos { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Vector3 position = transform.position;
        pos = new Vector2Int((int)position.x, (int)position.y);
    }
}