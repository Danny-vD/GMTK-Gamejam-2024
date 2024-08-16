using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
	public struct MovementComponent : IComponentData
	{
		public float3 Direction;
		public float Speed;
	}
}