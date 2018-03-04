using UnityEngine;

namespace DistributionPrototype.Util
{
	// Adapted from: https://github.com/Flafla2/Remote2D-Engine
	// https://github.com/Flafla2/Remote2D-Engine/blob/master/Remote2D/src/com/remote/remote2d/engine/logic/Noise2D.java
	public static class NoiseGenerator
	{
		public static Grid2D<float> UnityNoise(int width, int height)
		{
			float xOrigin = Random.Range(0f, 1000f);
			float yOrigin = Random.Range(0f, 1000f);
			Grid2D<float> noise = new Grid2D<float>(width, height);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					noise.Set(x, y, Mathf.PerlinNoise((xOrigin + x) / 10f, (yOrigin + y) / 10f));
				}
			}

			return noise;
		}

		public static Grid2D<float> WhiteNoise(int width, int height)
		{
			Grid2D<float> noise = new Grid2D<float>(width, height);

			for (int i = 0; i < width * height; i++)
			{
				noise.Set(i, Random.value);
			}

			return noise;
		}

		public static Grid2D<float> PerlinNoise(int width, int height, int octaveCount)
		{
			return PerlinNoise(WhiteNoise(width, height), octaveCount);
		}

		public static Grid2D<float> PerlinNoise(Grid2D<float> baseNoise, int octaveCount)
		{
			int width = baseNoise.Width;
			int height = baseNoise.Height;

			Grid2D<float>[] smoothNoiseList = new Grid2D<float>[octaveCount];

			// Generate smooth noises
			for (int i = 0; i < octaveCount; i++)
			{
				smoothNoiseList[i] = SmoothNoise(baseNoise, i);
			}

			Grid2D<float> perlinNoise = new Grid2D<float>(width, height);
			float amplitude = 1.0f;
			float totalAmplitude = 0.0f;
			float persistance = 0.5f;

			// Blend noises together
			for (int octave = octaveCount - 1; octave >= 0; octave--)
			{
				amplitude *= persistance;
				totalAmplitude += amplitude;

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						perlinNoise.Set(i, j, perlinNoise.Get(i, j) + smoothNoiseList[octave].Get(i, j) * amplitude);
					}
				}
			}

			// Normalisation
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					perlinNoise.Set(i, j, perlinNoise.Get(i, j) / totalAmplitude);
				}
			}

			return perlinNoise;
		}

		public static Grid2D<float> SmoothNoise(Grid2D<float> baseNoise, int octave)
		{
			int width = baseNoise.Width;
			int height = baseNoise.Height;

			Grid2D<float> noise = new Grid2D<float>(width, height);

			int samplePeriod = 1 << octave; // calculates 2 ^ k
			float sampleFrequency = 1.0f / samplePeriod;

			for (int i = 0; i < width; i++)
			{
				// Calculate the horizontal sampling indices
				int sample_i0 = (i / samplePeriod) * samplePeriod;
				int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
				float horizontal_blend = (i - sample_i0) * sampleFrequency;

				for (int j = 0; j < height; j++)
				{
					// Calculate the vertical sampling indices
					int sample_j0 = (j / samplePeriod) * samplePeriod;
					int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
					float vertical_blend = (j - sample_j0) * sampleFrequency;

					// Blend the top two corners
					float top = Mathf.Lerp(baseNoise.Get(sample_i0, sample_j0), baseNoise.Get(sample_i1, sample_j0), horizontal_blend);

					// Blend the bottom two corners
					float bottom = Mathf.Lerp(baseNoise.Get(sample_i0, sample_j1), baseNoise.Get(sample_i1, sample_j1),
						horizontal_blend);

					// Final blend
					noise.Set(i, j, Mathf.Lerp(top, bottom, vertical_blend));
				}
			}

			return noise;
		}

		public static Grid2D<float> ReduceEdges(Grid2D<float> baseNoise)
		{
			float a = 0.015f;
			float b = 1.09f;
			float c = 0.60f;

			int width = baseNoise.Width;
			int height = baseNoise.Height;

			Grid2D<float> result = new Grid2D<float>(width, height);
			float max = 0f;

			for (int i = 0; i < width; i++)
			{
				float centerPosX = i - (width * 0.5f);
				for (int j = 0; j < height; j++)
				{
					float current = baseNoise.Get(i, j);
					float centerPosY = j - (height * 0.5f);

					float distance =
						2 * Mathf.Sqrt(current * centerPosX * current * centerPosX +
						               current * centerPosY * current * centerPosY); // 2*sqrt(nx*nx + ny*ny) euclidian distance
					if (distance > max)
					{
						max = distance;
					}
					result.Set(i, j, distance);
				}
			}

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					float current = baseNoise.Get(i, j);
					float distance = result.Get(i, j) / max;
					result.Set(i, j, (current + a) * (1 - b * Mathf.Pow(distance, c)));
				}
			}

			return result;
		}
	}
}