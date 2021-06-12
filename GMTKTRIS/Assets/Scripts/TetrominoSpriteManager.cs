using System;
using UnityEngine;

public class TetrominoSpriteManager : MonoBehaviour
{
    [SerializeField] private TetrominoSpriteData[] spriteData;
    
    
    public static TetrominoSpriteManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Sprite GetSprite(int TetrominoID, int ComponentID)
    {
        foreach (TetrominoSpriteData spriteData in spriteData)
        {
            if (spriteData.ID == TetrominoID)
            {
                return spriteData.Images[ComponentID];
            }
        }
        Debug.Log("Couldn't find sprite data for ID " + TetrominoID);

        return null;
    }
}

[System.Serializable]
public struct TetrominoSpriteData
{
    [SerializeField]public int ID;
    [SerializeField]public Sprite[] Images;
}