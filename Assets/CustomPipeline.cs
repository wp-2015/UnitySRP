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
            /***********************************渲染天空盒***********************************/
            context.DrawSkybox(camera);

            /***********************************渲染物体***********************************/
            //渲染Layer--------获取filtSet
            FilteringSettings filtSet = new FilteringSettings(RenderQueueRange.all);

            //设置裁剪---------获取cullResults
            ScriptableCullingParameters cullParam = new ScriptableCullingParameters();
            camera.TryGetCullingParameters(out cullParam);
            CullingResults cullResults = context.Cull(ref cullParam);

            //设置渲染顺序-----获取drawSet
            SortingSettings sortSet = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
            //这里的CustomTag是与Shader里面对应的 Tags { "LightMode" = "CustomTag" }
            DrawingSettings drawSet = new DrawingSettings(new ShaderTagId("CustomTag"), sortSet);

            //设置光照信息，将光照信息传入Shader
            commandBuffer.Clear();
            commandBuffer.ClearRenderTarget(true, true, Color.gray);
            //将shader中需要的属性参数映射为ID，加速传参
            var _V4LightDir = Shader.PropertyToID("_V4LightDir");
            var _LightColor = Shader.PropertyToID("_LightColor");
            var _CameraPos = Shader.PropertyToID("_CameraPos");
            commandBuffer.SetGlobalVector(_CameraPos, camera.transform.position);
            var lights = cullResults.visibleLights;
            commandBuffer.name = "Render Lights";
            foreach (var light in lights)
            {
                //判断灯光类型
                if (light.lightType != LightType.Directional) continue;
                //获取灯光参数,平行光朝向即为灯光Z轴方向。矩阵第一到三列分别为xyz轴项，第四列为位置。
                Vector4 lightpos = light.localToWorldMatrix.GetColumn(2);
                //灯光方向反向。默认管线中，unity提供的平行光方向也是灯光反向。光照计算决定
                Vector4 lightDir = -lightpos;
                //方向的第四个值(W值)为0，点为1.
                lightDir.w = 0;
                //这边获取的灯光的finalColor是灯光颜色乘上强度之后的值，也正好是shader需要的值
                Color lightColor = light.finalColor;
                //利用CommandBuffer进行参数传递。
                commandBuffer.SetGlobalVector(_V4LightDir, lightDir);
                commandBuffer.SetGlobalVector(_LightColor, lightColor);
                break;
            }
            //执行CommandBuffer中的指令
            context.ExecuteCommandBuffer(commandBuffer);

            //最终设置
            context.DrawRenderers(cullResults, ref drawSet, ref filtSet);

            /***********************************提交***********************************/
            context.Submit();
        }
    }
}
