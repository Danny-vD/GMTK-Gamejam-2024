using ECS.Components.DragNDrop.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace ECS.Components.DragNDrop.Aspects
{
	public readonly partial struct DraggingAspect : IAspect
	{
		public readonly Entity Entity;

		public readonly RefRW<LocalTransform> LocalTransformRW;
		public readonly RefRO<IsDraggedTag> IsDraggedTagRO;
	}
}