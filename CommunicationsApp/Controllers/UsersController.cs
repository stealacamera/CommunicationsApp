using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.Users.Queries.QueryByEmail;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

//[Authorize]
public class UsersController : BaseController
{
    #region API
    [HttpPost("users/query")]
    public async Task<IList<User>> Query([FromQuery] string query)
    {
        QueryByEmailAndUsernameCommand command = new(query);
        return await Sender.Send(command);
    }
    #endregion
}
