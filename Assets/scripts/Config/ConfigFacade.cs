#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DistributionPrototype.Config
{
	/// <summary>
	/// Provides access to configuration ScriptableObject instances 
	/// and values. This class could better encapsulate these instances
	/// to centralize and control modification, but for the scope of this 
	/// prototype the complexity overhead is probably not worth it.
	/// </summary>
	public class ConfigFacade : MonoBehaviour
	{
		[SerializeField]
		private NoiseConfig _sourceNoiseConfig;

		[SerializeField]
		private ObjectDistributionConfig _sourceDistributionConfig;

		[SerializeField]
		private bool _debugPerformance;

		private NoiseConfig _noiseConfig;
		private ObjectDistributionConfig _distributionConfig;

		public NoiseConfig NoiseConfig
		{
			get { return _noiseConfig; }
		}

		public ObjectDistributionConfig DistributionConfig
		{
			get { return _distributionConfig; }
		}

		public bool DebugPerformance
		{
			get { return _debugPerformance; }
		}

		private void Awake()
		{
			// Instantiate new copies, so the original 
			// ScriptableObjects remain unmodified
			_noiseConfig = Instantiate(_sourceNoiseConfig);
			_distributionConfig = Instantiate(_sourceDistributionConfig);
		}

		/// <summary>
		/// Saves all changes made to current instances to the actual
		/// ScriptableObject asset files.
		/// </summary>
		public void PersistChanges()
		{
#if UNITY_EDITOR
			var noiseConfigPath = AssetDatabase.GetAssetPath(_sourceNoiseConfig);
			var distributionConfigPath = AssetDatabase.GetAssetPath(_sourceDistributionConfig);

			_sourceNoiseConfig = Instantiate(_noiseConfig);
			_sourceDistributionConfig = Instantiate(_distributionConfig);

			AssetDatabase.CreateAsset(_sourceNoiseConfig, noiseConfigPath);
			AssetDatabase.CreateAsset(_sourceDistributionConfig, distributionConfigPath);
			AssetDatabase.SaveAssets();
#endif
		}
	}
}
