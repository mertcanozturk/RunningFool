using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts
{
    [System.Serializable]

    public class EnemyInfo
    {
        public string Enemy;
        public int CreationTime;
        public int LifeTime;
        public float Speed;
        public int RotationZ;
    }
    [System.Serializable]

    public class LevelInfo
    {
        public int Level;
        public float CharacterSpeed;
        public string Sprite;
        public string Background;
        public float CoinRate;
        public List<EnemyInfo> Enemies;
    }

    [System.Serializable]
    public class JsonDataConvert
    {
        public string Planet;
        public List<LevelInfo> Levels;
    }
} 




