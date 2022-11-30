using System;
using System.Threading.Tasks;
using Dawn;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OLabWebAPI.Services.TurkTalk.Contracts;
using OLabWebAPI.TurkTalk.Contracts;

namespace OLabWebAPI.Services.TurkTalk
{
  /// <summary>
  /// 
  /// </summary>
  public partial class TurkTalkHub : Hub
  {
    /// <summary>
    /// Moderator assigns a learner (remove from atrium)
    /// </summary>
    /// <param name="learner">Learner to remove</param>
    /// <param name="topicName">Topic id</param>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task AssignAttendee(Learner learner, string topicName)
    {
      try
      {
        Guard.Argument(topicName).NotNull(nameof(topicName));
        _logger.LogInformation($"AssignAttendeeASync: '{learner}', {topicName}");

        var topic = _conference.GetCreateTopic(learner.TopicName, false);
        if (topic == null)
          return;

        topic.RemoveFromAtrium(learner);

        // add the moderator to the command channel for
        // the assigned learner
        learner.ConnectionId = Context.ConnectionId;
        await topic.Conference.AddConnectionToGroupAsync(learner);

      }
      catch (Exception ex)
      {
        _logger.LogError($"AssignAttendeeASync exception: {ex.Message}");
      }
    }
  }
}
