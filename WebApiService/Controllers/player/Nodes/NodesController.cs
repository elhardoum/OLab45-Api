using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OLabWebAPI.Common;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Dto;
using OLabWebAPI.Endpoints.Player;
using OLabWebAPI.Model;
using OLabWebAPI.Services;
using System;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.WebApi.Player
{
    [Route("olab/api/v3/nodes")]
    [ApiController]
    public partial class NodesController : OlabController
    {
        private readonly NodesEndpoint _endpoint;

        public NodesController(ILogger<NodesController> logger, OLabDBContext context) : base(logger, context)
        {
            _endpoint = new NodesEndpoint(this.logger, context);
        }

        /// <summary>
        /// Get full map node, with relations
        /// </summary>
        /// <param name="nodeId">Node id (0, if root node)</param>
        /// <returns>MapsNodesFullRelationsDto response</returns>
        [HttpGet("{nodeId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetNodeTranslatedAsync(uint nodeId)
        {
            try
            {
                MapsNodesFullRelationsDto dto = await _endpoint.GetNodeTranslatedAsync(nodeId);
                return OLabObjectResult<MapsNodesFullRelationsDto>.Result(dto);
            }
            catch (Exception ex)
            {
                if (ex is OLabUnauthorizedException)
                    return OLabUnauthorizedObjectResult<string>.Result(ex.Message);
                return OLabServerErrorResult.Result(ex.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{nodeId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutNodeAsync(uint id, [FromBody] MapNodesFullDto dto)
        {
            try
            {
                var auth = new OLabWebApiAuthorization(logger, dbContext, HttpContext);
                await _endpoint.PutNodeAsync(auth, id, dto);
            }
            catch (Exception ex)
            {
                if (ex is OLabUnauthorizedException)
                    return OLabUnauthorizedObjectResult<string>.Result(ex.Message);
                return OLabServerErrorResult.Result(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="nodeId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{nodeId}/links")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostLinkAsync(
          uint nodeId,
          [FromBody] MapNodeLinksPostDataDto data
        )
        {
            try
            {
                MapNodeLinksPostResponseDto dto = await _endpoint.PostLinkAsync(nodeId, data);
                return OLabObjectResult<MapNodeLinksPostResponseDto>.Result(dto);
            }
            catch (Exception ex)
            {
                if (ex is OLabUnauthorizedException)
                    return OLabUnauthorizedObjectResult<string>.Result(ex.Message);
                return OLabServerErrorResult.Result(ex.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("{nodeId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostNodeAsync(
          uint mapId,
          [FromBody] MapNodesPostDataDto data
        )
        {
            try
            {
                MapNodesPostResponseDto dto = await _endpoint.PostNodeAsync(mapId, data);
                return OLabObjectResult<MapNodesPostResponseDto>.Result(dto);
            }
            catch (Exception ex)
            {
                if (ex is OLabUnauthorizedException)
                    return OLabUnauthorizedObjectResult<string>.Result(ex.Message);
                return OLabServerErrorResult.Result(ex.Message);
            }
        }
    }
}
