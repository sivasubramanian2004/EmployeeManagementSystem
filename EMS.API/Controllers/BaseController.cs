using EMS.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMS.API.Controllers
{
    // EMS.API/Controllers/BaseApiController.cs
    public class BaseController : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User identity not found in token.");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity in token.");

            return userId;
        }
       
    }
}
