using System.Linq;
using UnityEditor;
using UnityEngine;

public class EmptyParentAtOrigin
{

    [MenuItem("GameObject/Create Empty Parent At Origin", true, 10)]
    static bool ValidateCreateEmptyParentAtOrigin()
    {
        if (Selection.transforms != null && Selection.transforms.Length > 0 && !Selection.transforms.Contains(null))
        {
            return true;
        }
        return false;
    }

    [MenuItem("GameObject/Create Empty Parent At Origin", false, 0)]
    static void CreateEmptyParentAtOrigin(MenuCommand command)
    {
        var selectedObjects = Selection.gameObjects;

        Transform existingParent = null;
        //gets the parent of the current object
        existingParent = (command.context as GameObject).transform.parent;

        //if existing parent is found don't make a new one
        if (existingParent != null)
        {
            return;
        }

        GameObject emptyParent = new GameObject("Empty Parent");

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(emptyParent, "Create " + emptyParent.name);

        // Set the parent of all selected objects to the new empty parent
        foreach (GameObject obj in selectedObjects)
        {
            Undo.SetTransformParent(obj.transform, emptyParent.transform, $"Set object as child of empty");
        }
    }
}
