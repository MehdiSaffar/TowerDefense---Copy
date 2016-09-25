using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(GUIManager))]
public class GUIMangerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GUIManager guiManager = target as GUIManager;
        if (GUILayout.Button("Awake")) {
            guiManager.Awake();
        }
    }
}
