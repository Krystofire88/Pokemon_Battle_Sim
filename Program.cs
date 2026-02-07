using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
public enum Status
{
    None = 0,
    Burn = 1,
    Freeze = 2,
    Paralysis = 3,
    Poison = 4,
    Toxic = 5,
    Sleep = 6,
    Confusion = 7,
    Infatuation = 8,
    Flinch = 9,
    Torment = 10,
    Drowsy = 11,
    EscapePrevent = 12,
    Bound = 13,
    Vinelash = 22,
    Wildfire = 23,
    Cannonade = 24,
}
public enum Stat
{
    None = 0,
    Atk = 1,
    Def = 2,
    Spa = 3,
    Spd = 4,
    Spe = 5,
    Acc = 6,
    Eva = 7
}
public enum Weather
{
    None = 0,
    Sun = 1,
    Rain = 2,
    Sandstorm = 3,
    Snow = 4,
    HarshSun = 5,
    HeavyRain = 6,
    StrongWinds = 7
}
public enum Terrain
{
    None = 0,
    Electric = 1,
    Grassy = 2,
    Misty = 3,
    Psychic = 4
}
public enum Type
{
    None = 0,
    Normal = 1,
    Fire = 2,
    Water = 3,
    Electric = 4,
    Grass = 5,
    Ice = 6,
    Fighting = 7,
    Poison = 8,
    Ground = 9,
    Flying = 10,
    Psychic = 11,
    Bug = 12,
    Rock = 13,
    Ghost = 14,
    Dragon = 15,
    Dark = 16,
    Steel = 17,
    Fairy = 18,
    Stellar = 19
}
public enum Split
{
    Physical = 1,
    Special = 2,
    Status = 3
}
public class Trainer
{
    public string name { get; }
    int numPoke = 0;
    public List<Pokemon> team = new List<Pokemon>();
    public int wins { get; set; } = 0;
    public bool ace { get; set; }
    public Trainer(string name, bool ace)
    {
        this.name = name;
        this.ace = ace;
    }
    public void AddPokemon(Pokemon pokemon)
    {
        if (pokemon.moveNum == 0 || pokemon == null || pokemon.species == null) return;
        if (numPoke < 6)
        {
            team.Add(pokemon);
            numPoke++;
        }
    }
    public void RemovePokemon(Pokemon pokemon)
    {
        if (team.Contains(pokemon))
        {
            team.Remove(pokemon);
            numPoke--;
        }
    }
    public void HealTeam()
    {
        foreach (Pokemon p in team)
        {
            p.Heal();
        }
    }
    public Pokemon ShouldSwitch(Pokemon active, Pokemon opp, int ai)
    {
        if (active.hp <= 0)
        { 
            if (ai == 1) return active;
            if (active.statusNonVol == Status.EscapePrevent) return active;
        }
        int switchTo = 0;
        if ((0.25 >= (double)(active.hp / active.maxHP)) && active.hp > 0)
        {
            return active;
        }
        double highestScore = 0;
        int availablePk = team.Count();
        if (ace)
        {
            availablePk = team.Count() - 1;

            bool onlyAceLeft = true;
            for (int i = 0; i < availablePk; i++)
            {
                if (team[i].hp > 0)
                {
                    onlyAceLeft = false;
                    break;
                }
            }
            if (onlyAceLeft && team[team.Count() - 1].hp > 0)
            {
                return team[team.Count() - 1];
            }
        }
        for (int i = 0; i < availablePk; i++)
        {
            if (team[i].hp > 0 && team[i] != active)
            {
                double typing = Program.MatchUp(team[i].species.type1, opp.species.type1) * Program.MatchUp(team[i].species.type2, opp.species.type1) * Program.MatchUp(team[i].species.type1, opp.species.type2) * Program.MatchUp(team[i].species.type2, opp.species.type2);

                if (ai == 4)
                {
                    if (highestScore < Program.FindBestMove(team[i], opp) * typing)
                    {
                        highestScore = Program.FindBestMove(team[i], opp) * typing;
                        switchTo = i;
                    }
                }
                else
                {
                    if (highestScore < typing)
                    {
                        highestScore = typing;
                        switchTo = i;
                    }
                }
            }
        }
        double currentPossible = Program.FindBestMove(active, opp) * Program.MatchUp(active.species.type1, opp.species.type1) * Program.MatchUp(active.species.type2, opp.species.type1) * Program.MatchUp(active.species.type1, opp.species.type2) * Program.MatchUp(active.species.type2, opp.species.type2);
        if (highestScore < currentPossible * 2 && active.hp > 0)
        {
            return active;
        }
        return team[switchTo];
    }
    public bool AbleToBattle()
    {
        bool able = false;
        foreach (Pokemon p in team)
        {
            if (p.hp > 0)
            {
                able = true; break;
            }
        }
        return able;
    }
    public bool LastInBattle()
    {
        bool last = true;
        bool check = false;
        foreach (Pokemon p in team)
        {
            if (p.hp > 0)
            {
                if (check) last = false;
                check = true;
                break;
            }
        }
        return last;
    }
}
public class Species
{
    public string name { get; }
    public Type type1 { get; }
    public Type type2 { get; }
    public int Hp { get; }
    public int Atk { get; }
    public int Def { get; }
    public int Spa { get; }
    public int Spd { get; }
    public int Spe { get; }
    public string ability1 { get; }
    public string ability2 { get; }
    public string abilityH { get; }
    public bool noRatio { get; }
    public int ratio { get; }
    public bool mega { get; }
    public bool gmax { get; }
    public Species(string name, Type type1, Type type2, int Hp, int Atk, int Def, int Spa, int Spd, int Spe, string ability1, string ability2, string abilityH, bool noRatio, int ratio, bool mega, bool gmax)
    {
        this.name = name;
        this.type1 = type1;
        this.type2 = type2;
        this.Hp = Hp;
        this.Atk = Atk;
        this.Def = Def;
        this.Spa = Spa;
        this.Spd = Spd;
        this.Spe = Spe;
        this.ability1 = ability1;
        this.ability2 = ability2;
        this.abilityH = abilityH;
        this.noRatio = noRatio;
        this.ratio = ratio;
        this.mega = mega;
        this.gmax = gmax;
    }
}
public class Pokemon
{
    public Species species { get; set; }
    public string name { get; set; }
    public bool gender { get; }
    public int level { get; }
    public int maxHP { get; private set; }
    public int hp { get; set; }
    public int ability { get; }
    public Status statusNonVol { get; set; } = Status.None;
    public List<Status> statusVol { get; set; } = new List<Status>();
    public int HpIV, HpEV, AtkIV, AtkEV, DefIV, DefEV, SpaIV, SpaEV, SpdIV, SpdEV, SpeIV, SpeEV;
    public int AtkMod, DefMod, SpaMod, SpdMod, SpeMod, AccMod, EvaMod;
    public string nature { get; }
    public Item heldItem { get; set; }
    public bool gmax { get; set; }
    public int dMaxLevel { get; }
    public bool isDmax { get; set; } = false;
    public Type tera { get; }
    public bool terastallized { get; set; } = false;
    public int dMaxTimer { get; set; } = 0;
    public int sleepTimer { get; set; } = 0;
    public bool yawn { get; set; } = false;
    public int confusionTimer { get; set; } = 0;
    public int toxicCounter { get; set; } = 0;
    public int tormentCounter { get; set; } = 0;
    public int boundCounter { get; set; } = 0;
    public int critRatio { get; set; } = 24;
    public bool chargingMove { get; set; } = false;
    public bool reCharge { get; set; } = false;
    public bool invurnable { get; set; } = false;
    public bool maxInvurnable { get; set; }
    public int protectTimes { get; set; } = 0;
    public Move lastMove { get; set; } = null;
    public Move selectedMove { get; set; } = null;
    public bool usedZMove { get; set; } = false;
    public bool ZMove { get; set; } = false;
    public bool moveFirst { get; set; } = false;
    public bool lockedMove { get; set; } = false;
    public int lockTimer { get; set; } = 0; 
    public bool endure { get; set; } = false;
    public int furyCutter { get; set; } = 0;
    public int echoedVoice { get; set; } = 0;
    public Move[] moveSetBase = new Move[4];
    public Move[] moveSet = new Move[4];
    public int moveNum { get; private set; } = 0;
    public int wins { get; set; }
    public Pokemon(Species species, string name, bool gender, int level, int ability, int HpIV, int HpEV, int AtkIV, int AtkEV, int DefIV, int DefEV, int SpaIV, int SpaEV, int SpdIV, int SpdEV, int SpeIV, int SpeEV, string nature, Item heldItem, bool gmax, int dMaxLevel, Type tera)
    {
        this.species = species;
        this.name = name;
        this.gender = gender;
        this.level = level;
        this.ability = ability;
        this.HpIV = HpIV;
        this.HpEV = HpEV;
        this.AtkIV = AtkIV;
        this.AtkEV = AtkEV;
        this.DefIV = DefIV;
        this.DefEV = DefEV;
        this.SpaIV = SpaIV;
        this.SpaEV = SpaEV;
        this.SpdIV = SpdIV;
        this.SpdEV = SpdEV;
        this.SpeIV = SpeIV;
        this.SpeEV = SpeEV;
        this.AtkMod = 0;
        this.DefMod = 0;
        this.SpaMod = 0;
        this.SpdMod = 0;
        this.SpeMod = 0;
        this.AccMod = 0;
        this.EvaMod = 0;
        this.nature = nature;
        this.heldItem = heldItem;
        this.gmax = gmax;
        this.dMaxLevel = dMaxLevel;
        this.tera = tera;
        this.maxHP = CalcHp();
        this.hp = maxHP;
    }
    public Pokemon(Species species, int level)
    {
        this.species = species;
        this.name = species.name;
        int gender = Random.Shared.Next(1, 101);
        bool g = false;
        if (gender <= species.ratio)
        {
            g = true;
        }
        this.gender = g;
        int ability = 1;
        if (species.ability2 != "")
        {
            ability = Random.Shared.Next(1, 3);
        }
        this.ability = ability;
        this.level = level;
        this.HpIV = Random.Shared.Next(0, 32);
        this.HpEV = 0;
        this.AtkIV = Random.Shared.Next(0, 32);
        this.AtkEV = 0;
        this.DefIV = Random.Shared.Next(0, 32);
        this.DefEV = 0;
        this.SpaIV = Random.Shared.Next(0, 32);
        this.SpaEV = 0;
        this.SpdIV = Random.Shared.Next(0, 32);
        this.SpdEV = 0;
        this.SpeIV = Random.Shared.Next(0, 32);
        this.SpeEV = 0;
        this.AtkMod = 0;
        this.DefMod = 0;
        this.SpaMod = 0;
        this.SpdMod = 0;
        this.SpeMod = 0;
        this.AccMod = 0;
        this.EvaMod = 0;
        string[] natures = {
        "Hardy", "Lonely", "Brave", "Adamant", "Naughty",
        "Bold", "Docile", "Relaxed", "Impish", "Lax",
        "Timid", "Hasty", "Serious", "Jolly", "Naive",
        "Modest", "Mild", "Quiet", "Bashful", "Rash",
        "Calm", "Gentle", "Sassy", "Careful", "Quirky"
        };

        string n = natures[Random.Shared.Next(0, 25)];
        this.nature = n;
        this.heldItem = null;
        bool gMax = false;
        if (0 == Random.Shared.Next(0, 101))
        {
            gMax = true;
        }
        this.gmax = gMax;
        this.dMaxLevel = 10;
        Type typ = species.type1;
        if (species.type2 != 0)
        {
            if (Random.Shared.Next(0, 2) == 1) typ = species.type2;
        }
        this.tera = typ;
        this.maxHP = CalcHp();
        this.hp = maxHP;
        this.wins = 0;
    }
    public void AddMove(Move move)
    {
        if (move == null || move.moveB == null) return;
        if (moveNum < 4)
        {
            moveSetBase[moveNum] = move;
            moveSet[moveNum] = move;
            moveNum++;
        }
    }
    public bool CheckForMove(MoveB moveb)
    {
        bool isMove = false;
        for (int i = 0; i < 4; i++)
        {
            if (moveSet[i] == null)
            {
                break;
            }
            if (moveSet[i].moveB == moveb)
            {
                isMove = true;
                break;
            }
        }
        return isMove;
    }
    public void Modifiers()
    {
        Console.Write($"Atk: {AtkMod} / ");
        Console.Write($"Def: {DefMod} / ");
        Console.Write($"Spa: {SpaMod} / ");
        Console.Write($"Spd: {SpdMod} / ");
        Console.Write($"Acc: {AccMod} / ");
        Console.Write($"Eva: {EvaMod} / ");
        Console.WriteLine($"Spe: {SpeMod} ");
    }
    public void ClearMods()
    {
        AtkMod = 0;
        DefMod = 0;
        SpaMod = 0;
        SpdMod = 0;
        SpeMod = 0;
        AccMod = 0;
        EvaMod = 0;
    }
    public void Heal()
    {
        hp = maxHP;
        HealConditions();
        ClearMods();
        foreach (Move m in moveSet)
        {
            if (m != null)
            {
                m.PP = m.moveB.maxPP;
            }
        }
        UnMegaEvolve();
        UnDmax();
        UnTerastallize();
        dMaxTimer = 0;
        lastMove = null;
        invurnable = false;
        chargingMove = false;
        reCharge = false;
        usedZMove = false;
        ZMove = false;
        moveFirst = false;
    }
    public void HealConditions()
    {
        statusNonVol = 0;
        statusVol.Clear();
        sleepTimer = 0;
        confusionTimer = 0;
        toxicCounter = 0;
        tormentCounter = 0;
    }
    public void PokeInfo()
    {
        if (name != species.name)
        {
            Console.Write(name);
            Console.Write($"({species.name})");
        }
        else
        {
            Console.Write(species.name);
        }
        if (species.noRatio != true)
        {
            if (gender == true) Console.Write(" (M) ");
            else Console.Write(" (F) ");
        }
        if (heldItem != null)
        {
            Console.WriteLine($" @ {heldItem.name}");
        }
        else
        {
            Console.WriteLine();
        }
        Console.WriteLine($"Ability: {FetchAbility(ability)}");
        Console.WriteLine($"Level: {level}");
        Console.WriteLine($"Tera Type: {GetType(tera)}");


        var evParts = new List<string>();
        if (HpEV != 0) evParts.Add($"{HpEV} HP");
        if (AtkEV != 0) evParts.Add($"{AtkEV} Atk");
        if (DefEV != 0) evParts.Add($"{DefEV} Def");
        if (SpaEV != 0) evParts.Add($"{SpaEV} Spa");
        if (SpdEV != 0) evParts.Add($"{SpdEV} Spd");
        if (SpeEV != 0) evParts.Add($"{SpeEV} Spe");

        if (evParts.Count > 0)
        {
            Console.WriteLine($"EVs: {string.Join(" / ", evParts)}");
        }

        Console.WriteLine($"{nature} Nature");

        var ivParts = new List<string>();
        if (HpIV != 0) ivParts.Add($"{HpIV} HP");
        if (AtkIV != 0) ivParts.Add($"{AtkIV} Atk");
        if (DefIV != 0) ivParts.Add($"{DefIV} Def");
        if (SpaIV != 0) ivParts.Add($"{SpaIV} Spa");
        if (SpdIV != 0) ivParts.Add($"{SpdIV} Spd");
        if (SpeIV != 0) ivParts.Add($"{SpeIV} Spe");

        if (ivParts.Count > 0)
        {
            Console.WriteLine($"IVs: {string.Join(" / ", ivParts)}");
        }

        for (int i = 0; i < moveSet.Length; i++)
        {
            if (moveSet[i] == null)
            {
                i += 4;
            }
            else
            {
                Console.WriteLine($"-{moveSet[i].moveB.name}");
            }
        }
        Console.WriteLine();
    }
    public string GetType(Type type)
    {
        return type switch
        {
            Type.None => "",
            Type.Normal => "Normal",
            Type.Fire => "Fire",
            Type.Water => "Water",
            Type.Electric => "Electric",
            Type.Grass => "Grass",
            Type.Ice => "Ice",
            Type.Fighting => "Fighting",
            Type.Poison => "Poison",
            Type.Ground => "Ground",
            Type.Flying => "Flying",
            Type.Psychic => "Psychic",
            Type.Bug => "Bug",
            Type.Rock => "Rock",
            Type.Ghost => "Ghost",
            Type.Dragon => "Dragon",
            Type.Dark => "Dark",
            Type.Steel => "Steel",
            Type.Fairy => "Fairy",
            _ => "error"
        };
    }
    public double GetMod(int value)
    {
        return value switch
        {
            -6 => 0.25,
            -5 => 0.28,
            -4 => 0.33,
            -3 => 0.4,
            -2 => 0.5,
            -1 => 0.67,
            0 => 1.0,
            1 => 1.5,
            2 => 2.0,
            3 => 2.5,
            4 => 3.0,
            5 => 3.5,
            6 => 4.0,
            _ => 0.0
        };
    }
    public string FetchAbility(int ability)
    {
        return ability switch
        {
            1 => species.ability1,
            2 => species.ability2,
            3 => species.abilityH,
            _ => "error"
        };
    }
    public int CalcAtkStat()
    {
        double naturee = 1.00;
        string[] UpAtk = new string[4];
        string[] DownAtk = new string[4];
        UpAtk = ["Adamant", "Lonely", "Naughty", "Brave"];
        DownAtk = ["Modest", "Bold", "Calm", "Timid"];
        if (UpAtk.Contains(nature))
        {
            naturee = 1.1;
        }
        else if (DownAtk.Contains(nature))
        {
            naturee = 0.9;
        }
        else
        {
            naturee = 1.0;
        }
        int stat = Convert.ToInt32(Math.Floor((Math.Floor(((2 * species.Atk + AtkIV + Math.Floor((double)AtkEV / 4)) * level) / 100) + 5) * naturee));
        return stat;
    }
    public int CalcDefStat()
    {
        double naturee = 1.00;
        string[] UpDef = new string[4];
        string[] DownDef = new string[4];
        UpDef = ["Bold", "Impish", "Lax", "Relaxed"];
        DownDef = ["Lonely", "Mild", "Gentle", "Hasty"];
        if (UpDef.Contains(nature))
        {
            naturee = 1.1;
        }
        else if (DownDef.Contains(nature))
        {
            naturee = 0.9;
        }
        else
        {
            naturee = 1.0;
        }
        int stat = Convert.ToInt32(Math.Floor((Math.Floor(((2 * species.Def + DefIV + Math.Floor((double)DefEV / 4)) * level) / 100) + 5) * naturee));
        return stat;
    }
    public int CalcSpaStat()
    {
        double naturee = 1.00;
        string[] UpSpa = new string[4];
        string[] DownSpa = new string[4];
        UpSpa = ["Modest", "Mild", "Rash", "Quiet"];
        DownSpa = ["Adamant", "Impish", "Careful", "Jolly"];
        if (UpSpa.Contains(nature))
        {
            naturee = 1.1;
        }
        else if (DownSpa.Contains(nature))
        {
            naturee = 0.9;
        }
        else
        {
            naturee = 1.0;
        }
        int stat = Convert.ToInt32(Math.Floor((Math.Floor(((2 * species.Spa + SpaIV + Math.Floor((double)SpaEV / 4)) * level) / 100) + 5) * naturee));
        return stat;
    }
    public int CalcSpdStat()
    {
        double naturee = 1.00;
        string[] UpSpd = new string[4];
        string[] DownSpd = new string[4];
        UpSpd = ["Calm", "Gentle", "Careful", "Sassy"];
        DownSpd = ["Naughty", "Lax", "Rash", "Naive"];
        if (UpSpd.Contains(nature))
        {
            naturee = 1.1;
        }
        else if (DownSpd.Contains(nature))
        {
            naturee = 0.9;
        }
        else
        {
            naturee = 1.0;
        }
        int stat = Convert.ToInt32(Math.Floor((Math.Floor(((2 * species.Spd + SpdIV + Math.Floor((double)SpdEV / 4)) * level) / 100) + 5) * naturee));
        return stat;
    }
    public int CalcSpeStat()
    {
        double naturee = 1.00;
        string[] UpSpe = new string[4];
        string[] DownSpe = new string[4];
        UpSpe = ["Timid", "Hasty", "Jolly", "Naive"];
        DownSpe = ["Brave", "Relaxed", "Quiet", "Sassy"];
        if (UpSpe.Contains(nature))
        {
            naturee = 1.1;
        }
        else if (DownSpe.Contains(nature))
        {
            naturee = 0.9;
        }
        else
        {
            naturee = 1.0;
        }
        int stat = Convert.ToInt32(Math.Floor((Math.Floor(((2 * species.Spe + SpeIV + Math.Floor((double)SpeEV / 4)) * level) / 100) + 5) * naturee));
        return stat;
    }
    public int CalcHp()
    {
        int hp = Convert.ToInt32(Math.Floor(((2 * species.Hp + HpIV + Math.Floor((double)HpEV / 4)) * level) / 100) + level + 10);
        return hp;
    }
    public Move PickMove(Pokemon opp, int ai)
    {
        Move[] currentMoveSet = moveSet;
        if (moveNum == 1) return currentMoveSet[0];
        int move = 0;
        if (ai == 1)
        { 
            move = Random.Shared.Next(0, moveNum);
        }
        else if (ai == 4)
        {
            double highestScore = 0;
            for (int i = 0; i < moveNum; i++)
            {
                if (currentMoveSet[i].PP == 0) continue;
                double typing = Program.MatchUp(currentMoveSet[i].moveB.type, opp.species.type1) * Program.MatchUp(currentMoveSet[i].moveB.type, opp.species.type2);

                if (highestScore < typing * Program.FindBestMove(this, opp))
                {
                    highestScore = typing * Program.FindBestMove(this, opp);
                    move = i;
                }
            }
        }
        else
        {
            double highestScore = 0;
            for (int i = 0; i < moveNum; i++)
            {
                if (currentMoveSet[i].PP == 0) continue;
                double typing = Program.MatchUp(currentMoveSet[i].moveB.type, opp.species.type1) * Program.MatchUp(currentMoveSet[i].moveB.type, opp.species.type2);
                if (highestScore < typing)
                {
                    highestScore = typing;
                    move = i;
                }
            }
        }
        return moveSet[move];
    }
    public bool DoIMove()
    {
        if (reCharge)
        {
            Console.WriteLine($"{name} must recharge and can't move!");
            reCharge = false;
            return false;
        }
        else if (statusNonVol == Status.Sleep)
        {
            if (sleepTimer > 0)
            {
                sleepTimer--;
                Console.WriteLine($"{name} is asleep and can't move!");
                return false;
            }
            else
            {
                statusNonVol = Status.None;
                Console.WriteLine($"{name} woke up!");
                return true;
            }
        }
        else if (statusNonVol == Status.Freeze)
        {
            int thaw = Random.Shared.Next(0, 5);
            if (thaw == 0)
            {
                statusNonVol = Status.None;
                Console.WriteLine($"{name} thawed out!");
                return true;
            }
            else
            {
                Console.WriteLine($"{name} is frozen solid and can't move!");
                return false;
            }
        }
        else if (statusNonVol == Status.Paralysis)
        {
            int chance = Random.Shared.Next(0, 4);
            if (chance == 0)
            {
                Console.WriteLine($"{name} is paralyzed and can't move!");
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            foreach (Status statusN in statusVol)
            {
                switch (statusN)
                {
                    case Status.Confusion:
                        if (confusionTimer > 0)
                        {
                            confusionTimer--;
                            int check = Random.Shared.Next(0, 3);
                            if (check == 0)
                            {
                                Console.WriteLine($"{name} is confused and hurt itself in its confusion!");
                                int damage = Convert.ToInt32(Math.Floor((double)(((((2 * level) / 5) + 2) * 40 * CalcAtkStat()) / CalcDefStat()) / 50) + 2);
                                hp -= damage;
                                if (hp < 0) hp = 0;
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            statusVol.Remove(Status.Confusion);
                            Console.WriteLine($"{name} snapped out of its confusion!");
                            return true;
                        }
                    case
                    Status.Infatuation:
                        int infatuation = Random.Shared.Next(0, 2);
                        if (infatuation == 0)
                        {
                            Console.WriteLine($"{name} is immobilized by love!");
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    default:
                        return true;
                }
            }
            return true;
        }
    }
    public bool IsImmune(Status st)
    {
        switch (st)
        {
            case Status.Burn:
                if (species.type1 == Type.Fire || species.type2 == Type.Fire) return true;
                return false;
            case Status.Freeze:
                if (species.type1 == Type.Ice || species.type2 == Type.Ice) return true;
                return false;
            case Status.Paralysis:
                if (species.type1 == Type.Electric || species.type2 == Type.Electric) return true;
                return false;
            case Status.Poison:
                if (species.type1 == Type.Poison || species.type2 == Type.Poison || species.type1 == Type.Steel || species.type2 == Type.Steel) return true;
                return false;
            case Status.Toxic:
                if (species.type1 == Type.Poison || species.type2 == Type.Poison || species.type1 == Type.Steel || species.type2 == Type.Steel) return true;
                return false;
            default:
                return false;
        }
        
    }
    public bool CheckStone()
    {
        if (heldItem == null || species == null) return false;

        string pokemonName = species.name;

        if (pokemonName == "Charizard")
        {
            return heldItem.name == "Charizardite X" || heldItem.name == "Charizardite Y";
        }
        else if (pokemonName == "Mewtwo")
        {
            return heldItem.name == "Mewtwonite X" || heldItem.name == "Mewtwonite Y";
        }
        if (pokemonName == "Manectric" && heldItem.name == "Manectite")
        {
            return true;
        }

        if (pokemonName.Length > 2)
        {
            string truncatedName = pokemonName.Substring(0, pokemonName.Length - 2);
            return heldItem.name.StartsWith(truncatedName, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
    public void MegaEvo()
    {
        if (species.mega == true)
        {
            if (CheckStone())
            {
                MegaEvolve();
            }
            else
            {
                Console.WriteLine("Mega Evolution failed: No valid Mega Stone found.");
                return;
            }
        }
        else
        {
            Console.WriteLine("Mega Evolution failed: This species cannot Mega Evolve.");
            return;
        }
    }
    public void MegaEvolve()
    {
        foreach (Species s in Program.AllPokemon.Skip(Program.AllPokemon.Count - 75))
        {
            if (species.name == s.name.Replace("-Mega", ""))
            {
                Console.WriteLine($"{species.name} mega evolved into Mega {species.name}");
                if(name == species.name) name = s.name;
                species = s;
                break;
            }
            else if (species.name == "Charizard" && s.name == "Charizard-Mega-X" || species.name == "Mewtwo" && s.name == "Mewtwo-Mega-X")
            {
                if (species.name == "Charizard")
                    Console.Write($"Charizard mega evolved into {s.name} X");
                else
                    Console.Write($"Mewtwo mega evolved into {s.name} X");
                if (name == species.name) name = s.name;
                species = s;              
                break;
            }
            else if (species.name == "Charizard" && s.name == "Charizard-Mega-Y" || species.name == "Mewtwo" && s.name == "Mewtwo-Mega-Y")
            {
                if (species.name == "Charizard")
                    Console.Write($"Charizard mega evolved into {s.name} Y");
                else
                    Console.Write($"Mewtwo mega evolved into {s.name} Y");
                if(name == species.name) name = s.name;
                species = s;
                break;
            }
        }
    }
    public void UnMegaEvolve()
    {
        if (!species.name.Contains("-Mega") && !species.name.Contains("-Mega-Y") && !species.name.Contains("-Mega-X")) return;

        Species sp = this.species;
        if (species.name == "Charizard-Mega-X" || species.name == "Charizard-Mega-Y")
        {
            species = Program.AllPokemon.Find(sp => sp.name == "Charizard");
        }
        else if (species.name == "Mewtwo-Mega-X" || species.name == "Mewtwo-Mega-Y")
        {
            species = Program.AllPokemon.Find(sp => sp.name == "Mewtwo");
        }
        else
        {
            foreach (Species s in Program.AllPokemon)
            {
                if (species.name.Replace("-Mega", "") == s.name)
                {
                    species = s;
                    break;
                }
            }
        }
        Console.WriteLine(species.name + " returned to normal from Mega evolution");
    }
    public string GetGmaxName()
    {
        return species.name switch
        {
            "Charizard" => "G-Max Wildfire",
            "Blastoise" => "G-Max Cannonade",
            "Venusaur" => "G-Max Vine Lash",
            "Gengar" => "G-Max Terror",
            "Snorlax" => "G-Max Replenish",
            "Rillaboom" => "G-Max Drum Solo",
            "Cinderace" => "G-Max Fireball",
            "Inteleon" => "G-Max Hydrosnipe",
            "Corviknight" => "G-Max Wind Rage",
            "Duraludon" => "G-Max Depletion",
            "Machamp" => "G-Max Chi Strike",
            "Garbodor" => "G-Max Malodor",
            "Coalossal" => "G-Max Volcalith",
            "Lapras" => "G-Max Resonance",
            "Alcremie" => "G-Max Finale",
            "Toxtricity" => "G-Max Stun Shock",
            "Toxtricity-Low-Key" => "G-Max Stun Shock",
            "Appletun" => "G-Max Sweetness",
            "Flapple" => "G-Max Tartness",
            "Sandaconda" => "G-Max Sandblast",
            "Centiskorch" => "G-Max Centiferno",
            "Hatterene" => "G-Max Smite",
            "Grimmsnarl" => "G-Max Snooze",
            "Copperajah" => "G-Max Steelsurge",
            "Urshifu" => "G-Max One Blow",
            "Urshifu-Rapid-Srike" => "G-Max Rapid Flow",
            _ => "error"
        };

    }
    public void Dmax()
    {
        if (isDmax == false)
        {
            dMaxTimer = 3;
            double dmaxHpCoef = 1.50 + dMaxLevel * 0.05;
            isDmax = true;
            maxHP = (int)Math.Ceiling(maxHP * dmaxHpCoef);
            hp = (int)Math.Ceiling(hp * dmaxHpCoef);
            int g = 0;
            List<MoveB> Max = Program.AllMaxMoves.Take(19).ToList();
            List<MoveB> GMax = Program.AllMaxMoves.Skip(19).ToList();
            foreach (Move m in moveSet)
            {
                if (m != null)
                {
                    if(m.moveB.split == Split.Status)
                    {
                        moveSet[g] = new Move(Max[18], moveSet[g].PP);
                    }
                    else
                    {
                        if (gmax)
                        {
                            MoveB gmb = GMax.Find(gm => gm.name == (this.GetGmaxName()));
                            if (gmb != null && moveSet[g].moveB.type == gmb.type)
                            {
                                gmb.split = moveSet[g].moveB.split;
                                gmb.power = moveSet[g].MaxPower();
                                moveSet[g] = new Move(gmb, moveSet[g].PP);
                                g++;
                                continue;
                            }
                        }
                        MoveB mb = Max.Find(dm => dm.type == moveSet[g].moveB.type);
                        mb.split = moveSet[g].moveB.split;
                        mb.power = moveSet[g].MaxPower();
                        moveSet[g] = new Move(mb, moveSet[g].PP);
                    }
                    g++;
                }
            }
            if (gmax)
            {
                Console.WriteLine($"{species.name} gigantamaxed");
            }
            else
            {
                Console.WriteLine($"{species.name} dynamaxed");
            }
        }
    }
    public void UnDmax()
    {
        if (isDmax)
        {
            dMaxTimer = 0;
            double dmaxHpCoef = 1.50 + dMaxLevel * 0.05;
            isDmax = false;
            maxHP = (int)Math.Ceiling(maxHP / dmaxHpCoef);
            hp = (int)Math.Ceiling(hp / dmaxHpCoef);
            Console.WriteLine($"{species.name} returned to normal size");
            int g = 0;
            foreach (Move m in moveSet)
            {
                if (m != null)
                {
                    moveSet[g] = new Move(moveSetBase[g].moveB, moveSet[g].PP);
                    g++;
                }
            }
        }
    }
    public void Terastallize()
    {
        terastallized = true;
        Console.WriteLine($"{species.name} terastallized into a {GetType(tera)} type");
    }
    public void UnTerastallize()
    {
        if (terastallized) terastallized = false;
    }
}
public class MoveB
{
    public string name { get; }
    public Type type { get; set; }
    public int power { get; set;  }
    public Split split { get; set;  }
    public int acc { get; }
    public int maxPP { get; }
    public int priority { get; } = 0;
    public bool contact { get; }
    public bool protect { get; }
    public List<MoveEffect> effectList { get; private set; }
    public MoveB(string name, Type type, int power, Split split, int acc, int maxPP, int priority, bool contact, bool protect, List<MoveEffect> effectList)
    {
        this.name = name;
        this.type = type;
        this.power = power;
        this.split = split;
        this.acc = acc;
        this.maxPP = maxPP;
        this.priority = priority;
        this.contact = contact;
        this.protect = protect;
        this.effectList = effectList;
    }
}
public class MoveEffect
{
    public Status effectStatus { get; }
    public Stat effectStat { get; }
    public int effectChance { get; }
    public int effectPower { get; }
    public bool recoil { get; }
    public int multiHit { get; }
    public MoveEffect(Status effectStatus, Stat effectStat, int effectChance, int effectPower, bool recoil, int multiHit)
    {
        this.effectStatus = effectStatus;
        this.effectStat = effectStat;
        this.effectChance = effectChance;
        this.effectPower = effectPower;
        this.recoil = recoil;
        this.multiHit = multiHit;
    }
}
public class Move
{
    public MoveB moveB { get; private set; }
    public int PP { get; set; }
    public Move(MoveB moveB)
    {
        if (moveB == null) return;
        this.moveB = moveB;
        this.PP = moveB.maxPP;
    }
    public Move(MoveB moveB, int PP)
    {
        if (moveB == null) return;
        this.moveB = moveB;
        this.PP = PP;
    }
    public int MaxPower()
    {
        int basePower = moveB.power;
        if (moveB.type == Type.Fighting || moveB.type == Type.Poison)
        {
            return basePower switch
            {
                >= 0 and <= 40 => 70,
                >= 45 and <= 50 => 75,
                >= 55 and <= 60 => 80,
                >= 65 and <= 70 => 85,
                >= 75 and <= 100 => 90,
                >= 110 and <= 140 => 95,
                >= 150 and <= 250 => 100,
                _ => 0
            };
        }

        return basePower switch
        {
            >= 0 and <= 40 => 90,
            >= 45 and <= 50 => 100,
            >= 55 and <= 60 => 110,
            >= 65 and <= 70 => 120,
            >= 75 and <= 100 => 130,
            >= 110 and <= 140 => 140,
            >= 150 and <= 250 => 150,
            _ => 0
        };
    }
}
public class Item
{
    public string name { get; }
    string effect;
    bool mega;
    public bool Zcrystal { get; }
    public Type ZCrystalType { get; }
    public Item(string name, string effect, bool mega, bool zcrystal, Type zCrystalType)
    {
        this.name = name;
        this.effect = effect;
        this.mega = mega;
        Zcrystal = zcrystal;
        ZCrystalType = zCrystalType;
    }
}
public class Field
{
    public FieldSide sideA { get; set; } = new FieldSide();
    public FieldSide sideB { get; set; } = new FieldSide();
    public Weather weather { get; set; } = Weather.None;
    public Terrain terrain { get; set; } = Terrain.None;
    public int weatherTimer { get; set; } = 0;
    public int terrainTimer { get; set; } = 0;
    public bool gravity { get; set; } = false;
    public int gravityTimer { get; set; } = 0;
    public bool trickRoom { get; set; } = false;
    public int trickRoomTimer { get; set; } = 0;
    public bool wonderRoom { get; set; } = false;
    public int wonderRoomTimer { get; set; } = 0;
    public bool magicRoom { get; set; } = false;
    public int magicRoomTimer { get; set; } = 0;
    public Field(FieldSide sideA, FieldSide sideB)
    {
        this.sideA = sideA;
        this.sideB = sideB;
    }
    public void ClearHazards()
    {
        sideA.ClearHazards();
        sideB.ClearHazards();
    }
    public void ClearScreens()
    {
        sideA.ClearScreens();
        sideB.ClearScreens();
    }
    public void ChangeWeather(Weather newWeather)
    {
        if (weather != newWeather)
        {
            if (weather == Weather.HarshSun || weather == Weather.HeavyRain || weather == Weather.StrongWinds)
            { }
            else
            {
                weather = newWeather;
                weatherTimer = 5;
                Console.WriteLine($"The weather changed to {weather}!");
            }
        }
        else return;
    }
    public void ChangeTerrain(Terrain newTerrain)
    {
        if (terrain != newTerrain)
        {
            terrain = newTerrain;
            terrainTimer = 5;
            Console.WriteLine($"The terrain changed to {terrain}!");
        }
        else return;
    }
    public void PostTurnCheck()
    {
        sideA.PostTurnCheck();
        sideB.PostTurnCheck();
        if (weather != Weather.None)
        {
            weatherTimer--;
            if (weatherTimer == 0)
            {
                Console.WriteLine($"The {weather} weather wore off!");
                weather = Weather.None;
            }
        }
        if (terrain != Terrain.None)
        {
            terrainTimer--;
            if (terrainTimer == 0)
            {
                Console.WriteLine($"The {terrain} terrain wore off!");
                terrain = Terrain.None;
            }
        }
        if (gravity)
        {
            gravityTimer--;
            if (gravityTimer == 0)
            {
                Console.WriteLine("The gravity wore off!");
                gravity = false;
            }
        }
        if (trickRoom)
        {
            trickRoomTimer--;
            if (trickRoomTimer == 0)
            {
                Console.WriteLine("The Trick Room wore off!");
                trickRoom = false;
            }
        }
        if (wonderRoom)
        {
            wonderRoomTimer--;
            if (wonderRoomTimer == 0)
            {
                Console.WriteLine("The Wonder Room wore off!");
                wonderRoom = false;
            }
        }
    }
}
public class FieldSide
{
    public bool stealthRock { get; set; } = false;
    public bool sharpSteel { get; set; } = false;
    public int spikes { get; set; } = 0;
    public int spikesToxic { get; set; } = 0;
    public bool stickyWeb { get; set; } = false;
    public bool reflect { get; set; } = false;
    public int reflectTimer { get; set; } = 0;
    public bool lightScreen { get; set; } = false;
    public int lightScreenTimer { get; set; } = 0;
    public bool auroraVeil { get; set; } = false;
    public int auroraVeilTimer { get; set; } = 0;
    public bool tailwind { get; set; } = false;
    public int tailwindTimer { get; set; } = 0;
    public FieldSide()
    {
    }
    public void ClearHazards()
    {
        stealthRock = false;
        sharpSteel = false;
        spikes = 0;
        spikesToxic = 0;
        stickyWeb = false;
    }
    public void ClearScreens()
    {
        reflect = false;
        lightScreen = false;
        reflectTimer = 0;
        lightScreenTimer = 0;
    }
    public void PostTurnCheck()
    {
        if(reflect)
        {
            reflectTimer--;
            if (reflectTimer == 0)
            {
                Console.WriteLine("The Reflect wore off!");
                reflect = false;
            }
        }
        if (lightScreen)
        {
            lightScreenTimer--;
            if (lightScreenTimer == 0)
            {
                Console.WriteLine("The Light Screen wore off!");
                lightScreen = false;
            }
        }
        if (auroraVeil)
        {
            auroraVeilTimer--;
            if (auroraVeilTimer == 0)
            {
                Console.WriteLine("The Aurora Veil wore off!");
                auroraVeil = false;
            }
        }
        if (tailwind)
        {
            tailwindTimer--;
            if (tailwindTimer == 0)
            {
                Console.WriteLine("The Tailwind wore off!");
                tailwind = false;
            }
        }
    }
    public void Entry(Pokemon pk)
    {
        if (stealthRock)
        {
            double rockEff1 = 1.00;
            double rockEff2 = 1.00;
            rockEff1 = Program.MatchUp(Type.Rock, pk.species.type1);
            rockEff2 = Program.MatchUp(Type.Rock, pk.species.type2);
            double totalEff = rockEff1 * rockEff2;
            if (totalEff > 0)
            {
                int damage = Convert.ToInt32(Math.Floor((double)(pk.maxHP / 8) * totalEff));
                pk.hp -= damage;
                Console.WriteLine($"{pk.name} took {damage} damage from Stealth Rock!");
                if (pk.hp < 0) pk.hp = 0;
            }
        }
        if (sharpSteel)
        {
            double rockEff1 = 1.00;
            double rockEff2 = 1.00;
            rockEff1 = Program.MatchUp(Type.Steel, pk.species.type1);
            rockEff2 = Program.MatchUp(Type.Steel, pk.species.type2);
            double totalEff = rockEff1 * rockEff2;
            if (totalEff > 0)
            {
                int damage = Convert.ToInt32(Math.Floor((double)(pk.maxHP / 8) * totalEff));
                pk.hp -= damage;
                Console.WriteLine($"{pk.name} took {damage} damage from Sharp Steel!");
                if (pk.hp < 0) pk.hp = 0;
            }
        }
        if (spikes > 0)
        {
            int spikeDamage = 0;
            switch (spikes)
            {
                case 1:
                    spikeDamage = 8;
                    break;
                case 2:
                    spikeDamage = 6;
                    break;
                case 3:
                    spikeDamage = 4;
                    break;
            }
            int damage = Convert.ToInt32(Math.Floor((double)(pk.maxHP / spikeDamage)));
            pk.hp -= damage;
            Console.WriteLine($"{pk.name} took {damage} damage from Stealth Rock!");
            if (pk.hp < 0) pk.hp = 0;
        }
        if (spikesToxic > 0)
        {
            if (spikesToxic == 1)
            {
                if (!pk.IsImmune(Status.Poison))
                {
                    if (pk.statusNonVol != Status.None) return;
                    pk.statusNonVol = Status.Poison;
                    pk.toxicCounter = 0;
                    Console.WriteLine($"{pk.name} was poisoned by Toxic Spikes!");
                }
            }
            else if (spikesToxic == 2)
            {
                if (!pk.IsImmune(Status.Toxic))
                {
                    if (pk.statusNonVol != Status.None) return;
                    pk.statusNonVol = Status.Toxic;
                    pk.toxicCounter = 0;
                    Console.WriteLine($"{pk.name} was badly poisoned by Toxic Spikes!");
                }
            }
        }
        if (stickyWeb)
        {
            Program.InflictStatus(pk, new MoveEffect(Status.None, Stat.Spe, 100, -1, false, 1), null);
            Console.WriteLine($"{pk.name}'s Speed fell due to Sticky Web!");
        }
    }
}
public static class Program
{
    static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };
    public static List<Species> AllPokemon { get; } = JsonSerializer.Deserialize<List<Species>>(File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("Pokemon_Json_Path")?? throw new InvalidOperationException("Pokemon_Json_Path is not set."), "AllPokemon.json")));
    public static List<MoveB> AllMoves { get; } = JsonSerializer.Deserialize<List<MoveB>>(File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("Pokemon_Json_Path") ?? throw new InvalidOperationException("Pokemon_Json_Path is not set."), "AllMoves.json")), JsonOptions)!;
    public static List<MoveB> AllZMoves { get; } = JsonSerializer.Deserialize<List<MoveB>>(File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("Pokemon_Json_Path") ?? throw new InvalidOperationException("Pokemon_Json_Path is not set."), "AllZMoves.json")), JsonOptions)!;
    public static List<MoveB> AllMaxMoves { get; } = JsonSerializer.Deserialize<List<MoveB>>(File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("Pokemon_Json_Path") ?? throw new InvalidOperationException("Pokemon_Json_Path is not set."), "AllMaxMoves.json")), JsonOptions)!;
    public static Type GetTypeId(string typeName)
    {
        return typeName.ToLower() switch
        {
            "" => Type.None,
            "normal" => Type.Normal,
            "fire" => Type.Fire,
            "water" => Type.Water,
            "electric" => Type.Electric,
            "grass" => Type.Grass,
            "ice" => Type.Ice,
            "fighting" => Type.Fighting,
            "poison" => Type.Poison,
            "ground" => Type.Ground,
            "flying" => Type.Flying,
            "psychic" => Type.Psychic,
            "bug" => Type.Bug,
            "rock" => Type.Rock,
            "ghost" => Type.Ghost,
            "dragon" => Type.Dragon,
            "dark" => Type.Dark,
            "steel" => Type.Steel,
            "fairy" => Type.Fairy,
            _ => Type.None
        };
    }
    public static double MatchUp(Type attackType, Type defenseType)
    {
        if (defenseType == 0) return 1.0;

        double eff = 1.0;

        switch (attackType)
        {
            case Type.Normal:
                //Normal
                if (defenseType == Type.Rock || defenseType == Type.Steel) eff = 0.5; //vs Rock, Steel
                else if (defenseType == Type.Ghost) eff = 0.0;// vs Ghost
                break;

            case Type.Fire:
                // Fire
                if (defenseType == Type.Grass || defenseType == Type.Ice || defenseType == Type.Bug || defenseType == Type.Steel) eff = 2.0; //vs Grass, Ice, Bug, Steel
                else if (defenseType == Type.Fire || defenseType == Type.Water || defenseType == Type.Rock || defenseType == Type.Dragon) eff = 0.5; //vs Fire, Water, Rock, Dragon
                break;

            case Type.Water:
                // Water
                if (defenseType == Type.Fire || defenseType == Type.Ground || defenseType == Type.Rock) eff = 2.0;// vs Fire, Ground, Rock
                else if (defenseType == Type.Water || defenseType == Type.Grass || defenseType == Type.Dragon) eff = 0.5;// vs Water, Grass, Dragon
                break;

            case Type.Electric:
                // Electric
                if (defenseType == Type.Water || defenseType == Type.Flying) eff = 2.0; //vs Water, Flying
                else if (defenseType == Type.Electric || defenseType == Type.Grass || defenseType == Type.Dragon) eff = 0.5;// vs Electric, Grass, Dragon
                else if (defenseType == Type.Ground) eff = 0.0;// vs Ground
                break;

            case Type.Grass:
                // Grass
                if (defenseType == Type.Water || defenseType == Type.Ground || defenseType == Type.Rock) eff = 2.0;// vs Water, Ground, Rock
                else if (defenseType == Type.Fire || defenseType == Type.Grass || defenseType == Type.Poison || defenseType == Type.Flying || defenseType == Type.Bug || defenseType == Type.Dragon || defenseType == Type.Steel) eff = 0.5; //vs Fire, Grass, Poison, Flying, Bug, Dragon, Steel
                break;

            case Type.Ice:
                // Ice
                if (defenseType == Type.Grass || defenseType == Type.Ground || defenseType == Type.Flying || defenseType == Type.Dragon) eff = 2.0; //vs Grass, Ground, Flying, Dragon
                else if (defenseType == Type.Fire || defenseType == Type.Water || defenseType == Type.Ice || defenseType == Type.Steel) eff = 0.5;// vs Fire, Water, Ice, Steel
                break;

            case Type.Fighting:
                //   Fighting
                if (defenseType == Type.Normal || defenseType == Type.Ice || defenseType == Type.Rock || defenseType == Type.Dark || defenseType == Type.Steel) eff = 2.0; //vs Normal, Ice, Rock, Dark, Steel
                else if (defenseType == Type.Poison || defenseType == Type.Flying || defenseType == Type.Psychic || defenseType == Type.Bug || defenseType == Type.Fairy) eff = 0.5; //vs Poison, Flying, Psychic, Bug, Fairy
                else if (defenseType == Type.Ghost) eff = 0.0; //vs Ghost
                break;

            case Type.Poison:
                //  Poison
                if (defenseType == Type.Grass || defenseType == Type.Fairy) eff = 2.0;// vs Grass, Fairy
                else if (defenseType == Type.Poison || defenseType == Type.Ground || defenseType == Type.Rock || defenseType == Type.Ghost) eff = 0.5; //vs Poison, Ground, Rock, Ghost
                else if (defenseType == Type.Steel) eff = 0.0; //vs Steel
                break;

            case Type.Ground:
                //Ground
                if (defenseType == Type.Fire || defenseType == Type.Electric || defenseType == Type.Poison || defenseType == Type.Rock || defenseType == Type.Steel) eff = 2.0;// vs Fire, Electric, Poison, Rock, Steel
                else if (defenseType == Type.Grass || defenseType == Type.Bug) eff = 0.5; //vs Grass, Bug
                else if (defenseType == Type.Flying) eff = 0.0; //vs Flying
                break;

            case Type.Flying:
                // Flying
                if (defenseType == Type.Grass || defenseType == Type.Fighting || defenseType == Type.Bug) eff = 2.0; //vs Grass, Fighting, Bug
                else if (defenseType == Type.Electric || defenseType == Type.Rock || defenseType == Type.Steel) eff = 0.5; //vs Electric, Rock, Steel
                break;

            case Type.Psychic:
                //Psychic
                if (defenseType == Type.Fighting || defenseType == Type.Poison) eff = 2.0;// vs Fighting, Poison
                else if (defenseType == Type.Psychic || defenseType == Type.Steel) eff = 0.5;// vs Psychic, Steel
                else if (defenseType == Type.Dark) eff = 0.0;// vs Dark
                break;

            case Type.Bug:
                //  Bug
                if (defenseType == Type.Grass || defenseType == Type.Psychic || defenseType == Type.Dark) eff = 2.0;// vs Grass, Psychic, Dark
                else if (defenseType == Type.Fire || defenseType == Type.Fighting || defenseType == Type.Poison || defenseType == Type.Flying || defenseType == Type.Ghost || defenseType == Type.Steel || defenseType == Type.Fairy) eff = 0.5; //vs Fire, Fighting, Poison, Flying, Ghost, Steel, Fairy
                break;

            case Type.Rock:
                //  Rock
                if (defenseType == Type.Fire || defenseType == Type.Ice || defenseType == Type.Flying || defenseType == Type.Bug) eff = 2.0;// vs Fire, Ice, Flying, Bug
                else if (defenseType == Type.Fighting || defenseType == Type.Ground || defenseType == Type.Steel) eff = 0.5; //vs Fighting, Ground, Steel
                break;

            case Type.Ghost:
                //  Ghost
                if (defenseType == Type.Psychic || defenseType == Type.Ghost) eff = 2.0; //vs Psychic, Ghost
                else if (defenseType == Type.Dark) eff = 0.5;// vs Dark
                else if (defenseType == Type.Normal) eff = 0.0; //vs Normal
                break;

            case Type.Dragon:
                // Dragon
                if (defenseType == Type.Dragon) eff = 2.0;// vs Dragon
                else if (defenseType == Type.Steel) eff = 0.5; //vs Steel
                else if (defenseType == Type.Fairy) eff = 0.0; //vs Fairy
                break;

            case Type.Dark:
                // Dark
                if (defenseType == Type.Psychic || defenseType == Type.Ghost) eff = 2.0;// vs Psychic, Ghost
                else if (defenseType == Type.Fighting || defenseType == Type.Dark || defenseType == Type.Fairy) eff = 0.5;// vs Fighting, Dark, Fairy
                break;

            case Type.Steel:
                //Steel
                if (defenseType == Type.Ice || defenseType == Type.Rock || defenseType == Type.Fairy) eff = 2.0; //vs Ice, Rock, Fairy
                else if (defenseType == Type.Fire || defenseType == Type.Water || defenseType == Type.Electric || defenseType == Type.Steel) eff = 0.5; //vs Fire, Water, Electric, Steel
                break;

            case Type.Fairy:
                //Fairy
                if (defenseType == Type.Fighting || defenseType == Type.Dragon || defenseType == Type.Dark) eff = 2.0; //vs Fighting, Dragon, Dark
                else if (defenseType == Type.Fire || defenseType == Type.Poison || defenseType == Type.Steel) eff = 0.5; //vs Fire, Poison, Steel
                break;
        }

        return eff;
    }
    public static int FindBestMove(Pokemon atk, Pokemon opp)
    {
        Field field = new Field(new FieldSide(), new FieldSide());
        int HighestEffect = 0;
        for (int i = 0; i < atk.moveNum; i++)
        {
            if (HighestEffect < Damage(atk, opp, atk.moveSet[i], atk.moveSet[i].moveB.power, atk.CalcAtkStat(), opp.CalcDefStat() * opp.GetMod(opp.DefMod), true, field))
            {
                HighestEffect = Damage(atk, opp, atk.moveSet[i], atk.moveSet[i].moveB.power, atk.CalcAtkStat(), opp.CalcDefStat() * opp.GetMod(opp.DefMod), true, field);
            }
        }
        return HighestEffect;
    }
    public static bool CheckAcc(Move move, Pokemon pokemonA, Pokemon pokemonD)
    {
        if (move.moveB.acc == 101) return true;

        int check = Random.Shared.Next(1, 101);

        if (check <= (move.moveB.acc * pokemonA.GetMod(pokemonA.AccMod) * pokemonD.GetMod(pokemonD.EvaMod)))
        {
            return true;
        }
        return false;
    }
    public static void Move(Pokemon pokemonA, Pokemon pokemonD, Move move, Field field)
    {

        //G - Max Vine Lash
        //G - Max Wildfire
        //G - Max Cannonade

        //G - Max Volcalith
        //G - Max Sandblast
        //G - Max Centiferno

        //G - Max Replenish

        if (move.PP <= 0)
        {
            Console.WriteLine($"{pokemonA.species.name} has no PP left for {move.moveB.name}!");
            MoveB struggle = new MoveB("Struggle", 0, 50, Split.Physical, 100, 101, 0, true, false, new List<MoveEffect>());
            move = new Move(struggle);
            Console.WriteLine($"{pokemonA.species.name} used Struggle!");
            pokemonD.hp -= Damage(pokemonA, pokemonD, move, 50, (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod)), (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod)), false, field);
            if (pokemonD.hp < 0) pokemonD.hp = 0;
            pokemonA.hp -= Convert.ToInt32(Math.Floor(pokemonA.maxHP / 4.0));
            if (pokemonA.hp < 0) pokemonA.hp = 0;
            Console.WriteLine($"{pokemonA.species.name} is hit with recoil");
            Console.WriteLine($"recoil brought to: {pokemonA.hp}");
            return;
        }
        move.PP--;
        double pt = 1.00;
        if (move.moveB.protect)
        {
            pokemonA.protectTimes++;
            if (pokemonA.protectTimes > 1)
            {
                int check = Random.Shared.Next(1, 101);
                double protectChance = 1.0 / Math.Pow(3, pokemonA.protectTimes - 1);
                if (check > protectChance * 100)
                {
                    if (move.moveB.name == "Max Guard") pokemonA.maxInvurnable = false;
                    pokemonA.invurnable = false;
                    Console.WriteLine($"{pokemonA.name} tried to use Protect but failed!");
                    pokemonA.lastMove = move;
                    return;
                }
            }
            if (move.moveB.name == "Max Guard") pokemonA.maxInvurnable = true;
            pokemonA.invurnable = true;
            Console.WriteLine($"{pokemonA.name} used {move.moveB.name}!");
            if (move.moveB.name == "Kings Shield")
            {
                if (pokemonA.species.name == "Aegislash-Blade")
                {
                    pokemonA.species = Program.AllPokemon.Find(sp => sp.name == "Aegislash");
                    Console.WriteLine($"{pokemonA.name} transformed into {pokemonA.species.name}!");
                }
            }
            pokemonA.lastMove = move;
            return;
        }
        else if(pokemonA.lastMove != null && pokemonA.lastMove.moveB.protect)
        {
            pokemonA.invurnable = false;
            pokemonA.maxInvurnable = false;
            pokemonA.protectTimes = 0;
        }
        if (move.moveB.name == "Endure")
        {
            pokemonA.protectTimes++;
            if (pokemonA.protectTimes > 1)
            {
                int check = Random.Shared.Next(1, 101);
                double protectChance = 1.0 / Math.Pow(3, pokemonA.protectTimes - 1);
                if (check > protectChance * 100)
                {                  
                    Console.WriteLine($"{pokemonA.name} tried to use Endure but failed!");
                    pokemonA.lastMove = move;
                    return;
                }
            }
            pokemonA.endure = true;
            Console.WriteLine($"{pokemonA.name} will endure any attack this turn!");
            pokemonA.lastMove = move;
            return;
        }
        if (pokemonD.invurnable && (move.moveB.name != "Feint" && move.moveB.name != "Hyperspace Hole" && move.moveB.name != "Hyperspace Fury" && move.moveB.name != "Hyper Drill" && move.moveB.name != "Mighty Cleave"))
        {
            if (!pokemonD.maxInvurnable || (move.moveB.name == "G-Max One Blow" || move.moveB.name == "G-Max Rapid Flow"))
            {
                if (AllZMoves.Contains(move.moveB) || AllMaxMoves.Contains(move.moveB))
                {
                    pt = 0.25;
                    Console.WriteLine($"{move.moveB.name} hits through protect");
                }
                else
                {
                    Console.WriteLine($"{pokemonD.name} protected/is invurnable this turn!");
                    pokemonA.lastMove = move;
                }
                if (move.moveB.name == "Spiky Shield" && move.moveB.contact)
                {
                    pokemonA.hp -= Convert.ToInt32(Math.Floor(pokemonA.maxHP / 8.0));
                    if (pokemonA.hp < 0) pokemonD.hp = 0;
                }
                else if (move.moveB.name == "Kings Shield" && move.moveB.contact)
                {
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Atk, 100, -1, false, 1), field);
                }
                else if (move.moveB.name == "Baneful Bunker" && move.moveB.contact)
                {
                    InflictStatus(pokemonA, new MoveEffect(Status.Poison, Stat.None, 100, 0, false, 1), field);
                }
                else if (move.moveB.name == "Obstruct" && move.moveB.contact)
                {
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Def, 100, -2, false, 1), field);
                }
                else if (move.moveB.name == "Silk Trap" && move.moveB.contact)
                {
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Spe, 100, -1, false, 1), field);
                }
                else if (move.moveB.name == "Burning Bulwark" && move.moveB.contact)
                {
                    InflictStatus(pokemonA, new MoveEffect(Status.Burn, Stat.None, 100, 0, false, 1), field);
                }
            }
            return;
        }
        List<string> OHKO = new List<string> { "Fissure", "Guillotine", "Horn Drill", "Sheer Cold" };
        if (OHKO.Contains(move.moveB.name))
        {
            if(pokemonD.level > pokemonA.level) return;
            int OHKOcheck = 30 + pokemonA.level - pokemonD.level;
            if(!(move.moveB.name == "Sheer Cold" && (pokemonA.species.type1 == Type.Ice || pokemonA.species.type2 == Type.Ice)))
            {
                OHKOcheck = 20 + pokemonA.level - pokemonD.level;
            }
            int check = Random.Shared.Next(1, 101);

            if (check <= OHKOcheck)
            {
                pokemonD.hp = 0;
            }
            pokemonA.lastMove = move;
            return;
        }
        if (move.moveB.name == "Gigaton Hammer" && pokemonA.lastMove.moveB.name == "Gigaton Hammer")
        {
            return;
        }
        if (move.moveB.name == "Blood Moon" && pokemonA.lastMove.moveB.name == "Blood Moon")
        {
            return;
        }
        if (pokemonA.lastMove != null && move.moveB.name == pokemonA.lastMove.moveB.name && pokemonA.statusVol.Contains(Status.Torment))
        {
            return;
        }

        if (CheckAcc(move, pokemonA, pokemonD) == true || pokemonA.chargingMove)
        {
            if (move.moveB.split == Split.Status)
            {
                if (move.moveB.name == "Spite")
                {
                    if (pokemonD.lastMove == null) return;
                    pokemonD.lastMove.PP -= 4;
                    if (pokemonD.lastMove.PP < 0) pokemonD.lastMove.PP = 0;
                    return;
                }
                if (move.moveB.name == "Curse" && (pokemonA.species.type1 == Type.Ghost || pokemonA.species.type2 == Type.Ghost))
                {
                    Console.WriteLine("Ghost Curse not implemented");
                    return;
                }
                if (move.moveB.name == "Attract" || move.moveB.name == "G-Max Cuddle")
                {
                    if ((!pokemonD.species.noRatio) || (!pokemonA.species.noRatio))
                    {
                        if (pokemonA.gender == pokemonD.gender)
                            return;
                    }
                }
                if (move.moveB.name == "Memento")
                {
                    pokemonA.hp = 0;
                }
                if (move.moveB.name == "Trick" || move.moveB.name == "Switcheroo")
                {
                    Item temp = pokemonA.heldItem;
                    pokemonA.heldItem = pokemonD.heldItem;
                    pokemonD.heldItem = temp;
                    return;
                }
                if (move.moveB.name == "Tidy Up")
                {
                    field.ClearHazards();
                }
                if (move.moveB.name == "Strength Sap")
                {
                    pokemonA.hp += Convert.ToInt32(pokemonD.CalcAtkStat() * pokemonD.GetMod(pokemonD.AtkMod));
                    if (pokemonA.hp > pokemonA.maxHP) pokemonA.hp = pokemonA.maxHP;
                }
                if (move.moveB.name == "Jungle Healing" || move.moveB.name == "Lunar Blessing")
                {
                    Console.WriteLine("Not implemented");
                    return;
                }
                if (move.moveB.name == "Take Heart")
                {
                    pokemonA.HealConditions();
                }
                if (move.moveB.name == "Acupressure")
                {
                    int st = Random.Shared.Next(1, 8);
                    Stat stt = (Stat)st;
                    MoveEffect effect = new MoveEffect(Status.None, stt, 100, 2, false, 1);
                    InflictStatus(pokemonD, effect, field);
                    return;
                }
                if (move.moveB.name == "Topsy-Turvy")
                {
                    pokemonD.AtkMod *= -1;
                    pokemonD.DefMod *= -1;
                    pokemonD.SpaMod *= -1;
                    pokemonD.SpdMod *= -1;
                    pokemonD.SpeMod *= -1;
                    pokemonD.AccMod *= -1;
                    pokemonD.EvaMod *= -1;
                    return;
                }
                if (move.moveB.name == "Sunny Day")
                {
                    field.ChangeWeather(Weather.Sun);
                    return;
                }
                if (move.moveB.name == "Rain Dance")
                {
                    field.ChangeWeather(Weather.Rain);
                    return;
                }
                if (move.moveB.name == "Sandstorm")
                {
                    field.ChangeWeather(Weather.Sandstorm);
                    return;
                }
                if (move.moveB.name == "Snowscape")
                {
                    field.ChangeWeather(Weather.Snow);
                    return;
                }
                if ((move.moveB.name == "Aurora Veil" && field.weather == Weather.Snow) || move.moveB.name == "G-Max Resonance")
                {
                    if (field.sideA.auroraVeil) return;
                    field.sideA.auroraVeil = true;
                    field.sideA.auroraVeilTimer = 5;
                    return;
                }
                if (move.moveB.name == "Reflect")
                {
                    if (field.sideA.reflect) return;
                    field.sideA.reflect = true;
                    field.sideA.reflectTimer = 5;
                    return;
                }
                if (move.moveB.name == "Light Screen")
                {
                    if (field.sideA.lightScreen) return;
                    field.sideA.lightScreen = true;
                    field.sideA.lightScreenTimer = 5;
                    return;
                }
                if (move.moveB.name == "Stealth Rock")
                {
                    field.sideB.stealthRock = true;
                    return;
                }
                if (move.moveB.name == "Spikes")
                {
                    if (field.sideB.spikes < 3)
                    {
                        field.sideB.spikes++;
                    }
                    return;
                }
                if (move.moveB.name == "Sticky Web")
                {
                    field.sideB.stickyWeb = true;
                    return;
                }
                if (move.moveB.name == "Tailwind")
                {
                    if (field.sideA.tailwind) return;
                    field.sideA.tailwind = true;
                    field.sideA.tailwindTimer = 4;
                    return;
                }
                if (move.moveB.name == "Defog")
                {
                    bool auroraVeil = false;
                    int auroraVeilTimer = 0;
                    field.ClearHazards();
                    if(field.sideA.auroraVeil)
                    {
                        auroraVeil = true;
                        auroraVeilTimer = field.sideA.auroraVeilTimer;
                    }
                    field.ClearScreens();
                    if (auroraVeil)
                    {
                        field.sideA.auroraVeil = true;
                        field.sideA.auroraVeilTimer = auroraVeilTimer;
                    }
                }
                if (move.moveB.name == "Court Change")
                {
                    FieldSide temp = field.sideA;
                    field.sideA = field.sideB;
                    field.sideB = temp;
                    return;
                }
                if (move.moveB.name == "Magic Room")
                {
                    field.magicRoom = true;
                    field.magicRoomTimer = 5;
                    return;
                }
                if (move.moveB.name == "Wonder Room")
                {
                    field.wonderRoom = true;
                    field.wonderRoomTimer = 5;
                    return;
                }
                if (move.moveB.name == "Trick Room")
                {
                    field.trickRoom = true;
                    field.trickRoomTimer = 5;
                    return;
                }
                if (move.moveB.name == "Gravity")
                {
                    field.gravity = true;
                    field.gravityTimer = 5;
                    return;
                }
                if (move.moveB.name.Contains(" Terrain"))
                {
                    switch(move.moveB.name)
                    {
                        case "Electric Terrain":
                            field.ChangeTerrain(Terrain.Electric);
                            break;
                        case "Grassy Terrain":
                            field.ChangeTerrain(Terrain.Grassy);
                            break;
                        case "Misty Terrain":
                            field.ChangeTerrain(Terrain.Misty);
                            break;
                        case "Psychic Terrain":
                            field.ChangeTerrain(Terrain.Psychic);
                            break;
                    }
                }
                if (move.moveB.name == "Toxic Spikes")
                {
                    if (field.sideB.spikesToxic < 2)
                    {
                        field.sideB.spikesToxic++;
                    }
                    return;
                }
                if (move.moveB.name == "Geomancy" && !pokemonA.chargingMove)
                {
                    pokemonA.chargingMove = true;
                    Console.WriteLine($"{pokemonA.name} is charging up for {move.moveB.name}!");
                    pokemonA.lastMove = move;
                    return;
                }
                else if (move.moveB.name == "Geomancy" && pokemonA.chargingMove)
                {
                    pokemonA.chargingMove = false;
                }
                if (move.moveB.name == "No Retreat")
                {
                    Console.WriteLine("Imprisonment not implemented");
                }
                pokemonA.lastMove = move;
                foreach (MoveEffect effect in move.moveB.effectList)
                {
                    InflictStatus(pokemonD, effect, field);
                    if (effect.recoil)
                    {
                        if (effect.effectPower < 0)
                        {
                            int lifeSteel = Convert.ToInt32(Math.Floor((double)pokemonA.maxHP / Math.Abs(move.moveB.effectList[0].effectPower)));
                            pokemonA.hp += lifeSteel;
                            if (pokemonA.hp > pokemonA.maxHP) pokemonA.hp = pokemonA.maxHP;
                            if (move.moveB.name == "Roost")
                            {
                                Console.WriteLine("Flying not removed");
                            }
                            if (move.moveB.name == "Shore Up")
                            {
                                Console.WriteLine("Weather not checked");
                            }
                        }
                    }
                }
            }
            else
            {
                double attack = 0.0;
                double defense = 0.0;
                pokemonA.critRatio = 0;
                int power = move.moveB.power;
                if (move.moveB.name == "Photon Geyser" || move.moveB.name == "Light That Burns the Sky" || move.moveB.name == "Shell side Arm" || move.moveB.name == "Tera Blast" || move.moveB.name == "Tera Starstorm")
                {
                    if (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod) > pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod))
                    {
                        move.moveB.split = Split.Physical;
                    }
                    else
                    {
                        move.moveB.split = Split.Special;
                    }
                }
                if (move.moveB.split == Split.Physical)
                {
                    attack = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
                    if (field.weather == Weather.Snow)
                    { 
                        if (pokemonD.species.type1 == Type.Ice || pokemonD.species.type2 == Type.Ice)
                        {
                            defense = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod)) * 1.5;
                        }
                        else
                        {
                            defense = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                        }
                    }
                    else
                    {
                        defense = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                    }
                }
                else if (move.moveB.split == Split.Special)
                {
                    attack = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
                    if (field.weather == Weather.Sandstorm)
                    {
                        if (pokemonD.species.type1 == Type.Rock || pokemonD.species.type2 == Type.Rock)
                        {
                            defense = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod)) * 1.5;
                        }
                        else
                        {
                            defense = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                        }
                    }
                    else
                    {
                        defense = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                    }
                }
                List<string> critMoves = new List<string> { "Aeroblast", "Air Cutter", "Aqua Cutter", "Attack Order", "Blaze Kick", "Crabhammer", "Cross Chop", "Cross Poison", "Drill Run", "Esper Wing", "Ivy Cudgel", "Karate Chop", "Leaf Blade", "Night Slash", "Plasma Fists", "Poison Tail", "Psycho Cut", "Razor Leaf", "Razor Wind", "Shadow Blast", "Shadow Claw", "Sky Attack", "Slash", "Snipe Shot", "Spacial Rend", "Stone Edge", "Triple Arrows", "G-Max Chi Strike" };
                if (critMoves.Contains(move.moveB.name))
                {
                    pokemonA.critRatio++;
                }
                if (move.moveB.name == "10,000,000 Volt Thunderbolt")
                {
                    pokemonA.critRatio += 2;
                }
                List<string> garCritMoves = new List<string> { "Flower Trick", "Frost Breath", "Storm Throw", "Surging Strikes", "Wicked Blow" };
                if (garCritMoves.Contains(move.moveB.name))
                {
                    pokemonA.critRatio = 3;
                }
                if (move.moveB.name == "Psyshock" || move.moveB.name == "Psystrike" || move.moveB.name == "Secret Sword")
                {
                    if (field.weather == Weather.Snow)
                    {
                        if (pokemonD.species.type1 == Type.Ice || pokemonD.species.type2 == Type.Ice)
                        {
                            defense = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod)) * 1.5;
                        }
                        else
                        {
                            defense = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                        }
                    }
                    else
                    {
                        defense = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                    }
                }
                if (move.moveB.name == "Sacred Sword" || move.moveB.name == "Darkest Lariat")
                {
                    if (field.weather == Weather.Snow)
                    {
                        if (pokemonD.species.type1 == Type.Ice || pokemonD.species.type2 == Type.Ice)
                        {
                            defense = pokemonD.CalcDefStat() * 1.5;
                        }
                        else
                        {
                            defense = pokemonD.CalcDefStat();
                        }
                    }
                    else
                    {
                        defense = pokemonD.CalcDefStat();
                    }
                }
                if (move.moveB.name == "Nihil Light")
                {
                    if (field.weather == Weather.Sandstorm)
                    {
                        if (pokemonD.species.type1 == Type.Rock || pokemonD.species.type2 == Type.Rock)
                        {
                            defense = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod)) * 1.5;
                        }
                        else
                        {
                            defense = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                        }
                    }
                    else
                    {
                        defense = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                    }
                }
                if (move.moveB.name == "Body Press")
                {
                    attack = (pokemonA.CalcDefStat() * pokemonA.GetMod(pokemonA.DefMod));
                }
                if (move.moveB.name == "Foul Play")
                {
                    attack = (pokemonD.CalcAtkStat() * pokemonD.GetMod(pokemonD.AtkMod));
                }
                if (move.moveB.name == "Relic Song")
                {
                    if (pokemonA.species.name == "Meloetta")
                    {
                        Console.WriteLine("Meloetta changed to Aria Forme!");
                        pokemonA.species = Program.AllPokemon.Find(sp => sp.name == "Meloetta-Aria");
                    }
                    else if (pokemonA.species.name == "Meloetta-Aria")
                    {
                        Console.WriteLine("Meloetta changed to Pirouette Forme!");
                        pokemonA.species = Program.AllPokemon.Find(sp => sp.name == "Meloetta");
                    }

                }
                if (move.moveB.name == "Dragon Tail" || move.moveB.name == "Circle Throw")
                {
                    Console.WriteLine("Switching not implemented");
                }
                if (move.moveB.name == "Night Shade" || move.moveB.name == "Seismic Toss")
                {
                    pokemonD.hp -= pokemonA.level;
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    pokemonA.lastMove = move;
                    return;
                }
                if ((move.moveB.name == "Fake Out" || move.moveB.name == "First Impression") && pokemonA.lastMove != null)
                {
                    return;
                }
                if (move.moveB.name == "Bolt Beak" || move.moveB.name == "Fishious Rend")
                {
                    if (pokemonA.moveFirst) power *= 2;
                }
                if (move.moveB.name == "Venoshock" || move.moveB.name == "Barb Barrage")
                {
                    if (pokemonD.statusNonVol == Status.Poison || pokemonD.statusNonVol == Status.Toxic)
                    {
                        power *= 2;
                    }
                }
                if (move.moveB.name == "Hex" || move.moveB.name == "Infernal Parade")
                {
                    if (pokemonD.statusNonVol != Status.None)
                    {
                        power *= 2;
                    }
                }
                if (move.moveB.name == "Stomping Tantrum")
                {
                    Console.WriteLine("Double dmg not done");
                }
                if (move.moveB.name == "Spectral Thief")
                {
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Atk, 100, Math.Max(0, pokemonD.AtkMod), false, 1), field);
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Def, 100, Math.Max(0, pokemonD.DefMod), false, 1), field);
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Spa, 100, Math.Max(0, pokemonD.SpaMod), false, 1), field);
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Spd, 100, Math.Max(0, pokemonD.SpdMod), false, 1), field);
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Spe, 100, Math.Max(0, pokemonD.SpeMod), false, 1), field);
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Acc, 100, Math.Max(0, pokemonD.AccMod), false, 1), field);
                    InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Eva, 100, Math.Max(0, pokemonD.EvaMod), false, 1), field);
                }
                if (move.moveB.name == "Flail" || move.moveB.name == "Reversal")
                {
                    int N = (48 * pokemonA.hp) / pokemonA.maxHP;

                    if (N < 2)
                        power = 200;
                    else if (N < 4)
                        power = 150;
                    else if (N < 9)
                        power = 100;
                    else if (N < 16)
                        power = 80;
                    else if (N < 32)
                        power = 40;
                    else
                        power = 20;
                }
                if (move.moveB.name == "Eruption" || move.moveB.name == "Water Spout" || move.moveB.name == "Dragon Energy")
                {
                    power = Convert.ToInt32(Math.Floor((double)(150 * pokemonA.hp) / pokemonA.maxHP));
                }
                if (move.moveB.name == "Hard Press")
                {
                    power = 100 * Convert.ToInt32(Math.Floor((double)(pokemonD.hp) / pokemonD.maxHP));
                }
                if (move.moveB.name == "Gyro Ball")
                {
                    double spe1;
                    double spe2;
                    double para = 1.00;
                    if (pokemonA.statusNonVol == Status.Paralysis) para = 0.5;
                    spe1 = pokemonA.CalcSpeStat() * pokemonA.GetMod(pokemonA.SpeMod) * para;

                    para = 1;
                    if (pokemonD.statusNonVol == Status.Paralysis) para = 0.5;
                    spe2 = pokemonD.CalcSpeStat() * pokemonD.GetMod(pokemonD.SpeMod) * para;

                    power = Math.Min(150, (int)((25 * spe1) / spe2) + 1);
                }
                if (move.moveB.name == "Electro Ball")
                {
                    double spe1;
                    double spe2;
                    double para = 1.00;
                    if (pokemonA.statusNonVol == Status.Paralysis) para = 0.5;
                    spe1 = pokemonA.CalcSpeStat() * pokemonA.GetMod(pokemonA.SpeMod) * para;

                    para = 1;
                    if (pokemonD.statusNonVol == Status.Paralysis) para = 0.5;
                    spe2 = pokemonD.CalcSpeStat() * pokemonD.GetMod(pokemonD.SpeMod) * para;

                    double ratio = spe1 / spe2;
                    if (ratio >= 4.0)
                        power = 150;
                    else if (ratio >= 3.0)
                        power = 120;
                    else if (ratio >= 2.0)
                        power = 80;
                    else if (ratio >= 1.0)
                        power = 60;
                    else
                        power = 40;
                }
                if (move.moveB.name == "Revalation Dance")
                {
                    move.moveB.type = pokemonA.species.type1;
                }
                if (move.moveB.name == "Brine")
                {
                    if (pokemonA.hp < pokemonA.maxHP / 2)
                    {
                        power *= 2;
                    }
                }
                if (move.moveB.name == "Last Respects")
                {
                    Console.WriteLine("Last respects not done");
                }
                if (move.moveB.name == "Throat Chop")
                {
                    Console.WriteLine("Cannot use sound moves not implemented");
                }
                if (move.moveB.name == "Dire Claw")
                {
                    if (Random.Shared.Next(0, 2) == 0)
                    {
                        int check = Random.Shared.Next(0, 100);
                        if (check < 33)
                        {
                            InflictStatus(pokemonD, new MoveEffect(Status.Poison, Stat.None, 100, 0, false, 1), field);
                        }
                        else if (check < 66)
                        {
                            InflictStatus(pokemonD, new MoveEffect(Status.Paralysis, Stat.None, 100, 0, false, 1), field);
                        }
                        else
                        {
                            InflictStatus(pokemonD, new MoveEffect(Status.Sleep, Stat.None, 100, 0, false, 1), field);
                        }
                    }
                }
                if (move.moveB.name == "G-Max Befuddle")
                {
                    int check = Random.Shared.Next(0, 100);
                    if (check < 33)
                    {
                        InflictStatus(pokemonD, new MoveEffect(Status.Poison, Stat.None, 100, 0, false, 1), field);
                    }
                    else if (check < 66)
                    {
                        InflictStatus(pokemonD, new MoveEffect(Status.Paralysis, Stat.None, 100, 0, false, 1), field);
                    }
                    else
                    {
                        InflictStatus(pokemonD, new MoveEffect(Status.Sleep, Stat.None, 100, 0, false, 1), field);
                    }
                }
                if (move.moveB.name == "G-Max Stun Shock")
                {
                    if (Random.Shared.Next(0, 2) == 0)
                    {
                        InflictStatus(pokemonD, new MoveEffect(Status.Poison, Stat.None, 100, 0, false, 1), field);
                    }
                    else
                    {
                        InflictStatus(pokemonD, new MoveEffect(Status.Paralysis, Stat.None, 100, 0, false, 1), field);
                    }
                }
                if (move.moveB.name == "Facade")
                {
                    if (pokemonA.statusNonVol == Status.Burn || pokemonA.statusNonVol == Status.Paralysis || pokemonA.statusNonVol == Status.Poison || pokemonA.statusNonVol == Status.Toxic)
                    {
                        power *= 2;
                    }
                }
                if (move.moveB.name == "Psychic Noise")
                {
                    Console.WriteLine("Heal Block not implemented");
                }
                if (move.moveB.name == "Natures Madness" || move.moveB.name == "Ruination")
                {
                    pokemonD.hp -= Math.Max(1, Convert.ToInt32(Math.Floor((double)pokemonD.hp / 2)));
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    return;
                }
                if (move.moveB.name == "Guardian of Alola")
                {
                    pokemonD.hp -= Convert.ToInt32(Math.Floor(pokemonA.maxHP / (double)(4 / 3)));
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    return;
                }
                if (move.moveB.name == "Eerie Spell")
                {
                    pokemonD.lastMove.PP -= 3;
                    if (pokemonD.lastMove.PP < 0) pokemonD.lastMove.PP = 0;
                }
                if (move.moveB.name == "Knock Off")
                {
                    if (pokemonD.heldItem != null)
                    {
                        power = Convert.ToInt32(Math.Floor(power * 1.5));
                    }
                    pokemonD.heldItem = null;
                }
                if (move.moveB.name == "Poltergeist")
                {
                    if (pokemonD.heldItem == null) return;
                }
                if (move.moveB.name == "Last Resort")
                {
                    foreach (Move m in pokemonA.moveSet)
                    {
                        if (m != move && m.PP > 0)
                        {
                            return;
                        }
                    }
                }
                if (move.moveB.name == "Tar Shot")
                {
                    Console.WriteLine("Fire wekaness not dont yet");
                }
                if (move.moveB.name == "Spirit Shackle" || move.moveB.name == "Jaw Lock")
                {
                    Console.WriteLine("Imprisonment not implemented");
                }
                if (move.moveB.name == "Ancient Power")
                {
                    int chance = Random.Shared.Next(0, 100);
                    if (chance < 10)
                    {
                        for (int i = 1; i < 6; i++)
                        {
                            Stat stt = (Stat)i;
                            MoveEffect eff = new MoveEffect(Status.None, stt, 100, 1, false, 1);
                            InflictStatus(pokemonA, eff, field);
                        }
                    }
                }
                if (move.moveB.name == "Judgment" || move.moveB.name == "Multi-Attack")
                {
                    move.moveB.type = pokemonA.species.type1;
                }
                if (move.moveB.name == "Aura Wheel")
                {
                    if (pokemonA.species.name == "Morpeko")
                    {
                        move.moveB.type = Type.Electric;
                    }
                    else if (pokemonA.species.name == "Morpeko-Hangry")
                    {
                        move.moveB.type = Type.Dark;
                    }
                    else return;
                }
                if (move.moveB.name == "Raging Bull")
                {
                    if (pokemonA.species.name == "Tauros-Paldea-Combat")
                    {
                        move.moveB.type = Type.Fighting;
                    }
                    else if (pokemonA.species.name == "Tauros-Paldea-Blaze")
                    {
                        move.moveB.type = Type.Fire;
                    }
                    else if (pokemonA.species.name == "Tauros-Paldea-Aqua")
                    {
                        move.moveB.type = Type.Water;
                    }
                    else return;
                }
                if (move.moveB.name == "Ivy Cudgel")
                {
                    if (pokemonA.species.name == "Ogrepon")
                    {
                        move.moveB.type = Type.Grass;
                    }
                    else if (pokemonA.species.name == "Ogrepon-Wellspring")
                    {
                        move.moveB.type = Type.Water;
                    }
                    else if (pokemonA.species.name == "Ogrepon-Hearthflame")
                    {
                        move.moveB.type = Type.Fire;
                    }
                    else if (pokemonA.species.name == "Ogrepon-Cornerstone")
                    {
                        move.moveB.type = Type.Rock;
                    }
                    else return;
                }
                if (move.moveB.name == "Fickle Beam")
                {
                    if (Random.Shared.Next(0, 101) < 30)
                    {
                        power *= 2;
                    }
                }
                if (move.moveB.name == "Crush Grip")
                {
                    power = Convert.ToInt32(Math.Floor((double)(120 * pokemonD.hp) / pokemonD.maxHP));
                }
                if (move.moveB.name == "Triple Axel" || move.moveB.name == "Population Bomb")
                {
                    Console.WriteLine("Ill do it later trust");
                }
                if (move.moveB.name == "Ceaseless Edge")
                {
                    field.sideB.spikes++;
                    if (field.sideB.spikes > 3) field.sideB.spikes = 3;
                }
                if (move.moveB.name == "Stone Axe" || move.moveB.name == "G-Max Stonesurge")
                {
                    field.sideB.stealthRock = true;
                }
                if (move.moveB.name == "Mortal Spin" || move.moveB.name == "Rapid Spin")
                {
                    field.sideA.ClearHazards();
                }
                if (move.moveB.name == "Genesis Supernova")
                {
                    field.ChangeTerrain(Terrain.Psychic);
                }
                if (move.moveB.name == "Steel Roller")
                {
                    if (field.terrain == Terrain.None) return;
                    else
                    {
                        field.terrain = Terrain.None; 
                        field.terrainTimer = 0;
                    }
                }
                if (move.moveB.name == "Ice Spinner" || move.moveB.name == "Splintered Stormshards")
                {
                    field.terrain = Terrain.None;
                    field.terrainTimer = 0;
                }
                if (move.moveB.name == "G-Max Wind Rage")
                {
                    bool auroraVeil = false;
                    int auroraVeilTimer = 0;
                    field.ClearHazards();
                    if (field.sideA.auroraVeil)
                    {
                        auroraVeil = true;
                        auroraVeilTimer = field.sideA.auroraVeilTimer;
                    }
                    field.ClearScreens();
                    if (auroraVeil)
                    {
                        field.sideA.auroraVeil = true;
                        field.sideA.auroraVeilTimer = auroraVeilTimer;
                    }
                    field.terrain = Terrain.None;
                    field.terrainTimer = 0;
                }
                if (field.terrain == Terrain.Psychic)
                {
                    if (move.moveB.name == "Expanding Force")
                    {
                        power = Convert.ToInt32(Math.Floor(power * 1.5));
                    }
                    else if (move.moveB.type == Type.Psychic)
                    {
                        power = Convert.ToInt32(Math.Floor(power * 1.3));
                    }

                }
                if (field.terrain == Terrain.Electric)
                {
                    if (move.moveB.name == "Rising Voltage")
                    {
                        power *= 2;
                    }
                    else if (move.moveB.type == Type.Electric)
                    {
                        power = Convert.ToInt32(Math.Floor(power * 1.3));
                    }
                }
                if (field.terrain == Terrain.Misty)
                {
                    if (move.moveB.type == Type.Dragon)
                    {
                        power /= 2;
                    }
                }
                if (field.terrain == Terrain.Grassy)
                {
                    if (move.moveB.type == Type.Grass)
                    {
                        power = Convert.ToInt32(Math.Floor(power * 1.3));
                    }
                    if (move.moveB.name == "Bulldoze" || move.moveB.name == "Earthquake" || move.moveB.name == "Magnitude")
                    {
                        power /= 2;
                    }
                }
                if (move.moveB.name == "Max Mindstorm")
                {
                    field.ChangeTerrain(Terrain.Psychic);
                }
                if (move.moveB.name == "Max Lightning")
                {
                    field.ChangeTerrain(Terrain.Electric);
                }
                if (move.moveB.name == "Max Overgrowth")
                {
                    field.ChangeTerrain(Terrain.Grassy);
                }
                if (move.moveB.name == "Max Starfall")
                {
                    field.ChangeTerrain(Terrain.Misty);
                }
                if (move.moveB.name == "Max Flare")
                {
                    field.ChangeWeather(Weather.Sun);
                }
                if (move.moveB.name == "Max Geyser")
                {
                    field.ChangeWeather(Weather.Rain);
                }
                if (move.moveB.name == "Max Rockfall")
                {
                    field.ChangeWeather(Weather.Sandstorm);
                }
                if (move.moveB.name == "Max Hailstorm")
                {
                    field.ChangeWeather(Weather.Snow);
                }        
                if (move.moveB.name == "G-Max Gravitas")
                {
                    field.gravity = true;
                    field.gravityTimer = 5;
                }
                if (move.moveB.name == "G-Max Sweetness")
                {
                    pokemonA.HealConditions();
                }
                if (move.moveB.name == "G - Max Steelsurge")
                {
                    field.sideB.sharpSteel = true;
                }
                if (move.moveB.name == "G-Max Depletion")
                {
                    if (pokemonD.lastMove == null) return;
                    pokemonD.lastMove.PP -= 2;
                    if (pokemonD.lastMove.PP < 0) pokemonD.lastMove.PP = 0;
                    return;
                }

                List<string> ignoreAbilities = new List<string> { "Moongeist Beam", "Sunsteel Strike", "Searing Sunraze Smash", "Menacing Moonraze Maelstrom", "G-Max Drum Solo", "G-Max Fireball", "G-Max Hydrosnipe" };
                if (ignoreAbilities.Contains(move.moveB.name))
                {
                    Console.WriteLine("Ignore ability not implemented");
                }
                if (move.moveB.name == "Endeavor")
                {
                    if (pokemonD.hp < pokemonA.hp)
                    {
                        return;
                    }
                    else
                        pokemonD.hp = pokemonA.hp;
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    pokemonA.lastMove = move;
                    return;

                }
                if (move.moveB.name == "Final Gambit")
                {
                    pokemonD.hp -= pokemonA.hp;
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    pokemonA.hp = 0;
                    return;
                }
                if (move.moveB.name == "U-Turn" || move.moveB.name == "Volt Switch" || move.moveB.name == "Flip Turn")
                {
                    Console.WriteLine("Switch not implemented");
                }
                if (move.moveB.name == "Super Fang")
                {
                    int halfHp = Convert.ToInt32(Math.Floor((double)pokemonD.hp / 2));
                    pokemonD.hp -= halfHp;
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    pokemonA.lastMove = move;
                    return;
                }
                if (pokemonA.lastMove != null && pokemonA.lastMove.moveB.name == "Charge" && move.moveB.type == Type.Electric)
                {
                    power *= 2;
                }
                if (move.moveB.name == "Fury Cutter")
                {
                    switch(pokemonA.furyCutter)
                    {
                        case 0:
                            power = 40;
                            pokemonA.furyCutter = 1;
                            break;
                        case 1:
                                power = 80;
                            pokemonA.furyCutter = 2;
                            break;
                        case 2:
                                power = 160;
                            break;
                        default:
                            power = 40;
                            break;
                    }
                }
                if (move.moveB.name == "Echoed Voice")
                {
                    pokemonA.echoedVoice++;
                    power += (pokemonA.echoedVoice * 40);
                    if (power > 200) power = 200;
                }
                if (move.moveB.name == "Thousand Waves")
                {
                    Console.WriteLine("Does not prevent switching");
                }
                if (move.moveB.name == "Clear Smog")
                {
                    pokemonD.ClearMods();
                }
                if (move.moveB.name == "Stored Power" || move.moveB.name == "Power Trip")
                {
                    int statBoosts = Math.Max(0, pokemonA.AtkMod) + Math.Max(0, pokemonA.DefMod) + Math.Max(0, pokemonA.SpaMod) +  Math.Max(0, pokemonA.SpdMod) +  Math.Max(0, pokemonA.SpeMod) + Math.Max(0, pokemonA.AccMod) + Math.Max(0, pokemonA.EvaMod);
                    power += statBoosts * 20;

                }
                List<string> charge = new List<string> { "Solar Beam", "Solar Blade", "Sky Drop", "Fly", "Dig", "Dive", "Bounce", "Skull Bash", "Razor Wind", "Ice Burn", "Freeze Shock", "Meteor Beam", "Electro Shot" };
                List<string> invurnable = new List<string> { "Fly", "Dig", "Dive", "Bounce", "Phantom Force", "Shadow Force", "Sky Drop"};
                if (charge.Contains(move.moveB.name) && !pokemonA.chargingMove)
                {
                    pokemonA.chargingMove = true;
                    Console.WriteLine($"{pokemonA.name} is charging up for {move.moveB.name}!");
                    if (invurnable.Contains(move.moveB.name))
                    {
                        pokemonA.invurnable = true;
                    }
                    if(move.moveB.name == "Meteor Beam" || move.moveB.name == "Electro Shot")
                    {
                       InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Spa, 100, 1, false, 1), field);
                    }
                    pokemonA.lastMove = move;
                    return;
                }
                else if (charge.Contains(move.moveB.name) && pokemonA.chargingMove)
                {
                    pokemonA.chargingMove = false;
                    if (invurnable.Contains(move.moveB.name))
                    {
                        pokemonA.invurnable = false;
                    }
                }
                List<string> reChargeMoves = new List<string> { "Hyper Beam", "Giga Impact", "Blast Burn", "Eternabeam", "Frenzy Plant", "Hydro Cannon", "Meteor Assault", "Prismatic Laser", "Roar of Time", "Rock Wrecker"};
                if (reChargeMoves.Contains(move.moveB.name))
                {
                    pokemonA.reCharge = true;
                }
                List<string> rampage = new List<string> { "Outrage", "Thrash", "Petal Dance", "Raging Fury" };
                if (rampage.Contains(move.moveB.name))
                {
                    pokemonA.lockedMove = true;
                    pokemonA.lockTimer = 1;
                    pokemonA.lastMove = move;
                }
                if (move.moveB.name == "Payback")
                {
                    if (pokemonA.moveFirst == false)
                    {
                        power *= 2;
                    }
                }
                if (move.moveB.name == "Sucker Punch" || move.moveB.name == "Thunderclap")
                {
                    if (pokemonD.moveFirst) return;
                    if (pokemonD.selectedMove.moveB.split == Split.Status) return;

                }
                if (move.moveB.name == "Thief")
                {
                    if (pokemonD.heldItem != null && pokemonA.heldItem == null)
                    {
                        pokemonA.heldItem = pokemonD.heldItem;
                        pokemonD.heldItem = null;
                        Console.WriteLine($"{pokemonA.name} stole {pokemonA.heldItem.name} from {pokemonD.name}!");
                    }
                    else return;
                }
                if (move.moveB.name == "Acrobatics")
                {
                    if (pokemonA.heldItem == null)
                    {
                        power *= 2;
                    }
                }
                if (move.moveB.name == "Snore")
                {
                    if (pokemonA.statusNonVol != Status.Sleep)
                    {
                        Console.WriteLine($"{pokemonA.name} can't use Snore because it isn't asleep!");
                        pokemonA.lastMove = move;
                        return;
                    }
                }
                if (move.moveB.name == "Mind Blown")
                if ((move.moveB.name == "Dynamax Cannon" || move.moveB.name == "Behemoth Bash" || move.moveB.name == "Behemoth Blade") && pokemonD.isDmax)
                {
                        power *= 2;
                }
                if (pokemonD.lastMove != null && pokemonD.lastMove.moveB.name == "Glaive Rush")
                {
                    power *= 2;
                }

                pokemonA.lastMove = move;
                int numHits = 1;
                if (move.moveB.effectList != null && move.moveB.effectList.Count > 0) numHits = move.moveB.effectList[0].multiHit;
                for (int i = 0; i < numHits; i++)
                {
                    pokemonD.hp -= Convert.ToInt32(pt * Damage(pokemonA, pokemonD, move, power, attack, defense, false, field));
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    if (pokemonD.hp == 0 && move.moveB.name == "Fell Stinger")
                    {
                        InflictStatus(pokemonA, new MoveEffect(Status.None, Stat.Atk, 100, 3, false, 1), field);
                    }
                    if (move.moveB.effectList != null)
                    {
                        foreach (MoveEffect effect in move.moveB.effectList)
                        {
                            if (effect.recoil)
                            {
                                if (effect.effectPower > 0)
                                {
                                    int recoilDamage = Convert.ToInt32(Math.Floor((double)(pokemonA.maxHP / effect.effectPower)));
                                    pokemonA.hp -= recoilDamage;
                                    if (pokemonA.hp < 0) pokemonA.hp = 0;
                                    Console.WriteLine($"{pokemonA.name} is hit with recoil");
                                }
                                else
                                {
                                    double lfstl = Math.Abs(effect.effectPower);
                                    if(move.moveB.name == "Draining Kiss" || move.moveB.name == "Oblivion Wing")
                                    {
                                        lfstl = 4/3;
                                    }
                                    int lifeSteel = Convert.ToInt32(Math.Floor((double)pokemonA.maxHP / lfstl));
                                    pokemonA.hp += lifeSteel;
                                    if (pokemonA.hp > pokemonA.maxHP) pokemonA.hp = pokemonA.maxHP;
                                    Console.WriteLine($"{pokemonA.name} regained some hp");
                                }
                            }
                            else
                            {
                                InflictStatus(pokemonD, effect, field);
                            }
                        }
                    }
                }
                if (pokemonD.selectedMove.moveB.name == "Beak Blast" && move.moveB.contact)
                {
                    InflictStatus(pokemonA, new MoveEffect(Status.Burn, Stat.None, 100, 0, false, 1), field);
                }

                pokemonA.critRatio = 0;
            }
        }
        else
        {
            Console.WriteLine("haha you missed");
            if (move.moveB.name == "Axe Kick" || move.moveB.name == "Supercell Slam")
            {
                pokemonA.hp -= Convert.ToInt32(Math.Floor(pokemonA.maxHP / 2.0));
            }
            MoveB mis = new MoveB("Miss", Type.Normal, 0, Split.Status, 0, 0, 0, false, false, null);
            Move miss = new Move(mis);
            pokemonA.lastMove = miss;
        }
    }
    public static int Damage(Pokemon pokemonA, Pokemon pokemonD, Move move, int power, double atk, double def, bool test, Field field)
    {
        Type pkAtype1 = pokemonA.species.type1;
        Type pkAtype2 = pokemonA.species.type2;
        Type pkDtype1 = pokemonD.species.type1;
        Type pkDtype2 = pokemonD.species.type2;

        if (pokemonA.terastallized)
        {
            pkAtype1 = pokemonA.tera;
            pkAtype2 = 0;
        }
        if (pokemonD.terastallized)
        {
            pkDtype1 = pokemonD.tera;
            pkDtype2 = 0;
        }

        double stab = 1;

        if (pkAtype1 == move.moveB.type || pkAtype2 == move.moveB.type)
        {
            stab = 1.5;

            if (pokemonA.terastallized && (pokemonA.tera == pokemonA.species.type1 || pokemonA.tera == pokemonA.species.type2))
            {
                stab = 2.0;
            }
        }

        double status = 1.00;
        if (pokemonA.statusNonVol == Status.Burn && move.moveB.split == Split.Physical)
        {
            status = 0.50;
        }

        double eff1 = MatchUp(move.moveB.type, pkDtype1);
        double eff2 = MatchUp(move.moveB.type, pkDtype2);
        if (pokemonA.selectedMove != null)
        { 
            if (pokemonA.selectedMove.moveB.name == "Flying Press")
        {
            eff1 = MatchUp(Type.Fighting, pkDtype1) * MatchUp(Type.Flying, pkDtype1);
            eff2 = MatchUp(Type.Fighting, pkDtype2) * MatchUp(Type.Flying, pkDtype2);
        }
            if (pokemonA.selectedMove.moveB.name == "Freeze-Dry")
        {
            eff1 = MatchUp(Type.Ice, pkDtype1);
            eff2 = MatchUp(Type.Ice, pkDtype2);
            if (pkDtype1 == Type.Water) eff1 = 2.0;
            if (pkDtype2 == Type.Water) eff2 = 2.0;
        }
            if (pokemonA.selectedMove.moveB.name == "Thousand Arrows")
        {
            if (pkDtype1 == Type.Flying) eff1 = 1.0;
            if (pkDtype2 == Type.Flying) eff2 = 1.0;
        }
            if (pokemonA.selectedMove.moveB.name == "Collision Course" || pokemonA.selectedMove.moveB.name == "Electro Drift")
            {
                if (eff1 >= 2.0) eff1 *= 5461 / 4096;
                if (eff2 >= 2.0) eff2 *= 5461 / 4096;
            }
            if (pokemonA.selectedMove.moveB.name == "Nihil Light")
            {
                eff1 = MatchUp(Type.Dragon, pkDtype1);
                eff2 = MatchUp(Type.Dragon, pkDtype2);
                if (pkDtype1 == Type.Fairy) eff1 = 1.0;
                if (pkDtype2 == Type.Fairy) eff2 = 1.0;
            }
        }
        if (field.weather == Weather.StrongWinds)
        {
            if (pkDtype1 == Type.Flying)
            {
                eff1 = Math.Min(eff1, 1.0);
            }
            if (pkDtype2 == Type.Flying)
            {
                eff2 = Math.Min(eff2, 1.0);
            }
        }
        if (field.weather == Weather.Sun || field.weather == Weather.HarshSun)
        {
            if (move.moveB.type == Type.Fire || move.moveB.name == "Hydro Steam")
            {
                power = Convert.ToInt32(Math.Floor(power * 1.5));
            }
            else if (move.moveB.type == Type.Water)
            {
                if (field.weather == Weather.HarshSun) return 0; 
                power = Convert.ToInt32(Math.Floor(power * 0.5));
            }
        }
        else if (field.weather == Weather.Rain || field.weather == Weather.HeavyRain)
        {
            if (move.moveB.type == Type.Water)
            {
                power = Convert.ToInt32(Math.Floor(power * 1.5));
            }
            else if (move.moveB.type == Type.Fire)
            {
                if (field.weather == Weather.HeavyRain) return 0;
                power = Convert.ToInt32(Math.Floor(power * 0.5));
            }
        }



        double crit = 1;
        int rcrit = 24;
        switch (pokemonA.critRatio)
        {
            case 0:
                rcrit = 24;
                break;
            case 1:
                rcrit = 8;
                break;
            case 2:
                rcrit = 2;
                break;
            default:
                rcrit = 1;
                break;
        }
        if (Random.Shared.Next(0, rcrit) == 0 && !test)
        {
            crit = 1.5;
            Console.Write("Critical hit!");
        }

        double item = 1.00;
        double ran = Random.Shared.Next(85, 101) / 100.0;
        if (test) ran = 9.2;
        int dmg = Convert.ToInt32(Math.Round(((((((((2 * pokemonA.level) / 5) + 2) * power * ((double)atk / def)) / 50) * crit) + 2) * stab * eff1 * eff2 * ran * status * item), 0));// too complicated check https:bulbapedia.bulbagarden.net / wiki / Damage
        if (pokemonD.hp < dmg && !test)
        {
            Console.WriteLine($" It did {pokemonD.hp} damage!");
        }
        else if (!test)
        {
            Console.WriteLine($" It did {dmg} damage!");
        }

        return dmg;
    }
    public static void InflictStatus(Pokemon pk, MoveEffect effect, Field field)
    {
        if (effect.effectChance < 101)
        {
            int check = Random.Shared.Next(1, 101);

            if (check <= effect.effectChance)
            {
                if (effect.effectStat != Stat.None)
                {
                    switch (effect.effectStat)
                    {
                        case Stat.Atk:
                            if (pk.AtkMod < 6 && pk.AtkMod > -6)
                            {
                                pk.AtkMod += effect.effectPower;
                                if (pk.AtkMod > 6) pk.AtkMod = 6;
                                else if (pk.AtkMod < -6) pk.AtkMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Def:
                            if (pk.DefMod < 6 && pk.DefMod > -6)
                            {
                                pk.DefMod += effect.effectPower;
                                if (pk.DefMod > 6) pk.DefMod = 6;
                                else if (pk.DefMod < -6) pk.DefMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Spa:
                            if (pk.SpaMod < 6 && pk.SpaMod > -6)
                            {
                                pk.SpaMod += effect.effectPower;
                                if (pk.SpaMod > 6) pk.SpaMod = 6;
                                else if (pk.SpaMod < -6) pk.SpaMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Spd:
                            if (pk.SpdMod < 6 && pk.SpdMod > -6)
                            {
                                pk.SpdMod += effect.effectPower;
                                if (pk.SpdMod > 6) pk.SpdMod = 6;
                                else if (pk.SpdMod < -6) pk.SpdMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Acc:
                            if (pk.AccMod < 6 && pk.AccMod > -6)
                            {
                                pk.AccMod += effect.effectPower;
                                if (pk.AccMod > 6) pk.AccMod = 6;
                                else if (pk.AccMod < -6) pk.AccMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Eva:
                            if (pk.EvaMod < 6 && pk.EvaMod > -6)
                            {
                                pk.EvaMod += effect.effectPower;
                                if (pk.EvaMod > 6) pk.EvaMod = 6;
                                else if (pk.EvaMod < -6) pk.EvaMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Spe:
                            if (pk.SpeMod < 6 && pk.SpeMod > -6)
                            {
                                pk.SpeMod += effect.effectPower;
                                if (pk.SpeMod > 6) pk.SpeMod = 6;
                                else if (pk.SpeMod < -6) pk.SpeMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        default:
                            break;
                    }
                }
                List<Status> nonVolitile = new List<Status> {Status.Confusion, Status.Infatuation, Status.Flinch, Status.Torment }; 
                if (effect.effectStatus != Status.None)
                {
                    if (field != null && field.terrain != Terrain.Misty && pk.statusNonVol == Status.None && !nonVolitile.Contains(effect.effectStatus) && !pk.IsImmune(effect.effectStatus))
                    {
                        if(field.terrain == Terrain.Electric && effect.effectStatus == Status.Sleep)
                        {
                            return;
                        }
                        pk.statusNonVol = effect.effectStatus;
                    }
                    else if (!pk.statusVol.Contains(effect.effectStatus) && effect.effectStatus != Status.Flinch && !pk.IsImmune(effect.effectStatus))
                    {
                        if (field != null && field.terrain == Terrain.Electric && effect.effectStatus == Status.Drowsy)
                        {
                            return;
                        }
                        if (effect.effectStatus == Status.EscapePrevent && (pk.species.type1 == Type.Ghost || pk.species.type2 == Type.Ghost))
                        {
                            return;
                        }
                        List<Status> trap = new List<Status> { Status.Bound, Status.Vinelash, Status.Wildfire, Status.Cannonade};
                        if (pk.statusVol.Any(s => trap.Contains(s)))
                        {
                            return;
                        }
                        pk.statusVol.Add(effect.effectStatus);
                    }
                }
            }
        }
    }
    public static void PokeBattle(Pokemon pokemon1, Pokemon pokemon2, int ai)
    {
        Pokemon currentPokemon1 = pokemon1;
        Pokemon currentPokemon2 = pokemon2;
        FieldSide fieldSide1 = new FieldSide();
        FieldSide fieldSide2 = new FieldSide();
        Field field = new Field(fieldSide1, fieldSide2);
        currentPokemon1.dMaxTimer = 0;
        currentPokemon2.dMaxTimer = 0;
        while (currentPokemon1.hp > 0 && currentPokemon2.hp > 0)
        {
            Console.WriteLine("\n=next turn=\n");
            currentPokemon1.moveFirst = false;
            currentPokemon2.moveFirst = false;
            currentPokemon1.selectedMove = null;
            currentPokemon2.selectedMove = null;
            double spe1 = 0;
            double spe2 = 0;

            //Pkmn 1 turn
            if (currentPokemon1.chargingMove && currentPokemon1.invurnable)
            {
                currentPokemon1.selectedMove = currentPokemon1.lastMove;
            }
            else if (currentPokemon1.lockedMove)
            {
                currentPokemon1.selectedMove = currentPokemon1.lastMove;
                currentPokemon1.lockTimer++;
                if (currentPokemon1.lockTimer >= 3)
                {
                    if (Random.Shared.Next(0, 2) == 0 && currentPokemon1.lockTimer == 3)
                    {
                        currentPokemon1.lockedMove = false;
                        currentPokemon1.lockTimer = 0;
                    }
                }
            }
            else
            {
                currentPokemon1.selectedMove = currentPokemon1.PickMove(currentPokemon2, ai);
            }

            double para = 1;
            if (currentPokemon1.statusNonVol == Status.Paralysis) para = 0.5;
            spe1 = currentPokemon1.CalcSpeStat() * currentPokemon1.GetMod(currentPokemon1.SpeMod) * para;

            // Pkmn 2 turn
            if (currentPokemon2.chargingMove && currentPokemon2.invurnable)
            {
                currentPokemon2.selectedMove = currentPokemon2.lastMove;
            }
            else if (currentPokemon2.lockedMove)
            {
                currentPokemon2.selectedMove = currentPokemon2.lastMove;
                currentPokemon2.lockTimer++;
                if (currentPokemon2.lockTimer >= 3)
                {
                    if (Random.Shared.Next(0, 2) == 0 && currentPokemon2.lockTimer == 3)
                    {
                        currentPokemon2.lockedMove = false;
                        currentPokemon2.lockTimer = 0;
                    }
                }
            }
            else
            {
                currentPokemon2.selectedMove = currentPokemon2.PickMove(currentPokemon1, ai);
            }

            para = 1;
            if (currentPokemon2.statusNonVol == Status.Paralysis) para = 0.5;
            spe2 = currentPokemon2.CalcSpeStat() * currentPokemon2.GetMod(currentPokemon2.SpeMod) * para;

            int priority1 = 0;
            int priority2 = 0;
            if (currentPokemon1.selectedMove != null) priority1 = currentPokemon1.selectedMove.moveB.priority;
            if (currentPokemon2.selectedMove != null) priority2 = currentPokemon2.selectedMove.moveB.priority;

            if (priority1 > priority2)
            {
                currentPokemon1.moveFirst = true;
            }
            else if (priority1 < priority2)
            {
                currentPokemon2.moveFirst = true;
            }
            else
            {
                if (spe1 > spe2)
                {
                    currentPokemon1.moveFirst = true;
                }
                else if (spe1 < spe2)
                {
                    currentPokemon2.moveFirst = true;
                }
                else
                {
                    int tie = Random.Shared.Next(0, 2);
                    if (tie == 0)
                    {
                        currentPokemon1.moveFirst = true;
                    }
                    else 
                    {
                        currentPokemon2.moveFirst = true;
                    }
                }
            }
            if (currentPokemon1.moveFirst)
            { 
                if (currentPokemon1.selectedMove != null && currentPokemon1.DoIMove())
                {
                    ExecuteMove(currentPokemon1, currentPokemon2, currentPokemon1.selectedMove, field);
                }
                if (currentPokemon2.selectedMove != null && currentPokemon2.hp > 0)
                {
                    if (currentPokemon2.DoIMove())
                    {
                        if (currentPokemon1.hp <= 0 && currentPokemon2.selectedMove.moveB.split != Split.Status)
                            Console.WriteLine($"{currentPokemon2.selectedMove.moveB.name} failed");
                        else
                            ExecuteMove(currentPokemon2, currentPokemon1, currentPokemon2.selectedMove, field);
                    }
                }
            }
            else
            {
                if (currentPokemon2.selectedMove != null && currentPokemon2.DoIMove())
                {
                    ExecuteMove(currentPokemon2, currentPokemon1, currentPokemon2.selectedMove, field);
                }
                if (currentPokemon1.selectedMove != null && currentPokemon1.hp > 0)
                {
                    if (currentPokemon1.DoIMove())
                    {
                        if (currentPokemon2.hp <= 0 && currentPokemon1.selectedMove.moveB.split != Split.Status)
                            Console.WriteLine($"{currentPokemon1.selectedMove.moveB.name} failed");
                        else
                            ExecuteMove(currentPokemon1, currentPokemon2, currentPokemon1.selectedMove, field);
                    }
                }
            }

            PostTurnPokemonCheck(currentPokemon1, field);
            PostTurnPokemonCheck(currentPokemon2, field);
        }

        if (currentPokemon1.hp > 0)
        {
            Console.WriteLine($"{currentPokemon1.name} wins the battle!");
            currentPokemon1.wins++;
        }
        else
        {
            Console.WriteLine($"{currentPokemon2.name} wins the battle!");
            currentPokemon2.wins++;
        }

        currentPokemon1.Heal();
        currentPokemon2.Heal();
    }
    public static void TrainerBattle(Trainer team1, Trainer team2, int ai)
    {
        team1.HealTeam();
        team2.HealTeam();
        Pokemon currentPokemon1 = team1.team[0];
        Pokemon currentPokemon2 = team2.team[0];
        FieldSide fieldSide1 = new FieldSide();
        FieldSide fieldSide2 = new FieldSide();
        Field field = new Field(fieldSide1, fieldSide2);
        Console.WriteLine($"{team1.name} sent out {currentPokemon1.name}");
        Console.WriteLine($"{team2.name} sent out {currentPokemon2.name}");
        bool gimmick1 = false;
        bool gimmick2 = false;
        while (team1.AbleToBattle() && team2.AbleToBattle())
        {
            Console.WriteLine("\n=next turn=\n");
            currentPokemon1.moveFirst = false;
            currentPokemon2.moveFirst = false;
            currentPokemon1.selectedMove = null;
            currentPokemon2.selectedMove = null;
            double spe1 = 0;
            double spe2 = 0;

            if (currentPokemon1.hp <= 0)
            {
                currentPokemon1 = team1.ShouldSwitch(currentPokemon1, currentPokemon2, ai);
                Console.WriteLine($"{team1.name} sent out {currentPokemon1.name}");
                field.sideA.Entry(currentPokemon1);
                currentPokemon1.lastMove = null;
                if (gimmick1 == false)
                {
                    PreTurnTrainerCheck(currentPokemon1, team1, ai);
                    if (currentPokemon1.terastallized || currentPokemon1.isDmax || currentPokemon1.species.name.Contains("-Mega") || currentPokemon1.usedZMove)
                    {
                        gimmick1 = true;
                    }
                }

                if (currentPokemon1.chargingMove && currentPokemon1.invurnable)
                {
                    currentPokemon1.selectedMove = currentPokemon1.lastMove;
                }
                else if (currentPokemon1.lockedMove)
                {
                    currentPokemon1.selectedMove = currentPokemon1.lastMove;
                    currentPokemon1.lockTimer++;
                    if (currentPokemon1.lockTimer >= 3)
                    {
                        if (Random.Shared.Next(0, 2) == 0 && currentPokemon1.lockTimer == 3)
                        {
                            currentPokemon1.lockedMove = false;
                            currentPokemon1.lockTimer = 0;
                        }
                    }
                }
                else
                {
                    currentPokemon1.selectedMove = currentPokemon1.PickMove(currentPokemon2, ai);
                }
                double para = 1;
                if (currentPokemon1.statusNonVol == Status.Paralysis) para = 0.5;
                spe1 = currentPokemon1.CalcSpeStat() * currentPokemon1.GetMod(currentPokemon1.SpeMod) * para;
            }
            if (currentPokemon2.hp <= 0)
            {
                currentPokemon2 = team2.ShouldSwitch(currentPokemon2, currentPokemon1, ai);
                Console.WriteLine($"{team2.name} sent out {currentPokemon2.name}");
                field.sideB.Entry(currentPokemon2);
                currentPokemon2.lastMove = null;
                if (gimmick2 == false)
                {
                    PreTurnTrainerCheck(currentPokemon2, team2, ai);
                    if (currentPokemon2.terastallized || currentPokemon2.isDmax || currentPokemon2.species.name.Contains("-Mega") || currentPokemon2.usedZMove)
                    {
                        gimmick2 = true;
                    }
                }

                if (currentPokemon2.chargingMove && currentPokemon2.invurnable)
                {
                    currentPokemon2.selectedMove = currentPokemon2.lastMove;
                }
                else if (currentPokemon2.lockedMove)
                {
                    currentPokemon2.selectedMove = currentPokemon2.lastMove;
                    currentPokemon2.lockTimer++;
                    if (currentPokemon2.lockTimer >= 3)
                    {
                        if (Random.Shared.Next(0, 2) == 0 && currentPokemon2.lockTimer == 3)
                        {
                            currentPokemon2.lockedMove = false;
                            currentPokemon2.lockTimer = 0;
                        }
                    }
                }
                else
                {
                    currentPokemon2.selectedMove = currentPokemon2.PickMove(currentPokemon1, ai);
                }
                double para = 1;
                if (currentPokemon2.statusNonVol == Status.Paralysis) para = 0.5;
                spe2 = currentPokemon2.CalcSpeStat() * currentPokemon2.GetMod(currentPokemon2.SpeMod) * para;
            }

            if (currentPokemon1.hp > 0)
            {
                Pokemon preSwitch1 = currentPokemon1;
                currentPokemon1 = team1.ShouldSwitch(currentPokemon1, currentPokemon2, ai);
                if (preSwitch1 != currentPokemon1)
                {
                    field.sideA.Entry(currentPokemon1);
                    Console.WriteLine($"{team1.name} switched to {currentPokemon1.name}");
                    currentPokemon1.lastMove = null;
                }
                else
                {
                    if (gimmick1 == false)
                    {
                        PreTurnTrainerCheck(currentPokemon1, team1, ai);
                        if (currentPokemon1.terastallized || currentPokemon1.isDmax || currentPokemon1.species.name.Contains("-Mega"))
                        {
                            gimmick1 = true;
                        }
                    }

                    if (currentPokemon1.chargingMove && currentPokemon1.invurnable)
                    {
                        currentPokemon1.selectedMove = currentPokemon1.lastMove;
                    }
                    else if (currentPokemon1.lockedMove)
                    {
                        currentPokemon1.selectedMove = currentPokemon1.lastMove;
                        currentPokemon1.lockTimer++;
                        if (currentPokemon1.lockTimer >= 3)
                        {
                            if (Random.Shared.Next(0, 2) == 0 && currentPokemon1.lockTimer == 3)
                            {
                                currentPokemon1.lockedMove = false;
                                currentPokemon1.lockTimer = 0;
                            }
                        }
                    }
                    else
                    {
                        currentPokemon1.selectedMove = currentPokemon1.PickMove(currentPokemon2, ai);
                    }
                    double para = 1;
                    if (currentPokemon1.statusNonVol == Status.Paralysis) para = 0.5;
                    spe1 = currentPokemon1.CalcSpeStat() * currentPokemon1.GetMod(currentPokemon1.SpeMod) * para;
                }
            }
            if (currentPokemon2.hp > 0)
            {
                Pokemon preSwitch2 = currentPokemon2;
                currentPokemon2 = team2.ShouldSwitch(currentPokemon2, currentPokemon1, ai);
                if (preSwitch2 != currentPokemon2)
                {
                    field.sideB.Entry(currentPokemon2);
                    Console.WriteLine($"{team2.name} switched to {currentPokemon2.name}");
                    currentPokemon2.lastMove = null;
                }
                else
                {
                    if (gimmick2 == false)
                    {
                        PreTurnTrainerCheck(currentPokemon2, team2, ai);
                        if (currentPokemon2.terastallized || currentPokemon2.isDmax || currentPokemon2.species.name.Contains("-Mega") || currentPokemon2.usedZMove)
                        {
                            gimmick2 = true;
                        }
                    }

                    if (currentPokemon2.chargingMove && currentPokemon2.invurnable)
                    {
                        currentPokemon2.selectedMove = currentPokemon2.lastMove;
                    }
                    else if (currentPokemon2.lockedMove)
                    {
                        currentPokemon2.selectedMove = currentPokemon2.lastMove;
                        currentPokemon2.lockTimer++;
                        if (currentPokemon2.lockTimer >= 3)
                        {
                            if (Random.Shared.Next(0, 2) == 0 && currentPokemon2.lockTimer == 3)
                            {
                                currentPokemon2.lockedMove = false;
                                currentPokemon2.lockTimer = 0;
                            }
                        }
                    }
                    else
                    {
                        currentPokemon2.selectedMove = currentPokemon2.PickMove(currentPokemon1, ai);
                    }
                    double para = 1;
                    if (currentPokemon2.statusNonVol == Status.Paralysis) para = 0.5;
                    spe2 = currentPokemon2.CalcSpeStat() * currentPokemon2.GetMod(currentPokemon2.SpeMod) * para;
                }
            }

            if (currentPokemon1.lastMove != null) Console.WriteLine($"last move: {currentPokemon1.lastMove.moveB.name}");
            else Console.WriteLine("last move: none");
            if (currentPokemon2.lastMove != null) Console.WriteLine($"last move: {currentPokemon2.lastMove.moveB.name}");
            else Console.WriteLine("last move: none");
            int priority1 = 0;
            int priority2 = 0;
            if (currentPokemon1.selectedMove != null) 
            {               
                priority1 = currentPokemon1.selectedMove.moveB.priority;
                if (field.terrain == Terrain.Grassy && currentPokemon1.selectedMove.moveB.name == "Grassy Glide")
                {
                    priority1 += 1;
                }
                if (field.terrain == Terrain.Psychic && priority1 > 0)
                {
                    priority1 = 0;
                    currentPokemon1.selectedMove = null;
                }
            }
            if (currentPokemon2.selectedMove != null && currentPokemon2.selectedMove.moveB != null)
            {
                priority2 = currentPokemon2.selectedMove.moveB.priority;
                if (field.terrain == Terrain.Grassy && currentPokemon1.selectedMove.moveB.name == "Grassy Glide")
                {
                    priority1 += 1;
                }
                if (field.terrain == Terrain.Psychic && priority2 > 0)
                {
                    priority2 = 0;
                    currentPokemon2.selectedMove = null;
                }
            }


            if (priority1 > priority2)
            {
                if (!field.trickRoom)
                {
                    currentPokemon1.moveFirst = true;
                }
                else currentPokemon2.moveFirst = true;
            }
            else if (priority1 < priority2)
            {
                if (!field.trickRoom)
                {
                    currentPokemon2.moveFirst = true;
                }
                else currentPokemon1.moveFirst = true;
            }
            else
            {
                if (spe1 > spe2)
                {
                    if (!field.trickRoom)
                    {
                        currentPokemon1.moveFirst = true;
                    }
                    else currentPokemon2.moveFirst = true;
                }
                else if (spe1 < spe2)
                {
                    if (!field.trickRoom)
                    {
                        currentPokemon2.moveFirst = true;
                    }
                    else currentPokemon1.moveFirst = true;
                }
                else
                {
                    int tie = Random.Shared.Next(0, 2);
                    if (tie == 0)
                    {
                        if (!field.trickRoom)
                        {
                            currentPokemon1.moveFirst = true;
                        }
                        else currentPokemon2.moveFirst = true;
                    }
                    else
                    {
                        if (!field.trickRoom)
                        {
                            currentPokemon2.moveFirst = true;
                        }
                        else currentPokemon1.moveFirst = true;
                    }
                }
            }

            if (currentPokemon1.selectedMove != null) Console.WriteLine($"move: {currentPokemon1.selectedMove.moveB.name} - {currentPokemon1.selectedMove.PP}");
            else Console.WriteLine("none");
            if (currentPokemon2.selectedMove != null) Console.WriteLine($"move: {currentPokemon2.selectedMove.moveB.name} - {currentPokemon2.selectedMove.PP}");
            else Console.WriteLine("none");

            if (currentPokemon1.moveFirst)
            {
                if (currentPokemon1.selectedMove != null && currentPokemon1.DoIMove())
                {
                    ExecuteMove(currentPokemon1, currentPokemon2, currentPokemon1.selectedMove, field);
                }
                if (currentPokemon2.selectedMove != null && currentPokemon2.hp > 0)
                {
                    if (currentPokemon2.DoIMove())
                    {
                        if (currentPokemon1.hp <= 0 && currentPokemon2.selectedMove.moveB.split != Split.Status)
                            Console.WriteLine($"{currentPokemon2.selectedMove.moveB.name} failed");
                        else
                            ExecuteMove(currentPokemon2, currentPokemon1, currentPokemon2.selectedMove, field);
                    }
                }
            }
            else
            {
                if (currentPokemon2.selectedMove != null && currentPokemon2.DoIMove())
                {
                    ExecuteMove(currentPokemon2, currentPokemon1, currentPokemon2.selectedMove, field);
                }
                if (currentPokemon1.selectedMove != null && currentPokemon1.hp > 0)
                {
                    if (currentPokemon1.DoIMove())
                    {
                        if (currentPokemon2.hp <= 0 && currentPokemon1.selectedMove.moveB.split != Split.Status)
                            Console.WriteLine($"{currentPokemon1.selectedMove.moveB.name} failed");
                        else
                            ExecuteMove(currentPokemon1, currentPokemon2, currentPokemon1.selectedMove, field);
                    }
                }
            }

            PostTurnPokemonCheck(currentPokemon1, field);
            PostTurnPokemonCheck(currentPokemon2, field);
            field.PostTurnCheck();

            currentPokemon1.dMaxTimer--;
            currentPokemon2.dMaxTimer--;

            if (currentPokemon1.dMaxTimer == 0 && currentPokemon1.isDmax)
            {
                currentPokemon1.UnDmax();
            }
            if (currentPokemon2.dMaxTimer == 0 && currentPokemon2.isDmax)
            {
                currentPokemon2.UnDmax();
            }
        }
        if (team1.AbleToBattle())
        {
            Console.WriteLine($"{team1.name} wins the battle!");
            team1.wins++;                                                                                                                                             
        }                                                                                                                                                             
        else                                                                                                                                                          
        {                                                                                                                                                             
            Console.WriteLine($"{team2.name} wins the battle!");                                                                                                      
            team2.wins++;                                                                                                                                             
        }                                                                                                                                                             
        team1.HealTeam();                                                                                                                                             
        team2.HealTeam();                                                                                                                                             
    }                                                                                                                                                                 
    public static void ExecuteMove(Pokemon atk, Pokemon def, Move move, Field field)
    {
        Console.WriteLine($"{atk.name} used {move.moveB.name} against {def.name}");
        Move(atk, def, move, field);

        Console.WriteLine(def.hp);
        if (def.hp <= 0)
        {
            Console.WriteLine($"{def.name} fainted");
            def.UnDmax();
            def.UnMegaEvolve();
            def.UnTerastallize();
            def.dMaxTimer = 0;
        }
        if (atk.hp <= 0)
        {
            Console.WriteLine($"{atk.name} fainted");
            atk.UnDmax();
            atk.UnMegaEvolve();
            atk.UnTerastallize();
            atk.dMaxTimer = 0;
        }
    }
    public static void PreTurnTrainerCheck(Pokemon pk, Trainer tr, int ai)
    {
        if (pk.hp <= 0) return;
        if (ai == 1) return;

        bool isLastAlive = tr.LastInBattle();
        bool isAcePokemon = (pk == tr.team[tr.team.Count - 1]);
        bool canUseGimmick = false;

        if (tr.ace)
        {
            canUseGimmick = isAcePokemon && isLastAlive;
        }
        else
        {
            canUseGimmick = isLastAlive;
        }

        if (!canUseGimmick) return;

        if (pk.species.mega && pk.CheckStone())
        {
            pk.MegaEvo();
            return;
        }
        bool canZmove = false;
        foreach (Move move in pk.moveSet)
        {
            if (move != null && pk.heldItem != null && move.moveB.type == pk.heldItem.ZCrystalType)
            {
                canZmove = true;
                break;
            }
        }

        if (pk.heldItem != null && pk.heldItem.Zcrystal && canZmove)
        {
            pk.usedZMove = true;
            return;
        }

        if (pk.gmax && pk.species.gmax)
        {
            pk.Dmax();
            return;
        }

        else
        {
            int check = Random.Shared.Next(0, 100);
            if (check < 33)
            {
                pk.Dmax();
                return;
            }
            else if (check < 66)
            {
                pk.Terastallize();
                return;
            }
        }
    }
    public static void PostTurnPokemonCheck(Pokemon pk, Field field)
    {
        if (pk == null) return;
        if (pk.hp <= 0) return;
        switch (pk.statusNonVol)
        {
            case Status.Poison:
                int dmg = Convert.ToInt32(Math.Round(pk.maxHP / 8.0, 0));
                pk.hp -= dmg;
                Console.WriteLine($"{pk.name} is hurt by poison and lost {dmg} HP!");
                if (pk.hp < 0) pk.hp = 0;
                break;
            case Status.Toxic:
                if (pk.toxicCounter == 0) pk.toxicCounter = 1;
                int toxicDmg = Convert.ToInt32(Math.Round((pk.maxHP / 16.0) * pk.toxicCounter, 0));
                pk.hp -= toxicDmg;
                Console.WriteLine($"{pk.name} is hurt by toxic poison and lost {toxicDmg} HP!");
                pk.toxicCounter++;
                if (pk.hp < 0) pk.hp = 0;
                break;
            case Status.Burn:
                int burnDmg = Convert.ToInt32(Math.Round(pk.maxHP / 16.0, 0));
                pk.hp -= burnDmg;
                Console.WriteLine($"{pk.name} is hurt by its burn and lost {burnDmg} HP!");
                if (pk.hp < 0) pk.hp = 0;
                break;
        }
        if (pk.statusVol.Contains(Status.Torment))
        {
            if (pk.tormentCounter == 0) pk.tormentCounter = 1;
            else pk.tormentCounter++;

            if(pk.tormentCounter == 2)
            {
                pk.statusVol.Remove(Status.Torment);
            }
        }
        if (pk.statusVol.Contains(Status.Drowsy))
        {
            if(!pk.yawn) pk.yawn = true;
            else
            {
                pk.statusVol.Remove(Status.Drowsy);
                InflictStatus(pk, new MoveEffect(Status.Sleep, Stat.None, 100, 0, false, 1), field);
                Console.WriteLine($"{pk.name} fell asleep due to being Drowsy!");
                pk.yawn = false;
            }
        }
        if(pk.statusVol.Contains(Status.Bound) || pk.statusVol.Contains(Status.Vinelash) || pk.statusVol.Contains(Status.Wildfire) || pk.statusVol.Contains(Status.Cannonade))
        {
            double chip = 8.0;
            if (pk.boundCounter == 0) pk.boundCounter = 1;
            if (pk.boundCounter == 4)
            {
                pk.statusVol.Remove(Status.Bound);
                pk.statusVol.Remove(Status.Vinelash);
                pk.statusVol.Remove(Status.Wildfire);
                pk.statusVol.Remove(Status.Cannonade);
                pk.statusVol.Remove(Status.EscapePrevent);
                Console.WriteLine($"{pk.name} is no longer bound!");
                return;
            }
            if (pk.statusVol.Contains(Status.Vinelash) && (pk.species.type1 == Type.Grass || pk.species.type1 == Type.Grass)) return;
            if (pk.statusVol.Contains(Status.Wildfire) && (pk.species.type1 == Type.Fire || pk.species.type1 == Type.Fire)) return;
            if (pk.statusVol.Contains(Status.Cannonade) && (pk.species.type1 == Type.Water || pk.species.type1 == Type.Water)) return;
            if(pk.statusVol.Contains(Status.Vinelash) || pk.statusVol.Contains(Status.Wildfire) || pk.statusVol.Contains(Status.Cannonade)) chip = 6.0;

            int dmg = Convert.ToInt32(Math.Round(pk.maxHP / chip, 0));
            pk.hp -= dmg;
            Console.WriteLine($"{pk.name} is hurt by poison and lost {dmg} HP!");
            pk.boundCounter++;
            if (pk.hp < 0) pk.hp = 0;
        }
        if (field.weather == Weather.Sandstorm)
        {
            if (pk.species.type1 != Type.Rock && pk.species.type2 != Type.Rock && pk.species.type1 != Type.Ground && pk.species.type2 != Type.Ground && pk.species.type1 != Type.Steel && pk.species.type2 != Type.Steel)
            {
                int sandDmg = Convert.ToInt32(Math.Round(pk.maxHP / 16.0, 0));
                pk.hp -= sandDmg;
                Console.WriteLine($"{pk.name} is hurt by the sandstorm and lost {sandDmg} HP!");
                if (pk.hp < 0) pk.hp = 0;
            }

        }
        if(field.terrain == Terrain.Grassy)
        {
            int heal = Convert.ToInt32(Math.Round(pk.maxHP / 16.0, 0));
            pk.hp += heal;
            if (pk.hp > pk.maxHP) pk.hp = pk.maxHP;
            Console.WriteLine($"{pk.name} regained health from the grassy terrain and healed {heal} HP!");
        }
    }
    public static void RunAllPokemonBattles(List<Pokemon> pokemons, int battlesPerPair, int ai)
    {
        double prcnt = 0.0;
        for (int i = 0; i < pokemons.Count; i++)
        {
            prcnt = (i + 1) * 100 / pokemons.Count;
            for (int j = i + 1; j < pokemons.Count; j++)
            {
                for (int k = 0; k < battlesPerPair; k++)
                {
                    Console.Clear();
                    Console.WriteLine($"Battle {k + 1} between {pokemons[i].name} and {pokemons[j].name}");
                    Console.WriteLine($"{prcnt}");
                    PokeBattle(pokemons[i], pokemons[j], ai);
                }
            }
        }
    }
    public static void RunAllTrainerBattles(List<Trainer> trainers, int battlesPerPair, int ai)
    {
        for (int i = 0; i < trainers.Count; i++)
        {
            for (int j = i + 1; j < trainers.Count; j++)
            {
                for (int k = 0; k < battlesPerPair; k++)
                {
                    Console.WriteLine($"Trainer {trainers[i].name} vs Trainer {trainers[j].name} ({k + 1})");

                    TrainerBattle(trainers[i], trainers[j], ai);
                }
            }
        }
    }
    public static void MakePokeListEasy(List<Pokemon> PokemonList, List<Species> AllPokemon, List<MoveB> AllMoves, bool trainer)
    {
        int lvl = 0;
        bool levl = false;
        Console.WriteLine("same level for all pokemon? [input a number for yes else proceed by ENTER]");
        try
        {
            lvl = Convert.ToInt32(Console.ReadLine());
        }
        catch
        {
            levl = true;
        }
        Console.WriteLine("Enter how many you want pokemon if you type \"NO\" the species of the pokemon it will register all previus pokemon");
        int pk = int.MaxValue;
        if (trainer == true)
        {
            pk = 6;
        }
        for (int i = 0; i < pk; i++)
        {
            Console.Write($"Enter the name of the species of Pokemon {i + 1}: ");
            string checkSpecies = Console.ReadLine().ToLower();
            if (checkSpecies == "no") break;
            Species speciesInput = null;
            foreach (Species species in AllPokemon)
            {
                if (species == null) break;
                if (checkSpecies == species.name.ToLower())
                {
                    speciesInput = species;
                    break;
                }
            }
            if (levl == true)
            {
                Console.Write($"Enter the level of Pokemon {i + 1}: ");
                lvl = Convert.ToInt32(Console.ReadLine());
            }
            if (speciesInput != null)
            {
                Pokemon pokemon = new Pokemon(speciesInput, lvl);
                Console.WriteLine("Input atleast one move then type NO when finished");
                for (int j = 0; j < 4; j++)
                {
                    Console.Write($"Enter move name {j + 1}: ");
                    string move = Console.ReadLine().ToLower();
                    if (move == "no" && j > 0) break;
                    MoveB moveInput = null;
                    foreach (MoveB moveb in AllMoves)
                    {
                        if (moveb == null) break;
                        if (move == moveb.name.ToLower())
                        {
                            moveInput = moveb;
                            break;
                        }
                    }
                    if (moveInput == null)
                    {
                        j--;
                    }
                    else
                    {
                        Move addmove = new Move(moveInput);
                        pokemon.AddMove(addmove);
                    }
                }
                PokemonList.Add(pokemon);
            }
            else { i--; }
        }
    }
    public static void MakePokeListADV(List<Pokemon> PokemonList, List<Species> AllPokemon, List<MoveB> AllMoves, List<Item> AllItems, bool trainer)
    {
        int lvl = 0;
        bool levl = false;
        Console.WriteLine("same level for all pokemon? [input a number for yes else proceed by ENTER]");
        try
        {
            lvl = Convert.ToInt32(Console.ReadLine());
        }
        catch
        {
            levl = true;
        }
        Console.WriteLine("Enter how many you want pokemon if you type \"NO\" the species of the pokemon it will register all previus pokemon");
        int pk = int.MaxValue;
        if (trainer == true)
        {
            pk = 6;
        }
        for (int i = 0; i < pk; i++)
        {
            Console.Write($"Enter the name of the species of Pokemon {i + 1}: ");
            string checkSpecies = Console.ReadLine().ToLower();
            if (checkSpecies == "no") break;
            Species speciesInput = null;
            foreach (Species species in AllPokemon)
            {
                if (species == null) break;
                if (checkSpecies == species.name.ToLower())
                {
                    speciesInput = species;
                    break;
                }
            }
            Console.WriteLine($"Emter the nick name of Pokemon {i + 1} (Press 'ENTER' for no nick): ");
            string name = Console.ReadLine();
            Console.WriteLine($"Input gender of pokemon {i + 1} (♂ = True, ♀ = False): ");
            bool gender = Convert.ToBoolean(Console.ReadLine());
            if (levl == true)
            {
                Console.Write($"Enter the level of Pokemon {i + 1}: ");
                lvl = Convert.ToInt32(Console.ReadLine());
            }
            if (speciesInput != null)
            {
                Console.WriteLine($"Input the ability of Pokemon {i + 1} ( 1, 2, 3-hidden): ");
                int ability = Convert.ToInt32(Console.ReadLine());
                string[] statNames = { "HP", "Attack", "Defense", "Sp. Attack", "Sp. Defense", "Speed" };
                int[] IVs = new int[6];
                int[] EVs = new int[6];

                for (int k = 0; k < statNames.Length; k++)
                {
                    Console.Write($"Enter IV for {statNames[k]} (0–31): ");
                    IVs[k] = int.Parse(Console.ReadLine());

                    Console.Write($"Enter EV for {statNames[k]} (0–252): ");
                    EVs[k] = int.Parse(Console.ReadLine());
                }

                if (name == "")
                {
                    name = speciesInput.name;
                }
                if (speciesInput.abilityH == "" && ability == 3) ability = 1;
                else if (speciesInput.ability2 == "" && ability == 2) ability = 1;
                Console.WriteLine($"Input the nature of Pokemon {i + 1}: ");
                string nature = Console.ReadLine().ToLower();

                Console.WriteLine($"Input the item of Pokemon {i + 1} (Press 'ENTER' for no item): ");
                string checkItem = Console.ReadLine().ToLower();
                Item itemInput = null;
                if (checkItem == "")
                {

                    foreach (Item item in AllItems)
                    {
                        if (item == null) break;
                        if (checkItem == item.name.ToLower())
                        {
                            itemInput = item;
                            break;
                        }
                    }
                }

                Console.WriteLine($"INput if the Pokemon {i + 1} can Gmax: ");
                bool gmax = Convert.ToBoolean(Console.ReadLine());
                if (speciesInput.gmax == false) gmax = false;


                Console.WriteLine($"Input the tera type of Pokemon {i + 1}: ");
                string tera = Console.ReadLine().ToLower();
                Pokemon pokemon = new Pokemon(speciesInput, name, gender, lvl, ability, IVs[0], EVs[0], IVs[1], EVs[1], IVs[2], EVs[2], IVs[3], EVs[3], IVs[4], EVs[4], IVs[5], EVs[5], nature, itemInput, gmax, 10, GetTypeId(tera));
                Console.WriteLine("Input atleast one move then type NO when finished");
                for (int j = 0; j < 4; j++)
                {
                    Console.Write($"Enter move name {j + 1}: ");
                    string move = Console.ReadLine().ToLower();
                    if (move == "no" && j > 0) break;
                    MoveB moveInput = null;
                    foreach (MoveB moveb in AllMoves)
                    {
                        if (moveb == null) break;
                        if (move == moveb.name.ToLower())
                        {
                            moveInput = moveb;
                            break;
                        }
                    }
                    if (moveInput == null)
                    {
                        j--;
                    }
                    else
                    {
                        Move addmove = new Move(moveInput);
                        pokemon.AddMove(addmove);
                    }
                }

                PokemonList.Add(pokemon);
            }
            else { i--; }
        }
    }
    public static Pokemon PokePasteDecoder(string input, List<Species> AllPokemon, List<Item> AllItems, List<MoveB> AllMoves)
    {
        Species species = null;
        Item item = null;
        string name = "";
        string nature = "";
        int lvl = 100;
        int ability = 1;
        bool gender = true;
        bool gmax = false;
        int dmaxlvl = 10;
        int HpIV = 0, HpEV = 0, AtkIV = 0, AtkEV = 0, DefIV = 0, DefEV = 0, SpaIV = 0, SpaEV = 0, SpdIV = 0, SpdEV = 0, SpeIV = 0, SpeEV = 0;
        Type tera = Type.Normal;

        foreach (Species s in AllPokemon)
        {
            if (input.Substring(0, input.IndexOf("Ability:")).Contains(s.name))
            {
                if (input.Contains($"({s.name})"))
                {
                    species = s;
                    break;
                }
            }
        }

        if (species == null)
        {
            foreach (Species s in AllPokemon)
            {
                if (input.Substring(0, input.IndexOf("Ability:")).Contains(s.name))
                {
                    species = s;
                    name = species.name;
                    break;
                }
            }
        }
        else
        {
            name = input.Substring(0, input.IndexOf($" ({species.name})"));
        }

        if (input.Contains("(M)"))
        {
            gender = true;
        }
        else if (input.Contains("(F)"))
        {
            gender = true;
        }
        else
        {
            if (Random.Shared.Next(0, 2) == 0)
            {
                gender = true;
            }
            else
            {
                gender = false;
            }
        }

        string itemName = input.Substring(input.IndexOf("@ ") + 2, input.IndexOf("Ability: ") - input.IndexOf("@ ") - 5);

        foreach (Item it in AllItems)
        {
            if (it.name == itemName)
            {
                item = it;
            }
        }

        string abilityName = input.Substring(input.IndexOf("Ability: ") + 9, input.IndexOf("Level: ") - input.IndexOf("Ability: ") - 12);

        if (abilityName == species.ability1) ability = 1;
        else if (abilityName == species.ability2) ability = 2;
        else if (abilityName == species.abilityH) ability = 3;

        lvl = Convert.ToInt32(input.Substring(input.IndexOf("Level: ") + 7, 3));

        if (input.Contains("Tera Type: "))
        {
            string teraType = input.Substring(input.IndexOf("Tera Type: ") + 11, 8);
            string teraType2 = "";
            foreach (char c in teraType)
            {
                if (c == ' ')
                {
                    break;
                }
                else
                {
                    teraType2 += c;
                }
            }
            tera = GetTypeId(teraType2);
        }
        else if (input.Contains("Gigantamax: "))
        {
            if (input.Contains("Dynamax"))
            {
                dmaxlvl = Convert.ToInt32(input.Substring(input.IndexOf("Dynamax Level: ") + 15, 2));
            }
            string isgmax = input.Substring(input.IndexOf("Gigantamax: ") + 12, 3);
            if (isgmax == "Yes") gmax = true;
        }
        else if (input.Contains("Dynamax"))
        {
            dmaxlvl = Convert.ToInt32(input.Substring(input.IndexOf("Dynamax Level: ") + 15, 2));
        }
        string[] natures = {
    "Hardy", "Lonely", "Brave", "Adamant", "Naughty",
    "Bold", "Docile", "Relaxed", "Impish", "Lax",
    "Timid", "Hasty", "Serious", "Jolly", "Naive",
    "Modest", "Mild", "Quiet", "Bashful", "Rash",
    "Calm", "Gentle", "Sassy", "Careful", "Quirky"
            };

        foreach (string ntr in natures)
        {
            if (input.Contains($"{ntr} Nature"))
            {
                nature = ntr;
                break;
            }
        }

        string val;
        if (input.Contains("EVs: "))
        {

            string EvLine = input.Substring(input.IndexOf("EVs: ") + 4, input.IndexOf("IVs: ") - input.IndexOf("EVs: ") - 5);

            if (EvLine.Contains("HP"))
            {
                val = EvLine.Substring(EvLine.IndexOf("HP") - 3, 3);
                try
                {
                    Convert.ToInt32(val);
                }
                catch
                {
                    val = val.Remove(0, 1);
                }
                HpEV = Convert.ToInt32(val);
            }
            if (EvLine.Contains("Atk"))
            {
                val = EvLine.Substring(EvLine.IndexOf("Atk") - 3, 3);
                try
                {
                    Convert.ToInt32(val);
                }
                catch
                {
                    val = val.Remove(0, 1);
                }
                AtkEV = Convert.ToInt32(val);
            }
            if (EvLine.Contains("Def"))
            {
                val = EvLine.Substring(EvLine.IndexOf("Def") - 3, 3);
                try
                {
                    Convert.ToInt32(val);
                }
                catch
                {
                    val = val.Remove(0, 1);
                }
                DefEV = Convert.ToInt32(val);
            }
            if (EvLine.Contains("SpA"))
            {
                val = EvLine.Substring(EvLine.IndexOf("SpA") - 3, 3);
                try
                {
                    Convert.ToInt32(val);
                }
                catch
                {
                    val = val.Remove(0, 1);
                }
                SpaEV = Convert.ToInt32(val);
            }
            if (EvLine.Contains("SpD"))
            {
                val = EvLine.Substring(EvLine.IndexOf("SpD") - 3, 3);
                try
                {
                    Convert.ToInt32(val);
                }
                catch
                {
                    val = val.Remove(0, 1);
                }
                SpdEV = Convert.ToInt32(val);
            }
            if (EvLine.Contains("Spe"))
            {
                val = EvLine.Substring(EvLine.IndexOf("Spe") - 3, 3);
                try
                {
                    Convert.ToInt32(val);
                }
                catch
                {
                    val = val.Remove(0, 1);
                }
                SpeEV = Convert.ToInt32(val);
            }
        }
        if (input.Contains("IVs: "))
        {
            string IvLine = input.Substring(input.IndexOf("IVs: ") + 4, 40);
            if (IvLine.Contains("HP"))
            {
                HpIV = Convert.ToInt32(IvLine.Substring(IvLine.IndexOf("HP") - 3, 2));
            }
            if (IvLine.Contains("Atk"))
            {
                AtkIV = Convert.ToInt32(IvLine.Substring(IvLine.IndexOf("Atk") - 3, 2));
            }
            if (IvLine.Contains("Def"))
            {
                DefIV = Convert.ToInt32(IvLine.Substring(IvLine.IndexOf("Def") - 3, 2));
            }
            if (IvLine.Contains("SpA"))
            {
                SpaIV = Convert.ToInt32(IvLine.Substring(IvLine.IndexOf("SpA") - 3, 2));
            }
            if (IvLine.Contains("SpD"))
            {
                SpdIV = Convert.ToInt32(IvLine.Substring(IvLine.IndexOf("SpD") - 3, 2));
            }
            if (IvLine.Contains("Spe"))
            {
                SpeIV = Convert.ToInt32(IvLine.Substring(IvLine.IndexOf("Spe") - 3, 2));
            }
        }

        Pokemon pokemon = new Pokemon(species, name, gender, lvl, ability, HpIV, HpEV, AtkIV, AtkEV, DefIV, DefEV, SpaIV, SpaEV, SpdIV, SpdEV, SpeIV, SpeEV, nature, item, gmax, dmaxlvl, tera);
        int h = 0;
        List<MoveB> tempAllMoves = AllMoves;
        foreach (MoveB move in tempAllMoves)
        {
            if (input.Contains(move.name) && !pokemon.CheckForMove(move))
            {
                Move mv = new Move(move);
                pokemon.AddMove(mv);
                h++;
                if (h == 4)
                {
                    break;
                }
            }
        }
        return pokemon;
    }
    public static Species FetchMon(string pk)
    {
        foreach (Species s in AllPokemon)
        {
            if (s != null && s.name == pk)
            {
                return s;
            }
        }
        return null;
    }
    public static MoveB FetchMove(string mv)
    {
        foreach (MoveB m in AllMoves)
        {
            if (m != null && m.name == mv)
            {
                return m;
            }
        }
        return null;
    }
    public static void Main()
    {
        List<Item> AllItems = new List<Item>();
        Console.WriteLine("Trainer or pokemon");
        Console.WriteLine("[1] Pokemon");
        Console.WriteLine("[2] Trainer");
        Console.WriteLine("[3] Presets");
        int choice = Convert.ToInt32(Console.ReadLine());
        if (choice == 1)
        {
            List<Pokemon> PokemonList = new List<Pokemon>();
            Console.WriteLine("Easy(species and level are user input and else is randomized no items) or Advanced(all features)");
            Console.WriteLine("[1] Easy");
            Console.WriteLine("[2] Advanced");
            Console.WriteLine("[3] PokePaste");
            choice = Convert.ToInt32(Console.ReadLine());
            if (choice == 1)
            {
                MakePokeListEasy(PokemonList, AllPokemon, AllMoves, false);
            }
            else if (choice == 2)
            {
                MakePokeListADV(PokemonList, AllPokemon, AllMoves, AllItems, false);
            }
            else if (choice == 3)
            {
                for (int i = 0; ; i++)
                {
                    Console.WriteLine("Enter PokePaste or 'STOP' to stop");
                    string input = Console.ReadLine();
                    if (input == "STOP") break;
                    PokemonList.Add(PokePasteDecoder(input, AllPokemon, AllItems, AllMoves));
                }
            }
            Console.WriteLine("Enter how many battles you want per pair of pokemon");
            int battlesPerPair = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("how strong is the ai");
            Console.WriteLine("[1] Random");
            Console.WriteLine("[2] Normal(avg route trainer)");
            Console.WriteLine("[3] Smart(Gym/E4/Champion)");
            Console.WriteLine("[4] Perfect(near human)");
            int aiLevel = Convert.ToInt32(Console.ReadLine());
            RunAllPokemonBattles(PokemonList, battlesPerPair, aiLevel);
            foreach (Pokemon pk in PokemonList)
            {
                Console.WriteLine($"{pk.name} has {pk.wins} wins");
            }
        }
        else if (choice == 2)
        {
            List<Trainer> trainers = new List<Trainer>();
            Console.WriteLine("Easy(species and level are user input and else is randomized no items) or Advanced(all features)");
            Console.WriteLine("[1] Easy");
            Console.WriteLine("[2] Advanced");
            Console.WriteLine("[3] PokePaste");
            choice = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Should trainer save its ace for last (Y/N)");
            string aceLast = Console.ReadLine().ToLower();
            bool ace = false;
            if (aceLast == "y")
            {
                ace = true;
            }
            if (choice == 1)
            {
                for (int i = 0; true; i++)
                {
                    Console.WriteLine($"Enter Trainer {i + 1}. name ('NO' to stop): ");
                    string trainerName = Console.ReadLine();
                    if (trainerName.ToLower() == "no") break;
                    Trainer trainer = new Trainer(trainerName, ace);
                    MakePokeListEasy(trainer.team, AllPokemon, AllMoves, true);
                    trainers.Add(trainer);
                }
            }
            else if (choice == 2)
            {
                for (int i = 0; true; i++)
                {
                    Console.WriteLine($"Enter Trainer {i + 1}. name ('NO' to stop): ");
                    string trainerName = Console.ReadLine();
                    if (trainerName.ToLower() == "no") break;
                    Trainer trainer = new Trainer(trainerName, ace);
                    MakePokeListADV(trainer.team, AllPokemon, AllMoves, AllItems, true);
                    trainers.Add(trainer);
                }
            }
            else if (choice == 3)
            {
                for (int i = 0; true; i++)
                {
                    Console.WriteLine($"Enter Trainer {i + 1}. name ('NO' to stop): ");
                    string trainerName = Console.ReadLine();
                    if (trainerName.ToLower() == "no") break;
                    Trainer trainer = new Trainer(trainerName, ace);
                    for (int j = 0; j < 6; j++)
                    {
                        Console.WriteLine("Enter PokePaste or 'STOP' to stop");
                        string input = Console.ReadLine();
                        if (input == "STOP") break;
                        trainer.AddPokemon(PokePasteDecoder(input, AllPokemon, AllItems, AllMoves));
                    }
                    trainers.Add(trainer);
                }
            }
            Console.WriteLine("Enter how many battles you want per pair of pokemon");
            int battlesPerPair = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("how strong is the ai");
            Console.WriteLine("[1] Random");
            Console.WriteLine("[2] Normal(avg route trainer)");
            Console.WriteLine("[3] Smart(Gym/E4/Champion)");
            Console.WriteLine("[4] Perfect(near human)");
            int aiLevel = Convert.ToInt32(Console.ReadLine());
            RunAllTrainerBattles(trainers, battlesPerPair, aiLevel);
            foreach (Trainer tr in trainers)
            {
                Console.WriteLine($"{tr.name} has {tr.wins} wins");
            }
        }
        else if (choice == 3)
        {
            Console.WriteLine("Enter the name of the preset you want to use");
            string presetName = Console.ReadLine();
            if (presetName == "All")
            {
                List<Pokemon> PokemonList = new List<Pokemon>();
                int g = 0;
                List<int> invalidMon = new List<int>
                {
                    0,1106,1151,1152
                };
                foreach (Species s in AllPokemon)
                {
                    g++;
                    if ((!invalidMon.Contains(g)) && g < 1195)
                    {
                        Pokemon temp = new Pokemon(AllPokemon[g], 50);
                        if (s.Atk > s.Spa)
                        {
                            MoveB a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                        }
                        else if (s.Atk < s.Spa)
                        {
                            MoveB b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                                b1 = new Move(b);
                                temp.AddMove(b1);
                            }
                        }
                        else
                        {
                            MoveB a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                            MoveB b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                                b1 = new Move(b);
                                temp.AddMove(b1);
                            }
                        }
                        PokemonList.Add(temp);
                    }
                }

                Console.WriteLine("Enter how many battles you want per pair of pokemon");
                int battlesPerPair = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("how strong is the ai");
                Console.WriteLine("[1] Random");
                Console.WriteLine("[2] Normal(avg route trainer)");
                Console.WriteLine("[3] Smart(Gym/E4/Champion)");
                Console.WriteLine("[4] Perfect(near human)");
                int aiLevel = Convert.ToInt32(Console.ReadLine());
                RunAllPokemonBattles(PokemonList, battlesPerPair, aiLevel);
                Console.Beep(1000, 1000);
                foreach (Pokemon p in PokemonList.OrderByDescending(p => p.wins))
                {
                    Console.WriteLine($"{p.name} has achived {p.wins} wins!");
                }
            }
            else if (presetName == "Lion")
            {
                // Species species = new Species("Lion", 1, 0, 86, 109, 72, 68, 66, 106, "Rivalry", "Unnerve", "Moxie", false, 50, false, false);
                Species species = new Species("Lion", Type.Normal, 0, 62, 73, 58, 50, 54, 72, "Rivalry", "Unnerve", "Moxie", false, 50, false, false);
                Pokemon Lion = new Pokemon(species, 50);
                MoveB physical = new MoveB("Physical", 0, 60, Split.Physical, 100, 100, 0, true, false, null);
                Move physMove = new Move(physical);
                Lion.AddMove(physMove);
                List<Pokemon> PokemonList = new List<Pokemon>();
                int g = 0;
                List<int> invalidMon = new List<int>
                {
                    0,1106,1151,1152
                };
                foreach (Species s in AllPokemon)
                {
                    g++;
                    if ((!invalidMon.Contains(g)) && g < 1158)
                    {
                        Pokemon temp = new Pokemon(AllPokemon[g], 50);
                        if (s.Atk > s.Spa)
                        {
                            MoveB a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                        }
                        else if (s.Atk < s.Spa)
                        {
                            MoveB b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                                b1 = new Move(b);
                                temp.AddMove(b1);
                            }
                        }
                        else
                        {
                            MoveB a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, Split.Physical, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                            MoveB b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, Split.Special, 100, 100, 0, false, false, null);
                                b1 = new Move(b);
                                temp.AddMove(b1);
                            }
                        }
                        PokemonList.Add(temp);

                    }
                }

                Console.WriteLine("how strong is the ai");
                Console.WriteLine("[1] Random");
                Console.WriteLine("[2] Normal(avg route trainer)");
                Console.WriteLine("[3] Smart(Gym/E4/Champion)");
                Console.WriteLine("[4] Perfect(near human)");
                int aiLevel = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < 1000000000; i++)
                {
                    PokeBattle(PokemonList[0], Lion, aiLevel);

                    if (Lion.hp > 0)
                    {
                        Lion.Heal();
                        i--;
                        try
                        {
                            PokemonList.RemoveAt(0);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else
                    {
                        Lion.Heal();
                    }
                    if (PokemonList.Count() != 0)
                    {
                        Console.Clear();
                        Console.WriteLine($"{1000000000 - i} Lions are remaining. Current pokemon battler: {PokemonList[0].name}");
                    }
                    else break;
                }
                if (PokemonList.Count() == 0) Console.WriteLine("Lion won");
                else Console.WriteLine("Pokemon won");

                Console.WriteLine("Lion in progress");
            }
            else if (presetName == "Test Trainer")
            {
                List<Trainer> trainers = new List<Trainer>();
                Trainer trainer1 = new Trainer("Cynthia", true);
                Trainer trainer2 = new Trainer("Leon", true);

                Pokemon pk = new Pokemon(AllPokemon[407], 50);
                pk.AddMove(new Move(FetchMove("Energy Ball")));
                trainer1.AddPokemon(pk);
                pk.PokeInfo();

                pk = new Pokemon(AllPokemon[445], 50);
                pk.AddMove(new Move(FetchMove("Dragon Claw")));
                pk.AddMove(new Move(FetchMove("Earthquake")));
                pk.heldItem = new Item("Garchompite", "01&01&30", true, false, Type.None);
                trainer1.AddPokemon(pk);
                pk.PokeInfo();

                pk = new Pokemon(AllPokemon[681], 50);
                pk.AddMove(new Move(FetchMove("Iron Head")));
                trainer2.AddPokemon(pk);
                pk.PokeInfo();

                pk = new Pokemon(AllPokemon[6], 50);
                pk.AddMove(new Move(FetchMove("Flamethrower")));
                pk.AddMove(new Move(FetchMove("Air Slash")));
                pk.gmax = true;
                trainer2.AddPokemon(pk);
                pk.PokeInfo();


                trainers.Add(trainer1);
                trainers.Add(trainer2);


                Console.WriteLine(trainers[0].team[0].name);
                Console.WriteLine(trainers[0].team[0].species.type1);
                Console.WriteLine(trainers[0].team[0].species.type2);

                Console.WriteLine(trainers[0].team[1].name);
                Console.WriteLine(trainers[0].team[1].species.type1);
                Console.WriteLine(trainers[0].team[1].species.type2);

                Console.WriteLine(trainers[1].team[0].name);
                Console.WriteLine(trainers[1].team[0].species.type1);
                Console.WriteLine(trainers[1].team[0].species.type2);

                Console.WriteLine(trainers[1].team[1].name);
                Console.WriteLine(trainers[1].team[1].species.type1);
                Console.WriteLine(trainers[1].team[1].species.type2);

                Console.WriteLine("Enter how many battles you want per pair of pokemon");
                int battlesPerPair = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("how strong is the ai");
                Console.WriteLine("[1] Random");
                Console.WriteLine("[2] Normal(avg route trainer)");
                Console.WriteLine("[3] Smart(Gym/E4/Champion)");
                Console.WriteLine("[4] Perfect(near human)");
                int aiLevel = Convert.ToInt32(Console.ReadLine());
                RunAllTrainerBattles(trainers, battlesPerPair, aiLevel);
                foreach (Trainer tr in trainers)
                {
                    Console.WriteLine($"{tr.name} has {tr.wins} wins");
                }
            }

        }
    }
}