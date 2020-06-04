using UnityEngine;

public class CameraControl : MonoBehaviour
{

	[Tooltip("How much mouse movement effects the cameras rotation.")]
	public float sensitivity;

	[Tooltip("How much scrolling effects the cameras orbital radius.")]
	public float scrollSensitivity;

	[Tooltip("How much keyboard buttons effects the cameras orbital radius.")]
	public float scrollSensitivityKeyboard;

	[Tooltip("The two keys which control zooming in or out.")]
	public KeyCode[] zoomKeys = new KeyCode[]
	{
		KeyCode.E,
		KeyCode.Q
	};

	[Tooltip("The maxiumum angle on the y-axis the camera can go to, in radians (Straight up is 1.57).")]
	public float rotationYLimMax;

	[Tooltip("The minimum angle on the y-axis the camera can go to, in radians (Straight down is -1.57).")]
	public float rotationYLimMin;

	[Tooltip("The maximum radius allowed to be from the target.")]
	public float radiusLimMax;

	[Tooltip("The minimum radius allowed to be from the target.")]
	public float radiusLimMin;

	[Tooltip("The maximum distance an object can be selected from.")]
	public float selectionRange;

	[HideInInspector] public float radius;
	[HideInInspector] public Vector2 rotation;

	Vector2 mouseInitialPos;
	Vector2 initialRotation;
	Vector3 setPos;
	CameraPivotControl camPivotControl;


	private void Start()
	{
		camPivotControl = FindObjectOfType<CameraPivotControl>();

		// If no pivot script is found
		if (!camPivotControl)
		{
			Debug.LogError("No CameraPivotControl script found: Make sure there is a gameobject with the CameraPivotControl attached in the scene.", gameObject);
			gameObject.SetActive(false);
		}
	}

	void FixedUpdate ()
	{

		// Initial mouse press
		if (Input.GetMouseButtonDown(1))
		{
			mouseInitialPos = Input.mousePosition;
			initialRotation = rotation;
		}

		// Continued held press
		if (Input.GetMouseButton(1))
		{
			float mouseXChange = Input.mousePosition.x - mouseInitialPos.x;
			float mouseYChange = Input.mousePosition.y - mouseInitialPos.y;

			rotation.x = initialRotation.x - mouseXChange * sensitivity;
			rotation.y = initialRotation.y - mouseYChange * sensitivity;
		}

		// Limits y axis
		if (rotation.y > rotationYLimMax)
			rotation.y = rotationYLimMax;
		if (rotation.y < rotationYLimMin)
			rotation.y = rotationYLimMin;

		// Scroll with mouse
		if (Input.mouseScrollDelta.y != 0)
			radius -= Input.mouseScrollDelta.y * scrollSensitivity * radius;

		// Keyboard scrolling
		else
		{
			if (Input.GetKey(zoomKeys[1]))
				radius -= scrollSensitivityKeyboard * radius;
			if (Input.GetKey(zoomKeys[0]))
				radius += scrollSensitivityKeyboard * radius;
		}

		// Limits scroll / zoom
		if (radius > radiusLimMax)
			radius = radiusLimMax;
		if (radius < radiusLimMin)
			radius = radiusLimMin;

		// Adjust position relative to target position
		setPos = camPivotControl.transform.position;
		setPos.x += radius * Mathf.Cos(rotation.x) * Mathf.Cos(rotation.y);
		setPos.y += radius * Mathf.Sin(rotation.y);
		setPos.z += radius * Mathf.Sin(rotation.x) * Mathf.Cos(rotation.y);

		// Apply position and rotation to camera
		transform.position = setPos;
		transform.LookAt(camPivotControl.transform);

		// Project raycast from camera towards mouse
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, selectionRange))
		{
			// If clicked
			if (Input.GetMouseButtonDown(0))
			{
				camPivotControl.transform.position = hit.transform.position;
				camPivotControl.trackingTarget = hit.transform;
			}
		}
		else
		{
			// If clicked
			if (Input.GetMouseButtonDown(0))
			{
				camPivotControl.trackingTarget = null;
			}
		}
	}
}
