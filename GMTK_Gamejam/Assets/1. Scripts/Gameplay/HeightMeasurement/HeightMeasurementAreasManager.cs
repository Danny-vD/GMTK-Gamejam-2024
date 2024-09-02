using ECS.Components.Scoring.HeightMeasurer;
using Gameplay.Structs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using VDFramework;

namespace Gameplay.HeightMeasurement
{
	public class HeightMeasurementAreasManager : BetterMonoBehaviour
	{
		[SerializeField] 
		private ScoreAreaRaycastData[] raycastPoints;

		[SerializeField]
		private Vector3 raycastDirection = Vector3.left;

		[SerializeField]
		private float raycastDistance = 6;
		
		private EntityManager entityManager;
		private Entity heightMeasurementAreasManagerEntity;

		private GraphPositionManager graphPositionManager;
		
		private void Awake()
		{
			graphPositionManager = GetComponent<GraphPositionManager>();
		}

		private void OnEnable()
		{
			graphPositionManager.OnChangedPosition += SetPointsData;
		}
		
		private void OnDisable()
		{
			graphPositionManager.OnChangedPosition -= SetPointsData;
		}

		private void Start()
		{
			entityManager                       = World.DefaultGameObjectInjectionWorld.EntityManager;
			heightMeasurementAreasManagerEntity = entityManager.CreateEntity(ComponentType.ReadWrite<HeightMeasurementAreasManagerComponent>());
			
			HeightMeasurementAreasManagerComponent heightMeasurementAreasManagerComponent = GetHeightMeasurementAreasManagerComponent();

			heightMeasurementAreasManagerComponent.RaycastPoints    = new NativeArray<ScoreAreaRaycastDataECS>(raycastPoints.Length, Allocator.Persistent);
			
			for (int i = 0; i < heightMeasurementAreasManagerComponent.RaycastPoints.Length; i++)
			{
				heightMeasurementAreasManagerComponent.RaycastPoints[i] = raycastPoints[i];
			}
			
			heightMeasurementAreasManagerComponent.RaycastDistance  = raycastDistance;
			heightMeasurementAreasManagerComponent.RaycastDirection = raycastDirection;
			
			SetHeightMeasurementAreasManagerComponent(heightMeasurementAreasManagerComponent);
		}

		private HeightMeasurementAreasManagerComponent GetHeightMeasurementAreasManagerComponent()
		{
			return entityManager.GetComponentData<HeightMeasurementAreasManagerComponent>(heightMeasurementAreasManagerEntity);
		}
		
		private void SetHeightMeasurementAreasManagerComponent(HeightMeasurementAreasManagerComponent heightMeasurementAreasManagerComponent)
		{
			entityManager.SetComponentData(heightMeasurementAreasManagerEntity, heightMeasurementAreasManagerComponent);
		}

		private void SetPointsData()
		{
			HeightMeasurementAreasManagerComponent heightMeasurementAreasManagerComponent = GetHeightMeasurementAreasManagerComponent();

			for (int i = 0; i < heightMeasurementAreasManagerComponent.RaycastPoints.Length; i++)
			{
				heightMeasurementAreasManagerComponent.RaycastPoints[i] = raycastPoints[i];
			}
			
			SetHeightMeasurementAreasManagerComponent(heightMeasurementAreasManagerComponent);
		}
	}
}