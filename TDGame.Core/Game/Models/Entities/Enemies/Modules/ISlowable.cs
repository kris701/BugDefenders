namespace TDGame.Core.Game.Models.Entities.Enemies.Modules
{
    public interface ISlowable
    {
        public float Speed { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }

        public float GetSpeed();
    }
}
