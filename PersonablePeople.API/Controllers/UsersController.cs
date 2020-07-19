using System;
using System.Collections.Generic;
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
    public class UsersController : ControllerBase
    {
        private UserService UserService { get; }

        public UsersController(UserService userService)
        {
            UserService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers()
        {
            var foundContactsResult = await UserService.GetAllUsers();

            switch (foundContactsResult)
            {
                case BadRequestTypedResult<IEnumerable<UserOutDto>> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<IEnumerable<UserOutDto>> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case SuccessfulTypedResult<IEnumerable<UserOutDto>> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundContactsResult)));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<UserOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("get")]
        public async Task<IActionResult> GetUsersPost([FromBody] GetUserFilter getUserFilter)
        {
            var foundContactsResult = await UserService.GetUsers(getUserFilter);

            switch (foundContactsResult)
            {
                case BadRequestTypedResult<IEnumerable<UserOutDto>> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<IEnumerable<UserOutDto>> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<IEnumerable<UserOutDto>> _:
                    return NotFound();
                case SuccessfulTypedResult<IEnumerable<UserOutDto>> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundContactsResult)));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{userId:Guid}")]
        public async Task<IActionResult> GetSingleUser(Guid userId)
        {
            var foundLeadsResult = await UserService.GetUser(userId);

            switch (foundLeadsResult)
            {
                case BadRequestTypedResult<UserOutDto> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<UserOutDto> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<UserOutDto> _:
                    return NotFound();
                case SuccessfulTypedResult<UserOutDto> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(foundLeadsResult)));
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{userId:Guid}")]
        public async Task<IActionResult> UpdateContact(Guid userId, UpdateUserDtoIn updateUserIn)
        {
            var newLeadResult = await UserService.UpdateUser(userId, updateUserIn);

            switch (newLeadResult)
            {
                case BadRequestTypedResult<UserOutDto> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<UserOutDto> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<UserOutDto> _:
                    return NotFound();
                case SuccessfulTypedResult<UserOutDto> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(newLeadResult)));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<UserOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser(UpdateUserDtoIn updateUserIn)
        {
            var newLeadResult = await UserService.CreateUser(updateUserIn);

            switch (newLeadResult)
            {
                case BadRequestTypedResult<UserOutDto> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<UserOutDto> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<UserOutDto> _:
                    return NotFound();
                case SuccessfulTypedResult<UserOutDto> successfulTypedResult:
                    return Ok(successfulTypedResult.Value);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(newLeadResult)));
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<UserOutDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestOutDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{userId:Guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var newLeadResult = await UserService.DeleteUser(userId);

            switch (newLeadResult)
            {
                case BadRequestTypedResult<bool> badRequestTypedResult:
                    return BadRequest(badRequestTypedResult.Problem);
                case FailedTypedResult<bool> failedTypedResult:
                    return StatusCode(StatusCodes.Status500InternalServerError, failedTypedResult.Error);
                case NotFoundTypedResult<bool> _:
                    return NotFound();
                case SuccessfulTypedResult<bool> _:
                    return NoContent();
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, new ArgumentOutOfRangeException(nameof(newLeadResult)));
            }
        }
    }

    public class UpdateUserDtoIn
    {
        public NameEntity Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<UserRoles> Roles { get; set; }
        public Guid? ReportingTo { get; set; }
        public UserStatus Status { get; set; }
    }

    public class GetUserFilter
    {
        public string FirstNameLike { get; set; }
        public string LastNameLike { get; set; }
        public IEnumerable<UserRoles> Roles { get; set; }
        public Guid? ReportsTo { get; set; }
    }
}
