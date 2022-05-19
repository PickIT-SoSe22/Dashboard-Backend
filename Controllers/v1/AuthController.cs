using System.Text;
using Dashboard_Backend.Database;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard_Backend.Controllers.v1;

[ApiController, Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Generator _generator;
        private readonly DatabaseMain _database;

        public AuthController(Generator generator, DatabaseMain database)
        {
            _generator = generator;
            _database = database;
        }

        /// <summary>
        /// Creates a new JWT for a user based on their username and password
        /// </summary>
        /// <param name="newUser">Object containing username and password</param>
        /// <returns></returns>
        [HttpPost("new")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ConflictResult), 409)]
        [ProducesResponseType(typeof(StatusCodeResult), 500)]
        public async Task<IActionResult> NewAsync([FromBody] AuthNew newUser)
        {
            var user = await _database.Users.FirstOrDefaultAsync(u => u.Username == newUser.Username);
            if (user != null)
                return Conflict(new
                {
                    status = 409,
                    message = "A user with that name already exists!"
                });

            var salt = Encoding.UTF8.GetBytes(RandomHelper.RandomString());
            var pwHash = PasswordHelper.Hash(newUser.Password, salt);

            var inserted = await _database.InsertAsync(new UserModel
            {
                Username = newUser.Username,
                PasswordHash = pwHash,
                Salt = salt
            });

            if (inserted != 1)
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Account could not be created!"
                });

            var token = _generator.GenerateToken(newUser.Username);
            return Ok(new TokenResponse
            {
                Token = $"bearer {token}"
            });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        public async Task<IActionResult> LoginAsync([FromBody] AuthNew loginUser)
        {
            var user = await _database.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username);
            if (user == null)
                return NotFound(new
                {
                    status = 404,
                    message = "A user with that could not be found!"
                });

            var isSame = PasswordHelper.ConfirmPassword(user.PasswordHash, loginUser.Password, user.Salt);

            if (!isSame)
                return Unauthorized(new
                {
                    status = 401,
                    message = "Username or password are wrong!"
                });

            var token = _generator.GenerateToken(loginUser.Username);
            return Ok(new TokenResponse
            {
                Token = $"bearer {token}"
            });
        }

        /// <summary>
        /// Validates a JWT that is located in the Authorization header
        /// </summary>
        /// <param name="authorization">The JWT</param>
        /// <returns></returns>
        [HttpGet("validate"), JwtAuthentication]
        [ProducesResponseType(typeof(ValidatedToken), 200)]
        [ProducesResponseType(typeof(ValidatedToken), 409)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        public IActionResult Validate()
        {
            if (HttpContext.Items["Validated_Token"] is not ValidatedToken token)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "The authorization header was empty!"
                });
            }
            
            return Ok(token);
        }
    }