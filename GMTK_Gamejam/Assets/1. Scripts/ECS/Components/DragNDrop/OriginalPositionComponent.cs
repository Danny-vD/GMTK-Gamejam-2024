using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.DragNDrop
{
	public struct OriginalPositionComponent : IComponentData
	{
		public float DistanceBeforeStopMoving;
		
		public float3 OriginalPosition;
	}
}