using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
	public struct CubeSpawnerComponent : IComponentData
	{
		public Entity Prefab;
		
		public float3 SpawnPos;

		public double NextSpawnTime; // Set from the system
		public double SpawnRate;

		public float MovementSpeed;
	}
}