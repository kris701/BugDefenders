using TDGame.Core.Game.Models.Entities.Enemies.Modules;

namespace TDGame.Core.Users.Models.Buffs.BuffEffects
{
    public class EnemyBuffEffect : IBuffEffect
    {
        public Guid EnemyID { get; set; }
        public IEnemyModule Module { get; set; }

        public EnemyBuffEffect(Guid enemyID, IEnemyModule module)
        {
            EnemyID = enemyID;
            Module = module;
        }
    }
}
