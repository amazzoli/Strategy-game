using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Main class for the army managements. 
/// It stores all the information for the army combat (stats, enemies approached, skill controls, ...).
/// Must be attached to the parent object of the army prefab
/// </summary>
public class ArmyController : MonoBehaviour
{

    // PUBLIC VARIABLES

    /// <summary> Player owning the army </summary>
    public Player player;

    /// <summary> Army morale </summary>
    public float morale = 1;

    /// <summary> Class of the soldiers in the army. It contains the soldier stats, skills and stat modifiers.
    /// If it is null, means that the army is composed only of the hero. </summary>
    public Army army;

    /// <summary> Class of the hero in the army. </summary>
    public CombatUnit hero;

    /// <summary> Army symbol related to the army class (Sprite for the panels) </summary>
    public Sprite spriteSymbol;

    /// <summary> Army symbol related to the army class (Texture for the flag) </summary>
    public Texture textureSymbol;

    /// <summary> Symbol for the army engaged </summary>
    public GameObject battleSymbol;

    /// <summary> Body of the army. </summary>
    [HideInInspector]
    public Body body;

    /// <summary> Enemies engaged </summary>
    public EnemiesEngaged enemiesEngaged = new EnemiesEngaged();

    /// <summary> If the skill has been already used </summary>
    public bool skillUsed;

    /// <summary> The start parameter of the unit. </summary>
    public int startUnitSize, startEnergy;


    // PRIVATE VARIABLES
    GameController gc;
    public TextSpawner textSpawner;
    StatPanel statPanel;
    ErrorPanel errorPanel;
    ArmyBars bars;


    // MONOBEHAVIOUR METHODS

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        textSpawner = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Spawners").GetComponent<TextSpawner>();
        statPanel = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Stat Panel").GetComponent<StatPanel>();
        errorPanel = GameObject.FindGameObjectWithTag("Turn Panel").transform.GetChild(2).GetComponent<ErrorPanel>();
        bars = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("Spawners").GetComponent<ArmyBars>();
        body = transform.GetChild(0).transform.GetComponent<Body>();
        body.Resize();
        body.transform.GetComponent<Renderer>().material.mainTexture = player.armyBase;
        transform.GetChild(5).GetComponent<FlagColors>().SetFlag(textureSymbol);
        startUnitSize = army.nSoldiers;
        startEnergy = army.energy;
        if (hero != null) startUnitSize += hero.woundsNumber;
    }


    // PROPERTIES

    /// <returns><c>true</c> if this army is in combat; otherwise, <c>false</c>.</returns>
    public bool inCombat
    {
        get
        {
            if (enemiesEngaged.list.Count == 0)
                return false;
            else
                return true;
        }
    }


    // PUBLIC METHODS

    /// <summary>
    /// Sets the damage cause by wounds and checks if the unit is defeated.
    /// </summary>
    public void SetWoundDamage(int nWounds, ArmyController attacker, string actionName)
    {
        int oldNSoldiers = army.nSoldiers;
        army.UpdateSoldierList(nWounds);
        int newNSoldiers = army.nSoldiers;
        GameController.battleLog.Add(new DamageActionLog(attacker, this, actionName, EndStat.nSoldiers, oldNSoldiers - newNSoldiers));

        float moraleBaseDamage = (oldNSoldiers - newNSoldiers) * BattleF.maxMoraleDmgSoldierLost / (float)startUnitSize;

        if (army.woundsNumber > 1)
            textSpawner.SpawnWoundDamage(nWounds, this);
        textSpawner.SpawnSoldiersLost(oldNSoldiers - newNSoldiers, this);

        if (army.nSoldiers <= 0)
            ArmyDefeat(attacker);
        else
        {
            string soldiersLoss = "Loss of " + (oldNSoldiers - newNSoldiers).ToString() + " sodiers";
            SetMoraleDamage(moraleBaseDamage, attacker, soldiersLoss);
            body.Resize();
        }
    }


    /// <summary> Sets a morale damage and checks if the unit is defeated. </summary>
    public void SetMoraleDamage(float baseDamage, ArmyController attacker, string actionName)
    {
        float effectiveDamage = BattleF.GetMoraleDamage(baseDamage, army.mentalStrength);
        if ((morale - effectiveDamage) <= 0)
            effectiveDamage = morale;

        morale -= effectiveDamage;
        GameController.battleLog.Add(new DamageActionLog(attacker, this, actionName, EndStat.morale, effectiveDamage));

        textSpawner.SpawnMoraleDamage(effectiveDamage, this);
        if (morale <= 0)
            ArmyDefeat(attacker);
    }


    /// <summary> Sets a energy consuption and checks returns if there is enough energy. </summary>
    public void SetEnergyConsuption(int value)
    {
        if (value == 0)
            return;

        if ((army.energy - value) < 0)
        {
            errorPanel.LaunchErrorText("Not enough energy left");
        }
        else
        {
            army.energy -= value;
            statPanel.MakePanel(this);
            bars.SetBars(this);
            textSpawner.SpawnEnergyConsuption(value, this);
        }
    }


    /// <summary> Engage the specified enemy after a succesfull charge or attack. </summary>
    public void Engage(ArmyController enemy, Directions side)
    {
        enemiesEngaged.Add(enemy, Directions.north);
        enemy.enemiesEngaged.Add(this, side);
        battleSymbol.SetActive(true);
        battleSymbol.GetComponent<MeshRenderer>().material.renderQueue = 3098;
    }

    /// <summary> Disengage the specified enemy. </summary>
    public void Disengage(ArmyController enemy)
    {
        if (enemiesEngaged.GetDirection(enemy) == Directions.north && battleSymbol.activeSelf)
            battleSymbol.SetActive(false);
        enemiesEngaged.Remove(enemy);
    }


    public void AddStatModifier(StatModifier newMod, ArmyController caster)
    {
        bool modPresent = false;
        foreach (StatModifier statMod in army.statMods)
            if (statMod.title == newMod.title)
            {
                statMod.turnsLeft += (newMod.turnsLeft - 1);
                modPresent = true;
            }
        if (!modPresent)
            army.statMods.Add(newMod);
        GameController.battleLog.Add(new StatModActionLog(caster, this, newMod));
        textSpawner.SpawnNewStatModifier(newMod, this);
        if (caster == this)
            statPanel.MakePanel(this);
    }


    public void AddStatModifier(List<Stat> stats, List<float> values, int nTurns, string title, ArmyController caster)
    {
        StatModifier newMod = new StatModifier(stats, values, nTurns, title);
        AddStatModifier(newMod, caster);
    }


    public void AddStatModifier(Stat stat, float value, int nTurns, string title, ArmyController caster)
    {
        AddStatModifier(new List<Stat>() { stat }, new List<float>() { value }, nTurns, title, caster);
    }


    public void EndTurn()
    {
        skillUsed = false;
        Invoke("UpdateStatMod", 2 * Time.deltaTime);
    }


    // PRIVATE METHODS

    void ArmyDefeat(ArmyController attacker)
    {
        textSpawner.SpawnArmyText("Army defeated!", this, Color.black);
        GameController.battleLog.Add(new DefeatActionLog(this, attacker));

        gc.ArmyDefeated(this);
        GameController.ArmyTurn -= transform.GetChild(6).GetComponent<UnitPointer>().SetPointers;
        foreach (ArmyController enemyOnMe in enemiesEngaged.list)
            enemyOnMe.Disengage(this);
        Destroy(gameObject);
    }


    void UpdateStatMod()
    {
        List<StatModifier> modToRemove = new List<StatModifier>();
        foreach (StatModifier mod in army.statMods)
        {
            mod.turnsLeft -= 1;
            if (mod.turnsLeft == 0)
                modToRemove.Add(mod);
        }
        foreach (StatModifier mod in modToRemove)
        {
            textSpawner.SpawnEndStatModifier(mod, this);
            army.statMods.Remove(mod);
        }
    }
}
