using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLabWebAPI.Services.TurkTalk.Venue;
using System.Collections.Generic;

namespace OLabWebAPI.Services.TurkTalk.Contracts
{
    /// <summary>
    /// Defines a Learners Update command method
    /// </summary>
    public class LearnerListCommand : CommandMethod
    {
        public IList<Learner> Data { get; set; }

        // constructor for targetted group
        public LearnerListCommand(string groupName, IList<Learner> atriumLearners) : base(groupName, "learnerlist")
        {
            Data = atriumLearners;
        }

        // constructor for all moderators in a topic
        public LearnerListCommand(Topic topic, IList<Learner> atriumLearners) : base(topic.TopicModeratorsChannel, "learnerlist")
        {
            Data = atriumLearners;
        }
        public override string ToJson()
        {
            var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
            return JValue.Parse(rawJson).ToString(Formatting.Indented);
        }
    }
}