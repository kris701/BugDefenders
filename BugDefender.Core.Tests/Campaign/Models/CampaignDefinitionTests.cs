using BugDefender.Core.Campaign.Models;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Tests.Campaign.Models
{
    [TestClass]
    public class CampaignDefinitionTests
    {
        public static IEnumerable<object[]> CampaignIDs()
        {
            foreach (var id in ResourceManager.Campaigns.GetResources())
                yield return new object[] { id };
        }

        [TestMethod]
        public void AreIDsUnique()
        {
            ResourceManager.ReloadResources();
            var ids = ResourceManager.Campaigns.GetResources();

            Assert.AreEqual(ids.Count, ids.Distinct().Count());
        }

        [TestMethod]
        [DynamicData(nameof(CampaignIDs), DynamicDataSourceType.Method)]
        public void Is_AllChaptersReferenced(Guid campaignID)
        {
            // ARRANGE
            var campaign = ResourceManager.Campaigns.GetResource(campaignID);

            // ACT
            foreach (var chapter in campaign.Chapters)
                if (chapter.NextChapterID != null)
                    Assert.IsTrue(campaign.Chapters.Any(x => x.ID == chapter.NextChapterID));
        }

        [TestMethod]
        [DynamicData(nameof(CampaignIDs), DynamicDataSourceType.Method)]
        public void Can_AllCampaignsEnd(Guid campaignID)
        {
            // ARRANGE
            var campaign = ResourceManager.Campaigns.GetResource(campaignID);

            // ACT
            Assert.IsTrue(campaign.Chapters.Any(x => x.NextChapterID == null));
        }

        [TestMethod]
        [DynamicData(nameof(CampaignIDs), DynamicDataSourceType.Method)]
        public void Is_AllSpeakersReferenced(Guid campaignID)
        {
            // ARRANGE
            var campaign = ResourceManager.Campaigns.GetResource(campaignID);

            // ACT
            foreach (var chapter in campaign.Chapters)
                Assert.IsTrue(IsSpeakersReferenced(campaign.Speakers, chapter.Intro));
            Assert.IsTrue(IsSpeakersReferenced(campaign.Speakers, campaign.CampaignOver));
        }

        [TestMethod]
        [DynamicData(nameof(CampaignIDs), DynamicDataSourceType.Method)]
        public void Is_AllChaptersUsingValidMaps(Guid campaignID)
        {
            // ARRANGE
            var campaign = ResourceManager.Campaigns.GetResource(campaignID);
            var maps = ResourceManager.Maps.GetResources();

            // ACT
            foreach (var chapter in campaign.Chapters)
                Assert.IsTrue(maps.Contains(chapter.MapID));
        }

        [TestMethod]
        [DynamicData(nameof(CampaignIDs), DynamicDataSourceType.Method)]
        public void Is_AllChaptersUsingValidGameStyles(Guid campaignID)
        {
            // ARRANGE
            var campaign = ResourceManager.Campaigns.GetResource(campaignID);
            var gameStyles = ResourceManager.GameStyles.GetResources();

            // ACT
            foreach (var chapter in campaign.Chapters)
                Assert.IsTrue(gameStyles.Contains(chapter.GameStyleID));
        }

        private bool IsSpeakersReferenced(Dictionary<Guid, string> speakers, CutsceneDefinition cutscene)
        {
            foreach (var convo in cutscene.Conversation)
                if (!speakers.ContainsKey(convo.SpeakerID))
                    return false;
            return true;
        }
    }
}
