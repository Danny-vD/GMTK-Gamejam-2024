﻿using ECS.Components.DragNDrop.Tags;
using ECS.Components.PhysicsSimulation.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Utility.ECS;
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
			EndSimulationEntityCommandBufferSystem.Singleton entityCommandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb = entityCommandBufferSingleton.CreateCommandBuffer(World.Unmanaged);

			if (isDragging) // NOTE: technically this boolean is not needed
			{
				if (Input.GetMouseButtonUp(0))
				{
					EntityQuery draggedEntities = GetEntityQuery(typeof(IsDraggedTag));

					ecb.AddComponent<ShouldStartSimulatingTag>(draggedEntities, EntityQueryCaptureMode.AtRecord); // TODO: check if inside or outside the shapes area
					ecb.RemoveComponent<IsDraggedTag>(draggedEntities, EntityQueryCaptureMode.AtRecord);

					isDragging = false;
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
						}
					}
				}
			}
		}

		protected override void OnDestroy()
		{
			maincamera = null;
		}
	}
}