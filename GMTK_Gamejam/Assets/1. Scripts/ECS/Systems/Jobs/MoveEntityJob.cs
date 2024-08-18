using ECS.Components;
using ECS.Components.Aspects;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems.Jobs
{
	[BurstCompile]
	public partial struct MoveEntityJob : IJobEntity
	{
		public float DeltaTime;
		public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;
		
		[BurstCompile]
		private void Execute(MovementAspect movementAspect, [EntityIndexInQuery] int sortkey)
		{
			MovementComponent movementComponent = movementAspect.movementComponentRW.ValueRO;
			float3 moveDirection = DeltaTime * movementComponent.Speed * movementComponent.Direction;

			movementAspect.Translate(moveDirection);
			movementAspect.movementComponentRW.ValueRW.Speed -= 1 * DeltaTime;

			if (movementAspect.movementComponentRW.ValueRO.Speed <= 0)
			{
				EntityCommandBuffer.RemoveComponent<MovementComponent>(sortkey, movementAspect.Entity);
			}
		}
	}
}