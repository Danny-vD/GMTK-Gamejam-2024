using ECS.Components.DragNDrop;
using ECS.Components.DragNDrop.Tags;
using ECS.Components.PhysicsSimulation.Tags;
using ECS.Components.Scoring.HeightMeasurer.Tags;
using Gameplay.Events;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Utility.ECS;
using VDFramework.EventSystem;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace ECS.Systems
{
	//[UpdateInGroup(typeof(InitializationSystemGroup))]
	//[UpdateAfter(typeof(PhysicsInitializeGroup))]
	public partial class DragNDropSystem : SystemBase
	{
		private Camera maincamera;
		private bool isDragging = false;

		protected override void OnCreate()
		{
			maincamera = Camera.main;
		}

		protected override void OnStartRunning()
		{
			isDragging = false;
		}

		protected override void OnUpdate()
		{
			bool disallowDragging = SystemAPI.HasSingleton<ShouldMeasureHeightComponent>(); // Disable the dragNDrop when the height measuring is about to take place (to prevent cheating)

			if (!isDragging && disallowDragging)
			{
				return;
			}
			
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = entityCommandBufferSingleton.CreateCommandBuffer(World.Unmanaged);

			if (isDragging) // NOTE: technically this boolean is not needed
			{
				if (Input.GetMouseButtonUp(0) || disallowDragging)
				{
					EntityQuery draggedEntities = GetEntityQuery(typeof(IsDraggedTag));
					
					foreach (Entity entity in draggedEntities.ToEntityArray(Allocator.Temp))
					{
						LocalTransform localTransform = EntityManager.GetComponentData<LocalTransform>(entity);

						if (IsWithinClipboardArea(localTransform.Position))
						{
							ecb.AddComponent<ShouldMoveBackTowardsOriginalPositionComponent>(entity);
						}
						else
						{
							ecb.AddComponent<ShouldStartSimulatingTag>(entity);
						}
					}
					
					ecb.RemoveComponent<IsDraggedTag>(draggedEntities, EntityQueryCaptureMode.AtRecord);

					isDragging = false;
					EventManager.RaiseEvent(new StopDraggingEvent());
				}
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					Ray ray = maincamera.ScreenPointToRay(Input.mousePosition);
					float3 rayStart = ray.origin;
					float3 rayEnd = ray.GetPoint(50);

					CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

					if (RaycastHelper.CastRay(collisionWorld, rayStart, rayEnd, CollisionFilter.Default, out RaycastHit hit))
					{
						if (EntityManager.HasComponent<DraggableTag>(hit.Entity))
						{
							ecb.AddComponent<ShouldStopSimulatingTag>(hit.Entity);
							ecb.AddComponent<IsDraggedTag>(hit.Entity);

							isDragging = true;
							EventManager.RaiseEvent(new DraggingEvents());
						}

						if (EntityManager.HasComponent<ShouldMoveBackTowardsOriginalPositionComponent>(hit.Entity)) // Make sure the entity does not move back where it came from if it was doing that
						{
							ecb.RemoveComponent<ShouldMoveBackTowardsOriginalPositionComponent>(hit.Entity);
						}
					}
				}
			}
		}

		private bool IsWithinClipboardArea(float3 position)
		{
			Entity singletonEntity = SystemAPI.GetSingletonEntity<ClipboardComponent>();

			RefRO<ClipboardComponent> clipboardComponentRO = SystemAPI.GetComponentRO<ClipboardComponent>(singletonEntity);

			float3 topleft = clipboardComponentRO.ValueRO.TopLeftCorner;
			float3 bottomRight = clipboardComponentRO.ValueRO.BottomRightCorner;
			
			return position.y <= topleft.y && position.y >= bottomRight.y && position.x >= topleft.x && position.x <= bottomRight.x;
		}

		protected override void OnDestroy()
		{
			maincamera = null;
		}
	}
}