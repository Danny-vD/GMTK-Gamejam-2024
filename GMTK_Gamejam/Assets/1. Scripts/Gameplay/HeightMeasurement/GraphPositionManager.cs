using System;
using UnityEngine;
using VDFramework;
using Random = UnityEngine.Random;

namespace Gameplay.HeightMeasurement
{
	public class GraphPositionManager : BetterMonoBehaviour
	{
		public event Action OnChangedPosition = delegate { };

		[SerializeField]
		private Transform highestGraphPosition;
		
		[SerializeField]
		private Transform lowestGraphPosition;

		[ContextMenu("Set to random position")]
		public void SetToRandomPosition()
		{
			Vector3 higherBound = highestGraphPosition.position;
			Vector3 lowerBound = lowestGraphPosition.position;

			CachedTransform.position = Vector3.LerpUnclamped(lowerBound, higherBound, Random.value);
			
			OnChangedPosition.Invoke();
		}
	}
}