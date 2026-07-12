using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LighthouseMatch3.UI
{
    public sealed class BoardCellPool
    {
        private readonly Stack<GameObject> _available = new Stack<GameObject>();

        public GameObject Rent(Transform parent, string name)
        {
            GameObject cell = _available.Count > 0 ? _available.Pop() : CreateCell(name);
            cell.transform.SetParent(parent, false);
            cell.SetActive(true);
            cell.name = name;
            return cell;
        }

        public void ReturnAll(IEnumerable<GameObject> cells)
        {
            foreach (GameObject cell in cells)
            {
                cell.SetActive(false);
                cell.transform.SetParent(null, false);
                _available.Push(cell);
            }
        }

        private static GameObject CreateCell(string name) =>
            new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
    }
}
