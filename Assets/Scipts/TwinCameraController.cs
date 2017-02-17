using UnityEngine;
using UnityEngine.Rendering;

public class TwinCameraController : MonoBehaviour
{
	[SerializeField]
	private Camera _activeCamera;
	[SerializeField]
	private Camera _hiddenCamera;

	private CommandBuffer _depthHackBuffer;
	[SerializeField]
	private Renderer _depthHackQuad;

	public void SwapCameras()
	{
		_activeCamera.targetTexture = _hiddenCamera.targetTexture;
		_hiddenCamera.targetTexture = null;

		var swapCamera = _activeCamera;
		_activeCamera = _hiddenCamera;
		_hiddenCamera = swapCamera;

		DoDepthHack();
    }
	
	/// <summary>
	/// Set up our RT and 
	/// </summary>
	private void Awake()
	{
		var rt = new RenderTexture(Screen.width, Screen.height, 24);
		Shader.SetGlobalTexture("_TimeCrackTexture", rt);
		_hiddenCamera.targetTexture = rt;

		_depthHackBuffer = new CommandBuffer();
		_depthHackBuffer.ClearRenderTarget(true, true, Color.black, 0);
		_depthHackBuffer.name = "Fancy Depth Magic";
		_depthHackBuffer.DrawRenderer(_depthHackQuad, new Material(Shader.Find("Hidden/DepthHack")));

        DoDepthHack();
    }

	/// <summary>
	/// A depth buffer trick that makes drawing both scenes a bit nicer on the GPU.
	/// 
	/// The command buffer that we add clears the depth buffer to the minimum distance,
	/// then draws a quad which always passes the depth test, but fudges it's Z value to
	/// write the maximum distance into the buffer. This makes it so everything outside
	/// the quad it depth culled.
	/// </summary>
	private void DoDepthHack()
	{
		_hiddenCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _depthHackBuffer);
		_activeCamera.RemoveAllCommandBuffers();
    }
}
