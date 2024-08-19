using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Components.Aspects
{
	public readonly partial struct MovementAspect : IAspect
	{
		public readonly Entity Entity;

		public readonly RefRW<LocalTransform> localTransformRW;
		public readonly RefRW<MovementComponent> movementComponentRW;

		public void Translate(float3 delta)
		{
			localTransformRW.ValueRW.Position += delta;
		}
	}
}