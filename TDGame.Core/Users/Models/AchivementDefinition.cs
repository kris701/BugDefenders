﻿using TDGame.Core.Game.Models;
using TDGame.Core.Users.Models.UserCriterias;

namespace TDGame.Core.Users.Models
{
    public class AchivementDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IUserCriteria Criteria { get; set; }
    }
}
