using Sitecore.Analytics.Tracking.External;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.EmailCampaign.Cd.Pipelines.EmailOpened;
using Sitecore.ExM.Framework.Diagnostics;
using System.Net;

namespace Sitecore.Support.EmailCampaign.Cd.Pipelines.EmailOpened
{
    public class SaveTouchPointRecord
    {
        // Fields
        private readonly InteractionRegistryBase _interactionRegistry;
        private readonly ILogger _logger;
        private readonly string databaseName;


        // Methods
        public SaveTouchPointRecord(InteractionRegistryBase interactionRegistry, ILogger logger, string databaseName)
        {
                Assert.ArgumentNotNull(interactionRegistry, "interactionRegistry");
            Assert.ArgumentNotNull(logger, "logger");
            this._interactionRegistry = interactionRegistry;
            this._logger = logger;
            this.databaseName = databaseName;
        }

        public void Process(EmailOpenedPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.MessageItem != null, "args", "MessageItem not set");
            Assert.ArgumentCondition(args.TouchPointRecord != null, "args", "TouchPointRecord not set");
            Assert.ArgumentCondition(!ID.IsNullOrEmpty(args.ChannelId), "args", "ChannelId not set");
            if (args.MessageItem.ExcludeFromReports)
            {
                this._logger.LogDebug("Interaction not saved as " + args.MessageItem.ID + " has been excluded from reports.");
            }
            else
            {
                InteractionRecord interaction = new InteractionRecord(args.EmailOpen.SiteName, args.ChannelId.Guid, null)
                {
                    Ip = IPAddress.Parse(args.EmailOpen.IPAddress),
                    UserAgent = args.EmailOpen.UserAgent,
                    Language = args.TouchPointRecord.Language
                };
                interaction.TouchPoints.Add(args.TouchPointRecord);
                this._interactionRegistry.Register(databaseName, args.EmailOpen.ContactId.Guid, interaction);
            }
        }
    }


}