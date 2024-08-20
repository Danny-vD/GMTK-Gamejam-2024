using ECS.Components.DragNDrop.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace ECS.Components.MoveBackIfFallen.Aspect
{
	public readonly partial struct PotentialFallenBlockAspect : IAspect
	{
		public readonly Entity Entity;
		
		public readonly RefRW<LocalTransform> LocalTransformRW;
		
		public readonly RefRO<Simulate> SimulateRO;
		public readonly RefRO<DraggableTag> DraggableTag;
	}
}