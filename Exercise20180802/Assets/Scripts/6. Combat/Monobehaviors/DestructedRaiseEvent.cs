using UnityEngine;

public class DestructedRaiseEvent : MonoBehaviour, IDestructible
{
    NPCController mob;

    public void Awake()
    {
        mob = GetComponent<NPCController>();
    }

    public void OnDestruction(GameObject destroyer)
    {
        mob.OnMobDeath.Invoke(mob.mobType, transform.position);
    }
}
