using ECS.Components;
using Unity.Entities;

namespace ECS.Authoring.Bakers
{
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