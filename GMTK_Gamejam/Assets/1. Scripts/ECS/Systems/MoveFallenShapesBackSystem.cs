using ECS.Authoring;
using ECS.Components.DragNDrop;
using ECS.Components.MoveBackIfFallen.Aspect;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
	[BurstCompile]
	public partial struct MoveFallenShapesBackSystem : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<ClipboardComponent>();
			state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
			state.RequireForUpdate<PlatformComponentData>();
			state.RequireForUpdate<Simulate>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			PlatformComponentData platformComponent = SystemAPI.GetSingleton<PlatformComponentData>();
			float3 bottomPlatformPosition = platformComponent.BottomPlatformPosition;
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			
			ClipboardComponent clipboardComponent = SystemAPI.GetSingleton<ClipboardComponent>();
			
			new MoveFallenShapeBackJob()
			{
				MinMaxX = new float2(clipboardComponent.TopLeftCorner.x, clipboardComponent.BottomRightCorner.x),
				Random = Random.CreateFromIndex((uint)SystemAPI.Time.ElapsedTime),
				StartMovingBackY = bottomPlatformPosition.y - platformComponent.DistanceBelowPlatformBeforeMovedBack,
				ECB = entityCommandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
			}.ScheduleParallel();
		}
	}

	[BurstCompile]
	public partial struct MoveFallenShapeBackJob : IJobEntity
	{
		public Random Random;
		public float2 MinMaxX;
		
		public float StartMovingBackY;
		public EntityCommandBuffer.ParallelWriter ECB;

		[BurstCompile]
		public void Execute(PotentialFallenBlockAspect potentialFallenBlockAspect, [ChunkIndexInQuery] int sortKey)
		{
			if (potentialFallenBlockAspect.LocalTransformRW.ValueRO.Position.y <= StartMovingBackY)
			{
				float newX = Random.NextFloat(MinMaxX.x, MinMaxX.y);
				potentialFallenBlockAspect.LocalTransformRW.ValueRW.Position.x = newX;

				ECB.AddComponent<ShouldMoveBackTowardsOriginalPositionComponent>(sortKey, potentialFallenBlockAspect.Entity);
				ECB.SetComponentEnabled<Simulate>(sortKey, potentialFallenBlockAspect.Entity, false);
			}
		}
	}
}