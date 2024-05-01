using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VhsNoiseFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class VhsNoiseSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        
        public float intensity = 0.01f;
        public float deathNoise = 0.2f;
        public int pixelizationFactor = 188;
    }

    [SerializeField] private VhsNoiseSettings settings;

    private VhsNoisePass vhsNoisePass;

    public override void Create()
    {
        vhsNoisePass = new VhsNoisePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(vhsNoisePass);
    }


    public void setIntensity(float intensity)
    {
        settings.intensity = settings.intensity + (intensity * settings.deathNoise);
    }
}
