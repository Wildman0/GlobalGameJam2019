using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerCameraBehavior))]
public class PlayerCameraEditor : Editor
{
	private static GUIStyle normalStyle = null;
	private static GUIStyle toggleStyle = null;

	private static bool isMouseLook;
	private static bool isHeadMotion;

	public override void OnInspectorGUI()
	{
		//Reference to the script.
		PlayerCameraBehavior cam = (PlayerCameraBehavior)target;
		EditorGUILayout.Space();

		//Recording.
		Undo.RecordObject(cam, "Player Camera Behavior");

		//Setup the button styles.
		if(normalStyle == null)
		{
			normalStyle = "Button";
			toggleStyle = new GUIStyle(normalStyle);
			toggleStyle.normal.background = toggleStyle.active.background;
		}

		GUILayout.BeginHorizontal();

		//Movement button.
		if(GUILayout.Button("Mouse Look", isMouseLook ? toggleStyle : normalStyle))
		{
			if(!isMouseLook)
			{
				isMouseLook = true;
				isHeadMotion = false;
			}

			else if(isMouseLook) isMouseLook = false;
		}

		if(GUILayout.Button("Head Motion", isHeadMotion ? toggleStyle : normalStyle))
		{
			if(!isHeadMotion)
			{
				isMouseLook = false;
				isHeadMotion = true;
			}

			else if(isHeadMotion) isHeadMotion = false;
		}

		GUILayout.EndHorizontal();

		if(!isMouseLook && !isHeadMotion)
		{
			EditorGUILayout.LabelField("Camera mouse look, and head motion mechanics settings.", EditorStyles.centeredGreyMiniLabel);
		}

		EditorGUILayout.Space();

		if(isMouseLook)
		{
			//Info box.
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("----------------- Variables for the mouse look mechanics. -----------------", EditorStyles.centeredGreyMiniLabel);

			//Mouse look.
			EditorGUILayout.LabelField("General settings for the mouse look sytem.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				cam.playerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", cam.playerCamera, typeof(Camera), true);
				cam.allowRotation = EditorGUILayout.Toggle("Allow Rotation", cam.allowRotation);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Mouse look values.
			if(cam.allowRotation)
			{
				EditorGUILayout.LabelField("Variables for the mouse look system.", EditorStyles.miniBoldLabel);
				EditorGUI.indentLevel++;

					cam.mouseSensitivity = EditorGUILayout.FloatField("Mouse Sensitivity", cam.mouseSensitivity);
					cam.mouseSensitivity = Mathf.Clamp(cam.mouseSensitivity, 0, Mathf.Infinity);
					cam.mouseSmoothScale = EditorGUILayout.Slider("Mouse Smooth Scale", cam.mouseSmoothScale, 0, 1);
					cam.maxLookAngle = EditorGUILayout.Slider("Max Look Angle", cam.maxLookAngle, 0, 90);

				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}
		}

		if(isHeadMotion)
		{
			//Info box.
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("----------------- Variables for the head bobbing and landing motions. -----------------", EditorStyles.centeredGreyMiniLabel);

			//Head bobbing.
			EditorGUILayout.LabelField("General settings for the head motions.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				cam.headTransform = (Transform)EditorGUILayout.ObjectField("Head Transform", cam.headTransform, typeof(Transform), true);
				cam.useHeadBob = EditorGUILayout.Toggle("Use Head Bobbing", cam.useHeadBob);
				cam.useLandingMotion = EditorGUILayout.Toggle("Use Landing Motion", cam.useLandingMotion);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Head bobbing values.
			if(cam.useHeadBob)
			{
				EditorGUILayout.LabelField("Variables for the head bobbing system.", EditorStyles.miniBoldLabel);
				EditorGUI.indentLevel++;

					cam.walkBobbingSpeed = EditorGUILayout.FloatField("Walk Bobbing Speed", cam.walkBobbingSpeed);
					cam.walkBobbingSpeed = Mathf.Clamp(cam.walkBobbingSpeed, 0, Mathf.Infinity);
					cam.runBobbingSpeed = EditorGUILayout.FloatField("Run Bobbing Speed", cam.runBobbingSpeed);
					cam.runBobbingSpeed = Mathf.Clamp(cam.runBobbingSpeed, 0, Mathf.Infinity);
					cam.crouchBobbingSpeed = EditorGUILayout.FloatField("Crouch Bobbing Speed", cam.crouchBobbingSpeed);
					cam.crouchBobbingSpeed = Mathf.Clamp(cam.crouchBobbingSpeed, 0, Mathf.Infinity);
					cam.bobTiltFactor = EditorGUILayout.FloatField("Bob Tilt Factor", cam.bobTiltFactor);
					cam.bobTiltFactor = Mathf.Clamp(cam.bobTiltFactor, 0, Mathf.Infinity);
					cam.bobRotationMultiplier = EditorGUILayout.FloatField("Bob Rotation Multiplier", cam.bobRotationMultiplier);
					cam.bobRotationMultiplier = Mathf.Clamp(cam.bobRotationMultiplier, 0, Mathf.Infinity);
					cam.walkBobAmount = EditorGUILayout.Vector2Field("Walk Bob Amount", cam.walkBobAmount);
					cam.walkBobAmount.x = Mathf.Clamp(cam.walkBobAmount.x, 0, Mathf.Infinity);
					cam.walkBobAmount.y = Mathf.Clamp(cam.walkBobAmount.y, 0, Mathf.Infinity);
					cam.runBobAmount = EditorGUILayout.Vector2Field("Run Bob Amount", cam.runBobAmount);
					cam.runBobAmount.x = Mathf.Clamp(cam.runBobAmount.x, 0, Mathf.Infinity);
					cam.runBobAmount.y = Mathf.Clamp(cam.runBobAmount.y, 0, Mathf.Infinity);
					cam.crouchBobAmount = EditorGUILayout.Vector2Field("Crouch Bob Amount", cam.crouchBobAmount);
					cam.crouchBobAmount.x = Mathf.Clamp(cam.crouchBobAmount.x, 0, Mathf.Infinity);
					cam.crouchBobAmount.y = Mathf.Clamp(cam.crouchBobAmount.y, 0, Mathf.Infinity);

				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}

			//Landing animation values.
			if(cam.useLandingMotion)
			{
				EditorGUILayout.LabelField("Variables for the landing animation.", EditorStyles.miniBoldLabel);
				EditorGUI.indentLevel++;

					cam.minVelocity = EditorGUILayout.FloatField("Min Velocity", cam.minVelocity);
					cam.dropAmount = EditorGUILayout.FloatField("Drop Amount", cam.dropAmount);
					cam.dropAmount = Mathf.Clamp(cam.dropAmount, 0f, Mathf.Infinity);
					cam.dropTiltFactor = EditorGUILayout.FloatField("Drop Tilt Factor", cam.dropTiltFactor);
					cam.dropTiltFactor = Mathf.Clamp(cam.dropTiltFactor, 0f, 90f);
					cam.dropSpeed = EditorGUILayout.FloatField("Drop Speed", cam.dropSpeed);
					cam.dropSpeed = Mathf.Clamp(cam.dropSpeed, 0.01f, Mathf.Infinity);

				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}
		}

		//Making sure that the values are getting saved when entering play mode.
		if(GUI.changed) EditorUtility.SetDirty(cam);
	}
}
