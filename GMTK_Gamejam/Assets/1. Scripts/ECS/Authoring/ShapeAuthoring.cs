using ECS.Components.DragNDrop.Tags;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
	public class ShapeAuthoring : MonoBehaviour
	{
		public class ShapeAuthoringBaker : Baker<ShapeAuthoring>
		{
			public override void Bake(ShapeAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				
				AddComponent(entity, new DraggableTag());
			}
		}
	}
}