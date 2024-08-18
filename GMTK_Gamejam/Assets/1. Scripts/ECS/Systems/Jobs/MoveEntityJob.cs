using ECS.Components;
using ECS.Components.Aspects;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Jobs
{
	[BurstCompile]
	public partial struct MoveEntityJob : IJobEntity
	{
		public float DeltaTime;
		
		[BurstCompile]
		private void Execute(MovementAspect movementAspect)
		{
			// float3 moveDirection = DeltaTime * movementComponent.Speed * movementComponent.Direction;
			//
			// localTransform.Position += moveDirection;
			// 	
			// if (movementComponent.Speed > 0)
			// {
			// 	movementComponent.Speed -= 1 * SystemAPI.Time.DeltaTime;
			// }
			// else
			// {
			// 	movementComponent.Speed = 0;
			// }
			// 	
			// entityManager.SetComponentData(entity, movementComponent);
		}
	}
}