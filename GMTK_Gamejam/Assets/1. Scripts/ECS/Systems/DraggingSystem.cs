using ECS.Components.DragNDrop.Tags;
using ECS.Systems.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
	[RequireMatchingQueriesForUpdate, UpdateAfter(typeof(DragNDropSystem))]
	public partial class DraggingSystem : SystemBase
	{
		private EntityQuery draggedEntityQuery;
		private Camera maincamera;

		protected override void OnCreate()
		{
			draggedEntityQuery = GetEntityQuery(ComponentType.ReadOnly<IsDraggedTag>());
			RequireForUpdate(draggedEntityQuery);

			maincamera = Camera.main;
		}

		protected override void OnUpdate()
		{
			float3 mouseWorldPosition = maincamera.ScreenToWorldPoint(Input.mousePosition);
			
			EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
			
			foreach (Entity draggedEntity in draggedEntityQuery.ToEntityArray(Allocator.Temp))
			{
				LocalTransform localTransform = EntityManager.GetComponentData<LocalTransform>(draggedEntity);

				float scrollDelta = Input.mouseScrollDelta.y;

				if (scrollDelta != 0)
				{
					localTransform = localTransform.RotateZ(math.sign(scrollDelta) * 45 * SystemAPI.Time.DeltaTime);
				}
				
				float zPosition = localTransform.Position.z;
				mouseWorldPosition.z = zPosition;
				
				localTransform.Position = mouseWorldPosition;
				
				ecb.SetComponent(draggedEntity, localTransform);
			}
			
			ecb.Playback(EntityManager);
		}
	}
}