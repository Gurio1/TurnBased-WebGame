using Game.Utilities.Extensions;

namespace Game.Core.PlayerProfile.ValueObjects;

public class Stats
{
    private float armor;
    
    private float criticalChance;
    
    private float criticalDamage;
    
    private float currentHealth;
    
    private float damage;
    
    private float debuffResistance;
    
    private float dodgeChance;
    private float maxHealth;
        
    
    public float MaxHealth
    {
        //TODO: WTF?? DO i really need float?Do i want to use it?
        get => maxHealth.RoundTo1();
        set => maxHealth = value;
    }
    
    public float CurrentHealth
    {
        get => currentHealth.RoundTo1();
        set => currentHealth = value;
    }
    
    public float Armor
    {
        get => armor.RoundTo1();
        set => armor = value;
    }
    
    public float Damage
    {
        get => damage.RoundTo1();
        set => damage = value;
    }
    
    public float DebuffResistance
    {
        get => debuffResistance.RoundTo1();
        set => debuffResistance = value;
    }
    
    public float CriticalChance
    {
        get => criticalChance.RoundTo1();
        set => criticalChance = value;
    }
    
    public float CriticalDamage
    {
        get => criticalDamage.RoundTo1();
        set => criticalDamage = value;
    }
    
    public float DodgeChance
    {
        get => dodgeChance.RoundTo1();
        set => dodgeChance = value;
    }
}
