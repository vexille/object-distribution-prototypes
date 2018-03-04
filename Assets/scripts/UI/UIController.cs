using DistributionPrototype.Config;
using DistributionPrototype.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DistributionPrototype.UI
{
	public class UIController : MonoBehaviour
	{
		public event Action OnGenerate;
		
		private Dictionary<NoiseType, NoiseDisplayPanel> _noiseDisplayPanels;
		
		private void Awake()
		{
			_noiseDisplayPanels = new Dictionary<NoiseType, NoiseDisplayPanel>();
			var panels = gameObject.GetComponentsInChildren<NoiseDisplayPanel>(true);
			foreach (var panel in panels)
			{
				_noiseDisplayPanels.Add(panel.Type, panel);
			}
		}

		public void RenderNoise(NoiseType type, Grid2D<float> noise, float threshold)
		{
			_noiseDisplayPanels[type].RenderNoise(noise, threshold);

			// TODO: I'll fix this, I swear!
			switch (type)
			{
				case NoiseType.Unity:
					_noiseDisplayPanels[NoiseType.Unity].gameObject.SetActive(true);
					_noiseDisplayPanels[NoiseType.Custom].gameObject.SetActive(false);
					break;
				case NoiseType.Custom:
					_noiseDisplayPanels[NoiseType.Unity].gameObject.SetActive(false);
					_noiseDisplayPanels[NoiseType.Custom].gameObject.SetActive(true);
					break;
				default:
					break;
			}
		}

		public void OnGenerateClick()
		{
			if (OnGenerate != null) OnGenerate();
		}
	}
}