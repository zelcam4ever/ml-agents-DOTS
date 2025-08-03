using UnityEditor;
using UnityEngine;

public class BatchDuplicate : Editor
{
    private const int Columns = 10;
    private const int Rows = 10;
    private const float Spacing = 25; // The distance between each object

    // Defines the menu item's path and the method to call.
    [MenuItem("GameObject/Duplicate in Grid (10x10)", false, 0)]
    private static void DuplicateObjectInGrid()
    {
        // Get the currently selected GameObject.
        GameObject original = Selection.activeGameObject;

        Vector3 originPoint = original.transform.position;
        GameObject parentObject = new GameObject($"{original.name} Grid Duplicates");
        Undo.RegisterCreatedObjectUndo(parentObject, "Create Grid Parent");


        for (int x = 0; x < Columns; x++)
        {
            for (int z = 0; z < Rows; z++)
            {
                // Create a copy.
                GameObject copy = Instantiate(original);
                copy.transform.SetParent(parentObject.transform);

                // Give it a unique name.
                copy.name = $"{original.name}_({x},{z})";

                // Calculate the position in the grid.
                Vector3 position = new Vector3(x * Spacing, 0, z * Spacing);
                copy.transform.position = originPoint + position;

                // Register the creation so it can be undone.
                Undo.RegisterCreatedObjectUndo(copy, "Grid Duplicate");
            }
        }
    }

    // This method enables the menu item only if a GameObject is selected.
    [MenuItem("GameObject/Duplicate in Grid (10x10)", true)]
    private static bool ValidateDuplicateObject()
    {
        return Selection.activeGameObject != null;
    }
}