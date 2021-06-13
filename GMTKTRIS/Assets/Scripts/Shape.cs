using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Shape : MonoBehaviour
    {
        public Tetromino tetromino;
        public TetrominoRenderer renderer;
        public ShapeScript ShapeScript;

        private void Awake()
        {
            renderer = GetComponentInChildren<TetrominoRenderer>();
        }
    }
}