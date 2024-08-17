using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems
{
	[BurstCompile, RequireMatchingQueriesForUpdate]
	public partial struct CubeSpawnerSystem : ISystem
	{
		private EntityQuery spawnersQuery;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			spawnersQuery = state.GetEntityQuery(ComponentType.ReadWrite<CubeSpawnerComponent>());
			state.RequireForUpdate(spawnersQuery);
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach (Entity spawnerEntity in spawnersQuery.ToEntityArray(Allocator.TempJob))
			{
				RefRW<CubeSpawnerComponent> spawner = SystemAPI.GetComponentRW<CubeSpawnerComponent>(spawnerEntity);

				EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
				
				if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
				{
					RefRO<LocalTransform> spawnerTransform = SystemAPI.GetComponentRO<LocalTransform>(spawnerEntity);
					
					EntityManager entityManager = state.EntityManager;
					
					Entity spawnedEntity = commandBuffer.Instantiate(spawner.ValueRO.Prefab);
					
					commandBuffer.SetComponent(spawnedEntity, LocalTransform.FromPosition(spawnerTransform.ValueRO.Position));

					commandBuffer.AddComponent(spawnedEntity, new MovementComponent() { Direction = GetRandomDirection(ref state), Speed = spawner.ValueRO.MovementSpeed });
					spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;

					commandBuffer.Playback(entityManager);
				}
			}
		}

		private float3 GetRandomDirection(ref SystemState state)
		{
			return Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime / SystemAPI.Time.DeltaTime)).NextFloat3(new float3(-1, -1, -1), new float3(1, 1, 1));
		}
	}
}