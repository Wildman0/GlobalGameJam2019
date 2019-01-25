using UnityEngine;

[AddComponentMenu("Quantic Controller/Player Cursor Manager")]

public class PlayerCursorManager : MonoBehaviour
{
	public KeyCode toggleKey = KeyCode.Escape;
	public bool isLocked;

	private void Start()
	{
		//Automatically hide cursor when the game starts.
		HideCursor();
	}

	private void Update()
	{
		//Update the cursor when the right key is pressed.
		if(Input.GetKeyDown(toggleKey))
		{
			//Show the cursor.
			if(isLocked) ShowCursor();

			//Lock the cursor.
			else HideCursor();
		}
	}

	public void ShowCursor()
	{
		//Update the cursor.
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		//Update the state.
		isLocked = false;
	}

	public void HideCursor()
	{
		//Update the cursor.
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		//Update the state.
		isLocked = true;
	}
}
