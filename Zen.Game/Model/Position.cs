﻿using Newtonsoft.Json;

namespace Zen.Game.Model
{
    public class Position
    {
        public Position(int x, int y, int height = 0)
        {
            X = x;
            Y = y;
            Height = height;
        }

        [JsonIgnore]
        public int CentralRegionX => X / 8;
        [JsonIgnore]
        public int CentralRegionY => Y / 8;

        public int X { get; }
        public int Y { get; }
        public int Height { get; }

        [JsonIgnore]
        public int LocalX => GetLocalX(CentralRegionX);
        [JsonIgnore]
        public int LocalY => GetLocalY(CentralRegionY);

        public int GetLocalX(int centralRegionX) => X - (centralRegionX - 6) * 8;
        public int GetLocalY(int centralRegionY) => Y - (centralRegionY - 6) * 8;

        public bool IsWithinDistance(Position other)
        {
            var deltaX = other.X - X;
            var deltaY = other.Y - Y;
            return deltaX >= -16 && deltaX <= 15 && deltaY >= -16 && deltaY <= 15;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Position;
            if (other == null) return false;
            return Height == other.Height && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime * result + Height;
            result = prime * result + X;
            result = prime * result + Y;
            return result;
        }

        public override string ToString()
        {
            return $"Position [X={X}, Y={Y}, Height={Height}]";
        }
    }
}