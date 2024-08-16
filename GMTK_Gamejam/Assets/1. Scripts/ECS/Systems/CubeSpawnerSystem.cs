using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
	[BurstCompile]
	public partial struct CubeSpawnerSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			if (!SystemAPI.TryGetSingletonEntity<CubeSpawnerComponent>(out Entity cubeSpawnerEntity))
			{
				return;
			}

			RefRW<CubeSpawnerComponent> spawner = SystemAPI.GetComponentRW<CubeSpawnerComponent>(cubeSpawnerEntity);

			EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

			if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
			{
				Entity entity = commandBuffer.Instantiate(spawner.ValueRO.Prefab);

				commandBuffer.AddComponent(entity, new MovementComponent() { Direction = GetRandomDirection(ref state), Speed = spawner.ValueRO.MovementSpeed });
				spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;

				commandBuffer.Playback(state.EntityManager);
			}
		}

		private float3 GetRandomDirection(ref SystemState state)
		{
			return Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime / SystemAPI.Time.DeltaTime)).NextFloat3();
		}
	}
}