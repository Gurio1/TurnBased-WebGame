namespace Game.Core.Models;

public class Stats
{
    private float maxHealth;

    public float MaxHealth
    {
        get => maxHealth.RoundTo1();
        set => maxHealth = value;
    }

    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth.RoundTo1();
        set => currentHealth =  value;
    }

    private float armor;
    public float Armor
    {
        get => armor.RoundTo1();
        set => armor = value;
    }

    private float damage;
    public float Damage
    {
        get => damage.RoundTo1();
        set => damage = value;
    }

    private float debuffResistance;
    public float DebuffResistance
    {
        get => debuffResistance.RoundTo1();
        set => debuffResistance = value;
    }

    private float criticalChance;
    public float CriticalChance
    {
        get => criticalChance.RoundTo1();
        set => criticalChance = value;
    }

    private float criticalDamage;
    public float CriticalDamage
    {
        get => criticalDamage.RoundTo1();
        set => criticalDamage = value;
    }

    private float dodgeChance;
    public float DodgeChance
    {
        get => dodgeChance.RoundTo1();
        set => dodgeChance = value;
    }
}
