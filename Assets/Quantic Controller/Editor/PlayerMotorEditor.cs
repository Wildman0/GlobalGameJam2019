using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerMotorBehavior))]
public class PlayerMotorEditor : Editor
{
	private static GUIStyle normalStyle = null;
	private static GUIStyle toggleStyle = null;

	private static bool isMovement;
	private static bool isSounds;
	private static bool isStates;

	public override void OnInspectorGUI()
	{
		//Reference to the script.
		PlayerMotorBehavior motor = (PlayerMotorBehavior)target;
		EditorGUILayout.Space();

		//Recording.
		Undo.RecordObject(motor, "Player Motor Behavior");

		//Setup the button styles.
		if(normalStyle == null)
		{
			normalStyle = "Button";
			toggleStyle = new GUIStyle(normalStyle);
			toggleStyle.normal.background = toggleStyle.active.background;
		}

		GUILayout.BeginHorizontal();

		//Movement button.
		if(GUILayout.Button("Movement", isMovement ? toggleStyle : normalStyle))
		{
			if(!isMovement)
			{
				isMovement = true;
				isSounds = false;
				isStates = false;
			}

			else if(isMovement) isMovement = false;
		}

		//Sounds button.
		if(GUILayout.Button("Sounds", isSounds ? toggleStyle : normalStyle))
		{
			if(!isSounds)
			{
				isMovement = false;
				isSounds = true;
				isStates = false;
			}

			else if(isSounds) isSounds = false;
		}

		//States button.
		if(GUILayout.Button("States", isStates ? toggleStyle : normalStyle))
		{
			if(!isStates)
			{
				isMovement = false;
				isSounds = false;
				isStates = true;
			}

			else if(isStates) isStates = false;
		}

		GUILayout.EndHorizontal();

		if(!isMovement && !isSounds && !isStates)
		{
			EditorGUILayout.LabelField("Character's movement, gravity, and footstep mechanics settings.", EditorStyles.centeredGreyMiniLabel);
		}

		EditorGUILayout.Space();

		//Movement settings.
		if(isMovement)
		{
			//Info box.
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("----------------- Variables for the character's movement. -----------------", EditorStyles.centeredGreyMiniLabel);

			//Movement and jumping.
			EditorGUILayout.LabelField("General settings for the movement calculation.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.moveSpeed = EditorGUILayout.FloatField("Move Speed", motor.moveSpeed);
				motor.moveSpeed = Mathf.Clamp(motor.moveSpeed, 0, Mathf.Infinity);
				motor.runSpeed = EditorGUILayout.FloatField("Run Speed", motor.runSpeed);
				motor.runSpeed = Mathf.Clamp(motor.runSpeed, 0, Mathf.Infinity);
				motor.crouchSpeed = EditorGUILayout.FloatField("Crouch Speed", motor.crouchSpeed);
				motor.crouchSpeed = Mathf.Clamp(motor.crouchSpeed, 0, Mathf.Infinity);
				motor.moveSmoothScale = EditorGUILayout.Slider("Move Smooth Scale", motor.moveSmoothScale, 0, 1);
				EditorGUI.BeginDisabledGroup(!motor.useSlopeSliding);
				motor.slideSpeed = EditorGUILayout.Slider("Slide Speed", motor.slideSpeed, 0.01f, 15f);
				EditorGUI.EndDisabledGroup();
				motor.airControlPercent = EditorGUILayout.Slider("Air Control Percent", motor.airControlPercent, 0, 1);
				EditorGUILayout.HelpBox("Please note that the Air Control Percentage feature is currently experimental.", MessageType.Info);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Ascending and descending.
			EditorGUILayout.LabelField("Ascending and descending.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.ascendKey = (KeyCode)EditorGUILayout.EnumPopup("Ascend Key", motor.ascendKey);
				motor.descendKey = (KeyCode)EditorGUILayout.EnumPopup("Descend Key", motor.descendKey);
				motor.ascendSpeed = EditorGUILayout.FloatField("Ascend Speed", motor.ascendSpeed);
				motor.ascendSpeed = Mathf.Clamp(motor.ascendSpeed, 0, Mathf.Infinity);
				motor.descendSpeed = EditorGUILayout.FloatField("Descend Speed", motor.descendSpeed);
				motor.descendSpeed = Mathf.Clamp(motor.descendSpeed, 0, Mathf.Infinity);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Jumping.
			EditorGUILayout.LabelField("Jumping mechanics.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.jumpMethod = (PlayerMotorBehavior.JumpMethod)EditorGUILayout.EnumPopup("Jump Method", motor.jumpMethod);
				motor.jumpForce = EditorGUILayout.FloatField("Jump Force", motor.jumpForce);
				motor.jumpForce = Mathf.Clamp(motor.jumpForce, 0, Mathf.Infinity);
				motor.jumpsInRow = EditorGUILayout.IntField("Jumps In Row", motor.jumpsInRow);
				motor.jumpsInRow = Mathf.Clamp(motor.jumpsInRow, 1, 100);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Crouching.
			EditorGUILayout.LabelField("Crouching mechanics.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.crouchKey = (KeyCode)EditorGUILayout.EnumPopup("Crouch Key", motor.crouchKey);
				EditorGUIUtility.labelWidth = 0f;
				SerializedProperty crouchInteractLayer = serializedObject.FindProperty ("crouchInteractLayer");
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(crouchInteractLayer, true);
				if(EditorGUI.EndChangeCheck())
					serializedObject.ApplyModifiedProperties();
				EditorGUIUtility.labelWidth = 0f;
				motor.crouchHeight = EditorGUILayout.FloatField("Crouch Height", motor.crouchHeight);
				motor.crouchHeight = Mathf.Clamp(motor.crouchHeight, 0, Mathf.Infinity);
				motor.crouchSmoothScale = EditorGUILayout.Slider("Crouch Smooth Scale", motor.crouchSmoothScale, 0, 1);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Info box.
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("----------------- Variables for the gravity affecting the character. -----------------", EditorStyles.centeredGreyMiniLabel);

			//Gravity.
			EditorGUILayout.LabelField("General settings for gravity calculation.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.gravityAmount = EditorGUILayout.FloatField("Gravity Amount", motor.gravityAmount);
				motor.constantGravityForce = EditorGUILayout.FloatField("Constant Gravity Force", motor.constantGravityForce);
				motor.gravityScale = EditorGUILayout.Slider("Gravity Scale", motor.gravityScale, 0, 1);
				EditorGUILayout.HelpBox("Constant gravity force can only be applied in zero gravity.", MessageType.Info);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Gravity visualization.
			if(motor.drawVisualization)
			{
				EditorGUILayout.LabelField("Visualization of the gravity.", EditorStyles.miniBoldLabel);
				EditorGUI.indentLevel++;

				Rect gravityRect = EditorGUILayout.GetControlRect(false, 0);
				if(motor.gravityScale > 0)
					EditorGUI.ProgressBar(new Rect(gravityRect.position.x +20, gravityRect.position.y, gravityRect.size.x -25, 20), motor.gravityScale, "Gravity applied " + Mathf.Round(motor.gravityAmount * motor.gravityScale) + " with gravity percentage of " + Mathf.Round(motor.gravityScale * 100) + "%");

				else if(motor.gravityScale == 0)
					EditorGUI.ProgressBar(new Rect(gravityRect.position.x +20, gravityRect.position.y, gravityRect.size.x -25, 20), motor.gravityScale, "Zero gravity. Constant gravity force can be applied.");

				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();

				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
			}
		}

		//Player sounds.
		if(isSounds)
		{
			//Info box.
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("----------------- Variables for the character's footstep system. -----------------", EditorStyles.centeredGreyMiniLabel);

			//Footstep array info.
			EditorGUILayout.LabelField("This array contains all possible ground types and footstep sounds.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

			//Footstep array.
			EditorGUIUtility.labelWidth = 0f;
			SerializedProperty footsteps = serializedObject.FindProperty ("footstepTypes");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(footsteps, true);
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
			EditorGUIUtility.labelWidth = 0f;
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Footstep speed.
			EditorGUILayout.LabelField("Time between each footstep for each move state.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.walkStepTime = EditorGUILayout.FloatField("Walk Step Time", motor.walkStepTime);
				motor.walkStepTime = Mathf.Clamp(motor.walkStepTime, 0.01f, Mathf.Infinity);
				motor.runStepTime = EditorGUILayout.FloatField("Run Step Time", motor.runStepTime);
				motor.runStepTime = Mathf.Clamp(motor.runStepTime, 0.01f, Mathf.Infinity);
				motor.crouchStepTime = EditorGUILayout.FloatField("Crouch Step Time", motor.crouchStepTime);
				motor.crouchStepTime = Mathf.Clamp(motor.crouchStepTime, 0.01f, Mathf.Infinity);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Footstep volume multiplier.
			EditorGUILayout.LabelField("Footstep volume multiplier for each move state.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.walkStepVolume = EditorGUILayout.Slider("Walk Step Volume", motor.walkStepVolume, 0, 1);
				motor.runStepVolume = EditorGUILayout.Slider("Run Step Volume", motor.runStepVolume, 0, 1);
				motor.crouchStepVolume = EditorGUILayout.Slider("Crouch Step Volume", motor.crouchStepVolume, 0, 1);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Jump sound.
			EditorGUILayout.LabelField("----------------- Variables for the jumping and landing sounds. -----------------", EditorStyles.centeredGreyMiniLabel);
			EditorGUILayout.LabelField("Player jumping sound. (Disabled if no sound attached)", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.jumpSound = (AudioClip)EditorGUILayout.ObjectField("Jump Sound", motor.jumpSound, typeof(AudioClip), false);
				motor.jumpVolume = EditorGUILayout.Slider("Jump Volume", motor.jumpVolume, 0, 1);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Landing sound.
			if(motor.landingSoundBase == PlayerMotorBehavior.LandingSoundBase.specificSound)
				EditorGUILayout.LabelField("Player landing sound. (Disabled if no sound attached)", EditorStyles.miniBoldLabel);

			else if(motor.landingSoundBase == PlayerMotorBehavior.LandingSoundBase.currentGroundType)
				EditorGUILayout.LabelField("Player landing sound.", EditorStyles.miniBoldLabel);
			
			EditorGUI.indentLevel++;

				motor.landingSoundBase = (PlayerMotorBehavior.LandingSoundBase)EditorGUILayout.EnumPopup("Landing Sound Base", motor.landingSoundBase);
				motor.minVelocity = EditorGUILayout.FloatField("Min Velocity", motor.minVelocity);
				motor.landingVolume = EditorGUILayout.Slider("Landing Volume", motor.landingVolume, 0, 1);
				motor.footstepDelay = EditorGUILayout.Slider("Footstep Delay", motor.footstepDelay, 0, 1);

				if(motor.landingSoundBase == PlayerMotorBehavior.LandingSoundBase.specificSound)
					motor.landingSound = (AudioClip)EditorGUILayout.ObjectField("Landing Sound", motor.landingSound, typeof(AudioClip), false);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}

		//Active and passive states.
		if(isStates)
		{
			//Info box.
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("----------------- Character's active states. These states can be modified. -----------------", EditorStyles.centeredGreyMiniLabel);

			//Slope state.
			EditorGUILayout.LabelField("Enabling these will remove problems related to slopes in unity.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.useSlopeSliding = EditorGUILayout.Toggle("Use Slope Sliding", motor.useSlopeSliding);
				motor.useSlopeDetection = EditorGUILayout.Toggle("Use Slope Detection", motor.useSlopeDetection);
				if(motor.useSlopeDetection) motor.slopeDetectionRange = EditorGUILayout.Slider("Slope Detection Range", motor.slopeDetectionRange, 0.01f, 0.5f);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Debug states.
			EditorGUILayout.LabelField("These can help you in testing different things while developing.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.monitorVariables = EditorGUILayout.Toggle("Monitor Variables", motor.monitorVariables);
				motor.drawVisualization = EditorGUILayout.Toggle("Draw Visualization", motor.drawVisualization);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Active states.
			EditorGUILayout.LabelField("These states are affecting the player's movement seamlessly.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.canMove = EditorGUILayout.Toggle("Can Move", motor.canMove);
				motor.canRun = EditorGUILayout.Toggle("Can Run", motor.canRun);
				motor.canCrouch = EditorGUILayout.Toggle("Can Crouch", motor.canCrouch);
				motor.canJump = EditorGUILayout.Toggle("Can Jump", motor.canJump);
				motor.canAscend = EditorGUILayout.Toggle("Can Ascend", motor.canAscend);
				motor.canDescend = EditorGUILayout.Toggle("Can Descend", motor.canDescend);

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Ghost mode.
			EditorGUILayout.LabelField("Enabling this will allow you to go through walls while in zero gravity.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				motor.ghostMode = EditorGUILayout.Toggle("Ghost Mode", motor.ghostMode);
				if(motor.ghostMode)
				{
					motor.ignoredLayer = EditorGUILayout.LayerField("Ignored Layer", motor.ignoredLayer);
					EditorGUILayout.HelpBox("If you are using triggers, you might receive OnTriggerExit and OnTriggerEnter messages in response if you enable or disable this feature while the game is running.", MessageType.Info);
				}

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();

			//Info box.
			EditorGUILayout.LabelField("----------------- Character's passive states, updated by the system. -----------------", EditorStyles.centeredGreyMiniLabel);
			EditorGUILayout.LabelField("These states are updated by the system, use as read only.", EditorStyles.miniBoldLabel);
			EditorGUI.indentLevel++;

				//Passive state info.
				if(!Application.isPlaying)
				{
					EditorGUILayout.HelpBox("You have to start the game first to see the states.", MessageType.Info);
				}

				//Passive states showing.
				if(Application.isPlaying)
				{
					EditorGUILayout.Toggle("Is Ghosting", motor.isGhosting, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Zero Gravity", motor.isZeroGravity, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Grounded", motor.isGrounded, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Sliding", motor.isSliding, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Moving", motor.isMoving, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Running", motor.isRunning, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Crouching", motor.isCrouching, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Ascending", motor.isAscending, EditorStyles.radioButton);
					EditorGUILayout.Toggle("Is Descending", motor.isDescending, EditorStyles.radioButton);
				}

			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}

		//Making sure that the values are getting saved when entering play mode.
		if(GUI.changed) EditorUtility.SetDirty(motor);
	}
}
