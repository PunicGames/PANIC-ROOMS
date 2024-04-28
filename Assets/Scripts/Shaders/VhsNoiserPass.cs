using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VhsNoisePass : ScriptableRenderPass
{
    private VhsNoiseFeature.VhsNoiseSettings settings;
    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    private Material material;
    private int pixelScreenHeight, pixelScreenWidth;

    public VhsNoisePass(VhsNoiseFeature.VhsNoiseSettings settings)
    {
        this.settings = settings;
        renderPassEvent = settings.renderPassEvent;
        if (material == null)
            material = CoreUtils.CreateEngineMaterial("Hidden/VhsNoise");
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        pixelScreenHeight = settings.pixelizationFactor;
        pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

        material.SetFloat("_Intensity", settings.intensity);
        material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
        material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        material.SetVector("HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

        descriptor.height = pixelScreenHeight;
        descriptor.width = pixelScreenWidth;

        cmd.GetTemporaryRT(pixelBufferID, descriptor.width, descriptor.height, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        // Debuggig and profiling
        using (new ProfilingScope(cmd, new ProfilingSampler("VhsNoise Pass")))
        {
            cmd.Blit(colorBuffer, pixelBuffer, material);
            cmd.Blit(pixelBuffer, colorBuffer);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(pixelBufferID);
    }
}
