using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeScript : MonoBehaviour
{
    public List<SubComponents> components = new List<SubComponents>();

    private bool started = false;
    private bool modified = false;

    public void InitStarted()
    {
        started = true;
    }
    
    public void AddComponent(GameObject component, int row)
    {
        components.Add(new SubComponents(component, row));
    }

    public void RemoveComponentsInRow(int row, Sprite singleCat)
    {
        for (int i = 0; i < components.Count; i++)
        {
            var subComponents = components[i];
            if (subComponents.row == row)
            {
                modified = true;
                components.Remove(subComponents);
                i--;
            }
        }

        if (modified)
        {
            foreach (var subComponents in components)
            {
                var go = subComponents.go;
                go.GetComponent<SpriteRenderer>().sprite = singleCat;
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
