using ECS.Components.Scoring.HeightMeasurer;
using ECS.Components.Scoring.HeightMeasurer.Tags;
using Gameplay.Enums;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Utility.ECS;

namespace ECS.Systems
{
	[BurstCompile]
	public partial struct HeightMeasureSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PhysicsWorldSingleton>();
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			state.RequireForUpdate<ShouldMeasureHeight>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

			MultiplierName highestMultiplierReached = GetHighestHeight(ref state);

			EntityQuery measureWeightEntities = state.GetEntityQuery(ComponentType.ReadWrite<ShouldMeasureHeight>());

			ecb.AddComponent(measureWeightEntities, new HeightReachedComponent() { HighestMultiplierReached = highestMultiplierReached });
			ecb.RemoveComponent<ShouldMeasureHeight>(measureWeightEntities, EntityQueryCaptureMode.AtRecord);
		}

		[BurstCompile]
		private MultiplierName GetHighestHeight(ref SystemState state)
		{
			EntityQuery heightMeasureAreas = state.GetEntityQuery(ComponentType.ReadOnly<HeightMeasureAreaComponent>());

			float highestPoint = float.NegativeInfinity;

			MultiplierName highestMultiplierReached = MultiplierName.Fail;

			foreach (Entity entity in heightMeasureAreas.ToEntityArray(Allocator.Temp))
			{
				LocalTransform transform = state.EntityManager.GetComponentData<LocalTransform>(entity);
				float positionY = transform.Position.y;

				if (positionY < highestPoint)
				{
					continue;
				}

				HeightMeasureAreaComponent heightMeasureAreaComponent = state.EntityManager.GetComponentData<HeightMeasureAreaComponent>(entity);

				CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

				if (RaycastHelper.SphereCast(collisionWorld, transform.Position, heightMeasureAreaComponent.AreaRadius, heightMeasureAreaComponent.RaycastDirection, heightMeasureAreaComponent.RaycastDistance, CollisionFilter.Default, QueryInteraction.IgnoreTriggers))
				{
					highestPoint             = positionY;
					highestMultiplierReached = heightMeasureAreaComponent.ScoringMultiplier;
				}
			}

			return highestMultiplierReached;
		}
	}
}