using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TetrominoRenderer : MonoBehaviour
{
    public int id;
    private List<TetrominoComponent> components;
    private bool modifiedShape = false;
    private List<TetrominoComponent> unresolvedComponents;
    private TetrominoSpriteManager spriteManager;

    private void Awake()
    {
        components = GetComponentsInChildren<TetrominoComponent>().ToList();
    }

    private void Start()
    {
        spriteManager = TetrominoSpriteManager.Instance;
    }

    private void Update()
    {
        if (modifiedShape)
        {
            ResolveShape();
        }
        else
        {
            for (int i = 0; i < components.Count; i++)
            {
                TetrominoComponent component = components[i];
                Sprite sprite = spriteManager.GetSprite(id, i);
                if (sprite != null)
                    component.spriteRenderer.sprite = sprite;
            }
        }
    }

    private void ResolveShape()
    {
        if (components.Count > 0)
        {
            Console.WriteLine("Resolving Tetromino");
        }
        else
        {
            Console.WriteLine("TetrominoRenderer has no associated Tetromino components");
        }
    }
}

public enum TetrominoID
{
    ZTetromino = 0,
    STetromino = 1,
    ITetromino = 2,
    TTetromino = 3,
    OTetromino = 4,
    LTetromino = 5,
    JTetromino = 6,
}