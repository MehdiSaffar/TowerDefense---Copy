
public static class EventManager {
    public delegate void OnMoneyUpdate(int newMoney);
    public delegate void OnHealthUpdate(int newHealth);
    public delegate void OnWaveIndexUpdate(int newWaveIndex);
    public delegate void OnBaseDie();
    public delegate void OnTowerSell();


    public static event OnMoneyUpdate MoneyUpdate;
    public static event OnHealthUpdate HealthUpdate;
    public static event OnWaveIndexUpdate WaveIndexUpdate;
    public static event OnBaseDie BaseDie;
    public static event OnTowerSell TowerSell;

    public static void TriggerMoneyUpdate(int newMoney) {
        if(MoneyUpdate != null) MoneyUpdate(newMoney);
    }
    public static void TriggerHealthUpdate(int newHealth) {
        if(HealthUpdate != null) HealthUpdate(newHealth);
        if (newHealth == 0 && BaseDie != null) BaseDie();
    }
    public static void TriggerWaveIndexUpdate(int newWaveIndex) {
        if(WaveIndexUpdate != null) WaveIndexUpdate(newWaveIndex);
    }
    public static void TriggerBaseDie()
    {
        if (BaseDie != null) BaseDie();
    }
    public static void TriggerTowerSell()
    {
        if (TowerSell != null) TowerSell();
    }
}
