using ECS.Components.DragNDrop.Tags;
using ECS.Systems.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems
{
	[BurstCompile, RequireMatchingQueriesForUpdate]
	public partial struct DraggingSystem : ISystem
	{
		private EntityQuery draggedEntityQuery;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			draggedEntityQuery = state.GetEntityQuery(ComponentType.ReadOnly<IsDraggedTag>());
			state.RequireForUpdate(draggedEntityQuery);
		}
		
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			new DraggingJob().ScheduleParallel();
		}
	}
	
	
}