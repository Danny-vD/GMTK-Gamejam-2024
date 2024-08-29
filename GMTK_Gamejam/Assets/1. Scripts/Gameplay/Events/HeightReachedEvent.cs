using Gameplay.Enums;
using VDFramework.EventSystem;

namespace Gameplay.Events
{
	public class HeightReachedEvent : VDEvent<HeightReachedEvent>
	{
		public readonly MultiplierName HighestMultiplierReached;

		public HeightReachedEvent(MultiplierName highestMultiplierReached)
		{
			HighestMultiplierReached = highestMultiplierReached;
		}
	}
}