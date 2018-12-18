using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour, ISpawns
{
    //? why required fields must be public while being private in interface
    public Rigidbody itemSpawned { get; set; }
    public Renderer itemMaterial { get; set; }
    public ItemPickUp itemType { get; set; }

    //A list of items that are capable of being chosen when we're creating the spawn
    public ItemPickUps_SO[] itemDefinitions;    

    int whichToSpawn = 0;       //which item in the list is going to be utilized when it spawns
    int totalSpawnWeight = 0;   //total weight of all the items existing in the spawner
    int chosen = 0;


    void Start ()
    {
        //calculate total weight
        foreach (ItemPickUps_SO ip in itemDefinitions)
        {
            totalSpawnWeight += ip.spawnChanceWeight;
        }
	}

    public void CreateSpawn ()
    {
        //Spawn with weighted possibilities
        chosen = Random.Range(0, totalSpawnWeight);

        foreach (ItemPickUps_SO ip in itemDefinitions)
        {
            /**  
             *  Weighted Possibilities:
             * Simply creating a bunch of ranges and stacking them end to end.
             * When you pick a random number, it's going to fall within one of 
             * those ranges.
             * The higher the weight, the more likely that is to be picked
             * because it has a bigger range.
             **/
            whichToSpawn += ip.spawnChanceWeight;

            if(whichToSpawn > chosen)
            {
                itemSpawned = Instantiate(ip.itemSpawnObject, 
                    new Vector3(transform.position.x, transform.position.y, 
                    transform.position.z), Quaternion.identity);

                itemMaterial = itemSpawned.GetComponent<Renderer>();
                if(itemMaterial != null)
                    itemMaterial.material = ip.itemMaterial;

                itemType = itemSpawned.GetComponent<ItemPickUp>();
                itemType.itemDefinition = ip;

                break;
            }
        }
    }
}
