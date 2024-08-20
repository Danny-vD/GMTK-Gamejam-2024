using Gameplay.Enums;
using Unity.Entities;

namespace ECS.Components.Scoring.HeightMeasurer
{
	public struct HeightReachedComponent : IComponentData
	{
		public MultiplierName HighestMultiplierReached;
	}
}