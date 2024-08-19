using System.ComponentModel;
using Unity.Entities;

namespace ECS.Components.Scoring
{
	public struct ScoringComponent : IComponentData
	{
		public int Score;
	}
}