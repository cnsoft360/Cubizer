﻿using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Terrain))]
	public class TerrainCreator : MonoBehaviour
	{
		public int _chunkRadiusGC = 6;
		public int _chunkNumLimits = 512;

		public Vector2Int _chunkRadiusGenX = new Vector2Int(-3, 3);
		public Vector2Int _chunkRadiusGenY = new Vector2Int(-1, 3);
		public Vector2Int _chunkRadiusGenZ = new Vector2Int(-3, 3);

		public int _terrainSeed = 255;
		public int _terrainHeightLimitLow = -10;
		public int _terrainHeightLimitHigh = 20;

		public float _repeatRateUpdate = 0.1f;

		private Terrain _terrain;
		private GameObject _terrainGenerator;

		private void OnEnable()
		{
			StartCoroutine("UpdateChunkWithCoroutine");
		}

		private void OnDisable()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		private void Awake()
		{
			Math.Noise.simplex_seed(_terrainSeed);
		}

		private void Start()
		{
			_terrain = GetComponent<Terrain>();
			_terrainGenerator = GameObject.Find("TerrainGenerator");

			if (_terrainGenerator == null)
				UnityEngine.Debug.LogError("Please drag a TerrainGenerator into Hierarchy View.");
		}

		private void Reset()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		private IEnumerator UpdateChunkWithCoroutine()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			Vector2Int[] radius = new Vector2Int[] { _chunkRadiusGenX, _chunkRadiusGenY, _chunkRadiusGenZ };

			_terrain.UpdateChunkForDestroy(transform, _chunkRadiusGC);
			_terrain.UpdateChunkForCreate(transform, _terrainGenerator, radius, _chunkNumLimits, _terrainHeightLimitLow, _terrainHeightLimitHigh);

			StartCoroutine("UpdateChunkWithCoroutine");
		}
	}
}