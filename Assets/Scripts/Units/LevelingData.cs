using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Kind of a weird script, this one.
//It details what should happen to the player 
//after each level up.
public class LevelingData:MonoBehaviour {
    public float levelScaleMultiplier;
    public int levelScaleBias;
    
    [System.Serializable]
    public class Projectiles
    {
        public Ability lvl5Proj;
        public Ability lvl10Proj;
        public Ability lvl15Proj;
        public Ability lvl20Proj;
    }
    public Projectiles projectiles;

    public LevelUpPackage GetLevelStats(int level, int oldMaxExp)
    {
        switch (level)
        {
            case (3):
                return new LevelUpPackage(projectiles.lvl5Proj, 10, GetNextMaxExp(oldMaxExp));
            case (5):
                return new LevelUpPackage(projectiles.lvl10Proj, 10, GetNextMaxExp(oldMaxExp));
            case (7):
                return new LevelUpPackage(projectiles.lvl15Proj, 10, GetNextMaxExp(oldMaxExp));
            case (100):
                return new LevelUpPackage(projectiles.lvl20Proj, 10, GetNextMaxExp(oldMaxExp));
            default:
                return new LevelUpPackage(null, 10, GetNextMaxExp(oldMaxExp));
        }
    }
    
    private int GetNextMaxExp(int exp)
    {
        int result = (int)(exp * levelScaleMultiplier) + levelScaleBias;
        return result;
    }
}

//used to pass information between leveling system and player
public class LevelUpPackage
{
    public Ability ability;
    public int healthUp;
    public int nextLevel;
    public LevelUpPackage(Ability abil, int health, int nLvl)
    {
        ability = abil; healthUp = health; nextLevel = nLvl;
    }
}
