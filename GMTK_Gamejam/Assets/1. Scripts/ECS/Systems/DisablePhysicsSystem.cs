﻿using ECS.Components.PhysicsSimulation.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Systems
{
	[RequireMatchingQueriesForUpdate, BurstCompile, UpdateAfter(typeof(DragNDropSystem))]
	public partial struct DisablePhysicsSystem : ISystem
	{
		private EntityQuery startSimulatingQuery;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			startSimulatingQuery = state.GetEntityQuery(ComponentType.ReadWrite<ShouldStopSimulatingTag>());
			
			state.RequireForUpdate<ShouldStopSimulatingTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

			foreach (Entity entity in startSimulatingQuery.ToEntityArray(Allocator.Temp))
			{
				ecb.SetComponentEnabled<Simulate>(entity, false);
				ecb.RemoveComponent<ShouldStopSimulatingTag>(entity);
			}
		}
	}
}