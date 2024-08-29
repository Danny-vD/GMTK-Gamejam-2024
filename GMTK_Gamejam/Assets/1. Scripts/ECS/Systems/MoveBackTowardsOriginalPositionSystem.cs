using ECS.Components.DragNDrop;
using ECS.Components.DragNDrop.Aspects;
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

			state.RequireForUpdate<ShouldMoveBackTowardsOriginalPositionComponent>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			float deltaTime = SystemAPI.Time.DeltaTime;
			EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

			new MoveBackToOriginalPositionJob()
			{
				DeltaTime = deltaTime,
				ECB       = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
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
			if (!movingBackAspect.MoveTowardsOriginalPositionComponentRW.ValueRO.finishedMoving)
			{
				float3 originalPosition = movingBackAspect.OriginalPositionComponentRO.ValueRO.OriginalPosition;
				float3 currentPosition = movingBackAspect.LocalTransformRW.ValueRO.Position;

				movingBackAspect.LocalTransformRW.ValueRW.Position += movingBackAspect.MovementSpeedComponentRO.ValueRO.Speed * DeltaTime * math.normalize(originalPosition - currentPosition);

				if (math.distancesq(originalPosition, currentPosition) < movingBackAspect.OriginalPositionComponentRO.ValueRO.DistanceBeforeStopMoving)
				{
					movingBackAspect.LocalTransformRW.ValueRW.Position = originalPosition;
					
					movingBackAspect.MoveTowardsOriginalPositionComponentRW.ValueRW.finishedMoving = true;
				}
			}

			if (!movingBackAspect.MoveTowardsOriginalPositionComponentRW.ValueRO.finishedRotating)
			{
				quaternion originalRotation = movingBackAspect.OriginalPositionComponentRO.ValueRO.OriginalRotation;
				quaternion currentRotation = movingBackAspect.LocalTransformRW.ValueRO.Rotation;

				float angleDelta = math.Euler(originalRotation).z - math.Euler(currentRotation).z;
				float angleDeltaDegrees = angleDelta * math.TODEGREES;

				float delta = math.min(math.abs(angleDeltaDegrees), movingBackAspect.MovementSpeedComponentRO.ValueRO.RotationSpeed * DeltaTime);

				movingBackAspect.LocalTransformRW.ValueRW.Rotation = movingBackAspect.LocalTransformRW.ValueRW.RotateZ(delta).Rotation;

				if (angleDelta < 0.03f)
				{
					movingBackAspect.LocalTransformRW.ValueRW.Rotation = originalRotation;

					movingBackAspect.MoveTowardsOriginalPositionComponentRW.ValueRW.finishedRotating = true;
				}
			}

			if (movingBackAspect.MoveTowardsOriginalPositionComponentRW.ValueRO is { finishedMoving: true, finishedRotating: true })
			{
				ECB.RemoveComponent<ShouldMoveBackTowardsOriginalPositionComponent>(sortkey, movingBackAspect.Entity);
			}
		}
	}
}