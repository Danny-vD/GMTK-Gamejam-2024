using System;
using Gameplay.Enums;
using Unity.Mathematics;
using UnityEngine;

namespace Gameplay.Structs
{
	[Serializable]
	public struct ScoreAreaRaycastData
	{
		public static implicit operator ScoreAreaRaycastDataECS(ScoreAreaRaycastData data)
		{
			return new ScoreAreaRaycastDataECS()
			{
				OriginPoint       = data.OriginPoint.position,
				ScoringMultiplier = data.ScoringMultiplier,
				AreaRadius        = data.AreaRadius,
			};
		}
		
		public Transform OriginPoint;
		public MultiplierName ScoringMultiplier;
		public float AreaRadius;
	}
	
	public struct ScoreAreaRaycastDataECS
	{
		public float3 OriginPoint;
		public MultiplierName ScoringMultiplier;
		public float AreaRadius;
	}
}