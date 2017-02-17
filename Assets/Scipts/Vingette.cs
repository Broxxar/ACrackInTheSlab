using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Vingette : MonoBehaviour
{
	[SerializeField]
	private Shader _shader;
	[Range(0,1)]
	public float MinRadius = 0.3f;
	[Range(0, 1)]
	public float MaxRadius = 1.0f;
	[Range(0, 1)]
	public float Saturation = 1.0f;

	Material _material;

	void OnEnable()
	{
		_material = new Material(_shader);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		_material.SetFloat("_MinRadius", MinRadius);
		_material.SetFloat("_MaxRadius", MaxRadius);
		_material.SetFloat("_Saturation", Saturation);

		Graphics.Blit(src, dst, _material, 0);
	}
}
