using System;
using System.Net.NetworkInformation;
using UnityEngine;

namespace AH2728
{
    public class Bottle : MonoBehaviour
    {
        public Material bottleMaterial;
        public string bottleContents;
        [Range(0.0f, 1.0f)]
        public float liquidAmount;

        private float minimumLiquidCapacity = 0.0f;
        private float maximumLiquidCapacity = 1.0f;

        [SerializeField] private Vector3 bottleLocation;
        

        public bool isCapOn = true;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeLiquidAmount(float changeAmount)
        {
            //ToDo change the liquid amount and safeguard against going below or over the capacity of the bottle
            if (changeAmount < minimumLiquidCapacity) 
            {
                Console.WriteLine("Bottle contents can't be less than 0");
            }
            else if (changeAmount > maximumLiquidCapacity)
            {
                Console.WriteLine("Bottle contents can't exceed botle size, overflow.");
            }
            else
            {
                liquidAmount = changeAmount;
            }
        }
    }
}