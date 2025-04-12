namespace Game.Core.Models;

public class Stats
{
    private float _maxHealth;

    public float MaxHealth
    {
        get => _maxHealth.RoundTo1();
        set => _maxHealth = value;
    }

    private float _currentHealth;
    public float CurrentHealth
    {
        get => _currentHealth.RoundTo1();
        set => _currentHealth =  value;
    }

    private float _armor;
    public float Armor
    {
        get => _armor.RoundTo1();
        set => _armor = value;
    }

    private float _damage;
    public float Damage
    {
        get => _damage.RoundTo1();
        set => _damage = value;
    }

    private float _debuffResistance;
    public float DebuffResistance
    {
        get => _debuffResistance.RoundTo1();
        set => _debuffResistance = value;
    }

    private float _criticalChance;
    public float CriticalChance
    {
        get => _criticalChance.RoundTo1();
        set => _criticalChance = value;
    }

    private float _criticalDamage;
    public float CriticalDamage
    {
        get => _criticalDamage.RoundTo1();
        set => _criticalDamage = value;
    }

    private float _dodgeChance;
    public float DodgeChance
    {
        get => _dodgeChance.RoundTo1();
        set => _dodgeChance = value;
    }
}