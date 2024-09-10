using ECS.Components.DragNDrop.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Aspects;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Systems
{
	[RequireMatchingQueriesForUpdate, UpdateInGroup(typeof(InitializationSystemGroup))] //  BeforePhysicsSystemGroup
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
			float3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			//EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
			
			foreach (Entity draggdEntity in draggedEntityQuery.ToEntityArray(Allocator.Temp))
			{
				Entity draggedEntity = draggdEntity;
				
				if (EntityManager.HasComponent<Parent>(draggedEntity))
				{
					draggedEntity = EntityManager.GetComponentData<Parent>(draggedEntity).Value;
				}

				RigidBodyAspect rigidBodyAspect = EntityManager.GetAspect<RigidBodyAspect>(draggdEntity);
				LocalTransform localTransform = EntityManager.GetComponentData<LocalTransform>(draggedEntity);

				float scrollDelta = Input.mouseScrollDelta.y;

				if (scrollDelta != 0)
				{
					localTransform = localTransform.RotateZ(math.sign(scrollDelta) * 45 * SystemAPI.Time.DeltaTime); // TODO: reading input in a fixed time step does not work optimally, move to a different system
				}
				
				float zPosition = localTransform.Position.z;
				mouseWorldPosition.z = 0;

				rigidBodyAspect.Rotation = localTransform.Rotation;
				rigidBodyAspect.Position = mouseWorldPosition;
				//localTransform.Position  = mouseWorldPosition;
				
				//ecb.SetComponent(draggedEntity, localTransform);
			}
			
			//ecb.Playback(EntityManager);
		}
	}
}