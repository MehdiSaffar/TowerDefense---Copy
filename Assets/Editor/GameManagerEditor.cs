using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
	public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GameManager gameManager = target as GameManager;
        if(GUILayout.Button("Awake")) {
            gameManager.Awake();
        }
    }
}
