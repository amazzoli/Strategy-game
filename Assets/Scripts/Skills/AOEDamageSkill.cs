using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public abstract class AOEDamageSkill : DamageSkill
{
    protected AOEArea area;
    protected float extAreaRadius = 5;
    protected int nSubAreas = 1;
    //public List<int> armyIndexInSubArea = new List<int>();
    Coroutine areaMovement;


    protected override void TargetChoice()
    {
        AOEArea areaModel = GameObject.FindGameObjectWithTag("GameController").GetComponent<Resources>().area;
        area = GameObject.Instantiate(areaModel, new Vector3(0, -10, 0), Quaternion.identity);
        area.MakeArea(nSubAreas, extAreaRadius);
        MakeTheAreaMove();
    }


    protected override void SetStats()
    {
        precision = new List<float>(new float[targets.Count]);
        attack = new List<float>(new float[targets.Count]);
        defense = new List<float>(new float[targets.Count]);
        damageProb = new List<float>(new float[targets.Count]);
        nHits = new List<int>(new int [targets.Count]);
        statMods = new List<StatModifier>(new StatModifier[targets.Count]);

        for (int i = 0; i < nSubAreas; i++)
        {
            foreach (ArmyController armyCtrl in area.GetOverlappedArmies(i))
            {
                int armyIndex = targets.IndexOf(armyCtrl);
                nHits[armyIndex] = GetNHitsInSubArea(i, armyIndex);
                precision[armyIndex] = GetPrecisionInSubArea(i, armyIndex);
                attack[armyIndex] = GetAttackInSubArea(i, armyIndex);
                defense[armyIndex] = GetDefenseInSubArea(i, armyIndex);
                damageProb[armyIndex] = GetDamageProbInSubArea(i, armyIndex);
                statMods[armyIndex] = GetStatModInSubArea(i, armyIndex);
            }
        }
    }


    protected override void ResetTargetChoice()
    {
        if (area != null)
        {
            area.StopMyMovement();
            GameObject.Destroy(area.gameObject);
        }
    }

    protected abstract void MakeTheAreaMove();

    protected abstract int GetNHitsInSubArea(int subAreaIndex, int targetIndex);

    protected abstract float GetPrecisionInSubArea(int subAreaIndex, int targetIndex);

    protected abstract float GetAttackInSubArea(int subAreaIndex, int targetIndex);

    protected abstract float GetDefenseInSubArea(int subAreaIndex, int targetIndex);

    protected virtual float GetDamageProbInSubArea(int subAreaIndex, int targetIndex)
    {
        return BattleF.GetHitProbability(GetAttackInSubArea(subAreaIndex, targetIndex), GetDefenseInSubArea(subAreaIndex, targetIndex));
    }

    protected virtual StatModifier GetStatModInSubArea(int i, int armyIndex) { return null; }
}