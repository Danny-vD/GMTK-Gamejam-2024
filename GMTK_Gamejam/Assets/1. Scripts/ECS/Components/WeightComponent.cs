using Unity.Entities;

namespace ECS.Components
{
	public struct WeightComponent : IComponentData
	{
		public float Weight;

		public Entity myEntity;
	}
}