using System;
using ECS.Authoring.Scoring;
using ECS.Components.DragNDrop.Tags;
using ECS.Components.Scoring;
using ECS.Components.Scoring.HeightMeasurer;
using ECS.Components.Scoring.Tags;
using Gameplay.Enums;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ECS.Systems
{
	[BurstCompile]
	public partial struct ScoreSystem : ISystem
	{
		private EntityQuery viableScoreQuery;
		private ComponentLookup<ScoringComponent> scoringComponentLookup;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

			// ReSharper disable once Unity.BurstFunctionSignatureContainsManagedTypes
			viableScoreQuery = state.GetEntityQuery(ComponentType.ReadOnly<ScoringComponent>(), ComponentType.Exclude<Simulate>(), ComponentType.ReadOnly<DraggableTag>(), ComponentType.Exclude<IsDraggedTag>());
			state.RequireForUpdate(viableScoreQuery);

			state.RequireForUpdate<HeightReachedComponent>();
			state.RequireForUpdate<MultiplierDetailsComponent>();
			state.RequireForUpdate<ShouldCalculateScoreTag>();

			scoringComponentLookup = state.GetComponentLookup<ScoringComponent>(true);
		}

		//[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			scoringComponentLookup.Update(ref state);
			
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

			int score = 0;

			foreach (Entity entity in viableScoreQuery.ToEntityArray(Allocator.Temp))
			{
				ScoringComponent scoringComponent = scoringComponentLookup[entity];
				score += scoringComponent.Score;
			}

			HeightReachedComponent heightReachedComponent = SystemAPI.GetSingleton<HeightReachedComponent>();
			MultiplierDetailsComponent multiplierDetails = SystemAPI.GetSingleton<MultiplierDetailsComponent>();

			int multiplier = heightReachedComponent.HighestMultiplierReached switch
			{
				MultiplierName.Fail => multiplierDetails.FailMultiplier,
				MultiplierName.Okay => multiplierDetails.OkayMultiplier,
				MultiplierName.Good => multiplierDetails.GoodMultiplier,
				MultiplierName.Perfect => multiplierDetails.PerfectMultiplier,
				_ => multiplierDetails.FailMultiplier,
			};

			Entity singletonEntity = SystemAPI.GetSingletonEntity<ShouldCalculateScoreTag>();
			ecb.RemoveComponent<ShouldCalculateScoreTag>(singletonEntity);
			ecb.AddComponent(singletonEntity, new ScoreCalculatedComponent() { Score = score, Multiplier = multiplier, FinalScore = score * multiplier });
			Debug.Log($"score: {score} * {multiplier} = {score * multiplier}");
		}
	}
}