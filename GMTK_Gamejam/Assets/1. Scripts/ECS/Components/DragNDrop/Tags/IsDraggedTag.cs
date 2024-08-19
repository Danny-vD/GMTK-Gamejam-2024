using Unity.Entities;

namespace ECS.Components.DragNDrop.Tags
{
	/// <summary>
	/// Used to mark that something is currently being dragged by the mouse cursor
	/// </summary>
	public struct IsDraggedTag : IComponentData
	{
	}
}