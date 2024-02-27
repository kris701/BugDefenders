namespace TDGame.Core.Game.Models.Entities
{
    public interface ISlowingAttribute
    {
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
    }
}
