// VolumetricFogFeature.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumetricFogFeature : ScriptableRendererFeature
{
    class VolumetricFogPass : ScriptableRenderPass
    {
        private Material fogMaterial;
        private int tempRTID;
        private string profilerTag = "VolumetricFog";

        public VolumetricFogPass(Material mat)
        {
            fogMaterial = mat;
            tempRTID = Shader.PropertyToID("_TempVolumetricFogRT");
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            // Safely get the current camera color target
            RenderTargetIdentifier cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Allocate temporary RT
            cmd.GetTemporaryRT(tempRTID, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Bilinear);

            // Blit camera color to temp RT and apply fog
            cmd.Blit(cameraColorTarget, tempRTID, fogMaterial);

            // Blit back to camera color target
            cmd.Blit(tempRTID, cameraColorTarget);

            // Release temporary RT
            cmd.ReleaseTemporaryRT(tempRTID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [System.Serializable]
    public class VolumetricFogSettings
    {
        public Material fogMaterial = null;
    }

    public VolumetricFogSettings settings = new VolumetricFogSettings();
    private VolumetricFogPass fogPass;

    public override void Create()
    {
        if (settings.fogMaterial != null)
        {
            fogPass = new VolumetricFogPass(settings.fogMaterial);
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (fogPass != null)
        {
            renderer.EnqueuePass(fogPass);
        }
    }
}
