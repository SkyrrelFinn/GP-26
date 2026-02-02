using UnityEngine;

namespace AA0000
{
    public class SpawnLocationController : MonoBehaviour
    {
		[Header("Spawning location controls")]
		public Transform spawnLocation;
		public float spawnDistance = 1.0f;
		public float angleStep = 45f; // Degrees per step

		[SerializeField] private float currentAngle = 0f; // Tracks the current angle
		[SerializeField] private int stepCount = 1; // Spiral growth step
		[SerializeField] private Vector3 originalPosition; // Stores the initial spawn location

		// Check for if spawnLocation exists
		private void Start()
		{
			spawnLocation = GameObject.Find("SpawnLocation").transform;

			if (spawnLocation == null)
			{
				Debug.LogError("Spawn Location not assigned!");
				return;
			}

			// Store the original position to base spiral movement on it
			originalPosition = spawnLocation.position;
		}

		// Move the spawn location in a spiral pattern
		internal void MoveSpawnLocation()
		{
			if (spawnLocation == null) return;

			// Calculate new position in a spiral pattern
			float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * spawnDistance * stepCount;
			float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * spawnDistance * stepCount;

			// Update the spawn location's position relative to its original position
			spawnLocation.position = originalPosition + new Vector3(x, 0, z);

			// Increase angle for next move
			currentAngle += angleStep;

			// Expand spiral step after a full rotation (360 degrees)
			if (currentAngle >= 360f)
			{
				currentAngle = 0f;
				stepCount++;
			}
		}
	} 
}
