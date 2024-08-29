using ECS.Components.DragNDrop.Tags;
using ECS.Components.PhysicsSimulation.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;

namespace ECS.Systems
{
	[BurstCompile, RequireMatchingQueriesForUpdate, UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial struct DisableIntertiaSystem : ISystem
	{
		private EntityQuery disableIntertiaQuery;

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

			//new DisableInertiaJob().ScheduleParallel();
		}
	}

	[BurstCompile]
	public partial struct ResetRotationJob : IJobEntity
	{
		[BurstCompile]
		public void Execute(RigidBodyAspect rigidBodyAspect)
		{
			quaternion quaternion = rigidBodyAspect.Rotation;
			float4 quaternionValue = quaternion.value;
			quaternionValue.xy = float2.zero;
			quaternion.value   = quaternionValue;
			
			rigidBodyAspect.Rotation = quaternion;

			//float inertiaZ = rigidBodyAspect.Inertia.z;
			//rigidBodyAspect.Inertia = new float3(float.PositiveInfinity, float.PositiveInfinity, inertiaZ);
		}
	}
}