using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerStaminaBehavior))]
public class PlayerStaminaEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//Reference to the script.
		PlayerStaminaBehavior stamina = (PlayerStaminaBehavior)target;
		EditorGUILayout.Space();

		//Recording.
		Undo.RecordObject(stamina, "Player Stamina Behavior");

		//Reference.
		EditorGUILayout.LabelField("General settings of the script.", EditorStyles.miniBoldLabel);
		EditorGUI.indentLevel++;

		EditorGUI.BeginDisabledGroup(stamina.autoAssign);
		stamina.motor = (PlayerMotorBehavior)EditorGUILayout.ObjectField("Motor", stamina.motor, typeof(PlayerMotorBehavior), true);
		EditorGUI.EndDisabledGroup();
		stamina.autoAssign = EditorGUILayout.Toggle("Auto Assign Script", stamina.autoAssign);
		stamina.drawDefaultStaminaBar = EditorGUILayout.Toggle("Draw Default Stamina Bar", stamina.drawDefaultStaminaBar);

		EditorGUI.indentLevel--;
		EditorGUILayout.Space();

		//Stamina.
		EditorGUILayout.LabelField("Settings for the stamina system.", EditorStyles.miniBoldLabel);
		EditorGUI.indentLevel++;

			stamina.maxStamina = EditorGUILayout.FloatField("Max Stamina", stamina.maxStamina);
			stamina.maxStamina = Mathf.Clamp(stamina.maxStamina, 0, Mathf.Infinity);
			stamina.currentStamina = EditorGUILayout.FloatField("Current Stamina", stamina.currentStamina);
			stamina.currentStamina = Mathf.Clamp(stamina.currentStamina, 0, stamina.maxStamina);
			stamina.drainSpeed = EditorGUILayout.FloatField("Drain Speed", stamina.drainSpeed);
			stamina.drainSpeed = Mathf.Clamp(stamina.drainSpeed, 0, Mathf.Infinity);
			stamina.regenSpeed = EditorGUILayout.FloatField("Regen Speed", stamina.regenSpeed);
			stamina.regenSpeed = Mathf.Clamp(stamina.regenSpeed, 0, Mathf.Infinity);
			stamina.minStaminaAfterExhaust = EditorGUILayout.FloatField("Min Stamina After Exhaust", stamina.minStaminaAfterExhaust);
			stamina.minStaminaAfterExhaust = Mathf.Clamp(stamina.minStaminaAfterExhaust, 0, stamina.maxStamina);

		EditorGUI.indentLevel--;
		EditorGUILayout.Space();

			//Stamina Percent.
			EditorGUILayout.LabelField("Current stamina in percentage. Use this to calculate UI progress.", EditorStyles.miniBoldLabel);
			Rect staminaRect = EditorGUILayout.GetControlRect(false, 0);
			EditorGUI.ProgressBar(new Rect(staminaRect.position.x +20, staminaRect.position.y, staminaRect.size.x -25, 20), stamina.currentStamina / stamina.maxStamina, "Current Stamina: " + (stamina.currentStamina / stamina.maxStamina * 100).ToString("0.0") + "%");

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}
}
