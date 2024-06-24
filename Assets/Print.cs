using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(PrintRenderer), PostProcessEvent.AfterStack, "Custom/Print")]
public class Print : PostProcessEffectSettings
{
	[Range(0.0f, 1.0f), Tooltip("The input level cutoff between white and black")]
	public FloatParameter printBlackLevel = new FloatParameter
	{
		value = 0.5f
	};
	[Range(0.0f, 0.5f), Tooltip("Noise in the sampling process")]
	public FloatParameter sampleNoise = new FloatParameter
	{
		value = 0.1f
	};
	public ColorParameter inkColour = new ColorParameter
	{
		value = new Color(0.278f, 0.231f, 0.239f, 1.0f)
	};
	public ColorParameter paperColour = new ColorParameter
	{
		value = new Color(0.855f, 0.788f, 0.725f, 1.0f)
	};
	public TextureParameter watermark = new TextureParameter
	{
		value = null
	};
	public FloatParameter watermarkDepth = new FloatParameter
	{
		value = 0.1f
	};
	public Vector2Parameter watermarkOffset = new Vector2Parameter
	{
		value = new Vector2(1.0f, 1.0f)
	};
	public FloatParameter watermarkAngle = new FloatParameter
	{
		value = 0.0f
	};
	public FloatParameter watermarkScale = new FloatParameter
	{
		value = 1.0f
	};
	public Vector2Parameter watermarkPadding = new Vector2Parameter
	{
		value = new Vector2(1.0f, 1.0f)
	};
	public FloatParameter watermarkSkew = new FloatParameter
	{
		value = 1.0f
	};
}

public class PrintRenderer : PostProcessEffectRenderer<Print>
{
	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Print"));
		sheet.properties.SetFloat("_PrintBlackLevel", settings.printBlackLevel);
		sheet.properties.SetColor("_InkColour", settings.inkColour);
		sheet.properties.SetColor("_PaperColour", settings.paperColour);
		sheet.properties.SetFloat("_NoiseLevel", settings.sampleNoise);
		sheet.properties.SetFloat("_WatermarkDepth", settings.watermarkDepth);
		sheet.properties.SetVector("_WatermarkPadding", settings.watermarkPadding);
		sheet.properties.SetFloat("_WatermarkSkew", settings.watermarkSkew);
		Vector2 scale = new Vector2(1.0f * context.width / context.height, 1.0f) * settings.watermarkScale;
		Matrix4x4 matrix = Matrix4x4.TRS(settings.watermarkOffset, Quaternion.Euler(0.0f, 0.0f, settings.watermarkAngle), scale);
		sheet.properties.SetMatrix("_WatermarkMatrix", matrix);
		var watermark = settings.watermark.value;
		if (watermark == null)
		{
			watermark = Texture2D.whiteTexture;
		}
		sheet.properties.SetTexture("_Watermark", watermark);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
}
