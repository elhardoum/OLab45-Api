using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLabWebAPI.Endpoints.WebApi
{
    public class OlabController : ControllerBase
    {
        protected readonly OLabDBContext context;
        protected OLabLogger logger;
        protected string token;
        protected string BaseUrl => $"{Request.Scheme}://{Request.Host.Value}";
        protected string RequestPath => $"{Request.Path.ToString().Trim('/')}";
        protected HttpRequest request;

        public OlabController(ILogger logger, OLabDBContext context)
        {
            this.context = context;
            this.logger = new OLabLogger(logger);
        }

        [NonAction]
        protected async ValueTask<Maps> GetMapAsync(uint id)
        {
            Maps phys = await context.Maps.FirstOrDefaultAsync(x => x.Id == id);
            context.Entry(phys).Collection(b => b.MapNodes).Load();
            return phys;
        }

        /// <summary>
        /// Attach parent information to scoped object
        /// </summary>
        /// <param name="dto"></param>
        [NonAction]
        protected void AttachParentObject(ScopedObjectDto dto)
        {
            if (dto.ImageableType == Constants.ScopeLevelServer)
            {
                Servers obj = context.Servers.FirstOrDefault(x => x.Id == dto.ImageableId);
                dto.ParentObj.Id = obj.Id;
                dto.ParentObj.Name = obj.Name;
                dto.ParentObj.Description = obj.Description;
            }

            else if (dto.ImageableType == Constants.ScopeLevelMap)
            {
                Maps obj = context.Maps.FirstOrDefault(x => x.Id == dto.ImageableId);
                dto.ParentObj.Id = obj.Id;
                dto.ParentObj.Name = obj.Name;
                dto.ParentObj.Description = obj.Name;
            }

            else if (dto.ImageableType == Constants.ScopeLevelNode)
            {
                MapNodes obj = context.MapNodes.FirstOrDefault(x => x.Id == dto.ImageableId);
                dto.ParentObj.Id = obj.Id;
                dto.ParentObj.Name = obj.Title;
                dto.ParentObj.Description = obj.Title;
            }
        }

        [NonAction]
        protected async Task<MapNodes> GetMapRootNode(uint mapId, uint nodeId)
        {
            if (nodeId != 0)
                return await context.MapNodes
                  .Where(x => x.MapId == mapId && x.Id == nodeId)
                  .FirstOrDefaultAsync(x => x.Id == nodeId);

            MapNodes item = await context.MapNodes
                .Where(x => x.MapId == mapId && x.TypeId == 1)
                .FirstOrDefaultAsync(x => x.Id == nodeId);

            if (item == null)
                item = await context.MapNodes
                          .Where(x => x.MapId == mapId)
                          .OrderBy(x => x.Id)
                          .FirstAsync();

            return item;
        }

        /// <summary>
        /// Get nodes for map
        /// </summary>
        /// <param name="map">Parent map to query for</param>
        /// <param name="enableWikiTanslation">PErform WikiTag translation</param>
        /// <returns>List of mapnode dto's</returns>
        [NonAction]
        protected async Task<IList<MapNodesFullDto>> GetNodesAsync(Maps map, bool enableWikiTanslation = true)
        {
            List<MapNodes> physList = await context.MapNodes.Where(x => x.MapId == map.Id).ToListAsync();
            logger.LogDebug(string.Format("found {0} mapNodes", physList.Count));

            IList<MapNodesFullDto> dtoList = new ObjectMapper.MapNodesFullMapper(logger, enableWikiTanslation).PhysicalToDto(physList);
            return dtoList;
        }

        /// <summary>
        /// Get node for map
        /// </summary>
        /// <param name="map">Map object</param>
        /// <param name="nodeId">Node id</param>
        /// <param name="enableWikiTanslation">PErform WikiTag translation</param>
        /// <returns>MapsNodesFullRelationsDto</returns>
        [NonAction]
        protected async Task<MapsNodesFullRelationsDto> GetNodeAsync(Maps map, uint nodeId, bool enableWikiTanslation = true)
        {
            MapNodes phys = await context.MapNodes
              .FirstOrDefaultAsync(x => x.MapId == map.Id && x.Id == nodeId);

            if (phys == null)
                return new MapsNodesFullRelationsDto();

            // explicitly load the related objects.
            context.Entry(phys).Collection(b => b.MapNodeLinksNodeId1Navigation).Load();

            ObjectMapper.MapsNodesFullRelationsMapper builder = new ObjectMapper.MapsNodesFullRelationsMapper(logger, enableWikiTanslation);
            MapsNodesFullRelationsDto dto = builder.PhysicalToDto(phys);

            List<uint> linkedIds = phys.MapNodeLinksNodeId1Navigation.Select(x => x.NodeId2).Distinct().ToList();
            List<MapNodes> linkedNodes = context.MapNodes.Where(x => linkedIds.Contains(x.Id)).ToList();

            foreach (MapNodeLinksDto item in dto.MapNodeLinks)
            {
                MapNodes link = linkedNodes.Where(x => x.Id == item.DestinationId).FirstOrDefault();
                item.DestinationTitle = linkedNodes.Where(x => x.Id == item.DestinationId).Select(x => x.Title).FirstOrDefault();
                if (string.IsNullOrEmpty(item.LinkText))
                    item.LinkText = item.DestinationTitle;
            }

            return dto;
        }

        /// <summary>
        /// Get a mapnode
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <returns></returns>
        [NonAction]
        public async ValueTask<MapNodes> GetMapNodeAsync(uint nodeId)
        {
            MapNodes item = await context.MapNodes
                .FirstOrDefaultAsync(x => x.Id == nodeId);

            // explicitly load the related objects.
            context.Entry(item).Collection(b => b.MapNodeLinksNodeId1Navigation).Load();

            return item;
        }

        /// <summary>
        /// Get question response
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [NonAction]
        protected async ValueTask<SystemQuestionResponses> GetQuestionResponseAsync(uint id)
        {
            SystemQuestionResponses item = await context.SystemQuestionResponses.FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        /// <summary>
        /// Get constant
        /// </summary>
        /// <param name="id">question id</param>
        /// <returns></returns>
        [NonAction]
        protected async ValueTask<SystemConstants> GetConstantAsync(uint id)
        {
            SystemConstants item = await context.SystemConstants
                .FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="id">file id</param>
        /// <returns></returns>
        [NonAction]
        protected async ValueTask<SystemFiles> GetFileAsync(uint id)
        {
            SystemFiles item = await context.SystemFiles
                .FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        /// <summary>
        /// Get question
        /// </summary>
        /// <param name="id">question id</param>
        /// <returns></returns>
        [NonAction]
        protected async ValueTask<SystemQuestions> GetQuestionSimpleAsync(uint id)
        {
            SystemQuestions item = await context.SystemQuestions
                .FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        /// <summary>
        /// Get question with responses
        /// </summary>
        /// <param name="id">question id</param>
        /// <returns></returns>
        [NonAction]
        protected async ValueTask<SystemQuestions> GetQuestionAsync(uint id)
        {
            SystemQuestions item = await context.SystemQuestions
                .Include(x => x.SystemQuestionResponses)
                .FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="sinceTime"></param>
        /// <param name="scopeLevel"></param>
        /// <returns></returns>
        [NonAction]
        protected async ValueTask<ScopedObjects> GetScopedObjectsDynamicAsync(
          uint parentId,
          uint sinceTime,
          string scopeLevel)
        {
            ScopedObjects phys = new ScopedObjects
            {
                Counters = await GetScopedCountersAsync(scopeLevel, parentId, sinceTime)
            };

            return phys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="scopeLevel"></param>
        /// <returns></returns>
        [NonAction]
        protected async ValueTask<ScopedObjects> GetScopedObjectsAllAsync(
          uint parentId,
          string scopeLevel)
        {
            ScopedObjects phys = new ScopedObjects
            {
                Constants = await GetScopedConstantsAsync(parentId, scopeLevel),
                Questions = await GetScopedQuestionsAsync(parentId, scopeLevel),
                Files = await GetScopedFilesAsync(parentId, scopeLevel),
                Scripts = await GetScopedScriptsAsync(parentId, scopeLevel),
                Themes = await GetScopedThemesAsync(parentId, scopeLevel),
                Counters = await GetScopedCountersAsync(scopeLevel, parentId, 0)
            };

            if (scopeLevel == Constants.ScopeLevelMap)
            {
                List<SystemCounterActions> items = new List<SystemCounterActions>();
                items.AddRange(await context.SystemCounterActions.Where(x =>
                    x.MapId == parentId).ToListAsync());

                phys.CounterActions.AddRange(items);
            }

            return phys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="scopeLevel"></param>
        /// <returns></returns>
        [NonAction]
        protected async Task<List<SystemConstants>> GetScopedConstantsAsync(uint parentId, string scopeLevel)
        {
            List<SystemConstants> items = new List<SystemConstants>();

            items.AddRange(await context.SystemConstants.Where(x =>
              x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="scopeLevel"></param>
        /// <returns></returns>
        [NonAction]
        protected async Task<List<SystemFiles>> GetScopedFilesAsync(uint parentId, string scopeLevel)
        {
            List<SystemFiles> items = new List<SystemFiles>();

            items.AddRange(await context.SystemFiles.Where(x =>
              x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="scopeLevel"></param>
        /// <returns></returns>
        [NonAction]
        protected async Task<List<SystemQuestions>> GetScopedQuestionsAsync(uint parentId, string scopeLevel)
        {
            List<SystemQuestions> items = new List<SystemQuestions>();

            items.AddRange(await context.SystemQuestions
              .Where(x => x.ImageableType == scopeLevel && x.ImageableId == parentId)
              .Include("SystemQuestionResponses")
              .ToListAsync());

            // order the responses by Order field
            foreach (SystemQuestions item in items)
                item.SystemQuestionResponses = item.SystemQuestionResponses.OrderBy(x => x.Order).ToList();

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="scopeLevel"></param>
        /// <returns></returns>
        [NonAction]
        protected async Task<List<SystemThemes>> GetScopedThemesAsync(uint parentId, string scopeLevel)
        {
            List<SystemThemes> items = new List<SystemThemes>();

            items.AddRange(await context.SystemThemes.Where(x =>
              x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="scopeLevel"></param>
        /// <returns></returns>
        [NonAction]
        protected async Task<List<SystemScripts>> GetScopedScriptsAsync(uint parentId, string scopeLevel)
        {
            List<SystemScripts> items = new List<SystemScripts>();

            items.AddRange(await context.SystemScripts.Where(x =>
              x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());

            return items;
        }

        /// <summary>
        /// Get counter 
        /// </summary>
        /// <param name="id">counter id</param>
        /// <returns>Counter</returns>
        [NonAction]
        protected async Task<SystemCounters> GetCounterAsync(uint id)
        {
            SystemCounters phys = await context.SystemCounters.SingleOrDefaultAsync(x => x.Id == id);
            if (phys.Value == null)
                phys.Value = new List<byte>().ToArray();
            if (phys.StartValue == null)
                phys.StartValue = new List<byte>().ToArray();
            return phys;
        }

        /// <summary>
        /// Get counters associated with a 'parent' object 
        /// </summary>
        /// <param name="scopeLevel">Scope level of parent (Maps, MapNodes, etc)</param>
        /// <param name="parentId">Id of parent object</param>
        /// <param name="sinceTime">(optional) looks for values changed since a (unix) time</param>
        /// <returns>List of counters</returns>
        [NonAction]
        protected async Task<List<SystemCounters>> GetScopedCountersAsync(string scopeLevel, uint parentId, uint sinceTime = 0)
        {
            List<SystemCounters> items = new List<SystemCounters>();

            if (sinceTime != 0)
            {
                // generate DateTime from sinceTime
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(sinceTime).ToLocalTime();
                items.AddRange(await context.SystemCounters.Where(x =>
                  x.ImageableType == scopeLevel && x.ImageableId == parentId && x.UpdatedAt >= dateTime).ToListAsync());
            }
            else
            {
                items.AddRange(await context.SystemCounters.Where(x =>
                  x.ImageableType == scopeLevel && x.ImageableId == parentId).ToListAsync());
            }

            return items;
        }
    }
}