using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Custom/CustomPipeline")]
public class CustomPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomPipeline();
    }
}