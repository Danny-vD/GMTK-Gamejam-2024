using UnityEngine;

namespace ECS.Authoring
{
	public class CubeSpawnerAuthoring : MonoBehaviour
	{
		[Header("Spawn settings")]
		public GameObject Prefab;

		[Header("Spawning logic")]
		public float Spawnrate = 1f;

		[Header("Movement Logic")]
		public float MovementSpeed;
	}
}