using System;
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
        public static int[,] GenerateLevel(int level, int width, int height)
        {
            Random random = new Random(level);
            int maxDurability = Math.Min(level / 5, 10);
            int bricks = Math.Min(level * 5, width * height - 5);
            if (bricks <= 0) bricks = width * height;

            int[,] grid = new int[width, height];
            for (int i = 0; i < bricks; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(width);
                    y = random.Next(height);
                } while (grid[x, y] > 0);
                grid[x, y] = random.Next(1, maxDurability + 1);
            }

            return grid;
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
    }

}
