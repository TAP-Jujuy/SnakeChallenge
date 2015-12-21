﻿namespace Snake
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

    public class Settings
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static int Speed { get; set; }
        public static int Score { get; set; }
        public static int Points { get; set; }
        public static bool GameOver { get; set; }
        public static bool Win { get; set; }
        public static Direction direction { get; set; }

        public Settings()
        {
            Width = 48;
            Height = 48;
            Speed = 12;
            Score = 0;
            Points = 100;
            GameOver = false;
            Win = false;
            direction = Direction.Down;
        }
    }
}
