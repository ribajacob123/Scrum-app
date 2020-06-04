using UnityEngine;

public class CameraPivotControl : MonoBehaviour
{

	[Tooltip("Speed the player can move.")]
	public float speed;

	[Tooltip("The four movement keys - Up, Down, Left, Right.")]
	public KeyCode[] controlKeys = new KeyCode[]
	{
		KeyCode.W,
		KeyCode.S,
		KeyCode.A,
		KeyCode.D
	};

	[HideInInspector] public Transform trackingTarget;

	CameraControl camControl;
	Vector3 fwd;
	Vector3 left;

	private void Start ()
	{
		camControl = FindObjectOfType<CameraControl>();

		// If no control script is found
		if (!camControl)
		{
			Debug.LogError("No CameraControl script found: Make sure the main camera has the CameraControl attached in this scene.", gameObject);
			gameObject.SetActive(false);
		}
	}

	void Update ()
	{

		// If there is a target being tracked
		if (trackingTarget)
		{
			// Break from tracking if any movement key is inputted
			for (int i = 0; i < controlKeys.Length; i++)
				if (Input.GetKey(controlKeys[i]))
					trackingTarget = null;

			// Set position to target
			transform.position = trackingTarget.position;
		}

		// Manual control
		else
		{
			// Set direction values
			fwd = new Vector3(-Mathf.Cos(camControl.rotation.x), 0, -Mathf.Sin(camControl.rotation.x));
			left = new Vector3(-Mathf.Sin(-camControl.rotation.x), 0, -Mathf.Cos(-camControl.rotation.x));

			// Up key
			if (Input.GetKey(controlKeys[0]))
				transform.Translate(fwd * speed * camControl.radius * Time.deltaTime);

			// Down key
			if (Input.GetKey(controlKeys[1]))
				transform.Translate(-fwd * speed * camControl.radius * Time.deltaTime);

			// Left key
			if (Input.GetKey(controlKeys[2]))
				transform.Translate(left * speed * camControl.radius * Time.deltaTime);

			// Right key
			if (Input.GetKey(controlKeys[3]))
				transform.Translate(-left * speed * camControl.radius * Time.deltaTime);
		}

	}
}
