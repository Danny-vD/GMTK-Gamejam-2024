using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
	public struct CubeSpawnerComponent : IComponentData
	{
		public Entity Prefab;
		
		public float3 SpawnPos;

		public double NextSpawnTime;
		public double SpawnRate;

		public float MovementSpeed;
	}
}