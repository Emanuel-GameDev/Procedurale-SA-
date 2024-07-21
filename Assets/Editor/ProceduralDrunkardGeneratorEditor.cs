using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralDrunkardGeneration))]
public class ProceduralDrunkardGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        var proceduralDrunkardGeneration = (ProceduralDrunkardGeneration)target;

        if (GUILayout.Button("Generate Rooms"))
        {
            proceduralDrunkardGeneration.GenerateRoomsFromEditor();
            EditorUtility.SetDirty(proceduralDrunkardGeneration);
        }

        if (GUILayout.Button("Populate Rooms"))
        {
            proceduralDrunkardGeneration.PopulateRooms();
            EditorUtility.SetDirty(proceduralDrunkardGeneration);
        }

        if (GUILayout.Button("Fulfill Rooms"))
        {
            proceduralDrunkardGeneration.FulfillRooms();
            EditorUtility.SetDirty(proceduralDrunkardGeneration);
        }
    }
}
