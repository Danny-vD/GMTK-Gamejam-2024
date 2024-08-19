using Unity.Mathematics;
using Unity.Physics;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Utility.ECS
{
	public static class RaycastHelper
	{
		public static bool CastRay(CollisionWorld collisionWorld, float3 rayStart, float3 rayEnd, CollisionFilter collisionFilter, out RaycastHit raycastHit)
		{
			RaycastInput raycastInput = new RaycastInput()
			{
				Start = rayStart,
				End = rayEnd,
				Filter = collisionFilter,
			};
			
			return CastRay(collisionWorld, raycastInput, out raycastHit);
		}
		
		public static bool CastRay(CollisionWorld collisionWorld, RaycastInput raycastInput, out RaycastHit raycastHit)
		{
			return collisionWorld.CastRay(raycastInput, out raycastHit);
		}
	}
}