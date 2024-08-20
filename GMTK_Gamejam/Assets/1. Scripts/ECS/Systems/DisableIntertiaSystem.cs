using ECS.Authoring;
using ECS.Components.DragNDrop.Tags;
using ECS.Components.PhysicsSimulation.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace ECS.Systems
{
	[BurstCompile, RequireMatchingQueriesForUpdate, UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial struct DisableIntertiaSystem : ISystem
	{
		private EntityQuery disableIntertiaQuery;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			disableIntertiaQuery = state.GetEntityQuery(ComponentType.ReadWrite<ShouldDisableInertiaXYTag>(), ComponentType.ReadOnly<DraggableTag>(), ComponentType.ReadWrite<PhysicsMass>());

			state.RequireForUpdate<ShouldDisableInertiaXYTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

			foreach (Entity entity in disableIntertiaQuery.ToEntityArray(Allocator.Temp))
			{
				PhysicsMass physicsMass = SystemAPI.GetComponent<PhysicsMass>(entity);

				float3 inverseInertia = physicsMass.InverseInertia;
				inverseInertia.xy          = new float2(0, 0);
				physicsMass.InverseInertia = inverseInertia;

				ecb.SetComponent(entity, physicsMass);
				ecb.RemoveComponent<ShouldDisableInertiaXYTag>(entity);
			}
		}
	}
}