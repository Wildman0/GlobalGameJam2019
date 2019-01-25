using UnityEngine;

[AddComponentMenu("Quantic Controller/Player Time Effector")]

public class PlayerTimeEffector : MonoBehaviour
{
	public PlayerMotorBehavior motor;
	public bool autoAssign = true;
	public bool overrideMouseSensitivity = true;
	public bool slowDownAudio = true;

	public float slowedTimeScale = 0.05f;
	public float timeChangeSpeed = 5f;

	public float normalMouseSensitivity = 5f;
	public float slowedMouseSensitivity = 2f;

	private PlayerCameraBehavior cam;
	private AudioSource[] sources;

	private void Start()
	{
		//Initialization.
		if(autoAssign) motor = FindObjectOfType<PlayerMotorBehavior>();
		cam = motor.GetComponent<PlayerCameraBehavior>();

		//Gather audio sources if required.
		if(slowDownAudio) sources = FindObjectsOfType<AudioSource>();

		//Override feature which look weird in slow motion.
		cam.mouseSmoothScale = 0f;
	}

	private void Update()
	{
		//Running different methods every frame.
		OverrideTimeScale();
		OverrideMouseSensitivity();
		SlowDownAudios();
	}

	private void OverrideTimeScale()
	{
		//Override the slope detection feature.
		motor.useSlopeDetection = Time.timeScale >= 0.75f ? true : false;

		//Calculate the target time scale.
		float targetTimeScale = motor.isMoving ? 1f : slowedTimeScale;
		float smoothedTimeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime * timeChangeSpeed);

		//Apply the new time scale.
		Time.timeScale = smoothedTimeScale;
		Time.fixedDeltaTime = 0.02f * smoothedTimeScale;
	}

	private void OverrideMouseSensitivity()
	{
		//Check if we should override the sensitivity.
		if(!overrideMouseSensitivity) return;

		//Calculate the target mouse sensitivity.
		float targetSensitivity = motor.isMoving ? normalMouseSensitivity : slowedMouseSensitivity;
		float smoothedSensitivity = Mathf.Lerp(cam.mouseSensitivity, targetSensitivity, Time.unscaledDeltaTime * timeChangeSpeed * 0.5f);

		//Apply the new mouse sensitivity.
		cam.mouseSensitivity = smoothedSensitivity;
	}

	private void SlowDownAudios()
	{
		//Check if we should slow down the sounds.
		if(!slowDownAudio) return;

		//Start a loop to update the playback speed of the audio sources.
		foreach(AudioSource source in sources) source.pitch = Time.timeScale;
	}
}
