using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Authoring
{
	public class PlatformAuthoring : MonoBehaviour
	{
		public float DistanceBelowPlatformBeforeMovedBack = 0.25f;
		
		public class PlatformAuthoringBaker : Baker<PlatformAuthoring>
		{
			public override void Bake(PlatformAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);

				AddComponent(entity, new PlatformComponentData()
				{
					BottomPlatformPosition = authoring.transform.position,
					DistanceBelowPlatformBeforeMovedBack = authoring.DistanceBelowPlatformBeforeMovedBack,
				});
			}
		}
	}

	public struct PlatformComponentData : IComponentData
	{
		public float3 BottomPlatformPosition;
		public float DistanceBelowPlatformBeforeMovedBack;
	}
}