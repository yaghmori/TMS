using AutoMapper;
using Infrastructure.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using TMS.Core.Application;
using TMS.Core.Domain.Entities;
using TMS.Core.Domain.Enums;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Web.Server.Swagger;

namespace TMS.Client.API.Controllers
{
    [Authorize]
    [Route("api/v1/tenant/siloitems")]
    [ApiController]
    [SwaggerOperationFilter(typeof(HttpHeaderOperationFilter))]
    public class SiloItemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<TenantDbContext> _clientUnitOfWork;

        public SiloItemController(IMapper mapper, IUnitOfWork<TenantDbContext> unitOfWork)
        {
            _mapper = mapper;
            _clientUnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all silo with their child items
        /// </summary>
        /// <returns>List of SiloItemResponse</returns>
        [HttpGet]

        public async Task<IActionResult> GetAllSiloItems()
        {
            try
            {
                var siloItems = await _clientUnitOfWork.SiloItems.GetAllAsync(predicate: x => x.ItemType == SiloItemTypeEnum.Silo,
                    selector: SelectExpressions.SiloItems.GetSiloItemResponse);

                return Ok(siloItems.OrderBy(x => x.Index));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        /// <summary>
        /// Get all Flatten
        /// </summary>
        /// <returns>List of SiloItemResponse</returns>

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var siloItems = await _clientUnitOfWork.SiloItems.GetAllAsync(
                    include: i => i.Include(x => x.SiloItems).Include(x => x.Parent));

                return Ok(siloItems.OrderBy(x => x.Index));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }


        /// <summary>
        /// Get all Flatten by ItemType
        /// </summary>
        /// <returns>List of SiloItemResponse</returns>

        [HttpGet("ItemType")]
        public async Task<IActionResult> GetByItemType(SiloItemTypeEnum type)
        {
            try
            {
                var siloItems = await _clientUnitOfWork.SiloItems.GetAllAsync(
                    predicate: x => x.ItemType == type,
                    include: i => i.Include(x => x.SiloItems).Include(x => x.Parent));

                return Ok(siloItems.OrderBy(x => x.Index));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }


        /// <summary>
        /// Get ancestors of given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Ancestors")]
        public async Task<IActionResult> GetAncestorsById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid request id");

                List<SiloItemResponse> parents = new();
                var siloItem = await _clientUnitOfWork.SiloItems.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(id),
                    include: source => source.Include(i => i.Parent).ThenInclude(i => i.Parent));
                //var siloItem1 = await _siloRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == Guid.Parse(id), 
                //    include: source => source.Include(i => i.Parent),
                //    selector:source=> Shared.SelectExpressions.SiloItem.SiloItemResponse);

                switch (siloItem.ItemType)
                {
                    case SiloItemTypeEnum.Silo:
                        break;
                    case SiloItemTypeEnum.Loop:
                        parents.Add(new SiloItemResponse
                        {
                            Id = siloItem.Parent.Id.ToString(),
                            Index = siloItem.Parent.Index,
                            Name = siloItem.Parent.Name,
                            ItemType = siloItem.Parent.ItemType,
                            Feature = siloItem.Parent.Feature,
                        });
                        break;
                    case SiloItemTypeEnum.Cable:
                        parents.Add(new SiloItemResponse
                        {
                            Id = siloItem.Parent.Parent.Id.ToString(),
                            Index = siloItem.Parent.Parent.Index,
                            Name = siloItem.Parent.Parent.Name,
                            ItemType = siloItem.Parent.Parent.ItemType,
                            Feature = siloItem.Parent.Parent.Feature,
                        });
                        parents.Add(new SiloItemResponse
                        {
                            Id = siloItem.Parent.Id.ToString(),
                            Index = siloItem.Parent.Index,
                            Name = siloItem.Parent.Name,
                            ItemType = siloItem.Parent.ItemType,
                            Feature = siloItem.Parent.Feature,
                        });
                        break;
                    case SiloItemTypeEnum.TempSensor:
                        if (siloItem.Feature == SensorFeatureEnum.None)
                        {
                            parents.Add(new SiloItemResponse
                            {
                                Id = siloItem.Parent.Parent.ParentId.ToString(),
                                Index = siloItem.Parent.Parent.Parent.Index,
                                Name = siloItem.Parent.Parent.Parent.Name,
                                ItemType = siloItem.Parent.Parent.Parent.ItemType,
                                Feature = siloItem.Parent.Parent.Parent.Feature,
                            });
                            parents.Add(new SiloItemResponse
                            {
                                Id = siloItem.Parent.Parent.Id.ToString(),
                                Index = siloItem.Parent.Parent.Index,
                                Name = siloItem.Parent.Parent.Name,
                                ItemType = siloItem.Parent.Parent.ItemType,
                                Feature = siloItem.Parent.Parent.Feature,
                            });
                            parents.Add(new SiloItemResponse
                            {
                                Id = siloItem.Parent.Id.ToString(),
                                Index = siloItem.Parent.Index,
                                Name = siloItem.Parent.Name,
                                ItemType = siloItem.Parent.ItemType,
                                Feature = siloItem.Parent.Feature,
                            });
                        }
                        else
                        {
                            parents.Add(new SiloItemResponse
                            {
                                Id = siloItem.Parent.Id.ToString(),
                                Index = siloItem.Parent.Index,
                                Name = siloItem.Parent.Name,
                                ItemType = siloItem.Parent.ItemType,
                                Feature = siloItem.Parent.Feature,
                            });

                        }
                        break;
                    case SiloItemTypeEnum.HumiditySensor:
                        parents.Add(new SiloItemResponse
                        {
                            Id = siloItem.Parent.Id.ToString(),
                            Index = siloItem.Parent.Index,
                            Name = siloItem.Parent.Name,
                            ItemType = siloItem.Parent.ItemType,
                            Feature = siloItem.Parent.Feature,
                        });

                        break;
                    default:
                        break;
                }
                return Ok(parents);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        /// <summary>
        /// Get history of siloItem by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="paged"></param>
        /// <returns>Paged list of SensorHistoryRespnse</returns>
        [HttpGet("{id}/History")]
        public async Task<IActionResult> GetHistoriesById(string id, string? query = null, int page = 0, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, bool paged = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid request id");

                var history = await _clientUnitOfWork.SiloItems.GetPagedSensorHistoryAsync(Guid.Parse(id),
                    selector: SelectExpressions.SensorHistories.SensorHistoryResponse,
                    query, startDate, endDate);

                return Ok(history);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        /// <summary>
        /// Get child items
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of SiloItemResponse</returns>
        [HttpGet("{id}/Child")]
        public async Task<IActionResult> GetChildById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid request id");

                var siloItems = await _clientUnitOfWork.SiloItems.GetAllAsync(predicate: x => x.ParentId.ToString().Equals(id),
                    selector: SelectExpressions.SiloItems.GetSiloItemResponse);

                return Ok(siloItems.ToList());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }
        /// <summary>
        /// Get Parent item
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SiloItemResponse</returns>
        [HttpGet("{id}/Parent")]
        public async Task<IActionResult> GetParentById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid request id");

                var siloItem = await _clientUnitOfWork.SiloItems.FindAsync(Guid.Parse(id));
                if (siloItem == null)
                    return NotFound("SiloItem not found.");

                var parent = await _clientUnitOfWork.SiloItems.GetFirstOrDefaultAsync(predicate: x => x.Id == siloItem.ParentId,
                    selector: SelectExpressions.SiloItems.FullSiloItemResponse);

                return Ok(parent);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        /// <summary>
        /// Get SiloItem by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SiloItemResponse</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSiloItemById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid request id");

                var siloItem = await _clientUnitOfWork.SiloItems.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(id),
                    selector: t => new SiloItemResponse
                    {
                        Id = t.Id.ToString(),
                        Name = t.Name,
                        IsExpanded = t.IsExpanded,
                        AirCondition = t.AirCondition,
                        Feature = t.Feature,
                        IsMapped = t.IsMapped,
                        ItemType = t.ItemType,
                        ParentId = t.ParentId.ToString(),
                        Value = t.Value,
                        WarningServiceType = t.WarningServiceType,
                        Index = t.Index,
                        Box = t.Box,
                        Line = t.Line,
                        Address = t.Address,
                        FalseValueCount = t.FalseValueCount,
                        IsReadOnly = t.IsReadOnly,
                        FalseValue = t.FalseValue,
                        ChildLimit = t.ChildLimit,
                        Description = t.Description,
                        IsActive = t.IsActive,
                        HighTemp = t.HighTemp,
                        LowTemp = t.LowTemp,
                        Parent = null,
                        Length = t.Length,
                        SensorSpace = t.SensorSpace,
                        Offset = t.Offset,
                        ReadIndex = t.ReadIndex,
                        ReadDateTime = t.ReadDateTime,
                        LoopDiameter = t.LoopDiameter,
                        SiloDiameter = t.SiloDiameter,
                        TechNo = t.TechNo,
                        SiloHeight = t.SiloHeight,
                        Rom = t.Rom,
                        FeaturedSensors = t.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                        {
                            Id = f.Id.ToString(),
                            Name = f.Name,
                            IsExpanded = f.IsExpanded,
                            AirCondition = f.AirCondition,
                            Feature = f.Feature,
                            IsMapped = f.IsMapped,
                            ItemType = f.ItemType,
                            ParentId = f.ParentId.ToString(),
                            Value = f.Value,
                            WarningServiceType = f.WarningServiceType,
                            Index = f.Index,
                            Box = f.Box,
                            Line = f.Line,
                            Address = f.Address,
                            FalseValueCount = f.FalseValueCount,
                            IsReadOnly = f.IsReadOnly,
                            FalseValue = f.FalseValue,
                            ChildLimit = f.ChildLimit,
                            Description = f.Description,
                            IsActive = f.IsActive,
                            HighTemp = f.HighTemp,
                            LowTemp = f.LowTemp,
                        }).OrderBy(o => o.Index).ToList(),
                        SiloItems = t.SiloItems.Where(x => x.Feature == SensorFeatureEnum.None).Select(l => new SiloItemResponse
                        {
                            Id = l.Id.ToString(),
                            Name = l.Name,
                            IsExpanded = l.IsExpanded,
                            AirCondition = l.AirCondition,
                            Feature = l.Feature,
                            IsMapped = l.IsMapped,
                            ItemType = l.ItemType,
                            ParentId = l.ParentId.ToString(),
                            Value = l.Value,
                            WarningServiceType = l.WarningServiceType,
                            Index = l.Index,
                            Box = l.Box,
                            Line = l.Line,
                            Address = l.Address,
                            FalseValueCount = l.FalseValueCount,
                            IsReadOnly = l.IsReadOnly,
                            FalseValue = l.FalseValue,
                            ChildLimit = l.ChildLimit,
                            Description = l.Description,
                            IsActive = l.IsActive,
                            HighTemp = l.HighTemp,
                            LowTemp = l.LowTemp,
                            Length = l.Length,
                            SensorSpace = l.SensorSpace,
                            Offset = l.Offset,
                            ReadIndex = l.ReadIndex,
                            ReadDateTime = l.ReadDateTime,
                            LoopDiameter = l.LoopDiameter,
                            SiloDiameter = l.SiloDiameter,
                            TechNo = l.TechNo,
                            SiloHeight = l.SiloHeight,
                            Rom = l.Rom,
                            Parent = new SiloItemResponse
                            {
                                Id = t.Id.ToString(),
                                Name = t.Name,
                                IsExpanded = t.IsExpanded,
                                AirCondition = t.AirCondition,
                                Feature = t.Feature,
                                IsMapped = t.IsMapped,
                                ItemType = t.ItemType,
                                ParentId = t.ParentId.ToString(),
                                Value = t.Value,
                                WarningServiceType = t.WarningServiceType,
                                Index = t.Index,
                                Box = t.Box,
                                Line = t.Line,
                                Address = t.Address,
                                FalseValueCount = t.FalseValueCount,
                                IsReadOnly = t.IsReadOnly,
                                FalseValue = t.FalseValue,
                                ChildLimit = t.ChildLimit,
                                Description = t.Description,
                                IsActive = t.IsActive,
                                HighTemp = t.HighTemp,
                                LowTemp = t.LowTemp,
                                Length = t.Length,
                                SensorSpace = t.SensorSpace,
                                Offset = t.Offset,
                                ReadIndex = t.ReadIndex,
                                ReadDateTime = t.ReadDateTime,
                                LoopDiameter = t.LoopDiameter,
                                SiloDiameter = t.SiloDiameter,
                                TechNo = t.TechNo,
                                SiloHeight = t.SiloHeight,
                                Rom = t.Rom,
                            },
                            FeaturedSensors = l.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                            {
                                Id = f.Id.ToString(),
                                Name = f.Name,
                                IsExpanded = f.IsExpanded,
                                AirCondition = f.AirCondition,
                                Feature = f.Feature,
                                IsMapped = f.IsMapped,
                                ItemType = f.ItemType,
                                ParentId = f.ParentId.ToString(),
                                Value = f.Value,
                                WarningServiceType = f.WarningServiceType,
                                Index = f.Index,
                                Box = f.Box,
                                Line = f.Line,
                                Address = f.Address,
                                FalseValueCount = f.FalseValueCount,
                                IsReadOnly = f.IsReadOnly,
                                FalseValue = f.FalseValue,
                                ChildLimit = f.ChildLimit,
                                Description = f.Description,
                                IsActive = f.IsActive,
                                HighTemp = f.HighTemp,
                                LowTemp = f.LowTemp,
                            }).OrderBy(o => o.Index).ToList(),
                            SiloItems = l.SiloItems.Select(c => new SiloItemResponse
                            {
                                Id = c.Id.ToString(),
                                Name = c.Name,
                                IsExpanded = c.IsExpanded,
                                AirCondition = c.AirCondition,
                                Feature = c.Feature,
                                IsMapped = c.IsMapped,
                                ItemType = c.ItemType,
                                ParentId = c.ParentId.ToString(),
                                Value = c.Value,
                                WarningServiceType = c.WarningServiceType,
                                Index = c.Index,
                                Box = c.Box,
                                Line = c.Line,
                                Address = c.Address,
                                FalseValueCount = c.FalseValueCount,
                                IsReadOnly = c.IsReadOnly,
                                FalseValue = c.FalseValue,
                                ChildLimit = c.ChildLimit,
                                Description = c.Description,
                                IsActive = c.IsActive,
                                HighTemp = c.HighTemp,
                                LowTemp = c.LowTemp,
                                Length = c.Length,
                                SensorSpace = c.SensorSpace,
                                Offset = c.Offset,
                                ReadIndex = c.ReadIndex,
                                ReadDateTime = c.ReadDateTime,
                                LoopDiameter = c.LoopDiameter,
                                SiloDiameter = c.SiloDiameter,
                                TechNo = c.TechNo,
                                SiloHeight = c.SiloHeight,
                                Rom = c.Rom,
                                Parent = new SiloItemResponse
                                {
                                    Id = l.Id.ToString(),
                                    Name = l.Name,
                                    IsExpanded = l.IsExpanded,
                                    AirCondition = l.AirCondition,
                                    Feature = l.Feature,
                                    IsMapped = l.IsMapped,
                                    ItemType = l.ItemType,
                                    ParentId = l.ParentId.ToString(),
                                    Value = l.Value,
                                    WarningServiceType = l.WarningServiceType,
                                    Index = l.Index,
                                    Box = l.Box,
                                    Line = l.Line,
                                    Address = l.Address,
                                    FalseValueCount = l.FalseValueCount,
                                    IsReadOnly = l.IsReadOnly,
                                    FalseValue = l.FalseValue,
                                    ChildLimit = l.ChildLimit,
                                    Description = l.Description,
                                    IsActive = l.IsActive,
                                    HighTemp = l.HighTemp,
                                    LowTemp = l.LowTemp,
                                    Length = l.Length,
                                    SensorSpace = l.SensorSpace,
                                    Offset = l.Offset,
                                    ReadIndex = l.ReadIndex,
                                    ReadDateTime = l.ReadDateTime,
                                    LoopDiameter = l.LoopDiameter,
                                    SiloDiameter = l.SiloDiameter,
                                    TechNo = l.TechNo,
                                    SiloHeight = l.SiloHeight,
                                    Rom = l.Rom,
                                },

                                FeaturedSensors = c.SiloItems.Where(x => x.Feature != SensorFeatureEnum.None).Select(f => new SiloItemResponse
                                {
                                    Id = f.Id.ToString(),
                                    Name = f.Name,
                                    IsExpanded = f.IsExpanded,
                                    AirCondition = f.AirCondition,
                                    Feature = f.Feature,
                                    IsMapped = f.IsMapped,
                                    ItemType = f.ItemType,
                                    ParentId = f.ParentId.ToString(),
                                    Value = f.Value,
                                    WarningServiceType = f.WarningServiceType,
                                    Index = f.Index,
                                    Box = f.Box,
                                    Line = f.Line,
                                    Address = f.Address,
                                    FalseValueCount = f.FalseValueCount,
                                    IsReadOnly = f.IsReadOnly,
                                    FalseValue = f.FalseValue,
                                    ChildLimit = f.ChildLimit,
                                    Description = f.Description,
                                    IsActive = f.IsActive,
                                    HighTemp = f.HighTemp,
                                    LowTemp = f.LowTemp,
                                    Length = f.Length,
                                    SensorSpace = f.SensorSpace,
                                    Offset = f.Offset,
                                    ReadIndex = f.ReadIndex,
                                    ReadDateTime = f.ReadDateTime,
                                    LoopDiameter = f.LoopDiameter,
                                    SiloDiameter = f.SiloDiameter,
                                    TechNo = f.TechNo,
                                    SiloHeight = f.SiloHeight,
                                    Rom = f.Rom,
                                }).OrderBy(o => o.Index).ToList(),
                                SiloItems = c.SiloItems.Select(s => new SiloItemResponse
                                {
                                    Id = s.Id.ToString(),
                                    Name = s.Name,
                                    IsExpanded = s.IsExpanded,
                                    AirCondition = s.AirCondition,
                                    Feature = s.Feature,
                                    IsMapped = s.IsMapped,
                                    ItemType = s.ItemType,
                                    ParentId = s.ParentId.ToString(),
                                    Value = s.Value,
                                    WarningServiceType = s.WarningServiceType,
                                    Index = s.Index,
                                    Box = s.Box,
                                    Line = s.Line,
                                    Address = s.Address,
                                    FalseValueCount = s.FalseValueCount,
                                    IsReadOnly = s.IsReadOnly,
                                    FalseValue = s.FalseValue,
                                    ChildLimit = s.ChildLimit,
                                    Description = s.Description,
                                    IsActive = s.IsActive,
                                    HighTemp = s.HighTemp,
                                    LowTemp = s.LowTemp,
                                    Length = s.Length,
                                    SensorSpace = s.SensorSpace,
                                    Offset = s.Offset,
                                    ReadIndex = s.ReadIndex,
                                    ReadDateTime = s.ReadDateTime,
                                    LoopDiameter = s.LoopDiameter,
                                    SiloDiameter = s.SiloDiameter,
                                    TechNo = s.TechNo,
                                    SiloHeight = s.SiloHeight,
                                    Rom = s.Rom,
                                    Parent = new SiloItemResponse
                                    {
                                        Id = c.Id.ToString(),
                                        Name = c.Name,
                                        IsExpanded = c.IsExpanded,
                                        AirCondition = c.AirCondition,
                                        Feature = c.Feature,
                                        IsMapped = c.IsMapped,
                                        ItemType = c.ItemType,
                                        ParentId = c.ParentId.ToString(),
                                        Value = c.Value,
                                        WarningServiceType = c.WarningServiceType,
                                        Index = c.Index,
                                        Box = c.Box,
                                        Line = c.Line,
                                        Address = c.Address,
                                        FalseValueCount = c.FalseValueCount,
                                        IsReadOnly = c.IsReadOnly,
                                        FalseValue = c.FalseValue,
                                        ChildLimit = c.ChildLimit,
                                        Description = c.Description,
                                        IsActive = c.IsActive,
                                        HighTemp = c.HighTemp,
                                        LowTemp = c.LowTemp,
                                        Length = c.Length,
                                        SensorSpace = c.SensorSpace,
                                        Offset = c.Offset,
                                        ReadIndex = c.ReadIndex,
                                        ReadDateTime = c.ReadDateTime,
                                        LoopDiameter = c.LoopDiameter,
                                        SiloDiameter = c.SiloDiameter,
                                        TechNo = c.TechNo,
                                        SiloHeight = c.SiloHeight,
                                        Rom = c.Rom,
                                    }
                                }).OrderBy(x => x.Index).ToList()
                            }).OrderBy(x => x.Index).ToList()
                        }).OrderBy(x => x.Index).ToList()
                    });

                if (siloItem is null) return NotFound("SiloItem not found.");
                return Ok(siloItem);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Get last Index
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentId"></param>
        /// <returns>int</returns>
        [HttpGet("LastIndex")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLastIndex([FromQuery] SiloItemTypeEnum type, [FromQuery] string? parentId = null)
        {
            try
            {
                int lastIndex = 0;

                lastIndex = await _clientUnitOfWork.SiloItems.MaxAsync(
                    predicate: x => x.ItemType == type && x.ParentId.ToString().Equals(parentId),
                    selector: s => s.Index);
                return Ok(lastIndex);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Get last address
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentId"></param>
        /// <returns>int</returns>
        [HttpGet("LastAddress")]
        public async Task<IActionResult> GetLastAddress([FromQuery] SiloItemTypeEnum type, [FromQuery] string? parentId = null)
        {
            try
            {
                var lastIndex = await _clientUnitOfWork.SiloItems.MaxAsync(
                    predicate: x => x.ItemType == type && x.ParentId.ToString().Equals( parentId),
                    selector: s => s.Address);
                return Ok(lastIndex);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        /// <summary>
        /// Add new siloItem. if count specified more than one item will be added.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>List of string</returns>
        [HttpPost]
        public async Task<IActionResult> AddSiloItem(SiloItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState.SelectMany(x => x.Value.Errors));
                List<SiloItem> siloItems = new();
                for (int i = 0; i < request.Count; i++)
                {
                    var siloItem = _mapper.Map<SiloItem>(request);
                    siloItem.Index = request.Index + i;
                    var item = await _clientUnitOfWork.SiloItems.AddAsync(siloItem);
                    siloItems.Add(item.Entity);
                }
                await _clientUnitOfWork.SaveChangesAsync();

                return Ok(siloItems.Select(x => x.Id.ToString()).ToList());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Delete siloItem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSiloItemById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return BadRequest("Invalid request id");
                var siloItem = await _clientUnitOfWork.SiloItems.FindAsync(Guid.Parse(id));
                if (siloItem is null) return NotFound("SiloItem not found.");

                var itemToDelete = _clientUnitOfWork.SiloItems.GetChildFlatten(Guid.Parse(id)).ToList();
                _clientUnitOfWork.SiloItems.Remove(siloItem);
                await _clientUnitOfWork.SaveChangesAsync();
                return Ok("SiloItem successfully deleted");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Delete All SiloItems
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Reset")]
        public async Task<IActionResult> DeleteAllSiloItems()
        {
            try
            {
                _clientUnitOfWork.SiloItems.RemoveAll();
                await _clientUnitOfWork.SaveChangesAsync();
                return Ok("All items successfully have been deleted.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Update siloItem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSiloItemById(string id, [FromBody] JsonPatchDocument<SiloItemRequest> request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id)) return BadRequest("Invalid request id");
                if (!ModelState.IsValid) return BadRequest(ModelState.SelectMany(x => x.Value.Errors));

                var siloItem = await _clientUnitOfWork.SiloItems.FindAsync(Guid.Parse(id));
                if (siloItem is null) return NotFound("User not found!");
                var requestToPatch = _mapper.Map<SiloItemRequest>(siloItem);
                request.ApplyTo(requestToPatch);
                _mapper.Map(requestToPatch, siloItem);

                await _clientUnitOfWork.SaveChangesAsync();
                return Ok("SiloItem successfully updated.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }


    }
}
