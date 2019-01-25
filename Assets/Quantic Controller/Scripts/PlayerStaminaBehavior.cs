using UnityEngine;

[AddComponentMenu("Quantic Controller/Player Stamina Behavior")]

public class PlayerStaminaBehavior : MonoBehaviour
{
	public PlayerMotorBehavior motor;
	public bool autoAssign = true;
	public bool drawDefaultStaminaBar = true;

	public float maxStamina = 100;
	public float currentStamina = 100;
	public float drainSpeed = 10;
	public float regenSpeed = 7;
	public float minStaminaAfterExhaust = 15;

	public float staminaPercent;
	public bool isExhausted;

	private Texture2D barTexture;
	private Texture2D fillTexture;

	private void Start()
	{
		//Initialization.
		if(autoAssign) motor = FindObjectOfType<PlayerMotorBehavior>();

		//Create new 2x2 textures with RGB24 (color texture) format.
		barTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
		fillTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);

		//Generate the color for the bar texture.
		Color barColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);
		Color fillColor = new Color(1.0f, 1.0f, 1.0f);

		//Set the barTexture's color to grey at each pixel.
		barTexture.SetPixel(0, 0, barColor);
		barTexture.SetPixel(0, 1, barColor);
		barTexture.SetPixel(1, 0, barColor);
		barTexture.SetPixel(1, 1, barColor);

		//Set the fillTexture's color to white at each pixel.
		fillTexture.SetPixel(0, 0, fillColor);
		fillTexture.SetPixel(0, 1, fillColor);
		fillTexture.SetPixel(1, 0, fillColor);
		fillTexture.SetPixel(1, 1, fillColor);

		//Apply the modifications on the textures.
		barTexture.Apply();
		fillTexture.Apply();
	}

	private void Update()
	{
		//If we ran out of stamina, we get exhausted.
		if(currentStamina == 0) isExhausted = true;

		//Check if we are exhausted.
		if(isExhausted)
		{
			//Disable the running.
			if(motor.canRun) motor.canRun = false;

			//Check if we can run again.
			if(currentStamina >= minStaminaAfterExhaust)
			{
				//Remove the exhaustion effect.
				if(!motor.canRun) motor.canRun = true;
				isExhausted = false;
			}
		}

		//Check if the character is running.
		if(motor.isRunning)
		{
			//Drain stamina.
			if(currentStamina != 0) currentStamina -= drainSpeed * Time.deltaTime;
			currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
		}

		//Check if we are not running.
		else if(!motor.isRunning)
		{
			//Regenerate stamina.
			if(currentStamina != maxStamina) currentStamina += regenSpeed * Time.deltaTime;
			currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
		}

		//Convert the current stamina in percentage.
		staminaPercent = currentStamina / maxStamina * 100;
	}

	private void OnGUI()
	{
		//Check if the stamina bar should be displayed.
		if(!drawDefaultStaminaBar || currentStamina == maxStamina) return;

		//Calculate the positions.
		Rect barRect = new Rect(Screen.width /2 -Screen.width /2.5f /2, Screen.height -30, Screen.width /2.5f, 17);
		Rect fillRect = new Rect(Screen.width /2 -Screen.width /2.5f /2, Screen.height -30, Screen.width /2.5f * staminaPercent /100, 17);

		//Draw the stamina bar.
		GUI.DrawTexture(barRect, barTexture);
		GUI.DrawTexture(fillRect, fillTexture);
	}
}
