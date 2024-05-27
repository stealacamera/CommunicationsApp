using CommunicationsApp.Application.Behaviour.Operations.Users.Queries.QueryByEmailAndUsername;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

[Authorize]
public class UsersController : BaseController
{
    public UsersController(IConfiguration configuration) : base(configuration)
    {
    }

    #region API
    [HttpPost("users/query")]
    public async Task<IActionResult> Query(string query, bool excludeRequester = true)
    {
        QueryByEmailAndUsernameQuery command = new(query, excludeRequester ? GetCurrentUserId() : null);
        return Ok(await Sender.Send(command));
    }
    #endregion
}
