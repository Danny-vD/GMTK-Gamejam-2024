using ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
	public class CubeSpawnerAuthoring : MonoBehaviour
	{
		[Header("Spawn settings")]
		public GameObject Prefab;

		[Header("Spawning logic")]
		public float Spawnrate = 1f;

		[Header("Movement Logic")]
		public float MovementSpeed;
	}

	public class CubeSpawnerBaker : Baker<CubeSpawnerAuthoring>
	{
		public override void Bake(CubeSpawnerAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);

			AddComponent(entity, new CubeSpawnerComponent()
			{
				Prefab    = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
				SpawnPos  = authoring.transform.position,
				SpawnRate = authoring.Spawnrate,

				MovementSpeed = authoring.MovementSpeed,
			});
		}
	}
}