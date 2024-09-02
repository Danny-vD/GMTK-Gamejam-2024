using Gameplay.Enums;
using SerializableDictionaryPackage.SerializableDictionary;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Scoring
{
	public class MultiplierDetailsAuthoring : MonoBehaviour
	{
		public SerializableEnumDictionary<MultiplierName, int> Multipliers;
		
		private class MultiplierDetailsBaker : Baker<MultiplierDetailsAuthoring>
		{
			public override void Bake(MultiplierDetailsAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.None);
				AddComponent(entity, new MultiplierDetailsComponent()
				{
					FailMultiplier    = authoring.Multipliers[MultiplierName.Fail],
					OkayMultiplier    = authoring.Multipliers[MultiplierName.Okay],
					GoodMultiplier    = authoring.Multipliers[MultiplierName.Good],
					PerfectMultiplier = authoring.Multipliers[MultiplierName.Perfect],
				});
			}
		}
	}

	public struct MultiplierDetailsComponent : IComponentData
	{
		public int FailMultiplier;
		public int OkayMultiplier;
		public int GoodMultiplier;
		public int PerfectMultiplier;
	}
}