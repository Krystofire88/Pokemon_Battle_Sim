using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata;
using System.Net;

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
    Infatuation = 8
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
        if (ai == 1) return active;
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
    public int type1 { get; }
    public int type2 { get; }
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
    public Species(string name, int type1, int type2, int Hp, int Atk, int Def, int Spa, int Spd, int Spe, string ability1, string ability2, string abilityH, bool noRatio, int ratio, bool mega, bool gmax)
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
    bool gender;
    public int level { get; }
    public int maxHP { get; private set; }
    public int hp;
    int ability;
    public Status statusVol { get; set; } = Status.None;
    public List<Status> statusNov { get; set; }
    public int HpIV, HpEV, AtkIV, AtkEV, DefIV, DefEV, SpaIV, SpaEV, SpdIV, SpdEV, SpeIV, SpeEV;
    public int AtkMod, DefMod, SpaMod, SpdMod, SpeMod, AccMod, EvaMod;
    string nature;
    public Item heldItem { get; set; }
    public bool gmax { get; set; }
    int dMaxLevel;
    public bool isDmax { get; set; } = false;
    public int tera { get; }
    public bool terastallized { get; set; } = false;
    public int dMaxTimer { get; set; } = 0;
    public int sleepTimer { get; set; } = 0;
    public int confusionTimer { get; set; } = 0;
    public int toxicCounter { get; set; } = 0;
    public Move[] moveSet = new Move[4];
    public int moveNum { get; private set; } = 0;
    public int wins { get; set; }
    public Pokemon(Species species, string name, bool gender, int level, int ability, int HpIV, int HpEV, int AtkIV, int AtkEV, int DefIV, int DefEV, int SpaIV, int SpaEV, int SpdIV, int SpdEV, int SpeIV, int SpeEV, string nature, Item heldItem, bool gmax, int dMaxLevel, int tera)
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
        Random rnd = new Random();
        int gender = rnd.Next(1, 101);
        bool g = false;
        if (gender <= species.ratio)
        {
            g = true;
        }
        this.gender = g;
        int ability = 1;
        if (species.ability2 != "")
        {
            ability = rnd.Next(1, 3);
        }
        this.ability = ability;
        this.level = level;
        this.HpIV = rnd.Next(0, 32);
        this.HpEV = 0;
        this.AtkIV = rnd.Next(0, 32);
        this.AtkEV = 0;
        this.DefIV = rnd.Next(0, 32);
        this.DefEV = 0;
        this.SpaIV = rnd.Next(0, 32);
        this.SpaEV = 0;
        this.SpdIV = rnd.Next(0, 32);
        this.SpdEV = 0;
        this.SpeIV = rnd.Next(0, 32);
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

        string n = natures[rnd.Next(0, 25)];
        this.nature = n;
        this.heldItem = null;
        bool gMax = false;
        if (0 == rnd.Next(0, 101))
        {
            gMax = true;
        }
        this.gmax = gMax;
        this.dMaxLevel = 10;
        int typ = species.type1;
        if (species.type2 != 0)
        {
            if (rnd.Next(0, 2) == 1) typ = species.type2;
        }
        this.tera = typ;
        this.maxHP = CalcHp();
        this.hp = maxHP;
        this.wins = 0;
    }
    public void AddMove(Move move)
    {
        if (moveNum < 4)
        {
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
        statusVol = 0;
        statusNov.Clear();
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
    public string GetType(int type)
    {
        return type switch
        {
            0 => "",
            1 => "Normal",
            2 => "Fire",
            3 => "Water",
            4 => "Electric",
            5 => "Grass",
            6 => "Ice",
            7 => "Fighting",
            8 => "Poison",
            9 => "Ground",
            10 => "Flying",
            11 => "Psychic",
            12 => "Bug",
            13 => "Rock",
            14 => "Ghost",
            15 => "Dragon",
            16 => "Dark",
            17 => "Steel",
            18 => "Fairy",
            19 => "Stellar",
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
        if (isDmax)
        {
            for (int i = 0; i < moveNum; i++)
            {
                // currentMoveSet[i].FetchMaxMove();
            }
        }
        if (moveNum == 1) return currentMoveSet[0];
        Random rnd = new Random();
        int move = 0;
        if (ai == 1)
        {
            move = rnd.Next(0, moveNum);
        }
        else if (ai == 4)
        {
            double highestScore = 0;
            for (int i = 0; i < moveNum; i++)
            {
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
        Random rnd = new Random();
        if (statusVol == Status.Sleep)
        {
            if (sleepTimer > 0)
            {
                sleepTimer--;
                Console.WriteLine($"{species.name} is asleep and can't move!");
                return false;
            }
            else
            {
                statusVol = Status.None;
                Console.WriteLine($"{species.name} woke up!");
                return true;
            }
        }
        else if (statusVol == Status.Freeze)
        {
            int thaw = rnd.Next(0, 5);
            if (thaw == 0)
            {
                statusVol = Status.None;
                Console.WriteLine($"{species.name} thawed out!");
                return true;
            }
            else
            {
                Console.WriteLine($"{species.name} is frozen solid and can't move!");
                return false;
            }
        }
        else if (statusVol == Status.Paralysis)
        {
                int chance = rnd.Next(0, 4);
                if (chance == 0)
                {
                    Console.WriteLine($"{species.name} is paralyzed and can't move!");
                    return false;
                }
                else
                {
                    return true;
                }
        }
        else
        {
            foreach (Status statusN in statusNov)
            {
                switch (statusN)
                {
                    case Status.Confusion:
                        if (confusionTimer > 0)
                        {
                            confusionTimer--;
                            int check = rnd.Next(0, 3);
                            if (check == 0)
                            {
                                Console.WriteLine($"{species.name} is confused and hurt itself in its confusion!");
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
                            statusNov.Remove(Status.Confusion);
                            Console.WriteLine($"{species.name} snapped out of its confusion!");
                            return true;
                        }
                    case
                    Status.Infatuation:
                        int infatuation = rnd.Next(0, 2);
                        if (infatuation == 0)
                        {
                            Console.WriteLine($"{species.name} is immobilized by love!");
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
        string json = File.ReadAllText("AllPokemon.json");
        List<Species> AllPokemon = JsonSerializer.Deserialize<List<Species>>(json);

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
        string json = File.ReadAllText("AllPokemon.json");
        List<Species> AllPokemon = JsonSerializer.Deserialize<List<Species>>(json);
        foreach (Species s in AllPokemon.Skip(AllPokemon.Count - 75))
        {
            if (species.name == s.name.Replace("-Mega", ""))
            {
                Console.WriteLine($"{species.name} mega evolved into Mega {species.name}");
                species = s;
                break;
            }
            else if (species.name == "Charizard" && s.name == "Charizard-Mega-X" || species.name == "Mewtwo" && s.name == "Mewtwo-Mega-X")
            {
                if (species.name == "Charizard")
                    Console.Write($"Charizard mega evolved into {s.name} X");
                else
                    Console.Write($"Mewtwo mega evolved into {s.name} X");
                species = s;
                break;
            }
            else if (species.name == "Charizard" && s.name == "Charizard-Mega-Y" || species.name == "Mewtwo" && s.name == "Mewtwo-Mega-Y")
            {
                if (species.name == "Charizard")
                    Console.Write($"Charizard mega evolved into {s.name} Y");
                else
                    Console.Write($"Mewtwo mega evolved into {s.name} Y");
                species = s;
                break;
            }
        }
    }
    public void UnMegaEvolve()
    {
        if (!species.name.Contains("-Mega") && !species.name.Contains("-Mega-Y") && !species.name.Contains("-Mega-X")) return;

        string json = File.ReadAllText("AllPokemon.json");
        List<Species> AllPokemon = JsonSerializer.Deserialize<List<Species>>(json);
        Species sp = this.species;
        if (species.name == "Charizard-Mega-X" || species.name == "Charizard-Mega-Y")
        {
            species = AllPokemon.Find(sp => sp.name == "Charizard");
        }
        else if (species.name == "Mewtwo-Mega-X" || species.name == "Mewtwo-Mega-Y")
        {
            species = AllPokemon.Find(sp => sp.name == "Mewtwo");
        }
        else
        {
            foreach (Species s in AllPokemon)
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
    public string FetchZmoveName(int typeId)
    {
        return typeId switch
        {
            1 => "Breakneck Blitz",
            2 => "Inferno Overdrive",
            3 => "Hydro Vortex",
            4 => "Gigavolt Havoc",
            5 => "Bloom Doom",
            6 => "Subzero Slammer",
            7 => "All-Out Pummeling",
            8 => "Acid Downpour",
            9 => "Tectonic Rage",
            10 => "Supersonic Skystrike",
            11 => "Shattered Psyche",
            12 => "Savage Spin-Out",
            13 => "Continental Crush",
            14 => "Never-Ending Nightmare",
            15 => "Devastating Drake",
            16 => "Black Hole Eclipse",
            17 => "Corkscrew Crash",
            18 => "Twinkle Tackle",
            _ => "Unknown Z-Move"
        };
    }
    public static int ConvertToZMovePower(int basePower)
    {
        if (basePower <= 55) return 100;
        if (basePower >= 60 && basePower <= 65) return 120;
        if (basePower >= 70 && basePower <= 75) return 140;
        if (basePower >= 80 && basePower <= 85) return 160;
        if (basePower >= 90 && basePower <= 95) return 175;
        if (basePower == 100) return 180;
        if (basePower == 110) return 185;
        if (basePower >= 120 && basePower <= 125) return 190;
        if (basePower == 130) return 195;
        if (basePower >= 140) return 200;

        return 0;
    }
    public void ZMove(int slot)
    {

    }
    public void Dmax()
    {
        string json = File.ReadAllText("AllPokemon.json");
        List<Species> AllPokemon = JsonSerializer.Deserialize<List<Species>>(json);
        if (isDmax == false)
        {
            dMaxTimer = 3;
            double dmaxHpCoef = 1.50 + dMaxLevel * 0.05;
            isDmax = true;
            maxHP = (int)Math.Ceiling(maxHP * dmaxHpCoef);
            hp = (int)Math.Ceiling(hp * dmaxHpCoef);
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
    public int type { get; }
    public int power { get; }
    public int split { get; }
    public int acc { get; }
    public int maxPP { get; }
    public int priority { get; } = 0;
    public bool contact { get; }
    public bool protect { get; }
    public List<MoveEffect> effectList { get; private set; }
    public MoveB(string name, int type, int power, int split, int acc, int maxPP, int priority, bool contact, bool protect, List<MoveEffect> effectList)
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
    public MoveEffect(Status effectStatus, Stat effectStat, int effectChance, int effectPower)
    {
        this.effectStatus = effectStatus;
        this.effectStat = effectStat;
        this.effectChance = effectChance;
        this.effectPower = effectPower;
    }
}
public class Move
{
    public MoveB moveB { get; set; }
    public int PP { get; set; }
    public Move(MoveB moveB)
    {
        this.moveB = moveB;
        this.PP = moveB.maxPP;
    }

}
public class Item
{
    public string name { get; }
    string effect;
    bool mega;
    public Item(string name, string effect, bool mega)
    {
        this.name = name;
        this.effect = effect;
        this.mega = mega;
    }
}
public static class Program
{
    public static int GetTypeId(string typeName)
    {
        return typeName.ToLower() switch
        {
            "" => 0,
            "normal" => 1,
            "fire" => 2,
            "water" => 3,
            "electric" => 4,
            "grass" => 5,
            "ice" => 6,
            "fighting" => 7,
            "poison" => 8,
            "ground" => 9,
            "flying" => 10,
            "psychic" => 11,
            "bug" => 12,
            "rock" => 13,
            "ghost" => 14,
            "dragon" => 15,
            "dark" => 16,
            "steel" => 17,
            "fairy" => 18,
            _ => -1
        };
    }
    public static double MatchUp(int attackType, int defenseType)
    {
        if (defenseType == 0) return 1.0;

        double eff = 1.0;

        switch (attackType)
        {
            case 1:
                //Normal
                if (defenseType == 13 || defenseType == 17) eff = 0.5; //vs Rock, Steel
                else if (defenseType == 14) eff = 0.0;// vs Ghost
                break;

            case 2:
                // Fire
                if (defenseType == 5 || defenseType == 6 || defenseType == 12 || defenseType == 17) eff = 2.0; //vs Grass, Ice, Bug, Steel
                else if (defenseType == 2 || defenseType == 3 || defenseType == 13 || defenseType == 15) eff = 0.5; //vs Fire, Water, Rock, Dragon
                break;

            case 3:
                // Water
                if (defenseType == 2 || defenseType == 9 || defenseType == 13) eff = 2.0;// vs Fire, Ground, Rock
                else if (defenseType == 3 || defenseType == 5 || defenseType == 15) eff = 0.5;// vs Water, Grass, Dragon
                break;

            case 4:
                // Electric
                if (defenseType == 3 || defenseType == 10) eff = 2.0; //vs Water, Flying
                else if (defenseType == 4 || defenseType == 5 || defenseType == 15) eff = 0.5;// vs Electric, Grass, Dragon
                else if (defenseType == 9) eff = 0.0;// vs Ground
                break;

            case 5:
                // Grass
                if (defenseType == 3 || defenseType == 9 || defenseType == 13) eff = 2.0;// vs Water, Ground, Rock
                else if (defenseType == 2 || defenseType == 5 || defenseType == 8 || defenseType == 10 || defenseType == 12 || defenseType == 15 || defenseType == 17) eff = 0.5; //vs Fire, Grass, Poison, Flying, Bug, Dragon, Steel
                break;

            case 6:
                // Ice
                if (defenseType == 5 || defenseType == 9 || defenseType == 10 || defenseType == 15) eff = 2.0; //vs Grass, Ground, Flying, Dragon
                else if (defenseType == 2 || defenseType == 3 || defenseType == 6 || defenseType == 17) eff = 0.5;// vs Fire, Water, Ice, Steel
                break;

            case 7:
                //   Fighting
                if (defenseType == 1 || defenseType == 6 || defenseType == 13 || defenseType == 16 || defenseType == 17) eff = 2.0; //vs Normal, Ice, Rock, Dark, Steel
                else if (defenseType == 8 || defenseType == 10 || defenseType == 11 || defenseType == 12 || defenseType == 18) eff = 0.5; //vs Poison, Flying, Psychic, Bug, Fairy
                else if (defenseType == 14) eff = 0.0; //vs Ghost
                break;

            case 8:
                //  Poison
                if (defenseType == 5 || defenseType == 18) eff = 2.0;// vs Grass, Fairy
                else if (defenseType == 8 || defenseType == 9 || defenseType == 13 || defenseType == 14) eff = 0.5; //vs Poison, Ground, Rock, Ghost
                else if (defenseType == 17) eff = 0.0; //vs Steel
                break;

            case 9:
                //Ground
                if (defenseType == 2 || defenseType == 4 || defenseType == 8 || defenseType == 13 || defenseType == 17) eff = 2.0;// vs Fire, Electric, Poison, Rock, Steel
                else if (defenseType == 5 || defenseType == 12) eff = 0.5; //vs Grass, Bug
                else if (defenseType == 10) eff = 0.0; //vs Flying
                break;

            case 10:
                // Flying
                if (defenseType == 5 || defenseType == 7 || defenseType == 12) eff = 2.0; //vs Grass, Fighting, Bug
                else if (defenseType == 4 || defenseType == 13 || defenseType == 17) eff = 0.5; //vs Electric, Rock, Steel
                break;

            case 11:
                //Psychic
                if (defenseType == 7 || defenseType == 8) eff = 2.0;// vs Fighting, Poison
                else if (defenseType == 11 || defenseType == 17) eff = 0.5;// vs Psychic, Steel
                else if (defenseType == 16) eff = 0.0;// vs Dark
                break;

            case 12:
                //  Bug
                if (defenseType == 5 || defenseType == 11 || defenseType == 16) eff = 2.0;// vs Grass, Psychic, Dark
                else if (defenseType == 2 || defenseType == 7 || defenseType == 8 || defenseType == 10 || defenseType == 14 || defenseType == 17 || defenseType == 18) eff = 0.5; //vs Fire, Fighting, Poison, Flying, Ghost, Steel, Fairy
                break;

            case 13:
                //  Rock
                if (defenseType == 2 || defenseType == 6 || defenseType == 10 || defenseType == 12) eff = 2.0;// vs Fire, Ice, Flying, Bug
                else if (defenseType == 7 || defenseType == 9 || defenseType == 17) eff = 0.5; //vs Fighting, Ground, Steel
                break;

            case 14:
                //  Ghost
                if (defenseType == 11 || defenseType == 14) eff = 2.0; //vs Psychic, Ghost
                else if (defenseType == 16) eff = 0.5;// vs Dark
                else if (defenseType == 1) eff = 0.0; //vs Normal
                break;

            case 15:
                // Dragon
                if (defenseType == 15) eff = 2.0;// vs Dragon
                else if (defenseType == 17) eff = 0.5; //vs Steel
                else if (defenseType == 18) eff = 0.0; //vs Fairy
                break;

            case 16:
                // Dark
                if (defenseType == 11 || defenseType == 14) eff = 2.0;// vs Psychic, Ghost
                else if (defenseType == 7 || defenseType == 16 || defenseType == 18) eff = 0.5;// vs Fighting, Dark, Fairy
                break;

            case 17:
                //Steel
                if (defenseType == 6 || defenseType == 13 || defenseType == 18) eff = 2.0; //vs Ice, Rock, Fairy
                else if (defenseType == 2 || defenseType == 3 || defenseType == 4 || defenseType == 17) eff = 0.5; //vs Fire, Water, Electric, Steel
                break;

            case 18:
                //Fairy
                if (defenseType == 7 || defenseType == 15 || defenseType == 16) eff = 2.0; //vs Fighting, Dragon, Dark
                else if (defenseType == 2 || defenseType == 8 || defenseType == 17) eff = 0.5; //vs Fire, Poison, Steel
                break;
        }

        return eff;
    }
    public static int FindBestMove(Pokemon atk, Pokemon opp)
    {
        int HighestEffect = 0;
        for (int i = 0; i < atk.moveNum; i++)
        {
            if (HighestEffect < Damage(atk, opp, atk.moveSet[i], atk.moveSet[i].moveB.power, atk.CalcAtkStat(), opp.CalcDefStat() * opp.GetMod(opp.DefMod), 1.0, 23, true))
            {
                HighestEffect = Damage(atk, opp, atk.moveSet[i], atk.moveSet[i].moveB.power, atk.CalcAtkStat(), opp.CalcDefStat() * opp.GetMod(opp.DefMod), 1.0, 23, true);
            }
        }
        return HighestEffect;
    }
    public static bool CheckAcc(Move move, Pokemon pokemonA, Pokemon pokemonD)
    {
        if (move.moveB.acc == 101) return true;

        Random rnd = new Random();
        int check = rnd.Next(1, 101);

        if (check <= (move.moveB.acc * pokemonA.GetMod(pokemonA.AccMod) * pokemonD.GetMod(pokemonD.EvaMod)))
        {
            return true;
        }
        return false;
    }
    public static void Move(Pokemon pokemonA, Pokemon pokemonD, Move move)
    {
        move.PP--;
        if (CheckAcc(move, pokemonA, pokemonD) == true)
        {
            //{
            //    if (move.moveB.split == 3)
            //    {
            //        if (DecodeEffect(move.moveB.effect, 1) == 1) pokemon = pokemonA;
            //        else if (DecodeEffect(move.moveB.effect, 1) == 2) pokemon = pokemonD;
            //        InflictStatus(pokemon, move);
            //    }
            //    else
            //    {
            //        int power = 0;
            //        double atk = 1.00;
            //        double def = 1.00;
            //        double burn = 1.00;
            //        bool status = false;
            //        int rcrit = 23;
            //        switch (DecodeEffect(move.moveB.effect, 1))
            //        {
            //            case 01:
            //                if (move.moveB.split == 1)
            //                {
            //                    if (pokemonA.status == 1) burn = 0.50;
            //                    atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
            //                    def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
            //                }
            //                else
            //                {
            //                    atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
            //                    def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
            //                }
            //                if (DecodeEffect(move.moveB.effect, 3) != 0)
            //                {
            //                    status = true;
            //                    if (DecodeEffect(move.moveB.effect, 3) == 01) pokemon = pokemonA;
            //                    else if (DecodeEffect(move.moveB.effect, 3) != 01) pokemon = pokemonD;
            //                }
            //                power = DecodeEffect(move.moveB.effect, 2);
            //                break;
            //            case 02:
            //                if (move.moveB.split == 1)
            //                {
            //                    if (pokemonA.status == 1) burn = 0.50;
            //                    atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
            //                }
            //                else
            //                {
            //                    atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
            //                }
            //                if (DecodeEffect(move.moveB.effect, 3) != 0)
            //                {
            //                    status = true;
            //                    if (DecodeEffect(move.moveB.effect, 3) == 01) pokemon = pokemonA;
            //                    else if (DecodeEffect(move.moveB.effect, 3) != 01) pokemon = pokemonD;
            //                }
            //                def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
            //                power = DecodeEffect(move.moveB.effect, 2);
            //                break;
            //            case 03:
            //                if (move.moveB.split == 1)
            //                {
            //                    if (pokemonA.status == 1) burn = 0.50;
            //                    atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
            //                }
            //                else
            //                {
            //                    atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
            //                }
            //                if (DecodeEffect(move.moveB.effect, 3) != 0)
            //                {
            //                    status = true;
            //                    if (DecodeEffect(move.moveB.effect, 3) == 01) pokemon = pokemonA;
            //                    else if (DecodeEffect(move.moveB.effect, 3) != 01) pokemon = pokemonD;
            //                }
            //                def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
            //                power = DecodeEffect(move.moveB.effect, 2);
            //                break;
            //            case 04:
            //                if (move.moveB.split == 1)
            //                {
            //                    if (pokemonA.status == 1) burn = 0.50;
            //                    atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
            //                    def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
            //                }
            //                else
            //                {
            //                    atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
            //                    def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
            //                }
            //                if (DecodeEffect(move.moveB.effect, 3) != 0)
            //                {
            //                    status = true;
            //                    if (DecodeEffect(move.moveB.effect, 3) == 01) pokemon = pokemonA;
            //                    else if (DecodeEffect(move.moveB.effect, 3) != 01) pokemon = pokemonD;
            //                }
            //                power = DecodeEffect(move.moveB.effect, 2);
            //                rcrit = 7;
            //                break;
            //            case 05:
            //                if (move.moveB.split == 1)
            //                {
            //                    if (pokemonA.status == 1) burn = 0.50;
            //                    atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
            //                    def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
            //                }
            //                else
            //                {
            //                    atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
            //                    def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
            //                }
            //                if (DecodeEffect(move.moveB.effect, 3) != 0)
            //                {
            //                    status = true;
            //                    if (DecodeEffect(move.moveB.effect, 3) == 01) pokemon = pokemonA;
            //                    else if (DecodeEffect(move.moveB.effect, 3) != 01) pokemon = pokemonD;
            //                }
            //                power = DecodeEffect(move.moveB.effect, 2);
            //                break;
            //        }
            //        pokemonD.hp -= Damage(pokemonA, pokemonD, move, power, atk, def, burn, rcrit, false);
            //        if (pokemonD.hp < 0) pokemonD.hp = 0;
            //        if (status == true) InflictStatus(pokemon, move);
            //    }
        }
        else
        {
            Console.WriteLine("haha you missed");
        }
    }
    public static int Damage(Pokemon pokemonA, Pokemon pokemonD, Move move, int power, double atk, double def, double status, int rcrit, bool test)
    {
        int pkAtype1 = pokemonA.species.type1;
        int pkAtype2 = pokemonA.species.type2;
        int pkDtype1 = pokemonD.species.type1;
        int pkDtype2 = pokemonD.species.type2;

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

        double eff1 = MatchUp(move.moveB.type, pkDtype1);
        double eff2 = MatchUp(move.moveB.type, pkDtype2);

        Random rnd = new Random();
        double crit = 1;
        if (rcrit != 23) rcrit = 7;
        if (rnd.Next(0, rcrit) == 0 && !test)
        {
            crit = 1.5;
            Console.Write("Critical hit!");
        }

        double item = 1.00;
        double ran = rnd.Next(85, 101) / 100.0;
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
    public static void InflictStatus(Pokemon pk, MoveEffect effect)
    {
        if (effect.effectChance < 101)
        { 
            Random rnd = new Random();
            int check = rnd.Next(1, 101);

            if (check <= effect.effectChance)
            {
                if (effect.effectStat != Stat.None)
                { 
                    switch(effect.effectStat)
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
            }
            if (effect.effectStatus != Status.None)
            {
                if (pk.statusVol == Status.None)
                {
                    pk.statusVol = effect.effectStatus;
                }
                else if (!pk.statusNov.Contains(effect.effectStatus))
                {
                    pk.statusNov.Add(effect.effectStatus);
                }
            }
        }
    }
    public static void PokeBattle(Pokemon pokemon1, Pokemon pokemon2, int ai)
    {
        Pokemon currentPokemon1 = pokemon1;
        Pokemon currentPokemon2 = pokemon2;
        Random rnd = new Random();
        currentPokemon1.dMaxTimer = 0;
        currentPokemon2.dMaxTimer = 0;
        while (currentPokemon1.hp > 0 && currentPokemon2.hp > 0)
        {
            Move move1 = null;
            Move move2 = null;
            double spe1 = 0;
            double spe2 = 0;

            if (currentPokemon1.DoIMove())
            {
                move1 = currentPokemon1.PickMove(currentPokemon2, ai);
                double para = 1;
                if (currentPokemon1.statusVol == Status.Paralysis) para = 0.5;
                spe1 = currentPokemon1.CalcSpeStat() * currentPokemon1.GetMod(currentPokemon1.SpeMod) * para;
            }
            if (currentPokemon2.DoIMove())
            {
                move2 = currentPokemon2.PickMove(currentPokemon1, ai);
                double para = 1;
                if (currentPokemon2.statusVol == Status.Paralysis) para = 0.5;
                spe2 = currentPokemon2.CalcSpeStat() * currentPokemon2.GetMod(currentPokemon2.SpeMod) * para;
            }


            int priority1 = 0;
            int priority2 = 0;
            if (move1 != null) priority1 = move1.moveB.priority;
            if (move2 != null) priority2 = move2.moveB.priority;
            if (priority1 > priority2 && move1 != null)
            {
                ExecuteMove(currentPokemon1, currentPokemon2, move1);
                if (move2 != null && currentPokemon2.hp > 0)
                {
                    ExecuteMove(currentPokemon2, currentPokemon1, move2);
                }
            }
            else if (priority1 < priority2 && move2 != null)
            {
                ExecuteMove(currentPokemon2, currentPokemon1, move2);
                if (move1 != null && currentPokemon1.hp > 0)
                {
                    ExecuteMove(currentPokemon1, currentPokemon2, move1);
                }
            }
            else
            {
                if (spe1 > spe2 && move1 != null)
                {
                    ExecuteMove(currentPokemon1, currentPokemon2, move1);
                    if (move2 != null && currentPokemon2.hp > 0)
                    {
                        ExecuteMove(currentPokemon2, currentPokemon1, move2);
                    }
                }
                else if (spe1 < spe2 && move2 != null)
                {
                    ExecuteMove(currentPokemon2, currentPokemon1, move2);
                    if (move1 != null && currentPokemon1.hp > 0)
                    {
                        ExecuteMove(currentPokemon1, currentPokemon2, move1);
                    }
                }
                else
                {
                    int tie = rnd.Next(0, 2);
                    if (tie == 0 && move1 != null)
                    {
                        ExecuteMove(currentPokemon1, currentPokemon2, move1);
                        if (move2 != null && currentPokemon2.hp > 0)
                        {
                            ExecuteMove(currentPokemon2, currentPokemon1, move2);
                        }
                    }
                    else if (tie == 1 && move2 != null)
                    {
                        ExecuteMove(currentPokemon2, currentPokemon1, move2);
                        if (move1 != null && currentPokemon1.hp > 0)
                        {
                            ExecuteMove(currentPokemon1, currentPokemon2, move1);
                        }
                    }
                }
            }

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
        Console.WriteLine($"{team1.name} sent out {currentPokemon1.name}");
        Console.WriteLine($"{team2.name} sent out {currentPokemon2.name}");
        Random rnd = new Random();
        bool gimmick1 = false;
        bool gimmick2 = false;
        while (team1.AbleToBattle() && team2.AbleToBattle())
        {
            Console.WriteLine("\n=next turn=\n");
            Move move1 = null;
            Move move2 = null;
            double spe1 = 0;
            double spe2 = 0;
            if (currentPokemon1.hp <= 0)
            {
                currentPokemon1 = team1.ShouldSwitch(currentPokemon1, currentPokemon2, ai);
                Console.WriteLine($"{team1.name} sent out {currentPokemon1.name}");
                if (gimmick1 == false)
                {
                    PreTurnTrainerCheck(currentPokemon1, team1, ai);
                    if (currentPokemon1.terastallized || currentPokemon1.isDmax || currentPokemon1.species.name.Contains("-Mega"))
                    {
                        gimmick1 = true;
                    }
                }
                if (currentPokemon1.DoIMove())
                {
                    move1 = currentPokemon1.PickMove(currentPokemon2, ai);
                    double para = 1;
                    if (currentPokemon1.statusVol == Status.Paralysis) para = 0.5;
                    spe1 = currentPokemon1.CalcSpeStat() * currentPokemon1.GetMod(currentPokemon1.SpeMod) * para;
                }
            }
            if (currentPokemon2.hp <= 0)
            {
                currentPokemon2 = team2.ShouldSwitch(currentPokemon2, currentPokemon1, ai);
                Console.WriteLine($"{team2.name} sent out {currentPokemon2.name}");
                if (gimmick2 == false)
                {
                    PreTurnTrainerCheck(currentPokemon2, team2, ai);
                    if (currentPokemon2.terastallized || currentPokemon2.isDmax || currentPokemon2.species.name.Contains("-Mega"))
                    {
                        gimmick2 = true;
                    }
                }
                if (currentPokemon2.DoIMove())
                {
                    move2 = currentPokemon2.PickMove(currentPokemon1, ai);
                    double para = 1;
                    if (currentPokemon2.statusVol == Status.Paralysis) para = 0.5;
                    spe2 = currentPokemon2.CalcSpeStat() * currentPokemon2.GetMod(currentPokemon2.SpeMod) * para;
                }
            }

            if (currentPokemon1.hp > 0)
            {
                Pokemon preSwitch1 = currentPokemon1;
                currentPokemon1 = team1.ShouldSwitch(currentPokemon1, currentPokemon2, ai);
                if (preSwitch1 != currentPokemon1)
                {
                    Console.WriteLine($"{team1.name} switched to {currentPokemon1.name}");
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
                    if (currentPokemon1.DoIMove())
                    {
                        move1 = currentPokemon1.PickMove(currentPokemon2, ai);
                        double para = 1;
                        if (currentPokemon1.statusVol == Status.Paralysis) para = 0.5;
                        spe1 = currentPokemon1.CalcSpeStat() * currentPokemon1.GetMod(currentPokemon1.SpeMod) * para;
                    }
                }
            }
            if (currentPokemon2.hp > 0)
            {
                Pokemon preSwitch2 = currentPokemon2;
                currentPokemon2 = team2.ShouldSwitch(currentPokemon2, currentPokemon1, ai);
                if (preSwitch2 != currentPokemon2)
                {
                    Console.WriteLine($"{team2.name} switched to {currentPokemon2.name}");
                }
                else
                {
                    if (gimmick2 == false)
                    {
                        PreTurnTrainerCheck(currentPokemon2, team2, ai);
                        if (currentPokemon2.terastallized || currentPokemon2.isDmax || currentPokemon2.species.name.Contains("-Mega"))
                        {
                            gimmick2 = true;
                        }
                    }
                    if (currentPokemon2.DoIMove())
                    {
                        move2 = currentPokemon2.PickMove(currentPokemon1, ai);
                        double para = 1;
                        if (currentPokemon2.statusVol == Status.Paralysis) para = 0.5;
                        spe2 = currentPokemon2.CalcSpeStat() * currentPokemon2.GetMod(currentPokemon2.SpeMod) * para;
                    }
                }
            }

            int priority1 = 0;
            int priority2 = 0;
            if (move1 != null) priority1 = move1.moveB.priority;
            if (move2 != null) priority2 = move2.moveB.priority;
            if (priority1 > priority2 && move1 != null)
            {
                ExecuteMove(currentPokemon1, currentPokemon2, move1);
                if (move2 != null && currentPokemon2.hp > 0)
                {
                    ExecuteMove(currentPokemon2, currentPokemon1, move2);
                }
            }
            else if (priority1 < priority2 && move2 != null)
            {
                ExecuteMove(currentPokemon2, currentPokemon1, move2);
                if (move1 != null && currentPokemon1.hp > 0)
                {
                    ExecuteMove(currentPokemon1, currentPokemon2, move1);
                }
            }
            else
            {
                if (spe1 > spe2 && move1 != null)
                {
                    ExecuteMove(currentPokemon1, currentPokemon2, move1);
                    if (move2 != null && currentPokemon2.hp > 0)
                    {
                        ExecuteMove(currentPokemon2, currentPokemon1, move2);
                    }
                }
                else if (spe1 < spe2 && move2 != null)
                {
                    ExecuteMove(currentPokemon2, currentPokemon1, move2);
                    if (move1 != null && currentPokemon1.hp > 0)
                    {
                        ExecuteMove(currentPokemon1, currentPokemon2, move1);
                    }
                }
                else
                {
                    int tie = rnd.Next(0, 2);
                    if (tie == 0 && move1 != null)
                    {
                        ExecuteMove(currentPokemon1, currentPokemon2, move1);
                        if (move2 != null && currentPokemon2.hp > 0)
                        {
                            ExecuteMove(currentPokemon2, currentPokemon1, move2);
                        }
                    }
                    else if (tie == 1 && move2 != null)
                    {
                        ExecuteMove(currentPokemon2, currentPokemon1, move2);
                        if (move1 != null && currentPokemon1.hp > 0)
                        {
                            ExecuteMove(currentPokemon1, currentPokemon2, move1);
                        }
                    }
                }
            }

            PostTurnPokemonCheck(currentPokemon1);
            PostTurnPokemonCheck(currentPokemon2);

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
    public static void ExecuteMove(Pokemon atk, Pokemon def, Move move)
    {
        Console.WriteLine($"{atk.name} used {move.moveB.name} against {def.name}");
        Move(atk, def, move);
        Console.WriteLine(def.hp);
        if (def.hp <= 0)
        {
            Console.WriteLine($"{def.name} fainted");
            def.UnDmax();
            def.UnMegaEvolve();
            def.UnTerastallize();
            def.dMaxTimer = 0;
        }
    }
    public static void PreTurnTrainerCheck(Pokemon pk, Trainer tr, int ai)
    {
        if (pk.hp <= 0) return;
        if (ai == 1) return;
        Random rnd = new Random();

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

        if (pk.gmax && pk.species.gmax)
        {
            pk.Dmax();
            return;
        }
        else
        {
            int check = rnd.Next(0, 100);
            if (check < 50)
            {
                pk.Dmax();
                return;
            }
            else
            {
                pk.Terastallize();
                return;
            }
        }
    }
    public static void PreTurnPokemonCheck(Pokemon pk, int ai)
    {

    }
    public static void PostTurnPokemonCheck(Pokemon pk)
    {
        if (pk == null) return;
        if (pk.hp <= 0) return;
        switch (pk.statusVol)
        {
            case Status.Poison:
                int dmg = Convert.ToInt32(Math.Round(pk.CalcHp() / 8.0, 0));
                pk.hp -= dmg;
                Console.WriteLine($"{pk.name} is hurt by poison and lost {dmg} HP!");
                if (pk.hp < 0) pk.hp = 0;
                break;
            case Status.Toxic:
                if (pk.toxicCounter == 0) pk.toxicCounter = 1;
                int toxicDmg = Convert.ToInt32(Math.Round((pk.CalcHp() / 16.0) * pk.toxicCounter, 0));
                pk.hp -= toxicDmg;
                Console.WriteLine($"{pk.name} is hurt by toxic poison and lost {toxicDmg} HP!");
                pk.toxicCounter++;
                if (pk.hp < 0) pk.hp = 0;
                break;
            case Status.Burn:
                int burnDmg = Convert.ToInt32(Math.Round(pk.CalcHp() / 16.0, 0));
                pk.hp -= burnDmg;
                Console.WriteLine($"{pk.name} is hurt by its burn and lost {burnDmg} HP!");
                if (pk.hp < 0) pk.hp = 0;
                break;
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
                    Console.WriteLine($"Battle {k + 1} between {pokemons[i].name} and {pokemons[j].name}");
                    Console.WriteLine($"{prcnt}");
                    PokeBattle(pokemons[i], pokemons[j], ai);
                    Console.Clear();
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
        int tera = 1;

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
            Random rnd = new Random();
            if (rnd.Next(0, 2) == 0)
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
    public static void Main()
    {
        Random rnd = new Random();

        string jsonP = File.ReadAllText("AllPokemon.json");
        List<Species> AllPokemon = JsonSerializer.Deserialize<List<Species>>(jsonP);

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }, 
            PropertyNameCaseInsensitive = true
        };

        string jsonM = File.ReadAllText("AllMoves.json");
        List<MoveB> AllMoves = JsonSerializer.Deserialize<List<MoveB>>(jsonM, options);

        Item LifeOrb = new Item("Life Orb", "01&01&30", false);
        Item ExpertBelt = new Item("Expert Belt", "01&02&10", false);
        Item Galladite = new Item("Galladite", "03&07&00", true);

        List<Item> AllItems = new List<Item>
        {
            LifeOrb,
            ExpertBelt,
            Galladite
        };

        Pokemon Gallade = new Pokemon(AllPokemon[475], "Gallade", true, 69, 2, 31, 8, 31, 252, 31, 0, 31, 0, 31, 0, 31, 252, "Adamant", Galladite, false, 1, 7);
        Pokemon Gardevoir = new Pokemon(AllPokemon[282], "Gardevoir", false, 69, 1, 31, 8, 0, 0, 31, 0, 31, 252, 31, 0, 31, 252, "Modest", ExpertBelt, false, 1, 18);
        Gallade.MegaEvo();
        Console.WriteLine(Gallade.species.name);
        Gallade.UnMegaEvolve();
        Console.WriteLine(Gallade.species.name);

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
                    if ((!invalidMon.Contains(g)) && g < 1158)
                    {
                        Pokemon temp = new Pokemon(AllPokemon[g], 50);
                        if (s.Atk > s.Spa)
                        {
                            MoveB a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                        }
                        else if (s.Atk < s.Spa)
                        {
                            MoveB b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
                                b1 = new Move(b);
                                temp.AddMove(b1);
                            }
                        }
                        else
                        {
                            MoveB a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                            MoveB b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
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
                Species species = new Species("Lion", 1, 0, 86, 109, 72, 68, 66, 109, "Rivalry", "Unnerve", "Moxie", false, 50, false, false);
                Pokemon Lion = new Pokemon(species, 50);
                MoveB physical = new MoveB("Physical", 1, 100, 1, 100, 100, 0, true, false, null);
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
                            MoveB a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                        }
                        else if (s.Atk < s.Spa)
                        {
                            MoveB b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
                                b1 = new Move(b);
                                temp.AddMove(b1);
                            }
                        }
                        else
                        {
                            MoveB a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            if (s.type2 != 0)
                            {
                                a = new MoveB("Physical", s.type2, 60, 2, 100, 100, 0, true, false, null);
                                a1 = new Move(a);
                                temp.AddMove(a1);
                            }
                            MoveB b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                            if (s.type2 != 0)
                            {
                                b = new MoveB("Special", s.type2, 60, 2, 100, 100, 0, false, false, null);
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

                Console.WriteLine("Lion in progress");
            }
            else if (presetName == "Test Trainer")
            {
                List<Trainer> trainers = new List<Trainer>();
                Trainer trainer1 = new Trainer("Cynthia", true);
                Trainer trainer2 = new Trainer("Leon", true);

                Pokemon pk = new Pokemon(AllPokemon[407], 50);
                pk.AddMove(new Move(AllMoves[28]));
                trainer1.AddPokemon(pk);
                pk.PokeInfo();

                pk = new Pokemon(AllPokemon[445], 50);
                pk.AddMove(new Move(AllMoves[47]));
                pk.AddMove(new Move(AllMoves[35]));
                pk.heldItem = new Item("Garchompite", "01&01&30", true);
                trainer1.AddPokemon(pk);
                pk.PokeInfo();

                pk = new Pokemon(AllPokemon[681], 50);
                pk.AddMove(new Move(AllMoves[52]));
                trainer2.AddPokemon(pk);
                pk.PokeInfo();

                pk = new Pokemon(AllPokemon[6], 50);
                pk.AddMove(new Move(AllMoves[22]));
                pk.AddMove(new Move(AllMoves[38]));
                pk.gmax = true;
                trainer2.AddPokemon(pk);
                pk.PokeInfo();


                trainers.Add(trainer1);
                trainers.Add(trainer2);
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
        else if (choice == 69)
        {
            Console.WriteLine("Enter pokemon name to find its dex number");

            string help = Console.ReadLine();
            Species findDexNum = null;
            int pkmn = -2;
            foreach (Species s in AllPokemon)
            {
                pkmn++;
                if (s.name.ToLower() == help.ToLower())
                {
                    findDexNum = s;
                    break;
                }
            }
            if (findDexNum != null)
            {
                int line = 1850;
                int offset = 0;
                if (pkmn < 152) offset = 0;
                else if (pkmn < 252) offset = 1;
                else if (pkmn < 386) offset = 2;
                else if (pkmn < 493) offset = 3;
                else if (pkmn < 649) offset = 4;
                else if (pkmn < 721) offset = 5;
                else if (pkmn < 809) offset = 6;
                else if (pkmn < 897) offset = 7;
                else if (pkmn < 906) offset = 8;
                else if (pkmn < 1026) offset = 9;
                else if (pkmn < 1157) offset = 10;
                else if (pkmn < 1216) offset = 11;

                Console.WriteLine(line + pkmn + offset);
                Console.WriteLine(pkmn + 1);
            }
            else
            {
                Console.WriteLine("no pokemon found :O");
            }
        }
    }
}