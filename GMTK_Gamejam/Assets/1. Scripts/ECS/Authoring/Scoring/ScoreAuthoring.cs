using ECS.Components.Scoring;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Scoring
{
	public class ScoreAuthoring : MonoBehaviour
	{
		public int ShapePoints;

		public class ScoreAuthoringBaker : Baker<ScoreAuthoring>
		{
			public override void Bake(ScoreAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new ScoringComponent()
				{
					Score = authoring.ShapePoints,
				});
			}
		}
	}
}