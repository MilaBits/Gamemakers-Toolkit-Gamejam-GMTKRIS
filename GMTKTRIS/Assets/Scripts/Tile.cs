using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool Occupied;
    public int State;
    public SpriteRenderer Renderer;

    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color) => Renderer.color = color;

    public void UpdateVisual()
    {
        switch (State)
        {
            case 0:
                Renderer.color = Color.white;
                break;
            case 1:
                Renderer.color = Color.blue;
                break;
            case 2:
                Renderer.color = Color.red;
                break;
        }
    }

    public Color GetColor() => Renderer.color;

    public void SetSprite(Sprite sprite) => Renderer.sprite = sprite;
}