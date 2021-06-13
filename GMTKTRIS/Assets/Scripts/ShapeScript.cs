using System.Collections.Generic;
using UnityEngine;

public class ShapeScript : MonoBehaviour
{
    public List<SubComponents> components = new List<SubComponents>();

    private bool started = false;

    public void InitStarted()
    {
        started = true;
    }
    
    public void AddComponent(GameObject component, int row)
    {
        components.Add(new SubComponents(component, row));
    }

    public void RemoveComponentsInRow(int row)
    {
        foreach (var subComponents in components)
        {
            if (subComponents.row == row)
            {
                Debug.Log("Piece deleted");
            }
        }
    }

    public struct SubComponents
    {
        public SubComponents(GameObject _go, int _row)
        {
            row = _row;
            go = _go;
        }
        public GameObject go;
        public int row;
    }
}
