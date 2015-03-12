using System;
using System.Diagnostics;

namespace CleanLiving.Engine
{
    [DebuggerDisplay("GameTime: Id = {Id}")]
    public class GameTime
    {
        private Guid Id { get; } = Guid.NewGuid();

        public static GameTime Now { get; } = new GameTime();

        public GameTime Add(int x) { return new GameTime(); }
    }
}
