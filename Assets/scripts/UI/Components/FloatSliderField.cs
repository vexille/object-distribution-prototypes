using UnityEngine;

namespace DistributionPrototype.UI.Components
{
	public class FloatSliderField : BaseSliderField<float>
	{
		protected override float GetSliderValue(float value)
		{
			return Mathf.InverseLerp(_minValue, _maxValue, value);
		}

		protected override float GetValue(float sliderValue)
		{
			return Mathf.Lerp(_minValue, _maxValue, sliderValue);
		}

		protected override string GetStringValue(float value)
		{
			return value.ToString("0.##");
		}
	}
}
