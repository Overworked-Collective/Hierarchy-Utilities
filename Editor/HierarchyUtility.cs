using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HierarchyUtility
{
    [MenuItem("GameObject/Utility", true, -1)]

    [MenuItem("GameObject/Utility/Sort Children", true, -1)]
    static bool ValidateSortChildren()
    {
        if (Selection.transforms != null && Selection.transforms.Length == 1 && !Selection.transforms.Contains(null))
        {
            return true;
        }
        return false;   
    }

    [MenuItem("GameObject/Utility/Sort Children", false, -1)]
    static void SortChildren(MenuCommand command)
    {
        Transform parent = (command.context as GameObject).transform;
        Undo.RegisterChildrenOrderUndo(parent, "Sort children in object");

        List<Transform> children = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            children.Add(parent.GetChild(i));
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform selectedTransformMin = children[0];

            for (int o = 0; o < children.Count; o++)
            {
                int subStringIndex = 0;

                while(CharacterSum(selectedTransformMin.name.Substring(subStringIndex, 1).ToLower()) == 
                      CharacterSum(children[o].name.Substring(subStringIndex, 1).ToLower()))
                {
                    if (subStringIndex >= children[o].name.Length - 1 || subStringIndex >= selectedTransformMin.name.Length - 1)
                    {
                        break;
                    }
                    subStringIndex++;
                }

                int childValue = CharacterSum(children[o].name.Substring(subStringIndex, 1).ToLower());
                int selectedValue = CharacterSum(selectedTransformMin.name.Substring(subStringIndex, 1).ToLower());

                if (selectedValue > childValue)
                {
                    //found smaller child
                    selectedTransformMin = children[o];
                    continue;
                } 
                else if (selectedValue < childValue)
                {
                    //found larger child
                    continue;
                }
                
                if (selectedTransformMin.name.Length > children[o].name.Length)
                {
                    //last character checked was the same so shorter string is selected
                    selectedTransformMin = children[o];
                    continue;
                }
            }

            children.Remove(selectedTransformMin);

            selectedTransformMin.SetSiblingIndex(i);
        }
    }

    [MenuItem("GameObject/Utility/Print Child Count", true, 1)]
    static bool ValidatePrintChildCount()
    {
        if (Selection.transforms != null && Selection.transforms.Length == 1 && !Selection.transforms.Contains(null))
        {
            return true;
        }
        return false;
    }

    [MenuItem("GameObject/Utility/Print Child Count", false, 1)]
    static void PrintChildCount(MenuCommand command)
    {
        Debug.Log((command.context as GameObject).transform.childCount);
    }

    [MenuItem("GameObject/Utility/Reset Parent To Origin", true, 10)]
    static bool ValidateResetParentTransform()
    {
        if (Selection.transforms != null && Selection.transforms.Length == 1 && !Selection.transforms.Contains(null))
        {
            return true;
        }
        return false;
    }

    [MenuItem("GameObject/Utility/Reset Parent To Origin", false, 10)]
    static void ResetParentTransform(MenuCommand command)
    {
        Transform transform = (command.context as GameObject).transform;

        List<Transform> children = new List<Transform>();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            children.Add(transform.GetChild(i));
            //transform.GetChild(i).transform.SetParent(null);
            Undo.SetTransformParent(transform.GetChild(i), null, $"Set object as child of null");
        }

        Undo.RecordObject(transform, $"Recording parent transform");
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        foreach (Transform child in children)
        {
            //child.SetParent(transform);
            Undo.SetTransformParent(child, transform, $"Reset object as child of parent");
        }
    }

    [MenuItem("GameObject/Utility/Reset Parent To Center", true, 10)]
    static bool ValidateCenterParentTransform()
    {
        if (Selection.transforms != null && Selection.transforms.Length == 1 && !Selection.transforms.Contains(null))
        {
            return true;
        }
        return false;
    }

    [MenuItem("GameObject/Utility/Reset Parent To Center", false, 10)]
    static void CenterParentTransform(MenuCommand command)
    {
        Transform transform = (command.context as GameObject).transform;

        List<Transform> children = new List<Transform>();
        Vector3 _averagePosition = Vector3.zero;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            children.Add(transform.GetChild(i));
            //transform.GetChild(i).transform.SetParent(null);
            _averagePosition += transform.GetChild(i).position;
            Undo.SetTransformParent(transform.GetChild(i), null, $"Set object as child of null");
        }
        _averagePosition *= 1f / (float)children.Count;

        Undo.RecordObject(transform, $"Recording parent transform");
        transform.position = _averagePosition;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        foreach (Transform child in children)
        {
            //child.SetParent(transform);
            Undo.SetTransformParent(child, transform, $"Reset object as child of parent");
        }
    }

    static int CharacterSum(string str)
    {
        int sum = 0;
        for (int i = 0; i < str.Length; i++)
        {
            sum += str[i];
        }
        return sum;
    }
}
