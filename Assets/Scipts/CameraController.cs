using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
	private static CameraController _instance;

	public static CameraController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<CameraController>();
			}

			return _instance;
		}
	}

	public Vector3 CurrentCameraRotation
	{
		get { return _currentCameraRotation; }
	}

	public float RotationSmoothFactor;
	public float MouseLookSensitivity;

	public bool LockYaw;

	public Vector2 PitchClamp = new Vector2(-75, 75);

	Vector3 _targetCameraRotation;
	Vector3 _currentCameraRotation;
	float _defaultY;
	bool _shaking;
	private Camera _camera;

	bool _rotationControl = true;

	void Start()
	{
		_camera = GetComponentInChildren<Camera>();

		_defaultY = transform.localPosition.y;
		_targetCameraRotation = new Vector3(transform.eulerAngles.y, transform.eulerAngles.x);
		_currentCameraRotation = new Vector3(transform.eulerAngles.y, transform.eulerAngles.x);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void OnPreRender()
	{
		var matrix = _camera.worldToCameraMatrix;
		Shader.SetGlobalMatrix("MATRIX_I_V", matrix);
	}

	public void ReParent(Transform newParent, Vector3 localPos)
	{
		transform.SetParent(newParent);
		transform.localPosition = localPos;
	}

	private Vector3 _savedTargetRotation;
	private Vector3 _savedCurrentRotation;
	private Vector2 _savedPitchClamp;
	private float _savedSmoothFactor;
	private float _savedSensitivity;
	private float _savedFoV;

	public void PushSettings()
	{
		_savedTargetRotation = _targetCameraRotation;
		_savedCurrentRotation = _currentCameraRotation;
		_savedPitchClamp = PitchClamp;
		_savedSmoothFactor = RotationSmoothFactor;
		_savedSensitivity = MouseLookSensitivity;
		_savedFoV = _camera.fieldOfView;
	}

	public void PopSettings()
	{
		_targetCameraRotation = _savedTargetRotation;
		_currentCameraRotation = _savedCurrentRotation;
		PitchClamp = _savedPitchClamp;
		RotationSmoothFactor = _savedSmoothFactor;
		MouseLookSensitivity = _savedSensitivity;
		_camera.fieldOfView = _savedFoV;
	}

	public void ApplyStarSphereSettings()
	{
		_targetCameraRotation = Vector3.zero;
		_currentCameraRotation = Vector3.zero;
		PitchClamp = new Vector2(-35, 35);
		RotationSmoothFactor = 10.0f;
		MouseLookSensitivity = 1.0f;
		_camera.fieldOfView = 45;
	}

	public void UpdateRotation()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			_rotationControl = !_rotationControl;

		if (_rotationControl)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			Vector3 deltaMouse = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			_targetCameraRotation += deltaMouse * MouseLookSensitivity;
			_targetCameraRotation.y = Mathf.Clamp(_targetCameraRotation.y, PitchClamp.x, PitchClamp.y);

			if (LockYaw)
				_targetCameraRotation.x = Mathf.Clamp(_targetCameraRotation.x, -40, 40);
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		_currentCameraRotation = Vector3.Lerp(_currentCameraRotation, _targetCameraRotation, Time.deltaTime * RotationSmoothFactor);
		transform.rotation = Quaternion.AngleAxis(_currentCameraRotation.x, Vector3.up) * Quaternion.AngleAxis(-_currentCameraRotation.y, Vector3.right);

		if (_shaking)
		{
			transform.localPosition = new Vector3(0, _defaultY + Random.Range(-0.01f, 0.01f), 0);
		}
	}

	public static void Rotate(Vector3 deltaRotation)
	{
		Instance._targetCameraRotation += deltaRotation;
		Instance._currentCameraRotation += deltaRotation;
	}

	public static void StartShaking()
	{
		Instance._shaking = true;
	}

	public static void StopShaking()
	{
		Instance._shaking = false;
	}
}