using Dashboard_Backend.Database;
using Dashboard_Backend.Database.Models;
using Dashboard_Backend.Helpers;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard_Backend.Controllers.v1;

[ApiController, Route("api/v1/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly DatabaseMain _database;
    private readonly AccessRightCheck _accessRightCheck;

    public DeviceController(DatabaseMain database, AccessRightCheck accessRightCheck)
    {
        _database = database;
        _accessRightCheck = accessRightCheck;
    }

    [HttpPost("new"), JwtAuthentication]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(typeof(ConflictResult), 409)]
    [ProducesResponseType(typeof(StatusCodeResult), 500)]
    public async Task<IActionResult> NewAsync([FromBody] Device newDevice)
    {
        var checkKvp =
            await _accessRightCheck.CheckAccessRights(HttpContext.Items["Validated_Token"] as ValidatedToken);
        
        if (!checkKvp.Key || checkKvp.Value == null)
        {
            return BadRequest(new
            {
                status = 400,
                message = "The authorization header was empty or the logged in user does not exist!"
            });
        }

        var dbDevice = new DeviceModel()
        {
            DeviceName = newDevice.DeviceName,
            DeviceType = newDevice.DeviceType,
            UId = checkKvp.Value.UId
        };

        var insertedId = Convert.ToInt32(await _database.InsertWithIdentityAsync(dbDevice));

        dbDevice.DId = insertedId;
        
        return insertedId != 0 ? Ok(dbDevice) : StatusCode(500);
    }

    [HttpGet("{id}"), JwtAuthentication]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(typeof(ConflictResult), 409)]
    [ProducesResponseType(typeof(StatusCodeResult), 500)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        return Ok();
    }
}