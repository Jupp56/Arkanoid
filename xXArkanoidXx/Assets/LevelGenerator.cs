﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assets
{
    public static class LevelGenerator
    {
        public static Level GenerateLevel(int levelNum)
        {
            Random random = new Random(levelNum);
            int bricks = Math.Min(levelNum * 5, 13 * 18 - 5);
            if (bricks <= 0) bricks = 13 * 18;

            Level level = new Level();
            for (int i = 0; i < bricks; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(18);
                    y = random.Next(13);
                } while (level.bricks[x, y] > 0);
                level.bricks[x, y] = (Level.BrickType)random.Next(1, 10);
            }

            return level;
        }
        public static Level LoadLevelFromPath(string path)
        {
            string levelString = File.ReadAllText(path);
            Level level = JsonConvert.DeserializeObject<Level>(levelString);
            return level;
        }

        public static void SaveLevelToPath(string path, Level level)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(level));
        }
      
    }

    public class Level
    {
        public enum BrickType
        {
            empty,
            red,
            yellow,
            green,
            lightBlue,
            orange,
            white,
            blue,
            purple,
            double_,
            indestructible,
        }

        public BrickType[,] bricks = new BrickType[18,13];

        public List<EnemySpawn> enemies = new List<EnemySpawn>();
    }

    public class EnemySpawn
    {

        public enum SpawnDirection
        {
            right,
            left
        }

        public SpawnDirection spawnDirection = SpawnDirection.left;
        // Number of blocks after the destruction of which the enemy spawns
        public int blockCount = 0;
    }
}
