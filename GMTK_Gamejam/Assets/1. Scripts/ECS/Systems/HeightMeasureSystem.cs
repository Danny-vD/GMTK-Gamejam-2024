using System.Collections.Generic;
using ECS.Components.Scoring.HeightMeasurer;
using ECS.Components.Scoring.HeightMeasurer.Tags;
using ECS.Components.Scoring.Tags;
using Gameplay.Enums;
using Gameplay.Events;
using Gameplay.Structs;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Utility.ECS;
using VDFramework.EventSystem;

namespace ECS.Systems
{
	[BurstCompile]
	public partial struct HeightMeasureSystem : ISystem
	{
		private EntityQuery raycastDataQuery;

		private EntityQuery shouldMeasureHeightQuery;

		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PhysicsWorldSingleton>();
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

			state.RequireForUpdate<ShouldMeasureHeightComponent>();

			shouldMeasureHeightQuery = state.GetEntityQuery(ComponentType.ReadWrite<ShouldMeasureHeightComponent>());

			raycastDataQuery = state.GetEntityQuery(ComponentType.ReadOnly<HeightMeasurementAreasManagerComponent>());
		}

		public void OnUpdate(ref SystemState state)
		{
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

			Entity shouldMeasureHeightEntity = shouldMeasureHeightQuery.GetSingletonEntity();
			ShouldMeasureHeightComponent shouldMeasureHeightComponent = state.EntityManager.GetComponentData<ShouldMeasureHeightComponent>(shouldMeasureHeightEntity);

			if (shouldMeasureHeightComponent.WaitTimeBeforeMeasure > 0)
			{
				shouldMeasureHeightComponent.WaitTimeBeforeMeasure -= SystemAPI.Time.DeltaTime;

				ecb.SetComponent(shouldMeasureHeightEntity, shouldMeasureHeightComponent);
				return;
			}

			MultiplierName highestMultiplierReached = GetHighestHeight(ref state);

			EventManager.RaiseEvent(new HeightReachedEvent(highestMultiplierReached));

			ecb.AddComponent(shouldMeasureHeightEntity, new HeightReachedComponent() { HighestMultiplierReached = highestMultiplierReached });
			ecb.AddComponent<ShouldCalculateScoreTag>(shouldMeasureHeightEntity);
			ecb.RemoveComponent<ShouldMeasureHeightComponent>(shouldMeasureHeightEntity);
		}

		private MultiplierName GetHighestHeight(ref SystemState state)
		{
			MultiplierName highestMultiplierReached = MultiplierName.Fail;

			CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

			HeightMeasurementAreasManagerComponent heightMeasurementAreasManager = raycastDataQuery.GetSingleton<HeightMeasurementAreasManagerComponent>();

			foreach (ScoreAreaRaycastDataECS raycastData in heightMeasurementAreasManager.RaycastPoints)
			{
				if (RaycastHelper.SphereCast(collisionWorld, raycastData.OriginPoint, raycastData.AreaRadius, heightMeasurementAreasManager.RaycastDirection, heightMeasurementAreasManager.RaycastDistance,
						CollisionFilter.Default, QueryInteraction.IgnoreTriggers))
				{
					highestMultiplierReached = raycastData.ScoringMultiplier;
				}
				else
				{
					break;
				}
			}

			return highestMultiplierReached;
		}

		public struct HeightMeasureComparer : IComparer<Entity>
		{
			public EntityManager EntityManager;

			public int Compare(Entity lhs, Entity rhs)
			{
				LocalTransform lhsTransform = EntityManager.GetComponentData<LocalTransform>(lhs);
				LocalTransform rhsTransform = EntityManager.GetComponentData<LocalTransform>(rhs);

				return lhsTransform.Position.y.CompareTo(rhsTransform.Position.y);
			}
		}
	}
}