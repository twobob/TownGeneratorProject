using UnityEngine;
using System.Collections;

public class PlaceObjects : MonoBehaviour {

    GameObject[] objs;

   public  GameObject CoinPrefab;

   	public int SkimpFactorEnabled = 2;
	private int weJustSkimped = 0;
	
   public int TotalToSpawn;
   private int spawnedSoFar;
   
    // Use this for initialization
    void Start () {

      	  objs = GameObject.FindGameObjectsWithTag("Respawn");
		//	Spawn();

    }
    
    // Update is called once per frame
    void Update () {
    
	if (SkimpFactorEnabled >0)
			{
				weJustSkimped++;
				
				// Hardcode 0 or > 1
				if (SkimpFactorEnabled == 1)
					SkimpFactorEnabled++;
				
				
				if (weJustSkimped - SkimpFactorEnabled == 0)	
				{ 
				weJustSkimped = 0;
				//Debug.Log( weJustSkimped - SkimpFactorEnabled);
				Spawn();
				}
			}	
    }

	
	
	
	void Spawn(){
		
	 if (spawnedSoFar >= TotalToSpawn )
		return;
        
		foreach (var item in objs)
        {

            if (Random.Range(0,10) > 4 )
            {
                Instantiate(CoinPrefab, item.transform.position + (Vector3.up * 10), Quaternion.identity);

spawnedSoFar ++;
            }


        }

		
	}
	
	
	
	
	
	
	
	
	}
