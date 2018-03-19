using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

[CustomEditor(typeof(TileMapToMesh))]
public class TileMapToMeshEditor : Editor
{

    SerializedProperty navigationArea;

    void OnEnable()
    {
        navigationArea = serializedObject.FindProperty("navigationArea");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileMapToMesh instance = (TileMapToMesh)target;

        //get a dropDown of layers.
        instance.layerIndex = EditorGUILayout.LayerField("Layer for Objects:", instance.layerIndex);

        //create a toggle using navMeshModifer's bool.
        instance.navMeshModifier = EditorGUILayout.Toggle("Add NavMesh Modifier", instance.navMeshModifier);

        //if navMeshModifer toggle = true, show Area type field.
        if (instance.navMeshModifier)
        {
            //get a dropDown of Navigation Area's
            NavMeshComponentsGUIUtility.AreaPopup("Area Type", navigationArea);
            instance.navigationArea = navigationArea.intValue;
        }

        //create a toggle using meshCollider's bool
        instance.meshCollider = EditorGUILayout.Toggle("Add Mesh Collider", instance.meshCollider);

        //if meshCollider's toggle = true, show physics material field.
        if (instance.meshCollider)
        {
            //add an object field of type "PhysicsMaterial"
            instance.physicsMaterial = EditorGUILayout.ObjectField("Physics Material", instance.physicsMaterial, typeof(PhysicMaterial), true) as PhysicMaterial;
        }

        //create a button that runs Generate() on click.
        if (GUILayout.Button("Generate Mesh"))
        {
            instance.Generate();
        }
    }
}
