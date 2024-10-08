﻿using ECS.Components;
using ECS.Systems.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems
{
	[BurstCompile, RequireMatchingQueriesForUpdate]
	public partial struct MovementSystem : ISystem
	{
		private EntityQuery movementEntityQuery;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			movementEntityQuery = state.GetEntityQuery(ComponentType.ReadOnly<MovementComponent>());
			state.RequireForUpdate(movementEntityQuery);
		}

		// [BurstCompile]
		// public void OnUpdate(ref SystemState state)
		// {
		// 	EntityManager entityManager = state.EntityManager;
		// 	
		// 	foreach (Entity entity in movementEntityQuery.ToEntityArray(Allocator.Temp))
		// 	{
		// 		MovementComponent movementComponent = entityManager.GetComponentData<MovementComponent>(entity);
		// 		LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);
		// 		
		// 		float3 moveDirection = SystemAPI.Time.DeltaTime * movementComponent.Speed * movementComponent.Direction;
		//
		// 		localTransform.Position += moveDirection;
		// 		entityManager.SetComponentData(entity, localTransform);
		// 		
		// 		if (movementComponent.Speed > 0)
		// 		{
		// 			movementComponent.Speed -= 1 * SystemAPI.Time.DeltaTime;
		// 		}
		// 		else
		// 		{
		// 			movementComponent.Speed = 0;
		// 		}
		// 		
		// 		entityManager.SetComponentData(entity, movementComponent);
		// 	}
		// }
		
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			float deltaTime = SystemAPI.Time.DeltaTime; // Supposedly a bug when doing this directly?
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			new MoveEntityJob()
			{
				DeltaTime = deltaTime,
				EntityCommandBuffer = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
			}.ScheduleParallel();
		}
	}
}