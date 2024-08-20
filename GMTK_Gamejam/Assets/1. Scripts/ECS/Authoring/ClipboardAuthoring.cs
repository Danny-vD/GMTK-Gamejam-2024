using ECS.Components.DragNDrop;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
	public class ClipboardAuthoring : MonoBehaviour
	{
		public Transform TopLeftCorner;
		public Transform BottomRightCorner;

		public class ClipboardAuthoringBaker : Baker<ClipboardAuthoring>
		{
			public override void Bake(ClipboardAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				
				AddComponent(entity, new ClipboardComponent
				{
					TopLeftCorner     = authoring.TopLeftCorner.position,
					BottomRightCorner = authoring.BottomRightCorner.position,
				});
			}
		}
	}
}