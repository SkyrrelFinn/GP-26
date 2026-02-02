using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AA0000
{
	public class BaseObjectCreator : MonoBehaviour
	{
		[Header("Script controls")]
		public SelectedWay selectedWay = SelectedWay.CreationWay;
		public Key activationKey = Key.Q;

		[Header("Possibility 1 - Prefab way")]
		public GameObject prefabContainingBaseObject;

		[Header("Possibility 2 - Creation way: GameObject + Object as component approach")]
		public GameObject nonPrefabBaseObject; // Can be used for referring to the latest spawned object, but not necessary
		public string nonPrefabName = "CodeBaseObject";

		[Header("Optional: Spawning location controls")]
		public bool useSpawnContoller = true;
		public SpawnLocationController spawnLocationController;

		public List<BaseObject> baseObjects;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			//if (prefabContainingBaseObject.name == nonPrefabBaseObject.name)
			//{
			//	Debug.Log("We do not want that");
			//}

			if (!useSpawnContoller)
			{
				spawnLocationController = GetComponentInChildren<SpawnLocationController>();
				print("Trying to find this");
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (Keyboard.current[activationKey].wasPressedThisFrame)
			{
				InstantiateObjectInSelectedWay();
			}
		}

		private void InstantiateObjectInSelectedWay()
		{
			switch (selectedWay)
			{
				case SelectedWay.PrefabWay:
					UsePrefabWay();
					break;
				case SelectedWay.CreationWay:
					UseCreationWay();
					break;
				default:
					print("No such way available");
					break;
			}

		}

		private void UsePrefabWay()
		{
			spawnLocationController.MoveSpawnLocation();
			GameObject instantiatedGO = Instantiate(prefabContainingBaseObject, GetSpawnPosition(), Quaternion.identity);
			baseObjects.Add(instantiatedGO.GetComponent<BaseObject>());
		}

		private void UseCreationWay()
		{
			spawnLocationController.MoveSpawnLocation();

			// Option 0: Create a GameObject with specific name and defined components. Add components as an Type array -> AddComponent<BaseObject>() will not be needed
			//Type[] componentsToAdd = new Type[] { typeof(BaseObject) };
			//GameObject spawnedGameObject = new GameObject(nonPrefabName, componentsToAdd);

			// Option 1: Create a new instance of the GameObject class with specific name and creating + setting the spawnedGameObject to reference the created instance
			GameObject spawnedGameObject = new GameObject(nonPrefabName);
			spawnedGameObject.transform.position = GetSpawnPosition();
			spawnedGameObject.AddComponent<BaseObject>();

			BaseObject BOScript = spawnedGameObject.GetComponent<BaseObject>();
			string randomName = "randomName";
			BOScript.ChangeName(randomName);
			randomName = "notRandomName";

			baseObjects.Add(BOScript);

			// Option 2: Create a new instance of the GameObject class with specific name and setting the nonPrefabBaseObject to reference the created instance
			// Next time a GameObject is created, nonPrefabBaseObject will lose its reference to the previous one, but the GameObject will still exist
			//nonPrefabBaseObject = new GameObject(nonPrefabName);
			//nonPrefabBaseObject.transform.position = GetSpawnPosition();
			//nonPrefabBaseObject.AddComponent<BaseObject>();

			// Test 1 (Optional): Adding the same component again
			//spawnedGameObject.AddComponent<BaseObject>();

			// Optional: Set the nonPrefabBaseObject to point to the latest spawned GameObject
			//nonPrefabBaseObject = spawnedGameObject;						
		}

		private Vector3 GetSpawnPosition()
		{
			if (!useSpawnContoller)
			{
				return spawnLocationController.spawnLocation.position;
			}
			return transform.position;
		}

	}

	[Serializable]
	public enum SelectedWay
	{
		PrefabWay,
		CreationWay
	}
}
