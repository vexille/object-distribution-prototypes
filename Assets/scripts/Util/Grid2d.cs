namespace DistributionPrototype.Util
{
	/// <summary>
	/// Represents a bi-dimensional grid that can be accessed either by
	/// index or 2D coordinate, while using a regular array as the
	/// underlying data structure.
	/// </summary>
	/// <typeparam name="T">Type of the elements in the grid</typeparam>
	[System.Serializable]
	public class Grid2D<T>
	{
		private int _width;
		private int _height;
		private T[] _arr;

		/// <summary>
		/// Gets the defined width of the grid.
		/// </summary>
		public int Width
		{
			get { return this._width; }
		}

		/// <summary>
		/// Gets the defined height of the grid.
		/// </summary>
		public int Height
		{
			get { return this._height; }
		}

		/// <summary>
		/// Gets the total count of elements in the grid. <see cref="Count"/>
		/// will always be equal to <see cref="Width"/> x <see cref="Height"/>
		/// </summary>
		public int Count
		{
			get { return _arr.Length; }
		}

		/// <summary>
		/// Creates an uninitialized grid.
		/// </summary>
		public Grid2D()
		{
		}

		/// <summary>
		/// Creates and initializes a grid with the given dimensions.
		/// </summary>
		/// <param name="width">Width of the grid</param>
		/// <param name="height">Height of the grid</param>
		public Grid2D(int width, int height)
		{
			Initialize(width, height);
		}

		/// <summary>
		/// Initializes the grid with the given dimensions.
		/// </summary>
		/// <param name="width">Width of the grid</param>
		/// <param name="height">Height of the grid</param>
		public void Initialize(int width, int height)
		{
			_width = width;
			_height = height;
			_arr = new T[width * height];
		}

		/// <summary>
		/// Gets element at the given coordinates.
		/// </summary>
		/// <param name="x">x position of the element</param>
		/// <param name="y">y position of the element</param>
		/// <returns>Element at requested coordinates</returns>
		public T Get(int x, int y)
		{
			return Get(GetIndex(x, y));
		}

		/// <summary>
		/// Gets element at the given index of the underlying array.
		/// </summary>
		/// <param name="index">index of the element</param>
		/// <returns>Element at requested index</returns>
		public T Get(int index)
		{
			return _arr[index];
		}

		/// <summary>
		/// Sets the value of the element at the given coordinates.
		/// </summary>
		/// <param name="x">x position of the element to set</param>
		/// <param name="y">y position of the element to set</param>
		/// <param name="value">New value of the element</param>
		public void Set(int x, int y, T value)
		{
			Set(GetIndex(x, y), value);
		}

		/// <summary>
		/// Sets the value of the element at the given index.
		/// </summary>
		/// <param name="index">index of the element to set</param>
		/// <param name="value">New value of the element</param>
		public void Set(int index, T value)
		{
			_arr[index] = value;
		}

		/// <summary>
		/// Swap places of the elements at the given coordinates.
		/// </summary>
		/// <param name="fromX">x position of the first element</param>
		/// <param name="fromY">y position of the first element</param>
		/// <param name="toX">x position of the second element</param>
		/// <param name="toY">y position of the second element</param>
		public void Swap(int fromX, int fromY, int toX, int toY)
		{
			T temp = Get(fromX, fromY);
			Set(fromX, fromY, Get(toX, toY));
			Set(toX, toY, temp);
		}

		/// <summary>
		/// Gets the index value of a given coordinate set. Converts the values
		/// based on <see cref="Count"/>, <see cref="Width"/> and <see cref="Height"/>.
		/// </summary>
		/// <param name="x">x position of the coordinate to convert</param>
		/// <param name="y">y position of the coordinate to convert</param>
		/// <returns>Corresponding index value</returns>
		public int GetIndex(int x, int y)
		{
			return x + (y * _width);
		}

		/// <summary>
		/// Gets the x position value of a given index. Converts based on the values
		/// from <see cref="Count"/>, <see cref="Width"/> and <see cref="Height"/>.
		/// </summary>
		/// <param name="index">index value to convert</param>
		/// <returns>x position of the given index</returns>
		public int GetX(int index)
		{
			return index % _width;
		}

		/// <summary>
		/// Gets the y position value of a given index. Converts based on the values
		/// from <see cref="Count"/>, <see cref="Width"/> and <see cref="Height"/>.
		/// </summary>
		/// <param name="index">index value to convert</param>
		/// <returns>x position of the given index</returns>
		public int GetY(int index)
		{
			return index / _width;
		}
	}
}