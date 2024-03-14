using BugDefender.Core.Game;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.SavedGames
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "SaveType")]
    [JsonDerivedType(typeof(SurvivalSavedGame), typeDiscriminator: "Survival")]
    [JsonDerivedType(typeof(CampaignSavedGame), typeDiscriminator: "Campaign")]
    [JsonDerivedType(typeof(ChallengeSavedGame), typeDiscriminator: "Challenge")]
    public interface ISavedGame
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public GameContext Context { get; set; }
    }
}
