using Gameplay.Enums;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Scoring.HeightMeasurer
{
	public struct HeightMeasureAreaComponent : IComponentData
	{
		public float3 RaycastDirection;
		public MultiplierName ScoringMultiplier;
		public float AreaSize;
		public float RaycastDistance;
	}
}