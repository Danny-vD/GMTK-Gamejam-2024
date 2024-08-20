using ECS.Components.DragNDrop;
using ECS.Components.DragNDrop.Tags;
using ECS.Components.PhysicsSimulation.Tags;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
	public class ShapeAuthoring : MonoBehaviour
	{
		public float DistanceBeforeStopMoving = 0.001f;
		
		[Tooltip("How fast the shape moves back to their original position in units/second")]
		public float Speed = 1;
		
		public class ShapeAuthoringBaker : Baker<ShapeAuthoring>
		{
			public override void Bake(ShapeAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);

				AddComponent(entity, new DraggableTag());

				AddComponent(entity, new OriginalPositionComponent()
				{
					OriginalPosition = authoring.transform.position,
					DistanceBeforeStopMoving = authoring.DistanceBeforeStopMoving,
				});
				
				AddComponent(entity, new MovementSpeedComponent()
				{
					Speed = authoring.Speed,
				});
				
				AddComponent(entity, new ShouldStopSimulatingTag());
			}
		}
	}
}