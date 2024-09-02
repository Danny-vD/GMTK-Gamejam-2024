using Unity.Entities;

namespace ECS.Components.Scoring
{
	public struct ScoreCalculatedComponent : IComponentData
	{
		public int Score;
		public int Multiplier;

		public int FinalScore;
	}
}