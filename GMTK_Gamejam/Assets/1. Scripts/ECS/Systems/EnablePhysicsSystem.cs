using ECS.Components.DragNDrop.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace ECS.Systems
{
	[RequireMatchingQueriesForUpdate, BurstCompile, UpdateAfter(typeof(DisablePhysicsSystem))]
	public partial struct EnablePhysicsSystem : ISystem
	{
		private EntityQuery startSimulatingQuery;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			startSimulatingQuery = state.GetEntityQuery(typeof(ShouldStartSimulatingTag));
			
			state.RequireForUpdate<ShouldStartSimulatingTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

			foreach (Entity entity in startSimulatingQuery.ToEntityArray(Allocator.Temp))
			{
				ecb.SetComponentEnabled<Simulate>(entity, true);
				ecb.RemoveComponent<ShouldStartSimulatingTag>(entity);

				PhysicsVelocity physicsVelocity = state.EntityManager.GetComponentData<PhysicsVelocity>(entity);
				physicsVelocity.Angular = float3.zero;
				physicsVelocity.Linear = float3.zero;
				
				ecb.SetComponent(entity, physicsVelocity);
			}
		}
	}
}