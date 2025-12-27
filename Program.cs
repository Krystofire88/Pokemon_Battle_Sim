using System;
using System.Linq;
using System.Collections.Generic;
using Pkmn2;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Text.Json;

namespace Pkmn2;
public class Trainer
{
    public string name { get; }
    int numPoke = 0;
    public List<Pokemon> team = new List<Pokemon>();
    public int wins { get; set; } = 0;
    public Trainer(string name)
    {
        this.name = name;
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
    public bool CanSwitch()
    {
        int i = 0;
        bool can = false;
        foreach (Pokemon p in team)
        {
            if (p.hp > 0)
            {
                i++;
                if (i >= 2)
                {
                    can = true;
                    break;
                }
            }
        }
        return can;
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
    int maxHP;
    public int hp;
    int ability;
    public int status { get; set; }
    public int HpIV, HpEV, AtkIV, AtkEV, DefIV, DefEV, SpaIV, SpaEV, SpdIV, SpdEV, SpeIV, SpeEV;
    public int AtkMod, DefMod, SpaMod, SpdMod, SpeMod, AccMod, EvaMod;
    string nature;
    Item heldItem;
    bool gmax;
    int dmaxLevel;
    int tera;
    int gMAxtimer = 0;
    public Move[] moveSet = new Move[4];
    public int moveNum { get; private set; } = 0;
    public int wins { get; set; }
    public Pokemon(Species species, string name, bool gender, int level, int ability, int status, int HpIV, int HpEV, int AtkIV, int AtkEV, int DefIV, int DefEV, int SpaIV, int SpaEV, int SpdIV, int SpdEV, int SpeIV, int SpeEV, string nature, Item heldItem, bool gmax, int dmaxLevel, int tera)
    {
        this.species = species;
        this.name = name;
        this.gender = gender;
        this.level = level;
        this.ability = ability;
        this.status = status;
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
        this.dmaxLevel = dmaxLevel;
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
        this.status = 0;
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
        this.dmaxLevel = 10;
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
        status = 0;
        ClearMods();
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
    public void MegaEvo(bool meg, List<Species> AllPokemon)
    {
        if (meg == true)
        {
            if (species.mega == true)
            {
                if (CheckStone())
                {
                    MegaEvolve(AllPokemon);
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

    }
    public void MegaEvolve(List<Species> AllPokemon)
    {
        
        foreach(Species s in AllPokemon.Skip(AllPokemon.Count - 75)) 
        {
            if (species.name == s.name.Replace("-Mega", ""))
            {
                species = s;
                break;
            }

        }
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
    public Move ZMove(bool use, int slot)
    {
        if (use == true)
        {
            if (heldItem != null && heldItem.name.EndsWith("ium Z"))
            {
                if (moveNum > 0)
                {
                    Move zmove = moveSet[slot];
                    string ef = "01&";
                    ef = ef + Convert.ToString(ConvertToZMovePower(Program.DecodeEffect(zmove.moveB.effect, 1)));

                    MoveB Zmove = new MoveB(FetchZmoveName(zmove.moveB.type), zmove.moveB.type, zmove.moveB.split, 101, zmove.PP, ef);
                    Move Zmv = new Move(Zmove);


                    Console.WriteLine($"{name} used {zmove.moveB.name} as a Z-Move!");
                    return Zmv;

                }
                else
                {
                    Console.WriteLine($"{name} has no moves to use as a Z-Move.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"{name} cannot use a Z-Move without a valid Z-Crystal.");
                return null;
            }
        }
        else
        {
            return null;
        }

    }
}
public class MoveB
{
    public string name { get; }
    public int type { get; }
    public int split { get; }
    public int acc { get; }
    public int maxPP { get; }
    public string effect { get; }
    public MoveB(string name, int type, int split, int acc, int maxPP, string effect)
    {
        this.name = name;
        this.type = type;
        this.split = split;
        this.acc = acc;
        this.maxPP = maxPP;
        this.effect = effect;
    }
}
public class Move
{
    public MoveB moveB { get; }
    public int PP { get; set; }
    public Move(MoveB moveB)
    {
        this.moveB = moveB;
        this.PP = moveB.maxPP;
    }
    public Move(MoveB moveB, int PP)
    {
        this.moveB = moveB;
        this.PP = PP;
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
    public static string GetType(int type)
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
    public static int DecodeItem(string effect, int number)
    {
        return 1;
    }
    public static int DecodeEffect(string effect, int number)
    {
        string result = "";
        int t = 0;
        foreach (char car in effect)
        {
            if (car != '&' && number == (t + 1))
            {
                result += car;
            }
            else if (car == '&')
            {
                t++;
            }

        }
        if (result == "") result = "00";

        return Convert.ToInt32(result);
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
        Pokemon pokemon = null;
        if (CheckAcc(move, pokemonA, pokemonD) == true)
        {
            if (move.moveB.split == 3)
            {
                if (DecodeEffect(move.moveB.effect, 1) == 1) pokemon = pokemonA;
                else if (DecodeEffect(move.moveB.effect, 1) == 2) pokemon = pokemonD;
                InflictStatus(pokemon, move);
                pokemon.Modifiers();
            }
            else
            {
                int power = 0;
                double atk = 1.00;
                double def = 1.00;
                double burn = 1.00;
                bool status = false;
                int rcrit = 23;
                switch (DecodeEffect(move.moveB.effect, 1))
                {
                    case 01:
                        if (move.moveB.split == 1)
                        {
                            if (pokemonA.status == 1) burn = 0.50;
                            atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
                            def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                        }
                        else
                        {
                            atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
                            def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                        }
                        if (DecodeEffect(move.moveB.effect, 3) != 0)
                        {
                            status = true;
                            if (DecodeEffect(move.moveB.effect, 3) == 01) pokemon = pokemonA;
                            else if (DecodeEffect(move.moveB.effect, 3) != 01) pokemon = pokemonD;


                        }
                        power = DecodeEffect(move.moveB.effect, 2);
                        break;
                    case 02:
                        if (pokemonA.status == 1) burn = 0.50;
                        atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
                        def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                        power = DecodeEffect(move.moveB.effect, 2);
                        break;
                    case 03:
                        atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
                        def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                        power = DecodeEffect(move.moveB.effect, 2);
                        break;
                    case 04:
                        if (move.moveB.split == 1)
                        {
                            if (pokemonA.status == 1) burn = 0.50;
                            atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
                            def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                        }
                        else
                        {
                            atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
                            def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                        }
                        power = DecodeEffect(move.moveB.effect, 2);
                        rcrit = 7;
                        break;
                    case 41:
                        if (move.moveB.split == 1)
                        {
                            if (pokemonA.status == 1) burn = 0.50;
                            atk = (pokemonA.CalcAtkStat() * pokemonA.GetMod(pokemonA.AtkMod));
                            def = (pokemonD.CalcDefStat() * pokemonD.GetMod(pokemonD.DefMod));
                        }
                        else
                        {
                            atk = (pokemonA.CalcSpaStat() * pokemonA.GetMod(pokemonA.SpaMod));
                            def = (pokemonD.CalcSpdStat() * pokemonD.GetMod(pokemonD.SpdMod));
                        }
                        if (DecodeEffect(move.moveB.effect, 3) != 0)
                        {
                            status = true;
                            if (DecodeEffect(move.moveB.effect, 3) == 01) pokemon = pokemonA;
                            else if (DecodeEffect(move.moveB.effect, 3) != 01) pokemon = pokemonD;
                        }
                        power = DecodeEffect(move.moveB.effect, 2);
                        break;
                }
                pokemonD.hp -= Damage(pokemonA, pokemonD, move, power, atk, def, burn, rcrit);
                if (pokemonD.hp < 0) pokemonD.hp = 0;
                if (status == true) InflictStatus(pokemon, move);
            }
        }
        else
        {
            Console.WriteLine("haha you missed");
        }
    }
    public static int Damage(Pokemon pokemonA, Pokemon pokemonD, Move move, int power, double atk, double def, double status, int rcrit)
    {
        double stab = 1;
        if (pokemonA.species.type1 == move.moveB.type || pokemonA.species.type2 == move.moveB.type)
        {
            stab = 1.5;
        }

        double eff1 = MatchUp(move.moveB.type, pokemonD.species.type1);
        double eff2 = MatchUp(move.moveB.type, pokemonD.species.type2);

        Random rnd = new Random();
        double crit = 1;
        if (rcrit != 23) rcrit = 7;
        if (rnd.Next(0, 23) == 0)
        {
            crit = 1.5;
           Console.Write("Critical hit!");
        }
        double item = 1.00;
        double ran = rnd.Next(85, 101) / 100.0;

        int dmg = Convert.ToInt32(Math.Round(((((((((2 * pokemonA.level) / 5) + 2) * power * ((double)atk / def)) / 50) * crit) + 2) * stab * eff1 * eff2 * ran * status * item), 0));// too complicated check https:bulbapedia.bulbagarden.net / wiki / Damage
        if (pokemonD.hp < dmg)
        {
           Console.WriteLine($" It did {pokemonD.hp} damage!");
        }
        else
        {
           Console.WriteLine($" It did {dmg} damage!");
        }

        return dmg;
    }
    public static void InflictStatus(Pokemon pk, Move move)
    {
        Random rnd = new Random();
        int check = rnd.Next(0, 100);
        int abs = 1;
        int a = 5;
        if (move.moveB.split == 3) a -= 3;
        if (check <= DecodeEffect(move.moveB.effect, 4) || move.moveB.split == 3)
        {
            for (int i = a; i < a + 7; i++)
            {
                int effect = Math.Abs(DecodeEffect(move.moveB.effect, i));
                if (DecodeEffect(move.moveB.effect, i) < 0)
                {
                    abs = -1;
                }
                switch (effect)
                {
                    case 01:
                    // burn
                    case 02:
                    //freeze
                    case 03:
                    //paralysis
                    case 04:
                    //poison
                    case 05:
                    //toxic
                    case 06:
                    //sleep
                    case 07:
                    //confusion
                    case 08:
                    //infatuation
                    case 09:
                        pk.status = DecodeEffect(move.moveB.effect, i);
                        break;
                    case 11:
                        if (pk.AtkMod < 6 && pk.AtkMod > -6)
                        {
                            pk.AtkMod += 1 * abs;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 12:
                        if (pk.DefMod < 6 && pk.DefMod > -6)
                        {
                            pk.DefMod += 1 * abs;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 13:
                        if (pk.SpaMod < 6 && pk.SpaMod > -6)
                        {
                            pk.SpaMod += 1 * abs;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 14:
                        if (pk.SpdMod < 6 && pk.SpdMod > -6)
                        {
                            pk.SpdMod += 1 * abs;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 15:
                        if (pk.AccMod < 6 && pk.AccMod > -6)
                        {
                            pk.AccMod += 1 * abs;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 16:
                        if (pk.EvaMod < 6 && pk.EvaMod > -6)
                        {
                            pk.EvaMod += 1 * abs;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 17:
                        if (pk.SpeMod < 6 && pk.SpeMod > -6)
                        {
                            pk.SpeMod += 1 * abs;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 21:
                        if (pk.AtkMod < 6 && pk.AtkMod > -6)
                        {
                            pk.AtkMod += 2 * abs;
                            if (pk.AtkMod > 6) pk.AtkMod = 6;
                            else if (pk.AtkMod < -6) pk.AtkMod = 6;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 22:
                        if (pk.DefMod < 6 && pk.DefMod > -6)
                        {
                            pk.DefMod += 2 * abs;
                            if (pk.DefMod > 6) pk.DefMod = 6;
                            else if (pk.DefMod < -6) pk.DefMod = 6;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 23:
                        if (pk.SpaMod < 6 && pk.SpaMod > -6)
                        {
                            pk.SpaMod += 2 * abs;
                            if (pk.SpaMod > 6) pk.SpaMod = 6;
                            else if (pk.SpaMod < -6) pk.SpaMod = 6;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 24:
                        if (pk.SpdMod < 6 && pk.SpdMod > -6)
                        {
                            pk.SpdMod += 2 * abs;
                            if (pk.SpdMod > 6) pk.SpdMod = 6;
                            else if (pk.SpdMod < -6) pk.SpdMod = 6;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 25:
                        if (pk.AccMod < 6 && pk.AccMod > -6)
                        {
                            pk.AccMod += 2 * abs;
                            if (pk.AccMod > 6) pk.AccMod = 6;
                            else if (pk.AccMod < -6) pk.AccMod = 6;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 26:
                        if (pk.EvaMod < 6 && pk.EvaMod > -6)
                        {
                            pk.EvaMod += 2 * abs;
                            if (pk.EvaMod > 6) pk.EvaMod = 6;
                            else if (pk.EvaMod < -6) pk.EvaMod = 6;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                    case 27:
                        if (pk.SpeMod < 6 && pk.SpeMod > -6)
                        {
                            pk.SpeMod += 2 * abs;
                            if (pk.SpeMod > 6) pk.SpeMod = 6;
                            else if (pk.SpeMod < -6) pk.SpeMod = 6;
                        }
                        else
                        {
                            Console.WriteLine("It cant go higher");
                        }
                        break;
                }
            }
        }
    }
    public static int TurnFirst(Move move)
    {
        int c = DecodeEffect(move.moveB.effect, 1);
        if (c - 40 > 0 && c - 40 < 6)
        {
            return DecodeEffect(move.moveB.effect, 1) - 40;
        }
        else if (c - 50 > 0 && c - 50 < 8)
        {
            return DecodeEffect(move.moveB.effect, 1) - 60;
        }
        else return 0;
    }
    public static void Battle(Trainer team1, Trainer team2, bool trainer, int ai)
    {
        Pokemon currentPokemon1 = team1.team[0];
        Pokemon currentPokemon2 = team2.team[0];
        Random rnd = new Random();
        if (trainer && (team1.CanSwitch() || team2.CanSwitch()) && !(ai == 1))
        {
            
        }
        if (trainer && ((currentPokemon1.species.mega && currentPokemon1.CheckStone()) || (currentPokemon2.species.mega && currentPokemon2.CheckStone())))
        { 
        }
        //Pokemon pokemonF = null;
        //Pokemon pokemonS = null;
        //int priority1;
        //int priority2;
        //while (true)
        //{
        //    int movepk1 = rnd.Next(0, pokemon1.moveNum);
        //    int movepk2 = rnd.Next(0, pokemon2.moveNum);
        //    priority1 = TurnFirst(pokemon1.moveSet[movepk1]);
        //    priority2 = TurnFirst(pokemon2.moveSet[movepk2]);
        //    if (priority1 > priority2)
        //    {
        //        pokemonF = pokemon1;
        //        pokemonS = pokemon2;
        //    }
        //    else if (priority1 < priority2)
        //    {
        //        pokemonF = pokemon2;
        //        pokemonS = pokemon1;
        //        (movepk1, movepk2) = (movepk2, movepk1);
        //    }
        //    else
        //    {
        //        if (pokemon1.CalcSpeStat() * pokemon1.GetMod(pokemon1.SpeMod) > pokemon2.CalcSpeStat() * pokemon2.GetMod(pokemon2.SpeMod))
        //        {
        //            pokemonF = pokemon1;
        //            pokemonS = pokemon2;
        //        }
        //        else if (pokemon1.CalcSpeStat() * pokemon1.GetMod(pokemon1.SpeMod) == pokemon2.CalcSpeStat() * pokemon2.GetMod(pokemon2.SpeMod))
        //        {
        //            if (rnd.Next(0, 2) == 0)
        //            {
        //                pokemonF = pokemon1;
        //                pokemonS = pokemon2;
        //            }
        //            else
        //            {
        //                pokemonF = pokemon2;
        //                pokemonS = pokemon1;
        //                (movepk1, movepk2) = (movepk2, movepk1);
        //            }
        //        }
        //        else
        //        {
        //            pokemonF = pokemon2;
        //            pokemonS = pokemon1;
        //            (movepk1, movepk2) = (movepk2, movepk1);
        //        }
        //    }
        //   Console.WriteLine($"{pokemonF.name} used {pokemonF.moveSet[movepk1].moveB.name}");
        //    Move(pokemonF, pokemonS, pokemonF.moveSet[movepk1]);
        //   Console.WriteLine(pokemonS.hp);

        //    if (pokemonS.hp <= 0)
        //    {
        //       Console.WriteLine($"{pokemonS.name} fainted");
        //        pokemon2.wins++;
        //        break;
        //    }

        //   Console.WriteLine($"{pokemonS.name} used {pokemonS.moveSet[movepk2].moveB.name}");
        //    Move(pokemonS, pokemonF, pokemonS.moveSet[movepk2]);
        //    Console.WriteLine(pokemonF.hp);

        //    if (pokemonF.hp <= 0 && pokemon2.hp > 0)
        //    {
        //        Console.WriteLine($"{pokemonF.name} fainted");
        //        pokemon1.wins++;
        //        break;
        //    }
        //}
        //heal = true;
        //if (heal == true)
        //{
        //    pokemon1.Heal();
        //    pokemon2.Heal();
        //}
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
                    Trainer attacker = new Trainer("Attacker");
                    attacker.AddPokemon(pokemons[i]);
                    Trainer foe = new Trainer("Foe");
                    foe.AddPokemon(pokemons[j]);
                    Battle(attacker, foe, false, ai);
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

                    Battle(trainers[i], trainers[j], true, ai);
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
        Console.WriteLine("Enter how many you want pokemon if you type \"NO\" the species of the pokemon it will registr all previus pokemon");
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
        Console.WriteLine("Enter how many you want pokemon if you type \"NO\" the species of the pokemon it will registr all previus pokemon");
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
                Pokemon pokemon = new Pokemon(speciesInput, name, gender, lvl, ability, 0, IVs[0], EVs[0], IVs[1], EVs[1], IVs[2], EVs[2], IVs[3], EVs[3], IVs[4], EVs[4], IVs[5], EVs[5], nature, itemInput, gmax, 10, GetTypeId(tera));
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

        Pokemon pokemon = new Pokemon(species, name, gender, lvl, ability, 0, HpIV, HpEV, AtkIV, AtkEV, DefIV, DefEV, SpaIV, SpaEV, SpdIV, SpdEV, SpeIV, SpeEV, nature, item, gmax, dmaxlvl, tera);
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
        Pokemon pokemon1 = null;
        Pokemon pokemon2 = null;


        string json = File.ReadAllText("AllPokemon.json");
        List<Species> AllPokemon = JsonSerializer.Deserialize<List<Species>>(json);

    


        foreach (Species s in AllPokemon.Skip(AllPokemon.Count - 75))
        {
            Console.WriteLine(s.name);
        }

            Item LifeOrb = new Item("Life Orb", "01&01&30", false);
        Item ExpertBelt = new Item("Expert Belt", "01&02&10", false);
        Item Galladite = new Item("Galladite", "03&07&00", true);

        List<Item> AllItems = new List<Item>
        {
            LifeOrb,
            ExpertBelt,
            Galladite
        };
        Pokemon GalladeM = new Pokemon(AllPokemon[475], "Gallade", true, 69, 2, 0, 31, 8, 31, 252, 31, 0, 31, 0, 31, 0, 31, 252, "Adamant", Galladite, false, 1, 7);
GalladeM.MegaEvolve(AllPokemon);
        Console.WriteLine(GalladeM.species.name);
        Pokemon Gallade = new Pokemon(AllPokemon[475], "Gallade", true, 69, 2, 0, 31, 8, 31, 252, 31, 0, 31, 0, 31, 0, 31, 252, "Adamant", LifeOrb, false, 1, 7);
        Pokemon Gardevoir = new Pokemon(AllPokemon[282], "Gardevoir", false, 69, 1, 0, 31, 8, 0, 0, 31, 0, 31, 252, 31, 0, 31, 252, "Modest", ExpertBelt, false, 1, 18);
        Pokemon pecharunt = new Pokemon(AllPokemon[1025], "Pecharunt", true, 69, 1, 0, 31, 8, 31, 252, 31, 0, 31, 0, 31, 0, 31, 252, "Adamant", LifeOrb, false, 1, 7);

        MoveB sacredSword = new MoveB("Sacred Sword", 7, 1, 101, 15, "01&90");
        MoveB swordsDance = new MoveB("Swords Dance", 1, 3, 101, 40, "01&21");
        MoveB psychoCut = new MoveB("Psycho Cut", 11, 1, 101, 20, "01&70");
        MoveB nightSlash = new MoveB("Night Slash", 16, 1, 100, 20, "04&70");

        MoveB psychic = new MoveB("Psychic", 11, 2, 100, 20, "01&90");
        MoveB psyshock = new MoveB("Psyshock", 11, 2, 100, 20, "03&80");
        MoveB sandAttack = new MoveB("Sand Attack", 9, 3, 100, 40, "02&-15");
        MoveB hydroPump = new MoveB("Hydro Pump", 3, 2, 70, 5, "01&110");

        List<MoveB> AllMoves = new List<MoveB>
        {
          sacredSword,
          swordsDance,
          psychoCut,
          nightSlash,
          psychic,
          psyshock,
          sandAttack,
          hydroPump,
          new MoveB("Bite", 16, 1, 100, 25, "01&60"),
          new MoveB("Round", 1, 2, 100, 15, "01&60"),
          new MoveB("Leer", 1, 3, 100, 30, "02&-12"),
          new MoveB("Incinerate", 2, 2, 100, 15, "01&72"),
          new MoveB("Wicked Torque", 16, 1, 100, 10, "01&80"),
          new MoveB("Snarl", 16, 2, 95, 15, "02&55&-13"),
          new MoveB("Swift", 1, 2, 101, 20, "01&60"),
          new MoveB("Metal Sound", 17, 3, 85, 40, "02&-24"),
          new MoveB("Acrobatics", 10, 1, 100, 24, "01&110"),
          new MoveB("Close Comabt", 7, 1, 100, 5, "01&100"),
          new MoveB("Brick Break", 7, 1, 75, 24, "01&75"),
        };

        Move SacredSword = new Move(sacredSword);
        Move SwordsDance = new Move(swordsDance);
        Move PsychoCut = new Move(psychoCut);
        Move NightSlash = new Move(nightSlash);

        Move Psychic = new Move(psychic);
        Move Psyshock = new Move(psyshock);
        Move SandAttack = new Move(sandAttack);
        Move HydroPump = new Move(hydroPump);

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
            if (choice == 1)
            {
                for (int i = 0; true; i++)
                {
                    Console.WriteLine($"Enter Trainer {i + 1}. name ('NO' to stop): ");
                    string trainerName = Console.ReadLine();
                    if (trainerName.ToLower() == "no") break;
                    Trainer trainer = new Trainer(trainerName);
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
                    Trainer trainer = new Trainer(trainerName);
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
                    Trainer trainer = new Trainer(trainerName);
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
            Console.WriteLine("how strong is the ai");
            Console.WriteLine("[1] Random");
            Console.WriteLine("[2] Normal(avg route trainer)");
            Console.WriteLine("[3] Smart(Gym/E4/Champion)");
            Console.WriteLine("[4] Perfect(near human)");
            int aiLevel = Convert.ToInt32(Console.ReadLine());
            int battlesPerPair = Convert.ToInt32(Console.ReadLine());
            RunAllTrainerBattles(trainers, battlesPerPair, aiLevel);
        }
        else if (choice == 3)
        {
            Console.WriteLine("Enter the name of the preset you want to use");
            string presetName = Console.ReadLine();
            if (presetName == "All")
            {
                List<Pokemon> PokemonList = new List<Pokemon>();
                int g = 0;
                List<int>  invalidMon = new List<int>
                {
                    0,1106,1151,1152
                };
                foreach (Species s in AllPokemon)
                {
                    g++;
                    if ((!invalidMon.Contains(g)) && g < 1058)
                    {
                        Pokemon temp = new Pokemon(AllPokemon[g], 50);
                        if(s.Atk > s.Spa)
                        {
                            MoveB a = new MoveB("Physical", 0, 1, 100, 100, "01&60");
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                        }
                        else if(s.Atk < s.Spa)
                        {
                            MoveB b = new MoveB("Special", 0, 2, 100, 100, "01&60");
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                        }
                        else
                        {
                            MoveB a = new MoveB("Physical", 0, 1, 100, 100, "01&60");
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            MoveB b = new MoveB("Special", 0, 2, 100, 100, "01&60");
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
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
                foreach (Pokemon p in PokemonList.OrderByDescending(p => p.wins))
                {
                    Console.WriteLine($"{p.name} has achived {p.wins} wins!");
                }

            }
            else if (presetName == "Ball")
            {
                List<Pokemon> PokemonList = new List<Pokemon>();
                int g = 0;
                List<int> invalidMon = new List<int>
                {
                    0,1106,1151,1152
                };
                foreach (Species s in AllPokemon.Take(9))
                {
                g++;
                    if ((!invalidMon.Contains(g)) && g < 1058)
                    {
                        Pokemon temp = new Pokemon(AllPokemon[g], 50);
                        if (s.Atk > s.Spa)
                        {
                            MoveB a = new MoveB("Physical", 0, 1, 100, 100, "01&60");
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                        }
                        else if (s.Atk < s.Spa)
                        {
                            MoveB b = new MoveB("Special", 0, 2, 100, 100, "01&60");
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
                        }
                        else
                        {
                            MoveB a = new MoveB("Physical", 0, 1, 100, 100, "01&60");
                            Move a1 = new Move(a);
                            temp.AddMove(a1);
                            MoveB b = new MoveB("Special", 0, 2, 100, 100, "01&60");
                            Move b1 = new Move(b);
                            temp.AddMove(b1);
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
                foreach (Pokemon p in PokemonList.OrderByDescending(p => p.wins))
                {
                    Console.WriteLine($"{p.name} has achived {p.wins} wins!");
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
                Console.WriteLine(pkmn+1);
            }
            else
            {
                Console.WriteLine("no pokemon found :O");
            }
        }
       
    }
}