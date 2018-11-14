
public class Attack
{
    public int Damage { get; }
    public bool IsCritical { get; }

    public Attack(int damage, bool critical)
    {
        Damage = damage;
        IsCritical = critical;
    }
}
