namespace DistributionPrototype.Util
{
	[System.Serializable]
	public class Grid2D<T>
	{
		private int _width;
		private int _height;
		private T[] _arr;

		public int Width
		{
			get { return this._width; }
		}

		public int Height
		{
			get { return this._height; }
		}

		public int Count
		{
			get { return _arr.Length; }
		}

		public Grid2D() { }

		public Grid2D(int width, int height)
		{
			Initialize(width, height);
		}

		public void Initialize(int width, int height)
		{
			_width = width;
			_height = height;
			_arr = new T[width * height];
		}

		public void Initialize(int width, int height, T[] arr)
		{
			_width = width;
			_height = height;
			_arr = arr;
		}

		public T Get(int x, int y)
		{
			return Get(GetIndex(x, y));
		}

		public T Get(int index)
		{
			return _arr[index];
		}

		public void Set(int x, int y, T value)
		{
			Set(GetIndex(x, y), value);
		}

		public void Set(int index, T value)
		{
			_arr[index] = value;
		}

		public void Swap(int fromX, int fromY, int toX, int toY)
		{
			T temp = Get(fromX, fromY);
			Set(fromX, fromY, Get(toX, toY));
			Set(toX, toY, temp);
		}

		public int GetIndex(int x, int y)
		{
			return x + (y * _width);
		}

		public int GetX(int index)
		{
			return index % _width;
		}

		public int GetY(int index)
		{
			return index / _width;
		}
	}
}
