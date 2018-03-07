using System;
using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype.UI.Components
{
	public abstract class BaseSliderField<T> : MonoBehaviour
		// Roughly limit concrete implementations to numeric types
		where T : struct,
				  IComparable,
				  IComparable<T>
	{
		public delegate void ValueChangedCallback(T newValue);

		[SerializeField] private Slider _slider;
		[SerializeField] private Text _valueText;

		protected T _minValue;
		protected T _maxValue;

		private ValueChangedCallback _callback;

		public void Initialize(T currentValue, T minValue, T maxValue,
			ValueChangedCallback callback)
		{
			_callback = callback;
			_minValue = minValue;
			_maxValue = maxValue;

			_valueText.text = GetStringValue(currentValue);
			_slider.value = GetSliderValue(currentValue);

			_slider.onValueChanged.AddListener(OnSliderChanged);
		}

		/// <summary>
		/// Calculate the slider value [0,1] based on the given
		/// T typed value.
		/// </summary>
		/// <param name="value">Value to be converted</param>
		/// <returns>Normalized slider value</returns>
		protected abstract float GetSliderValue(T value);

		/// <summary>
		/// Calculate the T typed value based on the given
		/// slider value [0,1]
		/// </summary>
		/// <param name="sliderValue">Normalized slider value to be converted</param>
		/// <returns>Corresponding T value</returns>
		protected abstract T GetValue(float sliderValue);

		/// <summary>
		/// Convert the given T value to a string to provide more
		/// flexibility than a regular ToString() call
		/// </summary>
		/// <param name="value">T value to be converted</param>
		/// <returns>Corresponding string value</returns>
		protected abstract string GetStringValue(T value);

		private void OnSliderChanged(float sliderValue)
		{
			T newValue = GetValue(sliderValue);
			_valueText.text = GetStringValue(newValue);
			_callback(newValue);
		}
	}
}
