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
        public Rigidbody lvl5Proj;
        public Rigidbody lvl10Proj;
        public Rigidbody lvl15Proj;
        public Rigidbody lvl20Proj;
    }
    public Projectiles projectiles;

    public LevelUpPackage GetLevelStats(int level, int oldMaxExp)
    {
        switch (level)
        {
            case (2):
                return new LevelUpPackage(projectiles.lvl5Proj, 10, GetNextMaxExp(oldMaxExp));
            case (3):
                return new LevelUpPackage(projectiles.lvl10Proj, 10, GetNextMaxExp(oldMaxExp));
            case (4):
                return new LevelUpPackage(projectiles.lvl15Proj, 10, GetNextMaxExp(oldMaxExp));
            case (5):
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
    public Rigidbody projectile;
    public int healthUp;
    public int nextLevel;
    public LevelUpPackage(Rigidbody proj, int health, int nLvl)
    {
        projectile = proj; healthUp = health; nextLevel = nLvl;
    }
}
