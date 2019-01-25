using UnityEngine;

[AddComponentMenu("Quantic Controller/Player Camera Behavior")]

public class PlayerCameraBehavior : MonoBehaviour
{
	//Mouse look and player camera.
	public Camera playerCamera;
	public Transform headTransform;
	public float mouseSensitivity = 5;
	[Range(0, 1)] public float mouseSmoothScale = 0.05f;
	[Range(0, 90)] public float maxLookAngle = 75;
	public bool allowRotation = true;

	//Head bobbing.
	public float walkBobbingSpeed = 6f;
	public float runBobbingSpeed = 8.5f;
	public float crouchBobbingSpeed = 3.5f;
	public float bobTiltFactor = 0.2f;
	public float bobRotationMultiplier = 1f;
	public Vector2 walkBobAmount = new Vector2(0.05f, 0.1f);
	public Vector2 runBobAmount = new Vector2(0.12f, 0.18f);
	public Vector2 crouchBobAmount = new Vector2(0.15f, 0.15f);
	public bool useHeadBob = true;

	//Landing motion.
	public float minVelocity = -5;
	public float dropAmount = 0.1f;
	public float dropTiltFactor = 3.5f;
	public float dropSpeed = 0.15f;
	public bool useLandingMotion = true;

	private PlayerMotorBehavior motor;
	private CharacterController controller;

	private Vector3 headStartPoint;
	private Quaternion headStartRotation;
	private Vector3 rotationSmoothVelocity;
	private Vector3 currentRotation;

	private float yaw;
	private float pitch;
	private float translateX;
	private float translateY;
	private float dropTarget;
	private float dropProgress;
	private float dropVelocity;
	private float bobStrengthFactor;
	private float bobTimer;
	private float bobCycleSpeed;
	private Vector2 currentBobAmount;

	private bool playLanding;
	private float landingProgress;
	private float landingVelocity;

	private void Start()
	{
		//Initialization.
		motor = GetComponent<PlayerMotorBehavior>();
		controller = GetComponent<CharacterController>();

		//Get the starting position of the head transform.
		if(headTransform != null)
		{
			headStartPoint = headTransform.localPosition;
			headStartRotation = headTransform.localRotation;
		}

		//Keeping the players starting rotation.
		if(playerCamera != null)
		{
			yaw = transform.eulerAngles.y;
			pitch = playerCamera.transform.localEulerAngles.x;
			currentRotation.Set(pitch, yaw, playerCamera.transform.localEulerAngles.z);
		}
	}

	private void Update()
	{
		//Running different methods every frame.
		HandleMouseLook();
		HandleHeadBobbing();
		HandleLandingMotion();
	}

	private void HandleMouseLook()
	{
		//Disable the mouse look functionality.
		if(!allowRotation) return;

		//Get the current mouse input.
		yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
		pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

		//Clamp the camera rotation.
		pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

		//Calculate and rotate the camera.
		currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, mouseSmoothScale);

		//Rotating the player.
		Vector3 bodyRotation = new Vector3(0, currentRotation.y, 0);
		transform.localEulerAngles = bodyRotation;

		//Rotating the camera.
		Vector3 cameraRotation = new Vector3(currentRotation.x, 0, currentRotation.z);
		playerCamera.transform.localEulerAngles = cameraRotation;
	}

	private void HandleHeadBobbing()
	{
		//If the head transform is missing, report the error.
		if(useHeadBob && headTransform == null)
		{
			Debug.LogError("The head bobbing is enabled but there is no head transform attached. Head bobbing is disabled.");
			useHeadBob = false;
		}

		//We don't need this function if the head bobbing is disabled.
		if(!useHeadBob)
		{
			if(headTransform.localPosition != headStartPoint) headTransform.localPosition = Vector3.Lerp(headTransform.localPosition, headStartPoint, Time.deltaTime * 2);
			if(headTransform.localRotation != headStartRotation) headTransform.localRotation = Quaternion.Lerp(headTransform.localRotation, headStartRotation, Time.deltaTime * 2);
			return;
		}

		//Calculate the delta of the head bobbing.
		float delta = Mathf.Clamp01(Time.deltaTime * 6f);

		//Check if we are grounded and moving.
		if(motor.isGrounded && motor.isMoving)
		{
			//Switch to running cycle speed.
			if(motor.isRunning)
			{
				bobCycleSpeed = runBobbingSpeed;
				currentBobAmount = Vector2.LerpUnclamped(currentBobAmount, runBobAmount, delta);
			}

			//Switch to crouching cycle speed.
			else if(motor.isCrouching)
			{
				bobCycleSpeed = crouchBobbingSpeed;
				currentBobAmount = Vector2.LerpUnclamped(currentBobAmount, crouchBobAmount, delta);
			}

			//Switch to walking cycle speed.
			else
			{
				bobCycleSpeed = walkBobbingSpeed;
				currentBobAmount = Vector2.LerpUnclamped(currentBobAmount, walkBobAmount, delta);
			}

			//Update the timer of the head bobbing.
			bobTimer += bobCycleSpeed * Time.deltaTime;
			if(bobTimer >= Mathf.PI * 2f) bobTimer -= Mathf.PI * 2f;

			//Calculate the head bobbing motion.
			bobStrengthFactor = Mathf.Clamp01(controller.velocity.magnitude / 2.5f);
			translateX = Mathf.Sin(bobTimer) * currentBobAmount.x * bobStrengthFactor;
			translateY = Mathf.Cos(bobTimer * 2f) * currentBobAmount.y * bobStrengthFactor;
		}

		//We are not grounded or not moving, reset the head bobbing.
		else
		{
			//Reset the head bobbing motion.
			bobTimer = Mathf.LerpUnclamped(bobTimer, 0f, delta);
			bobStrengthFactor = Mathf.LerpUnclamped(bobStrengthFactor, 0f, delta);
			translateX = Mathf.LerpUnclamped(translateX, 0f, delta);
			translateY = Mathf.LerpUnclamped(translateY, 0f, delta);
		}

		//Apply the new position and rotation on the head.
		headTransform.localPosition = new Vector3(headStartPoint.x + translateX * 0.04f, headStartPoint.y - dropAmount * dropProgress + translateY * 0.1f, headStartPoint.z);
		headTransform.localRotation = Quaternion.Euler(-translateY * bobRotationMultiplier + dropTiltFactor * dropProgress, translateX * bobRotationMultiplier, -translateX * bobTiltFactor * bobRotationMultiplier);
	}

	private void HandleLandingMotion()
	{
		//Check if the landing motion is enabled.
		if(!useLandingMotion) return;

		//Start the landing motion.
		if(playLanding)
		{
			//Calculate the progress.
			landingProgress = Mathf.SmoothDamp(landingProgress, 1, ref landingVelocity, dropSpeed * 2);

			//If the progress hasn't reached the drop height yet, set the target.
			if(landingProgress < 0.5f) dropTarget = 1;
			else playLanding = false;
		}

		//Returning to the defaut height.
		if(!playLanding)
		{
			//Set the target and reset the progress.
			dropTarget = 0;
			landingProgress = 0;
		}

		//Calculate the modified height of the head.
		if(dropProgress != dropTarget) dropProgress = Mathf.SmoothDamp(dropProgress, dropTarget, ref dropVelocity, dropSpeed);
	}

	//Triggers the landing head motion.
	public void PlayLandingMotion()
	{
		//Start the landing motion if the it's enabled.
		if(useLandingMotion) playLanding = true;
	}
}
