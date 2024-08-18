using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Components.Aspects
{
	public readonly partial struct MovementAspect : IAspect
	{
		public MovementComponent MovementComponentRO => movementComponentReference.ValueRO;
		public LocalTransform LocalTransformRO => localTransformReference.ValueRO;
		
		public readonly Entity Entity;

		private readonly RefRW<LocalTransform> localTransformReference;
		private readonly RefRW<MovementComponent> movementComponentReference;

		public void Translate(float3 delta, float speedDecrease)
		{
			localTransformReference.ValueRW.Position += delta;

			movementComponentReference.ValueRW.Speed -= speedDecrease;
		}
	}
}