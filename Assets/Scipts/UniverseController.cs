using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UniverseController : MonoBehaviour
{
	public static bool Swapping
	{
		get; private set;
	}

	[SerializeField]
	private TwinCameraController _twinCameras;
	[Header("Swap Effect Stuff")]
	[SerializeField]
	private Vingette _vingette;
	[SerializeField]
	private AnimationCurve _innerVingette;
	[SerializeField]
	private AnimationCurve _outerVingette;
	[SerializeField]
	private AnimationCurve _saturation;
	[SerializeField]
	private Camera[] _cameras;
	[SerializeField]
	private AnimationCurve _fov;
	[SerializeField]
	private AnimationCurve _timeScale;
	[SerializeField]
	private Transform _itemTransform;
	[SerializeField]
	private AnimationCurve _itemPosition;

	private AudioSource _audio;
	private bool _swapTiggered;
	private readonly float _swapTime = 0.85f;

	void Awake()
	{
		SceneManager.LoadScene(1, LoadSceneMode.Additive);
		_audio = GetComponent<AudioSource>();
	}

	void SwapUniverses()
	{
		Swapping = true;
		StartCoroutine(SwapAsync());
	}

	void Update()
	{
		if (!Swapping && Input.GetMouseButtonDown(0))
		{
			StartCoroutine(SwapAsync());
		}
	}

	/// <summary>
	/// Controls a bunch of stuff like vingette and FoV over time and calls the swap cameras function after a fixed duration.
	/// </summary>
	IEnumerator SwapAsync()
	{
		Swapping = true;
		_swapTiggered = false;

		_audio.PlayOneShot(_audio.clip);

		for (float t = 0; t < 1.0f; t += Time.unscaledDeltaTime * 1.2f)
		{
			for (int i = 0; i < _cameras.Length; i++)
			{
				_cameras[i].fieldOfView = _fov.Evaluate(t);
			}
			_vingette.MinRadius = _innerVingette.Evaluate(t);
			_vingette.MaxRadius = _outerVingette.Evaluate(t);
			_vingette.Saturation = _saturation.Evaluate(t);
			Time.timeScale = _timeScale.Evaluate(t);

			_itemTransform.localPosition = new Vector3(-0.5f, -0.5f, _itemPosition.Evaluate(t));

			if (t > _swapTime && !_swapTiggered)
			{
				_swapTiggered = true;
				_twinCameras.SwapCameras();
			}

			yield return null;
		}

		// technically a huge lag spike could cause this to be missed in the coroutine so double check it here.
		if (!_swapTiggered)
		{
			_swapTiggered = true;
			_twinCameras.SwapCameras();
		}

		for (int i = 0; i < _cameras.Length; i++)
		{
			_cameras[i].fieldOfView = _fov.Evaluate(1.0f);
		}

		_vingette.MinRadius = _innerVingette.Evaluate(1.0f);
		_vingette.MaxRadius = _outerVingette.Evaluate(1.0f);
		_vingette.Saturation = 1.0f;
		_itemTransform.localPosition = new Vector3(-0.5f, -0.5f, 0.5f);

		Time.timeScale = 1.0f;

		Swapping = false;
	}
}
