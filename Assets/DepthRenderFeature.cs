using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class ReplaceMaterialSettings
    {
        public Material replacementMaterial = null;
    }
    static ShaderTagId[] s_ShaderTagIds => new ShaderTagId[]
       {
        new ShaderTagId("SRPDefaultUnlit"),
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM"),
        new ShaderTagId("UniversalForward")
       };
    public ReplaceMaterialSettings settings = new ReplaceMaterialSettings();

    class ReplaceMaterialRenderPass : ScriptableRenderPass
    {
        private Material replacementMaterial;

        public ReplaceMaterialRenderPass(Material replacementMaterial)
        {
            this.replacementMaterial = replacementMaterial;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("ReplaceMaterialPass");
            using (new ProfilingScope(cmd, new ProfilingSampler("ReplaceMaterialPass")))
            {
                var drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
                drawingSettings.overrideMaterial = replacementMaterial;

                for (int i = 0; i < s_ShaderTagIds.Length; ++i)
                {
                    drawingSettings.SetShaderPassName(i, s_ShaderTagIds[i]);
                }
                cmd.ClearRenderTarget(true, true, Color.white);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);

                var filteringSettings = new FilteringSettings(RenderQueueRange.all);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }
            

        }
    }

    ReplaceMaterialRenderPass replaceMaterialPass;

    public override void Create()
    {
        replaceMaterialPass = new ReplaceMaterialRenderPass(settings.replacementMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques
        };

        replaceMaterialPass.renderPassEvent = RenderPassEvent.AfterRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(replaceMaterialPass);
    }
}