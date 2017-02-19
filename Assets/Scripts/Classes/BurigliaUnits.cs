using UnityEngine;
using System.Collections;


public class Lancers : Army
{

    public Lancers()
    {
        faction = Faction.buriglia;
        armyClass = Class.Lancers;
        description = "<i>Buriglia</i>'s close combat unit. Its soldiers are disciplined and strong warriors. They are known for their reliable difense, made possible by their shields and their terrible row of sturdy lances.";
        woundsNumber = 1;
        energy = 5;
        movement = 18f;
        initiative = 6;
        phAtt = 4;
        attNumber = 1;
        phDef = 5f;
        neutralElemDef = 3.5f;
		mentalStrength = 5f;
        precision = 50;
        heightWidthRatio = 0.7f;
        areaPerSoldier = 1;
        skills.Add(new MeleeAttack());
        skills.Add(new WallOfShields());
        //elemResistence.Add(Element.fire);
        //statMods.Add (new StatModifier(Stat.elemDef, 2f, 2, "Prova", "Elemental defense increased by 2"));
        //statMods.Add (new StatModifier(Stat.phDef, 1f, 3, "Prova", "Physical defense increased by 3"));
    }
}


public class Macers : Army
{

    public Macers()
    {
        faction = Faction.buriglia;
        armyClass = Class.Macers;
        description = "<i>Buriglia</i>'s close combat unit. Its warriors are strong and brutal, they weild heavy maces which cause terrible damage to their opponents. Despite their frightening appearence, the camaraderie is strong among them, so that the lost of a friend, make their attacks more and more dreadful.";
        woundsNumber = 1;
        energy = 6;
        movement = 22f;
        initiative = 6;
        phAtt = 5;
        attNumber = 1;
        phDef = 3f;
        neutralElemDef = 3.5f;
		mentalStrength =4f;
        precision = 40;
        heightWidthRatio = 0.7f;
        areaPerSoldier = 1;
        skills.Add(new MeleeAttack());
        //statMods.Add (new StatModifier(Stat.elemDef, 2f, 2, "Prova", "Elemental defense increased by 2"));
        //statMods.Add (new StatModifier(Stat.phDef, 1f, 3, "Prova", "Physical defense increased by 3"));
    }
}



public class Shellfighters : Army
{

    public Shellfighters()
    {
        faction = Faction.buriglia;
        armyClass = Class.Shellfighters;
        description = "<i>Buriglia</i>'s close combat unit. Its components are skilled and fast fighters, expert in the use of the exotic weapon called Shellclaw. While they are good in terms of mobility, they lack in difense. But their ace in the hole is the wide range of skills the can use: from explosives attacks to blinding shots.";
        woundsNumber = 1;
        energy = 5;
        movement = 24;
        initiative = 7;
        phAtt = 4;
        attNumber = 1;
        phDef = 3.5f;
		mentalStrength = 3.5f;
        neutralElemDef = 3.5f;
        precision = 60;
        heightWidthRatio = 0.6f;
        areaPerSoldier = 1;
        skills.Add(new MeleeAttack());
        //elemWeakness.Add(Element.fire);
    }
}



public class Spongezookers : Army
{
    public Spongezookers()
    {
        faction = Faction.buriglia;
        armyClass = Class.Spongezookers;
        description = "<i>Buriglia</i>'s distant combat unit. Its components weild huges bazookas, made of sponge: this underwater technology can incredibly combine the power of gunpowder and water, making it a vary dangerous and strategic unit.";
        woundsNumber = 1;
        energy = 5;
        movement = 16f;
        initiative = 4f;
        phAtt = 3.5f;
        attNumber = 1;
        phDef = 4f;
        neutralElemDef = 3.5f;
		mentalStrength = 3f;
        precision = 60;
        heightWidthRatio = 1f;
        areaPerSoldier = 1;
        skills.Add(new MeleeAttack());
        skills.Add(new Bazooking());
        skills.Add(new BubbleCannon());
    }
}

