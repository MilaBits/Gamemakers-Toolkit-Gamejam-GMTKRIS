using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class Tetromino
    {
        public Vector2Int[] Shape;
        public Vector2Int Position;
        public Color Color;
        public Vector2 Rotation;
    }

    // public class TTetromino : Tetromino
    // {
    //     public TTetromino()
    //     {
    //         Shape = new[] {new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1)};
    //         Color = Color.magenta;
    //     }
    // }
    //
    // public class LTetromino : Tetromino
    // {
    //     public LTetromino()
    //     {
    //         Shape = new[] {1, 5, 9, 10};
    //         Color = new Color(1f, 0.51f, 0f);
    //     }
    // }
    //
    // public class JTetromino : Tetromino
    // {
    //     public JTetromino()
    //     {
    //         Shape = new[] {1, 5, 8, 9};
    //         Color = Color.blue;
    //     }
    // }
    //
    // public class ZTetromino : Tetromino
    // {
    //     public ZTetromino()
    //     {
    //         Shape = new[] {1, 4, 5, 9};
    //         Color = Color.red;
    //     }
    // }
    //
    // public class STetromino : Tetromino
    // {
    //     public STetromino()
    //     {
    //         Shape = new[] {1, 5, 6, 10};
    //         Color = Color.green;
    //     }
    // }
    //
    // public class ITetromino : Tetromino
    // {
    //     public ITetromino()
    //     {
    //         Shape = new[] {1, 5, 9, 13};
    //         Color = Color.cyan;
    //     }
    // }
    //
    // public class OTetromino : Tetromino
    // {
    //     public OTetromino()
    //     {
    //         Shape = new[] {5, 6, 9, 10};
    //         Color = Color.yellow;
    //     }
    // }
}