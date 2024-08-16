using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
{
	[BurstCompile]
	public partial struct MovementSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			EntityManager entityManager = state.EntityManager;

			NativeArray<Entity> allEntities = entityManager.GetAllEntities(Allocator.Temp);
			
			foreach (Entity entity in allEntities)
			{
				if (!entityManager.HasComponent<MovementComponent>(entity))
				{
					continue;
				}

				MovementComponent movementComponent = entityManager.GetComponentData<MovementComponent>(entity);
				LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

				float3 moveDirection = SystemAPI.Time.DeltaTime * movementComponent.Speed * movementComponent.Direction;

				localTransform.Position += moveDirection;
				entityManager.SetComponentData(entity, localTransform);
				
				if (movementComponent.Speed > 0)
				{
					movementComponent.Speed -= 1 * SystemAPI.Time.DeltaTime;
				}
				else
				{
					movementComponent.Speed = 0;
				}
				
				entityManager.SetComponentData(entity, movementComponent);
			}
		}
	}
}