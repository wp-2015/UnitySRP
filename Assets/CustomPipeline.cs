using UnityEngine;
using UnityEngine.Rendering;

public class CustomPipeline : RenderPipeline
{
    private CommandBuffer commandBuffer = new CommandBuffer() { name = "CustomCommandBuffer" };
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {

        foreach(var camera in cameras)
        {

            context.SetupCameraProperties(camera);
            /***********************************��Ⱦ��պ�***********************************/
            context.DrawSkybox(camera);

            /***********************************��Ⱦ����***********************************/
            //��ȾLayer--------��ȡfiltSet
            FilteringSettings filtSet = new FilteringSettings(RenderQueueRange.all);

            //���òü�---------��ȡcullResults
            ScriptableCullingParameters cullParam = new ScriptableCullingParameters();
            camera.TryGetCullingParameters(out cullParam);
            CullingResults cullResults = context.Cull(ref cullParam);

            //������Ⱦ˳��-----��ȡdrawSet
            SortingSettings sortSet = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
            //�����CustomTag����Shader�����Ӧ�� Tags { "LightMode" = "CustomTag" }
            DrawingSettings drawSet = new DrawingSettings(new ShaderTagId("CustomTag"), sortSet);

            //���ù�����Ϣ����������Ϣ����Shader
            commandBuffer.Clear();
            commandBuffer.ClearRenderTarget(true, true, Color.gray);
            //��shader����Ҫ�����Բ���ӳ��ΪID�����ٴ���
            var _V4LightDir = Shader.PropertyToID("_V4LightDir");
            var _LightColor = Shader.PropertyToID("_LightColor");
            var _CameraPos = Shader.PropertyToID("_CameraPos");
            commandBuffer.SetGlobalVector(_CameraPos, camera.transform.position);
            var lights = cullResults.visibleLights;
            commandBuffer.name = "Render Lights";
            foreach (var light in lights)
            {
                //�жϵƹ�����
                if (light.lightType != LightType.Directional) continue;
                //��ȡ�ƹ����,ƽ�й⳯��Ϊ�ƹ�Z�᷽�򡣾����һ�����зֱ�Ϊxyz���������Ϊλ�á�
                Vector4 lightpos = light.localToWorldMatrix.GetColumn(2);
                //�ƹⷽ����Ĭ�Ϲ����У�unity�ṩ��ƽ�йⷽ��Ҳ�ǵƹⷴ�򡣹��ռ������
                Vector4 lightDir = -lightpos;
                //����ĵ��ĸ�ֵ(Wֵ)Ϊ0����Ϊ1.
                lightDir.w = 0;
                //��߻�ȡ�ĵƹ��finalColor�ǵƹ���ɫ����ǿ��֮���ֵ��Ҳ������shader��Ҫ��ֵ
                Color lightColor = light.finalColor;
                //����CommandBuffer���в������ݡ�
                commandBuffer.SetGlobalVector(_V4LightDir, lightDir);
                commandBuffer.SetGlobalVector(_LightColor, lightColor);
                break;
            }
            //ִ��CommandBuffer�е�ָ��
            context.ExecuteCommandBuffer(commandBuffer);

            //��������
            context.DrawRenderers(cullResults, ref drawSet, ref filtSet);

            /***********************************�ύ***********************************/
            context.Submit();
        }
    }
}
