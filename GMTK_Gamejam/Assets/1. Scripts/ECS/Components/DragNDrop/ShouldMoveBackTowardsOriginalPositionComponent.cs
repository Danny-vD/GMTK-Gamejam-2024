using Unity.Entities;

namespace ECS.Components.DragNDrop
{
	public struct ShouldMoveBackTowardsOriginalPositionComponent : IComponentData
	{
		public bool finishedMoving;
		public bool finishedRotating;
	}
}