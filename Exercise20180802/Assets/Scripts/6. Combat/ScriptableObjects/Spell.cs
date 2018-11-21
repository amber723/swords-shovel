using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell.asset", menuName = "Attack/Spell")]
public class Spell : AttackDefinition 
{
    public Projectile ProjectileToFire;
    public float ProjectileSpeed;

    /*
     * HotSpot will be the spawn point for the projectiles
     * Layer for the collision layer our projectile will be on. To make sure enemy
     * spells do not collide with other enemies and player spells do not collide
     * with the player
     */
    public void Cast(GameObject Caster, Vector3 HotSpot, Vector3 Target, int Layer)
    {
        // Fire Projectile at target
        Projectile projectile = Instantiate(ProjectileToFire, HotSpot, Quaternion.identity);
        projectile.Fire(Caster, Target, ProjectileSpeed, Range);

        // Set Projectile's collision layer
        projectile.gameObject.layer = Layer;

        /**
         * Listen to Projectile Collided Event
         * When we listen to an event, we'll want to add an event handler to the
         * event object.
         * **/
        projectile.ProjectileCollided += OnProjectileCollided;
    }

    private void OnProjectileCollided(GameObject Caster, GameObject Target)
    {
        // Attack landed on target, create attack and attack the target

        // Make sure both the Caster and Target are still alive
        if (Caster == null || Target == null)
            return;

        // create the attack
        var casterStats = Caster.GetComponent<CharacterStats>();
        var targetStats = Target.GetComponent<CharacterStats>();

        var attack = CreateAttack(casterStats, targetStats);

        // Send attack to all attackable behaviors of the target
        var attackables = Target.GetComponentsInChildren(typeof(IAttackable));
        foreach(IAttackable a in attackables)
        {
            a.OnAttack(Caster, attack);
        }
    }
}
