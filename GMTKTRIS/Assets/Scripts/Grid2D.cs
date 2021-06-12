using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid2D<T> : IEnumerable<KeyValuePair<Vector2Int, T>> where T : class
{
    private Dictionary<Vector2Int, T> _positionToItem = new Dictionary<Vector2Int, T>();

    public bool TryGetAtPosition(Vector2Int pos, out T item) => _positionToItem.TryGetValue(pos, out item);
    public bool IsPositionEmpty(Vector2Int pos) => !_positionToItem.ContainsKey(pos);

    public bool TryInsertAtPosition(T item, Vector2Int pos)
    {
        if (!IsPositionEmpty(pos)) return false;

        if (_positionToItem.ContainsValue(item))
            _positionToItem.Remove(_positionToItem.First(x => x.Value == item).Key);
        _positionToItem.Add(pos, item);
        return true;
    }

    public bool RemoveItemAtPosition(Vector2Int pos) => _positionToItem.Remove(pos);

    public void InsertRow(int rowIdx, Action<T, Vector2Int> posChangeCallback)
    {
        _positionToItem.Where(pair => pair.Key.y >= rowIdx).OrderByDescending(pair => pair.Key.y).ToList().ForEach(
            item =>
            {
                _positionToItem.Remove(item.Key);
                var newPos = new Vector2Int(item.Key.x, item.Key.y + 1);
                _positionToItem.Add(newPos, item.Value);
                posChangeCallback.Invoke(item.Value, newPos);
            });
    }

    public void InsertColumn(int columnIdx, Action<T, Vector2Int> posChangeCallback)
    {
        _positionToItem.Where(pair => pair.Key.x >= columnIdx).OrderByDescending(pair => pair.Key.x).ToList()
            .ForEach(item =>
            {
                _positionToItem.Remove(item.Key);
                var newPos = new Vector2Int(item.Key.x + 1, item.Key.y);
                _positionToItem.Add(newPos, item.Value);
                posChangeCallback.Invoke(item.Value, newPos);
            });
    }

    public bool IsColumnEmpty(int columnIdx) => _positionToItem.All(pair => pair.Key.x != columnIdx);
    public bool IsRowEmpty(int rowIdx) => _positionToItem.All(pair => pair.Key.y != rowIdx);

    public bool RemoveColumnIfEmpty(int columnIdx, Action<T, Vector2Int> posChangeCallback)
    {
        if (!IsColumnEmpty(columnIdx)) return false;
        _positionToItem.Where(pair => pair.Key.x >= columnIdx).OrderBy(pair => pair.Key.x).ToList().ForEach(item =>
        {
            _positionToItem.Remove(item.Key);
            var newPos = new Vector2Int(item.Key.x - 1, item.Key.y);
            _positionToItem.Add(newPos, item.Value);
            posChangeCallback.Invoke(item.Value, newPos);
        });
        return true;
    }

    public bool RemoveRowIfEmpty(int rowIdx, Action<T, Vector2Int> posChangeCallback)
    {
        if (!IsRowEmpty(rowIdx)) return false;
        _positionToItem.Where(pair => pair.Key.y >= rowIdx).OrderBy(pair => pair.Key.y).ToList().ForEach(item =>
        {
            _positionToItem.Remove(item.Key);
            var newPos = new Vector2Int(item.Key.x, item.Key.y - 1);
            _positionToItem.Add(newPos, item.Value);
            posChangeCallback.Invoke(item.Value, newPos);
        });
        return true;
    }

    public IEnumerator<KeyValuePair<Vector2Int, T>> GetEnumerator()
    {
        foreach (var kvp in _positionToItem) yield return kvp;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}