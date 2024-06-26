﻿namespace BugDefender.Core.Users.Models.UserCriterias
{
    public class MoneyEarnedCriteria : IUserCriteria
    {
        public int Quantity { get; set; }

        public bool IsValid(StatsDefinition stats) => stats.TotalMoneyEarned >= Quantity;
        public override string ToString()
        {
            return $"Earn {Quantity} money.";
        }

        public string Progress(StatsDefinition stats)
        {
            return $"{Quantity - stats.TotalMoneyEarned} more money to earn";
        }
    }
}
