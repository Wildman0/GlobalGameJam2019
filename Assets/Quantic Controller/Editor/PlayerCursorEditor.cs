using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCursorManager))]
public class PlayerCursorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//Reference to the script.
		PlayerCursorManager cursor = (PlayerCursorManager)target;
		EditorGUILayout.Space();

		//Recording.
		Undo.RecordObject(cursor, "Player Cursor Manager");

		//Cursor.
		cursor.toggleKey = (KeyCode)EditorGUILayout.EnumPopup("Toggle Key", cursor.toggleKey);
		EditorGUILayout.Toggle("Is Locked", cursor.isLocked, EditorStyles.radioButton);
		EditorGUILayout.HelpBox("Keep in mind that locking the cursor may not work inside the editor, but it will work when you build the game.", MessageType.Info);

		//Making sure that the values are getting saved when entering play mode.
		if(GUI.changed) EditorUtility.SetDirty(cursor);
	}
}
