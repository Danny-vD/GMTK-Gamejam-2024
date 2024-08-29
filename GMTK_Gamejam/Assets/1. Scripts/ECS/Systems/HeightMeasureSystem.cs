using System.Collections.Generic;
using ECS.Components.Scoring.HeightMeasurer;
using ECS.Components.Scoring.HeightMeasurer.Tags;
using Gameplay.Enums;
using Gameplay.Events;
using Unity.Burst;
using Unity.Collections;
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
		private EntityQuery heightMeasureAreas;

		private EntityQuery shouldMeasureHeightQuery;

		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PhysicsWorldSingleton>();
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

			state.RequireForUpdate<ShouldMeasureHeightComponent>();

			shouldMeasureHeightQuery = state.GetEntityQuery(ComponentType.ReadWrite<ShouldMeasureHeightComponent>());

			heightMeasureAreas = state.GetEntityQuery(ComponentType.ReadOnly<HeightMeasureAreaComponent>());
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
			Debug.Log(highestMultiplierReached);

			ecb.AddComponent(shouldMeasureHeightEntity, new HeightReachedComponent() { HighestMultiplierReached = highestMultiplierReached });
			ecb.RemoveComponent<ShouldMeasureHeightComponent>(shouldMeasureHeightEntity);
		}

		private MultiplierName GetHighestHeight(ref SystemState state)
		{
			MultiplierName highestMultiplierReached = MultiplierName.Fail;
		
			CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
			
			NativeArray<Entity> heightMeasureAreaPoints = heightMeasureAreas.ToEntityArray(Allocator.Temp);
			heightMeasureAreaPoints.Sort<Entity, IComparer<Entity>>(new HeightMeasureComparer() { EntityManager = state.EntityManager });
		
			foreach (Entity entity in heightMeasureAreaPoints)
			{
				HeightMeasureAreaComponent heightMeasureAreaComponent = state.EntityManager.GetComponentData<HeightMeasureAreaComponent>(entity);
				LocalTransform transform = state.EntityManager.GetComponentData<LocalTransform>(entity);
		
				if (RaycastHelper.SphereCast(collisionWorld, transform.Position, heightMeasureAreaComponent.AreaRadius, heightMeasureAreaComponent.RaycastDirection, heightMeasureAreaComponent.RaycastDistance,
						CollisionFilter.Default, QueryInteraction.IgnoreTriggers))
				{
					highestMultiplierReached = heightMeasureAreaComponent.ScoringMultiplier;
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