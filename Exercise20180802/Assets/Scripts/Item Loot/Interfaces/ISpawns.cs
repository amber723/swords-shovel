using UnityEngine;

public interface ISpawns
{
    /* 
     * The Use of Interface:
     * If you want to create a spawner of any kind, and you want to follow the 
     * ISpawns methods and structure,
     * Then it has to have everything that we've listed in here as a minimum.
     * So anything that's a requirement for a spawn should be listed in this
     * interface to force that implementation.
     */

    Rigidbody itemSpawned { get; set; }     //the item going to be spawned
    Renderer itemMaterial { get; set; }
    ItemPickUp itemType { get; set; }

    void CreateSpawn();


}
