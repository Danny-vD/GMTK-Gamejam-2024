using Gameplay.Structs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.Scoring.HeightMeasurer
{
	public struct HeightMeasurementAreasManagerComponent : IComponentData
	{
		public NativeArray<ScoreAreaRaycastDataECS> RaycastPoints;
		
		public float3 RaycastDirection;
		public float RaycastDistance;
	}
}