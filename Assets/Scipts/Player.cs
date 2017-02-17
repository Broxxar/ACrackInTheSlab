using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public Vector3 Forward { get { return _cameraTransform.forward; } }

	[SerializeField]
	Transform _cameraTransform;
	[SerializeField]
	CharacterController _charController;
	[SerializeField]
	CameraController _cameraController;
	[SerializeField]
	Transform _fpsItemPivot;

	float _yForce = 0;
	float _walkSpeed = 8f;
	float _crouchSpeedFactor = 0.5f;
	float _jumpForce = 12.0f;
	float _stickToGroundForce = 2;
	float _gravity = 25;
	bool _crouched = false;
	float _cameraHeightNormal = 1.0f;
	float _cameraHeightCrouch = 0.5f;

	void Update()
	{
		UpdateMovement();
		UpdateCrouch();
		_cameraController.UpdateRotation();
		UpdateItemHold();
	}

	void UpdateMovement()
	{
		var forward = Vector3.Cross(_cameraTransform.right, Vector3.up).normalized;
		var right = Vector3.Cross(_cameraTransform.forward, Vector3.up).normalized;
		var movement = Vector3.zero;

		movement += Input.GetKey(KeyCode.W) ? forward : Vector3.zero;
		movement += Input.GetKey(KeyCode.A) ? right : Vector3.zero;
		movement += Input.GetKey(KeyCode.S) ? -forward : Vector3.zero;
		movement += Input.GetKey(KeyCode.D) ? -right : Vector3.zero;
		movement = movement.normalized;
		movement *= _walkSpeed * (_crouched ? _crouchSpeedFactor : 1.0f);

		if (_charController.isGrounded)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				_yForce = _jumpForce;
            }
			else
			{
				_yForce = -_stickToGroundForce;
			}
		}
		else
		{
			_yForce += -_gravity * Time.deltaTime;
		}

		movement.y = _yForce;

		_charController.Move(movement * Time.deltaTime);
	}

	void UpdateCrouch()
	{
		_crouched = Input.GetKey(KeyCode.LeftControl);
		var cameraPos = _crouched ? new Vector3(0, _cameraHeightCrouch) : new Vector3(0, _cameraHeightNormal);
		_cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, cameraPos, Time.deltaTime * 15);
	}

	void UpdateItemHold()
	{
		var targetEulers = _cameraController.CurrentCameraRotation;
		targetEulers.y = Mathf.Clamp(targetEulers.y, -40, 40);
		var targetRotation = Quaternion.Euler(-targetEulers.y, targetEulers.x, 0);
		_fpsItemPivot.rotation = Quaternion.Slerp(_fpsItemPivot.rotation, targetRotation, 15.0f * Time.deltaTime);
	}
}