﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cubizer
{
	using Vector3Int = Math.Vector3<int>;

	[Serializable]
	public class ChunkTreeManager : ChunkMapShort3<ChunkTree>
	{
		public ChunkTreeManager(Vector3Int size)
			: base(size)
		{
		}

		public ChunkTreeManager(Vector3Int size, int allocSize = 0xFF)
			: base(size, allocSize)
		{
		}

		public ChunkTreeManager(int bound_x, int bound_y, int bound_z, int allocSize = 0xFF)
			: base(bound_x, bound_y, bound_z, allocSize)
		{
		}

		public new bool Set(System.Int16 x, System.Int16 y, System.Int16 z, ChunkTree value)
		{
			if (_allocSize == 0)
				this.Create(0xFF);

			var index = HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					var element = _data[index].element;
					if (element != value && element != null)
						element.OnChunkDestroy();

					_data[index].element = value;

					return true;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (value != null)
			{
				_data[index] = new ChunkNode<Math.Vector3<System.Int16>, ChunkTree>(new Math.Vector3<System.Int16>(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public new FileStream Save(string path)
		{
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			var serializer = new BinaryFormatter();

			serializer.Serialize(stream, this);
			stream.Close();

			return stream;
		}

		public new ChunkTreeManager Load(string path)
		{
			var serializer = new BinaryFormatter();
			var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);

			return serializer.Deserialize(loadFile) as ChunkTreeManager;
		}

		private static int _hash_int(int key)
		{
			key = ~key + (key << 15);
			key = key ^ (key >> 12);
			key = key + (key << 2);
			key = key ^ (key >> 4);
			key = key * 2057;
			key = key ^ (key >> 16);
			return key;
		}

		public static int HashInt(int x, int y, int z)
		{
			return _hash_int(x) ^ _hash_int(y) ^ _hash_int(z);
		}
	}
}