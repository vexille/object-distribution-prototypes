using UnityEngine;

namespace DistributionPrototype.UI.Components
{
	public class IntSliderField : BaseSliderField<int>
	{
		protected override float GetSliderValue(int value)
		{
			return Mathf.InverseLerp(_minValue, _maxValue, value);
		}

		protected override int GetValue(float sliderValue)
		{
			return (int)Mathf.Lerp(_minValue, _maxValue, sliderValue);
		}

		protected override string GetStringValue(int value)
		{
			return value.ToString();
		}
	}
}
