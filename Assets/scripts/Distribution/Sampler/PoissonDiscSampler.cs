using System.Collections.Generic;
using UnityEngine;

namespace DistributionPrototype.Distribution.Sampler
{
	/// http://gregschlom.com/devlog/2014/06/29/Poisson-disc-sampling-Unity.html
	/// 
	/// Modified from the following:
	/// 
	/// Poisson-disc sampling using Bridson's algorithm.
	/// Adapted from Mike Bostock's Javascript source: http://bl.ocks.org/mbostock/19168c663618b7f07158
	///
	/// See here for more information about this algorithm:
	///   http://devmag.org.za/2009/05/03/poisson-disk-sampling/
	///   http://bl.ocks.org/mbostock/dbb02448b0f93e4c82c3
	///
	/// Usage:
	///   PoissonDiscSampler sampler = new PoissonDiscSampler(10, 5, 0.3f);
	///   foreach (Vector2 sample in sampler.Samples()) {
	///       // ... do something, like instantiate an object at (sample.x, sample.y) for example:
	///       Instantiate(someObject, new Vector3(sample.x, 0, sample.y), Quaternion.identity);
	///   }
	///
	/// Author: Gregory Schlomoff (gregory.schlomoff@gmail.com)
	/// Released in the public domain
	public class PoissonDiscSampler : ISampler
	{
		private const int k = 30; // Maximum number of attempts before marking a sample as inactive.

		private readonly Rect _rect;
		private readonly float _radius2; // radius squared
		private readonly float _cellSize;
		private Vector2[,] _grid;
		private List<Vector2> _activeSamples = new List<Vector2>();

		/// Create a sampler with the following parameters:
		///
		/// width:  each sample's x coordinate will be between [0, width]
		/// height: each sample's y coordinate will be between [0, height]
		/// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
		public PoissonDiscSampler(float width, float height, float radius)
		{
			_rect = new Rect(0, 0, width, height);
			_radius2 = radius * radius;
			_cellSize = radius / Mathf.Sqrt(2);
			Debug.Log(string.Format("width {0}, height {1}, radius {2}, cellsize {3}", width, height, radius, _cellSize));
			_grid = new Vector2[Mathf.CeilToInt(width / _cellSize),
				Mathf.CeilToInt(height / _cellSize)];
		}

		/// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
		///   foreach (Vector2 sample in sampler.Samples()) { ... }
		public IEnumerable<Vector2> Samples()
		{
			// First sample is choosen randomly
			yield return AddSample(new Vector2(Random.value * _rect.width, Random.value * _rect.height));

			while (_activeSamples.Count > 0)
			{
				// Pick a random active sample
				int i = (int) Random.value * _activeSamples.Count;
				Vector2 sample = _activeSamples[i];

				// Try `k` random candidates between [radius, 2 * radius] from that sample.
				bool found = false;
				for (int j = 0; j < k; ++j)
				{
					float angle = 2 * Mathf.PI * Random.value;
					var rad = _radius2; // *Mathf.PerlinNoise(sample.x, sample.y);
					float
						r = Mathf.Sqrt(Random.value * 3 * rad +
						               rad); // *Mathf.PerlinNoise(sample.x, sample.y); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
					Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

					// Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
					if (_rect.Contains(candidate) && IsFarEnough(candidate, rad))
					{
						found = true;
						yield return AddSample(candidate);
						break;
					}
				}

				// If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
				if (!found)
				{
					_activeSamples[i] = _activeSamples[_activeSamples.Count - 1];
					_activeSamples.RemoveAt(_activeSamples.Count - 1);
				}
			}
		}

		private bool IsFarEnough(Vector2 sample, float radius)
		{
			GridPos pos = new GridPos(sample, _cellSize);

			int xmin = Mathf.Max(pos.x - 2, 0);
			int ymin = Mathf.Max(pos.y - 2, 0);
			int xmax = Mathf.Min(pos.x + 2, _grid.GetLength(0) - 1);
			int ymax = Mathf.Min(pos.y + 2, _grid.GetLength(1) - 1);

			for (int y = ymin; y <= ymax; y++)
			{
				for (int x = xmin; x <= xmax; x++)
				{
					Vector2 s = _grid[x, y];
					if (s != Vector2.zero)
					{
						Vector2 d = s - sample;
						if (d.x * d.x + d.y * d.y < radius) return false;
					}
				}
			}

			return true;

			// Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
			// to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
			// and we might end up with another sample too close from (0, 0). This is a very minor issue.
		}

		/// Adds the sample to the active samples queue and the grid before returning it
		private Vector2 AddSample(Vector2 sample)
		{
			_activeSamples.Add(sample);
			GridPos pos = new GridPos(sample, _cellSize);
			_grid[pos.x, pos.y] = sample;
			return sample;
		}

		/// Helper struct to calculate the x and y indices of a sample in the grid
		private struct GridPos
		{
			public int x;
			public int y;

			public GridPos(Vector2 sample, float cellSize)
			{
				x = (int) (sample.x / cellSize);
				y = (int) (sample.y / cellSize);
			}
		}
	}
}