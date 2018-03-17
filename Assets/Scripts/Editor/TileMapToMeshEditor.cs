using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMapToMesh))]
public class TileMapToMeshEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileMapToMesh instance = (TileMapToMesh)target;

        instance.navMeshSurface = EditorGUILayout.Toggle("Add NavMesh Surface", instance.navMeshSurface);

        if (instance.navMeshSurface)
        {
            instance.navigationArea = EditorGUILayout.TextField("Navigation Area", instance.navigationArea);
        }

        if (GUILayout.Button("Generate Mesh"))
        {
            instance.Generate();
        }
    }
}
