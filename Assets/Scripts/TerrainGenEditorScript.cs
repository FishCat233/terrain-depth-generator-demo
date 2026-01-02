using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGen))]
public class TerrainGenEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGen terrainGen = (TerrainGen)target;

        GUILayout.Space(10);
        GUILayout.Label("操作", EditorStyles.boldLabel);

        if (GUILayout.Button("生成地形"))
        {
            terrainGen.GenerateTerrain();
        }
    }
}
