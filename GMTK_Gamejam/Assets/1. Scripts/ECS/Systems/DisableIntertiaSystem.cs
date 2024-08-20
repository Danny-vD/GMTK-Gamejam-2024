using ECS.Components.PhysicsSimulation.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace ECS.Systems
{
	[BurstCompile, RequireMatchingQueriesForUpdate, UpdateInGroup(typeof(InitializationSystemGroup))]
	public partial struct DisableIntertiaSystem : ISystem
	{
		private EntityQuery disableIntertiaQuery;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			disableIntertiaQuery = state.GetEntityQuery(ComponentType.ReadWrite<ShouldDisableInertiaXYTag>());

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
				inverseInertia.xy          = float2.zero;
				physicsMass.InverseInertia = inverseInertia;

				ecb.SetComponent(entity, physicsMass);
				ecb.RemoveComponent<ShouldDisableInertiaXYTag>(entity);
			}
		}
	}
}