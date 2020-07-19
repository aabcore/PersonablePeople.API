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
using PersonablePeople.API.Results;
using PersonablePeople.API.Services;

namespace PersonablePeople.API.Controllers
{
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private ContactService ContactService { get; }

        public ContactsController(ContactService contactService)
        {
            ContactService = contactService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContacts()
        {
            var foundContactsResult = await ContactService.GetAllContacts();

            switch (foundContactsResult)
            {
                case BadRequestTypedResult<IEnumerable<ContactOutDto>> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<IEnumerable<ContactOutDto>> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case SuccessfulTypedResult<IEnumerable<ContactOutDto>> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundContactsResult)));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<ContactOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("get")]
        public async Task<IActionResult> GetContactsPost([FromBody] GetContactFilter getContactFilter)
        {
            var foundContactsResult = await ContactService.GetContacts(getContactFilter);

            switch (foundContactsResult)
            {
                case BadRequestTypedResult<IEnumerable<ContactOutDto>> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<IEnumerable<ContactOutDto>> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<IEnumerable<ContactOutDto>> _:
                    return NotFound();
                case SuccessfulTypedResult<IEnumerable<ContactOutDto>> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundContactsResult)));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ContactOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{recordId:Guid}")]
        public async Task<IActionResult> GetSingleContact(Guid recordId)
        {
            var foundLeadsResult = await ContactService.GetContact(recordId);

            switch (foundLeadsResult)
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
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundLeadsResult)));
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(ContactOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{recordId:Guid}")]
        public async Task<IActionResult> UpdateContact(Guid recordId, UpdateContactDtoIn newLeadIn)
        {
            var newLeadResult = await ContactService.UpdateContact(recordId, newLeadIn);

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

    public class UpdateContactDtoIn
    {
            public AddressDtoOut MailingAddress { get; set; }
            public AddressDtoOut OtherAddress { get; set; }
            public NameDtoOut PrimaryName { get; set; }
            public NameDtoOut SecondaryName { get; set; }

            [Required]
            public Guid ModifiedBy { get; set; }

            public decimal? AnnualSalary { get; set; }
            public IEnumerable<string> Tags { get; set; }
            public ContactInfoDtoOut PrimaryContactInfo { get; set; }
            public ContactInfoDtoOut SecondaryContactInfo { get; set; }
        
    }

    public class GetContactFilter
    {
        public DateTimeOffset? CreatedTimeAfter { get; set; }
        public DateTimeOffset? CreatedTimeBefore { get; set; }
        public DateTimeOffset? ModifiedTimeAfter { get; set; }
        public DateTimeOffset? ModifiedTimeBefore { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
