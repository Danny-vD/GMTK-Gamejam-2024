using ECS.Components.DragNDrop.Aspects;
using ECS.Components.DragNDrop.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
	[BurstCompile, RequireMatchingQueriesForUpdate]
	public partial struct MoveBackTowardsOriginalPositionSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			state.RequireForUpdate<ShouldMoveBackTowardsOriginalPositionTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			float deltaTime = SystemAPI.Time.DeltaTime;
			EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			new MoveBackToOriginalPositionJob()
			{
				DeltaTime = deltaTime,
				ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
			}.ScheduleParallel();
		}
	}

	[BurstCompile]
	public partial struct MoveBackToOriginalPositionJob : IJobEntity
	{
		public float DeltaTime;
		public EntityCommandBuffer.ParallelWriter ECB;

		[BurstCompile]
		public void Execute(MovingBackAspect movingBackAspect, [EntityIndexInQuery] int sortkey)
		{
			float3 originalPosition = movingBackAspect.OriginalPositionComponentRO.ValueRO.OriginalPosition;
			float3 currentPosition = movingBackAspect.LocalTransformRW.ValueRO.Position;

			movingBackAspect.LocalTransformRW.ValueRW.Position += movingBackAspect.MovementSpeedComponentRO.ValueRO.Speed * DeltaTime * math.normalize(originalPosition - currentPosition);

			if (math.distancesq(originalPosition, currentPosition) < movingBackAspect.OriginalPositionComponentRO.ValueRO.DistanceBeforeStopMoving)
			{
				movingBackAspect.LocalTransformRW.ValueRW.Position = originalPosition;
				ECB.RemoveComponent<ShouldMoveBackTowardsOriginalPositionTag>(sortkey, movingBackAspect.Entity);
			}
		}
	}
}