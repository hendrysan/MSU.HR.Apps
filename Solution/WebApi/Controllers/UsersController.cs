using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Repositories.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterResponse([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _userRepository.Register(new RegisterRequest()
                {
                    UserInput = request.UserInput,
                    FullName = request.FullName,
                    Password = request.Password,
                });


                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
