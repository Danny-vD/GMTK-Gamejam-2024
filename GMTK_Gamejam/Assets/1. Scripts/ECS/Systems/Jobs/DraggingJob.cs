using ECS.Components.DragNDrop.Aspects;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace ECS.Systems.Jobs
{
	[BurstCompile]
	public partial struct DraggingJob : IJobEntity
	{
		[BurstCompile]
		public void Execute(DraggingAspect draggingAspect)
		{
			draggingAspect.LocalTransformRW.ValueRW.Position = Input.mousePosition;
		}
	}
}