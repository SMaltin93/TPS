using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectRandom : MonoBehaviour
{
   [SerializeField] private GameObject ObjectToSpawn;

  // [SerializeField] private GameObject SpawnPoint;
   // chose position diameter between 0 and 100 

   // make slider in unity

 [SerializeField] private float planEnd;

 [SerializeField] private float planStart;

   // FallHeight y as max 100 
 [SerializeField] private float FallHeight;



   public GameObject PlaceObject()
   { 
      Vector3 randomPosition = RandomPosition();
      GameObject obj = Instantiate(ObjectToSpawn, randomPosition, Quaternion.identity);
      return obj;
      
   }

   private Vector3 RandomPosition()
   {
      float x = Random.Range(planStart, planEnd);
      float y = FallHeight;
      float z = Random.Range(planStart, planEnd);
      Vector3 randomPosition = new Vector3(x, y, z);
      return randomPosition;        
   }


}
