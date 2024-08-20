using ECS.Components.Scoring.HeightMeasurer;
using Gameplay.Enums;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.HeightMeasurer
{
	public class HeightMeasureAreaAuthoring : MonoBehaviour
	{
		public Vector3 RaycastDirection = new Vector3(-1, 0, 0);
		public MultiplierName ScoringMultiplier;
		public float AreaSize = 0.2f;
		public float RaycastDistance = 6;

		public class HeightMeasureAreaBaker : Baker<HeightMeasureAreaAuthoring>
		{
			public override void Bake(HeightMeasureAreaAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				
				AddComponent(entity, new HeightMeasureAreaComponent
				{
					RaycastDirection = authoring.RaycastDirection,
					ScoringMultiplier = authoring.ScoringMultiplier,
					AreaSize = authoring.AreaSize,
					RaycastDistance = authoring.RaycastDistance,
				});
			}
		}
	}
}