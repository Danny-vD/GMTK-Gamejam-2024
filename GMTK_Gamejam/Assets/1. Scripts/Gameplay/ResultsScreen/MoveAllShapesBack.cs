using ECS.Components.DragNDrop;
using ECS.Components.DragNDrop.Tags;
using Unity.Collections;
using Unity.Entities;
using VDFramework;

namespace Gameplay.ResultsScreen
{
    public class MoveAllShapesBack : BetterMonoBehaviour
    {
        private EntityManager entityManager;
        private EntityQuery placedShapesQuery;

        private void Awake()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            placedShapesQuery   = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Simulate>(), ComponentType.ReadOnly<DraggableTag>());
        }

        public void SetAllShapesToMoveBack()
        {
            foreach (Entity entity in placedShapesQuery.ToEntityArray(Allocator.Temp))
            {
                entityManager.SetComponentEnabled<Simulate>(entity, false);
                entityManager.AddComponent<ShouldMoveBackTowardsOriginalPositionComponent>(entity);
            }
        }
    }
}
