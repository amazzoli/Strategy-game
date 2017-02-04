using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Static methods used by multiple classes during the battle scene
/// </summary>
public static class BattleF
{
    /// <summary>
    /// Morale damage caused by charges
    /// </summary>
    public static float moraleDmgCharge
    {
        get { return 4; }
    }


    /// <summary>
    /// Morale damage caused by attacks at lateral sides.
    /// </summary>
    public static float moraleDmgLateralAttack
    {
        get { return 4; }
    }


    /// <summary>
    /// Morale damage caused by attacks at the back side.
    /// </summary>
    public static float moraleDmgBackAttack
    {
        get { return 5.5f; }
    }


    /// <summary>
    /// Maximal morale damage caused by the army soldiers lost. 
    /// The effective damage is given by the fraction of lost soldiers time this number.
    /// </summary>
    public static float maxMoraleDmgSoldierLost
    {
        get { return 10f; }
    }


    /// <summary>
    /// Dictionary which associates a string to a direcion
    /// </summary>
    public static Dictionary<Directions, string> dirToString = new Dictionary<Directions, string>() {
        {Directions.north, "Forward"}, {Directions.east, "Right"}, {Directions.south, "Backward"}, {Directions.west, "Left"},
    };

    /// <summary>
    /// Dictionary which associates a deg angle to a direcion
    /// </summary>
    public static Dictionary<Directions, float> dirToAngle = new Dictionary<Directions, float>() {
        {Directions.north, 0}, {Directions.east, 90}, {Directions.south, 180}, {Directions.west, 270},
    };

    /// <summary>
    /// Movement due to the army rotation as the distance covered by the army corner
    /// </summary>
    /// <param name="degAngle">Angle of rotation in degrees</param> 
    /// <param name="diagonal">Army diagonal</param> 
    /// <returns>Movement covered</returns>
    public static float RotationMovement(float degAngle, float diagonal)
    {
        return Mathf.Abs(Mathf.Sin(degAngle * Mathf.Deg2Rad / 2.0f) * diagonal);
    }

    /// <summary>
    /// Returns the army size given the army parameters
    /// </summary>
    /// <param name="nSoldiers">Number of soldiers in the army</param>
    /// <param name="areaPerSoldier">Number proportional proportional to a single soldier dimension</param>
    /// <param name="ratio">Ration between the army height and width</param>
    /// <returns>Army size</returns>
    public static Vector2 GetArmySize(int nSoldiers, float areaPerSoldier, float ratio)
    {
        float minArea = 10, areaAt30 = 30;
        float area = (minArea + (nSoldiers - 1) * (minArea - areaAt30) / (1 - 30)) * areaPerSoldier;
        float width = Mathf.Sqrt(area / ratio);
        float height = ratio * width;
        return new Vector2(width, height);
    }

    /// <summary>
    /// Hit probability: sigmoid function dependent on (attack - defense), which tends to 1 for attack >> defense
    /// and to zero for defense >> attack.
    /// The saturation rapidity depends on k 
    /// </summary>
    /// <param name="attack">Unit attack value</param>
    /// <param name="defense">Enemy defense value</param>
    /// <returns>The probability that the unit can generate a wound to the enemy</returns>
    public static float GetHitProbability(float attack, float defense)
    {
        float k = Mathf.Log(9) / 4.0f; // such that a difference (att - def) of 4 has hit prob = 9/10
        return 1.0f / (1.0f + Mathf.Exp(-k * (attack - defense)));
    }

    /// <summary>
    /// Moral damage defended with the unit mental strength. Uses the same function of "GetHitProbability"
    /// </summary>
    /// <param name="damage">Morale damage</param>
    /// <param name="defenderMentalStrength">Unit mental strength</param>
    /// <returns>Damage</returns>
    public static float GetMoraleDamage(float damage, float defenderMentalStrength)
    {
        float k = Mathf.Log(9) / 4.0f; // such that a difference (att - def) of 4 has hit prob = 9/10
        return 1.0f / (1.0f + Mathf.Exp(-k * (damage - defenderMentalStrength - 4)));
    }

    /// <summary>
    /// Generate the number of successful events given the total number of events and the sucessful probability
    /// </summary>
    /// <param name="nEvents">N of events</param>
    /// <param name="probability">Successful probability</param>
    /// <returns>N of successful events</returns>
    public static int GetSuccessfulEvents(int nEvents, float probability)
    {
        int count = 0;
        for (int i = 0; i < nEvents; i++)
            if (Random.value < probability)
                count++;
        return count;
    }
}
