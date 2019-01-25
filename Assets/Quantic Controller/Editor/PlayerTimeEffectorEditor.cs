using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerTimeEffector))]
public class PlayerTimeEffectorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//Reference to the script.
		PlayerTimeEffector time = (PlayerTimeEffector)target;
		EditorGUILayout.Space();

		//Recording.
		Undo.RecordObject(time, "Player Time Effector");

		//Reference.
		EditorGUILayout.LabelField("General settings of the script.", EditorStyles.miniBoldLabel);
		EditorGUI.indentLevel++;

			EditorGUI.BeginDisabledGroup(time.autoAssign);
			time.motor = (PlayerMotorBehavior)EditorGUILayout.ObjectField("Motor", time.motor, typeof(PlayerMotorBehavior), true);
			EditorGUI.EndDisabledGroup();
			time.autoAssign = EditorGUILayout.Toggle("Auto Assign Script", time.autoAssign);
			time.overrideMouseSensitivity = EditorGUILayout.Toggle("Override Mouse Sensitivity", time.overrideMouseSensitivity);
			time.slowDownAudio = EditorGUILayout.Toggle("Slow Down Sounds", time.slowDownAudio);

		EditorGUI.indentLevel--;
		EditorGUILayout.Space();

		//Time Effector.
		EditorGUILayout.LabelField("Settings for the time overriding method.", EditorStyles.miniBoldLabel);
		EditorGUI.indentLevel++;

			time.slowedTimeScale = EditorGUILayout.FloatField("Slowed Time Scale", time.slowedTimeScale);
			time.slowedTimeScale = Mathf.Clamp(time.slowedTimeScale, 0, 1);
			time.timeChangeSpeed = EditorGUILayout.FloatField("Time Change Speed", time.timeChangeSpeed);
			time.timeChangeSpeed = Mathf.Clamp(time.timeChangeSpeed, 0.01f, Mathf.Infinity);

		EditorGUI.indentLevel--;
		EditorGUILayout.Space();

		//Mouse Sensitivity.
		if(time.overrideMouseSensitivity)
		{
			EditorGUILayout.LabelField("Settings for the mouse sensitivity overriding method.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				time.normalMouseSensitivity = EditorGUILayout.FloatField("Normal Sensitivity", time.normalMouseSensitivity);
				time.normalMouseSensitivity = Mathf.Clamp(time.normalMouseSensitivity, 0, Mathf.Infinity);
				time.slowedMouseSensitivity = EditorGUILayout.FloatField("Slowed Sensitivity", time.slowedMouseSensitivity);
				time.slowedMouseSensitivity = Mathf.Clamp(time.slowedMouseSensitivity, 0, Mathf.Infinity);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}

		//Note.
		EditorGUILayout.HelpBox("To prevent weird looking behaviors, the mouse smooth scale will be set to 0, and the slope detection feature will enabled/disabled during runtime.", MessageType.Info);
	}
}
