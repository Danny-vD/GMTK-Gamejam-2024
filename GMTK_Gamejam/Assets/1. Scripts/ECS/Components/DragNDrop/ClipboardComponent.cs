using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components.DragNDrop
{
	public struct ClipboardComponent : IComponentData
	{
		public float3 TopLeftCorner;
		public float3 BottomRightCorner;
	}
}