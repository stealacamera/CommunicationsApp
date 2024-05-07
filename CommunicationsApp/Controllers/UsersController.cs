using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

//[Authorize]
public class UsersController : BaseController
{
    #region API
    [HttpPost("users/query")]
    public async Task<IActionResult> Query([FromQuery] string query, bool excludeRequester = true)
    {
        QueryByEmailAndUsernameQuery command = new(query, excludeRequester ? GetCurrentUserId() : null);
        return Ok(await Sender.Send(command));
    }
    #endregion
}
