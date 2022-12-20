using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OLabWebAPI.Common;
using OLabWebAPI.Common.Exceptions;
using OLabWebAPI.Dto;
using OLabWebAPI.Endpoints.Designer;
using OLabWebAPI.Endpoints.WebApi.Player;
using OLabWebAPI.Model;
using OLabWebAPI.Services;
using System;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.WebApi.Designer
{
  [Route("olab/api/v3/designer/maps")]
  [ApiController]
  public partial class MapsController : OlabController
  {
    private readonly MapsEndpoint _endpoint;

    public MapsController(ILogger<ConstantsController> logger, OLabDBContext context) : base(logger, context)
    {
      _endpoint = new MapsEndpoint(this.logger, context);
    }

    /// <summary>
    /// Plays specific map node
    /// </summary>
    /// <param name="mapId">map id</param>
    /// <param name="nodeId">node id</param>
    /// <returns>IActionResult</returns>
    [HttpGet("{mapId}/node/{nodeId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetMapNodeAsync(uint mapId, uint nodeId)
    {
      try
      {
        OLabWebApiAuthorization auth = new OLabWebApiAuthorization(logger, context, HttpContext);
        MapsNodesFullRelationsDto dto = await _endpoint.GetMapNodeAsync(auth, mapId, nodeId);
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
    /// Get non-rendered nodes for a map
    /// </summary>
    /// <param name="mapId">Map id</param>
    /// <returns>IActionResult</returns>
    [HttpGet("{mapId}/nodes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetMapNodesAsync(uint mapId)
    {
      try
      {
        OLabWebApiAuthorization auth = new OLabWebApiAuthorization(logger, context, HttpContext);
        System.Collections.Generic.IList<MapNodesFullDto> dtoList = await _endpoint.GetMapNodesAsync(auth, mapId);
        return OLabObjectListResult<MapNodesFullDto>.Result(dtoList);
      }
      catch (Exception ex)
      {
        if (ex is OLabUnauthorizedException)
          return OLabUnauthorizedObjectResult<string>.Result(ex.Message);
        return OLabServerErrorResult.Result(ex.Message);
      }
    }

    /// <summary>
    /// Create a new node link
    /// </summary>
    /// <returns>IActionResult</returns>
    [HttpPost("{mapId}/nodes/{nodeId}/links")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PostMapNodeLinkAsync(uint mapId, uint nodeId, [FromBody] PostNewLinkRequest body)
    {
      try
      {
        OLabWebApiAuthorization auth = new OLabWebApiAuthorization(logger, context, HttpContext);
        PostNewLinkResponse dto = await _endpoint.PostMapNodeLinkAsync(auth, mapId, nodeId, body);
        return OLabObjectResult<PostNewLinkResponse>.Result(dto);
      }
      catch (Exception ex)
      {
        if (ex is OLabUnauthorizedException)
          return OLabUnauthorizedObjectResult<string>.Result(ex.Message);
        return OLabServerErrorResult.Result(ex.Message);
      }
    }

    /// <summary>
    /// Create a new node
    /// </summary>
    /// <returns>IActionResult</returns>
    [HttpPost("{mapId}/nodes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PostMapNodesAsync(PostNewNodeRequest body)
    {
      try
      {
        OLabWebApiAuthorization auth = new OLabWebApiAuthorization(logger, context, HttpContext);
        PostNewNodeResponse dto = await _endpoint.PostMapNodesAsync(auth, body);
        return OLabObjectResult<PostNewNodeResponse>.Result(dto);
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
    /// <returns></returns>
    [HttpGet("{id}/scopedobjects/raw")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetScopedObjectsRawAsync(uint id)
    {
      try
      {
        OLabWebApiAuthorization auth = new OLabWebApiAuthorization(logger, context, HttpContext);
        Dto.Designer.ScopedObjectsDto dto = await _endpoint.GetScopedObjectsRawAsync(auth, id);
        return OLabObjectResult<OLabWebAPI.Dto.Designer.ScopedObjectsDto>.Result(dto);
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
    /// <returns></returns>
    [HttpGet("{id}/scopedobjects")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetScopedObjectsAsync(uint id)
    {
      try
      {
        OLabWebApiAuthorization auth = new OLabWebApiAuthorization(logger, context, HttpContext);
        Dto.Designer.ScopedObjectsDto dto = await _endpoint.GetScopedObjectsAsync(auth, id);
        return OLabObjectResult<OLabWebAPI.Dto.Designer.ScopedObjectsDto>.Result(dto);
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
    /// <param name="enableWikiTranslation"></param>
    /// <returns></returns>
    private async Task<IActionResult> GetScopedObjectsAsync(
      uint id,
      bool enableWikiTranslation)
    {
      try
      {
        Dto.Designer.ScopedObjectsDto dto = await _endpoint.GetScopedObjectsAsync(id, enableWikiTranslation);
        DecorateDto(dto);
        return OLabObjectResult<Dto.Designer.ScopedObjectsDto>.Result(dto);
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
    /// <param name="dto"></param>
    private void DecorateDto(Dto.Designer.ScopedObjectsDto dto)
    {
      Type t = typeof(QuestionsController);
      RouteAttribute attribute =
          (RouteAttribute)Attribute.GetCustomAttribute(t, typeof(RouteAttribute));
      string questionRoute = attribute.Template;

      t = typeof(ConstantsController);
      attribute =
          (RouteAttribute)Attribute.GetCustomAttribute(t, typeof(RouteAttribute));
      string constantRoute = attribute.Template;

      t = typeof(CountersController);
      attribute =
          (RouteAttribute)Attribute.GetCustomAttribute(t, typeof(RouteAttribute));
      string counterRoute = attribute.Template;

      t = typeof(FilesController);
      attribute =
          (RouteAttribute)Attribute.GetCustomAttribute(t, typeof(RouteAttribute));
      string fileRoute = attribute.Template;

      foreach (Dto.Designer.ScopedObjectDto item in dto.Questions)
        item.Url = $"{BaseUrl}/{questionRoute}/{item.Id}";

      foreach (Dto.Designer.ScopedObjectDto item in dto.Counters)
        item.Url = $"{BaseUrl}/{counterRoute}/{item.Id}";

      foreach (Dto.Designer.ScopedObjectDto item in dto.Constants)
        item.Url = $"{BaseUrl}/{constantRoute}/{item.Id}";

      foreach (Dto.Designer.ScopedObjectDto item in dto.Files)
        item.Url = $"{BaseUrl}/{fileRoute}/{item.Id}";
    }
  }
}
