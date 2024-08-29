using Unity.Entities;

namespace ECS.Components.Scoring.HeightMeasurer.Tags
{
	public struct ShouldMeasureHeightComponent : IComponentData
	{
		public float WaitTimeBeforeMeasure;
	}
}