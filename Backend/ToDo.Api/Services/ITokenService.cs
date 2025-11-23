using ToDo.DAL.Models;

namespace ToDo.Api.Services;

public interface ITokenService
{
    string CreateToken(AppUser user);
}

