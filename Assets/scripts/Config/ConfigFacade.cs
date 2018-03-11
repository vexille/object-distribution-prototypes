#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zenject;

namespace DistributionPrototype.Config
{
	/// <summary>
	/// Provides access to configuration ScriptableObject instances 
	/// and values. This class could better encapsulate these instances
	/// to provide a central point for modifications, but for the scope of this 
	/// prototype the complexity overhead is probably not worth it.
	/// </summary>
	public class ConfigFacade
	{
		private readonly NoiseConfig _noiseConfig;
		private readonly ObjectDistributionConfig _distributionConfig;
		private readonly bool _debugPerformance;

		private NoiseConfig _sourceNoiseConfig;
		private ObjectDistributionConfig _sourceDistributionConfig;

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

		public ConfigFacade(
			NoiseConfig noiseConfig,
			ObjectDistributionConfig distributionConfig,
			[Inject(Id = "debugPerformance")] bool debugPerformance)
		{
			_sourceNoiseConfig = noiseConfig;
			_sourceDistributionConfig = distributionConfig;

			// Instantiate new copies, so the original 
			// ScriptableObjects remain unmodified
			_noiseConfig = Object.Instantiate(noiseConfig);
			_distributionConfig = Object.Instantiate(distributionConfig);
			_debugPerformance = debugPerformance;
		}

		/// <summary>
		/// Saves all changes made to current instances to the actual
		/// ScriptableObject asset files.
		/// 
		/// Note: Functionality only available in the editor.
		/// </summary>
		public void PersistChanges()
		{
#if UNITY_EDITOR
			var noiseConfigPath = AssetDatabase.GetAssetPath(_sourceNoiseConfig);
			var distributionConfigPath = AssetDatabase.GetAssetPath(_sourceDistributionConfig);

			// Copy the in-memory values of the config objects back into the source instances
			_sourceNoiseConfig = Object.Instantiate(_noiseConfig);
			_sourceDistributionConfig = Object.Instantiate(_distributionConfig);

			// Persist back to the file system
			AssetDatabase.CreateAsset(_sourceNoiseConfig, noiseConfigPath);
			AssetDatabase.CreateAsset(_sourceDistributionConfig, distributionConfigPath);
			AssetDatabase.SaveAssets();
#endif
		}
	}
}
