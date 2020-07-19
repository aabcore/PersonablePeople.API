using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonablePeople.API.Models;
using PersonablePeople.API.Models.ApiDtos;
using PersonablePeople.API.Models.Entities;
using PersonablePeople.API.Results;
using PersonablePeople.API.Services;

namespace PersonablePeople.API.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private LeadService LeadService { get; }

        public LeadsController(LeadService leadService)
        {
            LeadService = leadService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LeadOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLeads(bool converted)
        {
            var getLeadFilter = new GetLeadFilter()
            {
                Converted = converted
            };
            var foundLeadsResult = await LeadService.GetLeads(getLeadFilter);

            switch (foundLeadsResult)
            {
                case BadRequestTypedResult<IEnumerable<LeadOutDto>> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<IEnumerable<LeadOutDto>> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<IEnumerable<LeadOutDto>> _:
                    return NotFound();
                case SuccessfulTypedResult<IEnumerable<LeadOutDto>> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundLeadsResult)));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<LeadOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("get")]
        public async Task<IActionResult> GetLeadsPost([FromBody] GetLeadFilter getLeadFilter)
        {
            var foundLeadsResult = await LeadService.GetLeads(getLeadFilter);

            switch (foundLeadsResult)
            {
                case BadRequestTypedResult<IEnumerable<LeadOutDto>> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<IEnumerable<LeadOutDto>> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<IEnumerable<LeadOutDto>> _:
                    return NotFound();
                case SuccessfulTypedResult<IEnumerable<LeadOutDto>> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundLeadsResult)));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(LeadOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{recordId:Guid}")]
        public async Task<IActionResult> GetSingleLead(Guid recordId)
        {
            var foundLeadsResult = await LeadService.GetLead(recordId);

            switch (foundLeadsResult)
            {
                case BadRequestTypedResult<LeadOutDto> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<LeadOutDto> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<LeadOutDto> _:
                    return NotFound();
                case SuccessfulTypedResult<LeadOutDto> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundLeadsResult)));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<LeadOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateLead(NewLeadDtoIn newLeadIn)
        {
            var newLeadResult = await LeadService.CreateLead(newLeadIn);

            switch (newLeadResult)
            {
                case BadRequestTypedResult<LeadOutDto> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<LeadOutDto> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case SuccessfulTypedResult<LeadOutDto> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(newLeadResult)));
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(IEnumerable<LeadOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{recordId:Guid}")]
        public async Task<IActionResult> UpdateLead(Guid recordId, NewLeadDtoIn newLeadIn)
        {
            var newLeadResult = await LeadService.UpdateLead(recordId, newLeadIn);

            switch (newLeadResult)
            {
                case BadRequestTypedResult<LeadOutDto> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<LeadOutDto> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<LeadOutDto> _:
                    return NotFound();
                case SuccessfulTypedResult<LeadOutDto> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(newLeadResult)));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<ContactOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{recordId:Guid}/convert")]
        public async Task<IActionResult> ConvertLead(Guid recordId, ConvertLeadDtoIn convertLeadDtoIn)
        {
            var newLeadResult = await LeadService.ConvertLead(recordId, convertLeadDtoIn);

            switch (newLeadResult)
            {
                case BadRequestTypedResult<ContactOutDto> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<ContactOutDto> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<ContactOutDto> _:
                    return NotFound();
                case SuccessfulTypedResult<ContactOutDto> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(newLeadResult)));
            }
        }
    }

    public class ConvertLeadDtoIn
    {
        [Required]
        public Guid ModifiedBy { get; set; }
    }

    public class NewLeadDtoIn
    {
        public AddressDtoOut Address { get; set; }

        [Required]
        public NameDtoOut Name { get; set; }

        [Required]
        public Guid ModifiedBy { get; set; }

        public decimal? AnnualSalary { get; set; }
        public string LeadSource { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public ContactInfoDtoOut ContactInfo { get; set; }
    }

    public class GetLeadFilter
    {
        public DateTimeOffset? CreatedTimeAfter { get; set; }
        public DateTimeOffset? CreatedTimeBefore { get; set; }
        public DateTimeOffset? ModifiedTimeAfter { get; set; }
        public DateTimeOffset? ModifiedTimeBefore { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public bool? Converted { get; set; }
    }
}