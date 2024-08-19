using ECS.Components.DragNDrop.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
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
			if (isDragging) // NOTE: technically this boolean is not needed
			{
				if (Input.GetMouseButtonUp(0))
				{
					EntityManager.RemoveComponent<IsDraggedTag>(GetEntityQuery(typeof(IsDraggedTag)));
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

					if (RaycastHelper.CastRay(SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld, rayStart, rayEnd, CollisionFilter.Default, out RaycastHit hit))
					{
						if (EntityManager.HasComponent<DraggableTag>(hit.Entity))
						{
							EntityManager.AddComponent<IsDraggedTag>(hit.Entity);
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