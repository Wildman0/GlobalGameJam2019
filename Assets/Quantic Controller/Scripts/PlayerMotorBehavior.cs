using UnityEngine;
using System.Collections;

[System.Serializable]
public class FootstepType
{
	public string tagName = "New Footstep Type";
	[Range(0, 1)] public float volume = 1;
	public AudioClip[] sounds;
}

[AddComponentMenu("Quantic Controller/Player Motor Behavior")]

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerCameraBehavior))]
[RequireComponent(typeof(AudioSource))]

public class PlayerMotorBehavior : MonoBehaviour
{
	//Movement settings.
	public float moveSpeed = 5;
	public float runSpeed = 10;
	public float crouchSpeed = 2.5f;
	[Range(0, 1)] public float moveSmoothScale = 0.1f;
	[Range(0, 1)] public float airControlPercent = 0.7f;
	[Range(0.01f, 0.5f)] public float slopeDetectionRange = 0.3f;
	[Range(0.01f, 15f)] public float slideSpeed = 6f;
	public bool canMove = true;
	public bool canRun = true;
	public bool canCrouch = true;
	public bool useSlopeDetection = true;
	public bool useSlopeSliding = true;

	//Ascending and descending.
	public KeyCode ascendKey = KeyCode.Space;
	public KeyCode descendKey = KeyCode.LeftShift;
	public float ascendSpeed = 5;
	public float descendSpeed = 5;
	public bool canAscend = true;
	public bool canDescend = true;

	//Jumping.
	public enum JumpMethod {continuous, single};
	public JumpMethod jumpMethod;
	public float jumpForce = 10;
	public int jumpsInRow = 1;
	public bool canJump = true;

	//Crouching
	public KeyCode crouchKey = KeyCode.C;
	public LayerMask crouchInteractLayer;
	public float crouchHeight = 1;
	[Range(0, 1)] public float crouchSmoothScale = 0.2f;

	//Gravity.
	public float gravityAmount = -20;
	[Range(0, 1)] public float gravityScale = 1;
	public float constantGravityForce = 0;

	//Footsteps.
	public FootstepType[] footstepTypes;
	public float walkStepTime = 0.5f;
	public float runStepTime = 0.3f;
	public float crouchStepTime = 0.85f;
	[Range(0, 1)] public float walkStepVolume = 0.7f;
	[Range(0, 1)] public float runStepVolume = 1;
	[Range(0, 1)] public float crouchStepVolume = 0.4f;

	//Jump Sound.
	public AudioClip jumpSound;
	[Range(0, 1)] public float jumpVolume = 1;

	//Land sound.
	public enum LandingSoundBase {currentGroundType, specificSound}
	public LandingSoundBase landingSoundBase;
	public AudioClip landingSound;
	[Range(0, 1)] public float landingVolume = 1;
	[Range(0, 1)] public float footstepDelay = 0.3f;
	public float minVelocity = -5;

	//Ghost mode.
	public int ignoredLayer;
	public bool ghostMode;

	//Passive states.
	public bool isGhosting = false;
	public bool isZeroGravity = false;
	public bool isGrounded = false;
	public bool isSliding = false;
	public bool isMoving = false;
	public bool isRunning = false;
	public bool isCrouching = false;
	public bool isAscending = false;
	public bool isDescending = false;

	//Debug mode.
	public bool monitorVariables = true;
	public bool drawVisualization = true;

	private CharacterController controller;
	private PlayerCameraBehavior cam;

	private int jumpsLeft;
	private Vector3 hitNormal;
	private Vector3 moveDirection;

	private Vector2 speedSmoothVelocity;
	private Vector2 currentSpeed;
	private Vector2 targetSpeed;

	private float hitAngle;
	private float crouchSmoothVelocity;
	private float defaultCrouchHeight;
	private float turnSmoothVelocity;
	private float ZG_VelocityY;
	private float velocityY;

	private AudioClip currentStepSound;
	private float currentStepVolume;
	private float stepVolume;
	private float footstepTimer;
	private float footstepMagnitude;
	private bool canStandUp;
	private bool playedLanding;

	private void Start()
	{
		//Initialization.
		controller = GetComponent<CharacterController>();
		cam = GetComponent<PlayerCameraBehavior>();

		//Set the default crouch height.
		defaultCrouchHeight = controller.height;
	}

	private bool CustomGroundState()
	{
		//If we are in zero gravity, this can't be applied since we would stuck to the ground.
		if(isZeroGravity) return controller.isGrounded;

		//If we are already grounded, there is no need to adjust the character's position.
		if(controller.isGrounded) return true;

		//Get the bottom of the character, and use it as the starting point of the ray.
		Vector3 bottom = transform.position - Vector3.up * controller.height /2;
		RaycastHit hit;

		//If we are not grounded, but the ground is close to use, move the character down and update the state.
		if(Physics.Raycast(bottom, Vector3.down, out hit, slopeDetectionRange))
		{
			//If the velocity is negative, we are not jumping and the position should be fixed.
			if(controller.velocity.y < 0)
			{
				if(!isSliding) controller.Move(new Vector3(0, -hit.distance, 0));
				return true;
			}

			//We are just trying to jump, no need to fix the character's position.
			else return false;
		}

		//The ground is too far, no need to fix the character's position.
		else return false;
	}

	private bool CustomSlidingState()
	{
		//If we are in zero gravity, sliding can't be applied.
		if(isZeroGravity) return false;

		//Get the bottom of the character, and use it as the starting point of the ray.
		Vector3 center = transform.position;
		RaycastHit hit;

		//If we are standing on a steep slope, update the sliding state.
		if(Physics.SphereCast(center, controller.radius -0.05f, Vector3.down, out hit, controller.height /2 + 1f))
		{
			//Get the hit normal and calculate the angle.
			hitNormal = hit.normal;
			hitAngle = Vector3.Angle(Vector3.up, hitNormal);

			//Update the sliding state.
			if(hitAngle <= controller.slopeLimit) return false;
			else return true;
		}

		//We are not sliding.
		else return false;
	}

	private float ControlledSmoothScale()
	{
		//If we are grouned or in zero gravity or the percentage is 100%, this won't change anything so we return the base value.
		if(isZeroGravity || isGrounded || airControlPercent == 1) return moveSmoothScale;

		//If the percentage is 0%, we don't have any control over the character.
		if(airControlPercent == 0) return float.MaxValue;

		//If neither of the states above are true, we return the calculated modified smooth scale.
		return moveSmoothScale / airControlPercent;
	}

	private void Update()
	{
		//Running different methods every frame.
		RefreshStates();
		PlayFootsteps();
		CalculateGhostMode();
		CalculateMovement();
	}

	public void UpdateGravity(float newGravityScale, float newMoveSmoothScale, float additionalGravityForce)
	{
		//Clamp the smoothing values.
		newGravityScale = Mathf.Clamp(newGravityScale, 0, 1);
		newMoveSmoothScale = Mathf.Clamp(newMoveSmoothScale, 0, 1);

		//Apply the changes.
		gravityScale = newGravityScale;
		moveSmoothScale = newMoveSmoothScale;
		velocityY += additionalGravityForce;
	}

	private void CalculateMovement()
	{
		//Store the horizontal and vertical input.
		Vector2 inputDir;

		//Check if the character is sliding and get the input.
		inputDir = isSliding ? Vector2.zero : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

		//Adjusting the movement speed.
		if(canMove)
		{
			if(isRunning)
			{
				//Running.
				targetSpeed.x = runSpeed * inputDir.x;
				targetSpeed.y = runSpeed * inputDir.y;
				footstepMagnitude = runSpeed;
			}

			if(isCrouching)
			{
				//Crouching.
				targetSpeed.x = crouchSpeed * inputDir.x;
				targetSpeed.y = crouchSpeed * inputDir.y;
				footstepMagnitude = crouchSpeed;
			}

			if(!isRunning && !isCrouching)
			{
				//Walking.
				targetSpeed.x = moveSpeed * inputDir.x;
				targetSpeed.y = moveSpeed * inputDir.y;
				footstepMagnitude = moveSpeed;
			}
		}

		//We cannot move, adjust movement speed to 0.
		else if(!canMove)
		{
			targetSpeed.x = 0;
			targetSpeed.y = 0;
		}

		//Smooth out the movement speed.
		currentSpeed.x = Mathf.SmoothDamp(currentSpeed.x, targetSpeed.x, ref speedSmoothVelocity.x, ControlledSmoothScale());
		currentSpeed.y = Mathf.SmoothDamp(currentSpeed.y, targetSpeed.y, ref speedSmoothVelocity.y, ControlledSmoothScale());

		//Apply gravity.
		velocityY += Time.deltaTime * (gravityAmount * gravityScale);

		//Jumping mechanics.
		if(canJump && jumpsLeft > 0 && !isZeroGravity && canMove && !isSliding)
		{
			//Single jumping method.
			if(jumpMethod == JumpMethod.single && Input.GetKeyDown(KeyCode.Space))
			{
				//Set the velocity.
				velocityY = jumpForce;

				//Landing sound reset.
				playedLanding = false;

				//Jump sound.
				if(jumpSound != null)
					GetComponent<AudioSource>().PlayOneShot(jumpSound, jumpVolume);

				//Update the amount of jumps left.
				jumpsLeft--;
			}

			//Continuous jumping method.
			if(jumpMethod == JumpMethod.continuous)
			{
				if(isGrounded && Input.GetAxis ("Jump") != 0)
				{
					//Set the velocity.
					velocityY = jumpForce;

					//Landing sound reset.
					playedLanding = false;

					//Jump sound.
					if(jumpSound != null)
						GetComponent<AudioSource> ().PlayOneShot (jumpSound, jumpVolume);

					//Update the amount of jumps left.
					jumpsLeft--;
				}

				else if(!isGrounded && Input.GetKeyDown (KeyCode.Space))
				{
					//Set the velocity.
					velocityY = jumpForce;

					//Landing sound reset.
					playedLanding = false;

					//Jump sound.
					if(jumpSound != null)
						GetComponent<AudioSource> ().PlayOneShot (jumpSound, jumpVolume);

					//Update the amount of jumps left.
					jumpsLeft--;
				}
			}
		}

		//Crouching mechanics.
		if(canCrouch)
		{
			//Set the target height.
			float targetHeight = defaultCrouchHeight;

			//Update the target height if needed.
			if(isCrouching) targetHeight = crouchHeight;

			//Calculate and update the players height.
			float lastHeight = controller.height;
			controller.height = Mathf.SmoothDamp(controller.height, targetHeight, ref crouchSmoothVelocity, crouchSmoothScale);

			//Fix the position of the player.
			Vector3 fixedPos = new Vector3(transform.position.x, transform.position.y + (controller.height -lastHeight) /2, transform.position.z);
			transform.position = fixedPos;
		}

		//Standing.
		else if(!canCrouch && controller.height != defaultCrouchHeight)
		{
			//Calculate and update the players height.
			float lastHeight = controller.height;
			controller.height = Mathf.SmoothDamp(controller.height, defaultCrouchHeight, ref crouchSmoothVelocity, crouchSmoothScale);

			//Fix the position of the player.
			Vector3 fixedPos = new Vector3(transform.position.x, transform.position.y + (controller.height -lastHeight) /2, transform.position.z);
			transform.position = fixedPos;
		}

		//Ascending and descending functionality.
		if(isAscending) ZG_VelocityY = ascendSpeed + constantGravityForce;
		else if(isDescending) ZG_VelocityY = -descendSpeed + constantGravityForce;
		else ZG_VelocityY = 0 + constantGravityForce;

		//Calculating the movement as a vector.
		Vector3 inputVector = new Vector3(currentSpeed.x, 0, currentSpeed.y);

		//Direction in zero gravity.
		if(isZeroGravity)
			moveDirection = cam.playerCamera.transform.TransformDirection(inputVector) + Vector3.up * velocityY;

		//Direction in gravity.
		else if(!isZeroGravity)
		{
			moveDirection = transform.TransformDirection(inputVector) + Vector3.up * velocityY;
			if(isSliding) moveDirection += new Vector3(((1f - hitNormal.y) * hitNormal.x) * slideSpeed, 0f, ((1f - hitNormal.y) * hitNormal.z) * slideSpeed);
		}

		//Move the character.
		controller.Move(moveDirection * Time.deltaTime);

		//Grounded check.
		if(useSlopeDetection) isGrounded = CustomGroundState();
		else isGrounded = controller.isGrounded;

		//Sliding check.
		if(useSlopeSliding) isSliding = CustomSlidingState();
		else isSliding = false;

		//Smooth out the difference between the current velocity and the target velocity.
		if(isZeroGravity)
			velocityY = Mathf.Lerp(velocityY, ZG_VelocityY, Time.deltaTime / moveSmoothScale);

		//Reset the velocity.
		if(controller.isGrounded && !isZeroGravity)
			velocityY = 0;
	}

	private void RefreshStates()
	{
		//Getting the input.
		Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		//We are in zero gravity.
		if(gravityScale == 0) isZeroGravity = true;

		//We are affected by gravity.
		else if(gravityScale > 0) isZeroGravity = false;

		//We are ghosting.
		if(isZeroGravity && ghostMode) isGhosting = true;

		//We are not ghosting.
		else isGhosting = false;

		//We are moving.
		if(inputDir.magnitude != 0 && canMove) isMoving = true;

		//We are standing.
		else if(inputDir.magnitude == 0) isMoving = false;

		//We are running.
		if(Input.GetKey(KeyCode.LeftShift) && inputDir.y == 1 && !isZeroGravity && isMoving && canRun && !isCrouching) isRunning = true;

		//We are walking.
		else isRunning = false;

		//Ascending.
		if(Input.GetKey(ascendKey) && isZeroGravity && canAscend) isAscending = true;
		else isAscending = false;

		//Descending.
		if(Input.GetKey(descendKey) && isZeroGravity && canDescend && canMove)isDescending = true;
		else isDescending = false;

		//Drawing the debug.
		if(drawVisualization)
		{
			Debug.DrawLine(transform.position, transform.position + transform.forward * 1.5f, Color.white);
			Debug.DrawLine(transform.position + transform.forward * 1.5f, transform.position + transform.forward * 1.3f + transform.right * 0.2f, Color.white);
			Debug.DrawLine(transform.position + transform.forward * 1.5f, transform.position + transform.forward * 1.3f + transform.right * -0.2f, Color.white);

			Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(0, 0, controller.radius), Vector3.up * defaultCrouchHeight, Color.grey);
			Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(0, 0, -controller.radius), Vector3.up * defaultCrouchHeight, Color.grey);
			Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(controller.radius, 0, 0), Vector3.up * defaultCrouchHeight, Color.grey);
			Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(-controller.radius, 0, 0), Vector3.up * defaultCrouchHeight, Color.grey);
		}

		//Updating the standing up state.
		if(isCrouching)
		{
			Ray frontRay = new Ray(transform.position - Vector3.up * controller.height /2 + new Vector3(0, 0, controller.radius), Vector3.up);
			Ray backRay = new Ray(transform.position - Vector3.up * controller.height /2 + new Vector3(0, 0, -controller.radius), Vector3.up);
			Ray rightRay = new Ray(transform.position - Vector3.up * controller.height /2 + new Vector3(controller.radius, 0, 0), Vector3.up);
			Ray leftRay = new Ray(transform.position - Vector3.up * controller.height /2 + new Vector3(-controller.radius, 0, 0), Vector3.up);

			if(!canStandUp) canStandUp = true;

			if(Physics.Raycast(frontRay, defaultCrouchHeight, crouchInteractLayer))
			{
				canStandUp = false;
				Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(0, 0, controller.radius), Vector3.up * defaultCrouchHeight, Color.red);
			}

			if(Physics.Raycast(backRay, defaultCrouchHeight, crouchInteractLayer))
			{
				canStandUp = false;
				Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(0, 0, -controller.radius), Vector3.up * defaultCrouchHeight, Color.red);
			}

			if(Physics.Raycast(rightRay, defaultCrouchHeight, crouchInteractLayer))
			{
				canStandUp = false;
				Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(controller.radius, 0, 0), Vector3.up * defaultCrouchHeight, Color.red);
			}

			if(Physics.Raycast(leftRay, defaultCrouchHeight, crouchInteractLayer))
			{
				canStandUp = false;
				Debug.DrawRay(transform.position - Vector3.up * controller.height /2 + new Vector3(-controller.radius, 0, 0), Vector3.up * defaultCrouchHeight, Color.red);
			}
		}

		else if(!isCrouching) canStandUp = true;

		//Crouching
		if(Input.GetKey(crouchKey) && canCrouch && !isRunning && canMove && !isZeroGravity && isGrounded && !isSliding) isCrouching = true;

		//Standing.
		else if(!Input.GetKey(crouchKey) && canStandUp) isCrouching = false;

		//Reset the amount of jumps that can be made.
		if(isGrounded && jumpsLeft < jumpsInRow) jumpsLeft = jumpsInRow;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		//If there are any available footsteps.
		if(footstepTypes.Length != 0)
		{
			//Loop through the footstep types array.
			for(int i = 0; i < footstepTypes.Length; i++)
			{
				//If the ground type matches the current step in the array, continue.
				if(hit.transform.CompareTag(footstepTypes[i].tagName))
				{
					//If the array is not empty, set the current step sound and volume.
					if(footstepTypes[i].sounds.Length != 0)
					{
						//Set the current footstep sound and volume.
						currentStepSound = footstepTypes[i].sounds[Random.Range(0, footstepTypes[i].sounds.Length)];
						currentStepVolume = footstepTypes[i].volume;
						break;
					}
				}
			}
		}

		//If we had high enough velocity to play landing animation.
		if(velocityY < cam.minVelocity && !playedLanding)
		{
			//Play the landing animation.
			cam.PlayLandingMotion();
		}

		//If we had high enough velocity to play landing sound.
		if(velocityY < minVelocity && !playedLanding)
		{
			//Update the timer
			footstepTimer = Time.time + footstepDelay;

			//If the landing is based on the current ground type.
			if(landingSoundBase == LandingSoundBase.currentGroundType)
			{
				//Play landing sound.
				GetComponent<AudioSource>().PlayOneShot(currentStepSound, currentStepVolume * landingVolume);
			}

			//If the landing is based on a specific sound.
			else if(landingSoundBase == LandingSoundBase.specificSound)
			{
				//Play landing sound.
				if(landingSound != null)
					GetComponent<AudioSource>().PlayOneShot(landingSound, landingVolume);
			}

			//Update the landing sound state.
			playedLanding = true;
		}
	}

	private void PlayFootsteps()
	{
		//If we are grounded, continue.
		if(isGrounded && isMoving && canMove && footstepTypes.Length != 0)
		{
			//If we can play the next step sound.
			if(footstepTimer < Time.time)
			{
				//If running.
				if(isRunning)
				{
					footstepTimer = Time.time + runStepTime;
					stepVolume = currentStepVolume * runStepVolume;
				}

				//If crouching.
				else if(isCrouching)
				{
					footstepTimer = Time.time + crouchStepTime;
					stepVolume = currentStepVolume * crouchStepVolume;
				}

				//If walking.
				else if(!isRunning && !isCrouching)
				{
					footstepTimer = Time.time + walkStepTime;
					stepVolume = currentStepVolume * walkStepVolume;
				}

				//Play the current footstep with the current volume.
				if(controller.velocity.magnitude / footstepMagnitude > 0.07f) GetComponent<AudioSource>().PlayOneShot(currentStepSound, stepVolume);
			}
		}
	}

	private void CalculateGhostMode()
	{
		//If ghost mode is enabled, and currently the collision is active between the layers.
		if(isGhosting && !Physics.GetIgnoreLayerCollision(gameObject.layer, ignoredLayer)) 
		{
			//Disable the collision between the player's layer and the selected layer.
			Physics.IgnoreLayerCollision(gameObject.layer, ignoredLayer, true);
		}

		//If ghost mode is disabled, and currently the collision is disabled between the layers.
		if(!isGhosting && Physics.GetIgnoreLayerCollision(gameObject.layer, ignoredLayer)) 
		{
			//Enable the collision between the player's layer and the selected layer.
			Physics.IgnoreLayerCollision(gameObject.layer, ignoredLayer, false);
		}
	}

	private void OnGUI()
	{
		//Rendering the debug if its needed.
		if(!monitorVariables) return;

		//Creating the outline of the debug.
		GUI.Box(new Rect(15, 15, 310, 130), "");
		GUI.Box(new Rect(15, 150, 310, 209), "");

		//Enable gravity button.
		if(isZeroGravity)
		{
			if(GUI.Button(new Rect(15, 364, 310, 25), "Enable Gravity"))
			{
				//Update the gravity using the built-in function.
				UpdateGravity(1f, moveSmoothScale, 0f);
			}
		}

		//Disable gravity button.
		else if(!isZeroGravity)
		{
			if(GUI.Button(new Rect(15, 364, 310, 25), "Disable Gravity"))
			{
				//Update the gravity using the built-in function.
				UpdateGravity(0f, moveSmoothScale, 1.2f);
			}
		}

		//Creating an area for GUILayout to display debug information.
		GUILayout.BeginArea(new Rect(20, 20, 300, Screen.height));
		GUILayout.BeginVertical();

		//Move velocity.
		GUILayout.Label("Move Velocity " + controller.velocity);

		//Move smooth scale.
		GUILayout.Label("Move Smooth Scale (" + Mathf.RoundToInt(moveSmoothScale * 100) + "%)");
		moveSmoothScale = GUI.HorizontalSlider(new Rect(175, 30, 125, 10), moveSmoothScale, 0, 1);

		//Gravity scale.
		GUILayout.Label("Gravity Scale (" + Mathf.RoundToInt(gravityScale * 100) + "%)");
		gravityScale = GUI.HorizontalSlider(new Rect(175, 55, 125, 10), gravityScale, 0, 1);

		//Air control percent.
		GUILayout.Label("Air Control Percent (" + Mathf.RoundToInt(airControlPercent * 100) + "%)");
		airControlPercent = GUI.HorizontalSlider(new Rect(175, 80, 125, 10), airControlPercent, 0, 1);

		//Mouse smooth scale.
		GUILayout.Label("Mouse Smooth Scale (" + Mathf.RoundToInt(cam.mouseSmoothScale * 100) + "%)");
		cam.mouseSmoothScale = GUI.HorizontalSlider(new Rect(175, 105, 125, 10), cam.mouseSmoothScale, 0, 1);

		//Passive states.
		GUILayout.Space(11);
		GUILayout.Toggle(isGhosting, " Is Ghosting");
		GUILayout.Toggle(isZeroGravity, " Is Zero Gravity");
		GUILayout.Toggle(isGrounded, " Is Grounded");
		GUILayout.Toggle(isSliding, " Is Sliding");
		GUILayout.Toggle(isMoving, " Is Moving");
		GUILayout.Toggle(isRunning, " Is Running");
		GUILayout.Toggle(isCrouching, " Is Crouching");
		GUILayout.Toggle(isAscending, " Is Ascending");
		GUILayout.Toggle(isDescending, " Is Descending");

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}