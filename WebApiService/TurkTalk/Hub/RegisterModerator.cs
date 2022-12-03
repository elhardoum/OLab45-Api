using Common.Utils;
using Dawn;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OLabWebAPI.Services.TurkTalk.Contracts;
using System;
using System.Threading.Tasks;

namespace OLabWebAPI.Services.TurkTalk
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TurkTalkHub : Hub
    {
        /// <summary>
        /// Register moderator to atrium
        /// </summary>
        /// <param name="moderatorName">Moderator's name</param>
        /// <param name="roomName">Atrium name</param>
        /// <param name="roomName">Topic id</param>
        /// <param name="isbot">Moderator is a bot</param>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task RegisterModerator(string roomName, bool isBot)
        {
            try
            {
                Guard.Argument(roomName).NotNull(nameof(roomName));

                _logger.LogInformation($"RegisterModerator: '{roomName}', {isBot} ({ConnectionId.Shorten(Context.ConnectionId)})");

                Moderator moderator = new Moderator(roomName, Context);
                Venue.Room room = _conference.GetCreateUnmoderatedTopicRoom(moderator);

                // add room index to moderator info
                moderator.AssignToRoom(room.Index);
                _logger.LogInformation($"moderator: '{moderator}'");

                await room.AddModeratorAsync(moderator);
            }
            catch (Exception ex)
            {
                _logger.LogError($"RegisterModerator exception: {ex.Message}");
            }
        }
    }
}
