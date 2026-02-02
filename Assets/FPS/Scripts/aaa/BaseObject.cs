using System;
using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics;
using UnityEngine.ProBuilder.Shapes;
using static UnityEngine.Rendering.DebugUI;

namespace AA0000
{
    public class BaseObject : MonoBehaviour
    {
        [Header("Some (useless) data for the base object")]
        [SerializeField] private string objectName;
        public Material objectMaterial;
        public Transform objectTransform;

        [Header("Visualisation controls")]
        [SerializeField] private PrimitiveType visualisationGeometry = PrimitiveType.Sphere;
        public float visualisationSize = 0.5f;
		[SerializeField] private GameObject currentVisualisation;

		// Store previous value to detect changes
		[SerializeField] private PrimitiveType previousVisualisationGeometry;

		// Called when values are changed in the Inspector and the editor is in play mode
		private void OnValidate()
		{
			if (!Application.isPlaying) return;

			if (previousVisualisationGeometry != visualisationGeometry) // Check if specifically visualisationGeometry changed
			{
				//Debug.Log($"VisualisationGeometry changed from {previousVisualisationGeometry} to {visualisationGeometry}");
				previousVisualisationGeometry = visualisationGeometry; // Update stored value
				SwapVisualisation();						
			}
		}

		// Add visualisation for the object during Start method
		private void Start()
        {
            CreateVisualisation();
			SetBaseData();
        }
		public void UseObject()
        {
            //TODO: Make the object do something
        }

		private void SetBaseData()
		{
			objectName = "BaseObject";
			objectMaterial = GetComponentInChildren<Renderer>().material;
			objectTransform = GetComponent<Transform>();
		}

		private void CreateVisualisation()
		{
			if (currentVisualisation != null)
				Destroy(currentVisualisation); // Ensure no duplicates

			currentVisualisation = GameObject.CreatePrimitive(visualisationGeometry);
			currentVisualisation.name = "Visuals";
			currentVisualisation.transform.SetParent(transform);
			currentVisualisation.transform.localPosition = Vector3.zero;
			currentVisualisation.transform.localScale = Vector3.one * visualisationSize;
			//DestroyImmediate(currentVisualisation.GetComponent<Collider>()); // Remove collider

			AddColor(ref currentVisualisation);
		}

		private void SwapVisualisation()
		{
			Invoke("CreateVisualisation", 0.1f);
		}

		private void AddColor(ref GameObject visualisation)
		{ 
			Renderer renderer = visualisation.GetComponent<Renderer>();
			if (renderer != null)
				renderer.material.color = new Color(0.56f, 0.0f, 1.0f); // RGB for violet (approximate)		
		}

		public void ChangeName(string suggestedName = "faultyName") 		
		{
			// TODO: Actual safeguard
			//GameObject thisObjectGameObject = gameObject;
			//Transform thisObjectTransform = transform;
			if (suggestedName.Contains("Name"))
			{ 
				objectName = suggestedName;
			}
		}
	}
}