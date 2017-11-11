﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Terrain))]
	public class TerrainLoader : MonoBehaviour
	{
		public string rootPath;
		public string username = "/SaveData/";

		private Terrain _terrain;

		private void Start()
		{
			_terrain = GetComponent<Terrain>();
			_terrain.onSaveData += this.OnSaveData;
			_terrain.onLoadData += this.OnLoadData;

			rootPath = Application.persistentDataPath;

			CreateDirectory(rootPath);
		}

		private void Reset()
		{
			rootPath = Application.persistentDataPath;
		}

		private void OnSaveData(GameObject chunk)
		{
			var data = chunk.GetComponent<TerrainData>();
			if (data != null)
			{
				var archive = "chunk" + "_" + data.voxels.position.x + "_" + data.voxels.position.y + "_" + data.voxels.position.z;
				CreateFile(archive, data.voxels);
			}
		}

		private bool OnLoadData(Vector3Int position, out ChunkData chunk)
		{
			var archive = "chunk" + "_" + position.x + "_" + position.y + "_" + position.z;
			if (IsFileExists(archive))
				chunk = Load(archive);
			else
				chunk = null;

			return chunk != null;
		}

		private bool IsFileExists(string archive)
		{
			return File.Exists(rootPath + username + archive);
		}

		private bool IsDirectoryExists(string archive)
		{
			return Directory.Exists(archive);
		}

		private void CreateDirectory(string archive)
		{
			if (IsDirectoryExists(archive))
				return;
			Directory.CreateDirectory(archive);
		}

		private void CreateFile(string archive, ChunkData data)
		{
			UnityEngine.Debug.Assert(data != null);

			using (var stream = new FileStream(rootPath + username + archive, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, data.voxels);
			}
		}

		private ChunkData Load(string archive)
		{
			using (var stream = new FileStream(rootPath + username + archive, FileMode.Open, FileAccess.Read))
			{
				return new BinaryFormatter().Deserialize(stream) as ChunkData;
			}
		}
	}
}