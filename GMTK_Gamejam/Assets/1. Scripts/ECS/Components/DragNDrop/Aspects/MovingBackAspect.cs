using ECS.Components.DragNDrop.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace ECS.Components.DragNDrop.Aspects
{
	public readonly partial struct MovingBackAspect : IAspect
	{
		public readonly Entity Entity;

		public readonly RefRW<LocalTransform> LocalTransformRW;
		
		public readonly RefRO<OriginalPositionComponent> OriginalPositionComponentRO;
		public readonly RefRO<MovementSpeedComponent> MovementSpeedComponentRO;
		public readonly RefRW<ShouldMoveBackTowardsOriginalPositionComponent> MoveTowardsOriginalPositionComponentRW;
	}
}