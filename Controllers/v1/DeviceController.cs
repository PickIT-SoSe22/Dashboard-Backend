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

    /// <summary>
    /// Create a new device
    /// </summary>
    /// <param name="newDevice"></param>
    /// <returns></returns>
    [HttpPost("new"), JwtAuthentication]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(typeof(ConflictResult), 409)]
    [ProducesResponseType(typeof(StatusCodeResult), 500)]
    public async Task<IActionResult> NewAsync([FromBody] Device newDevice)
    {
        var (valid, user) = await _accessRightCheck.CheckAccessRights(HttpContext.Items["Validated_Token"] as ValidatedToken);
        
        if (!valid || user == null)
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
            UId = user.UId
        };

        var insertedId = Convert.ToInt32(await _database.InsertWithIdentityAsync(dbDevice));

        dbDevice.DId = insertedId;
        
        return insertedId != 0 ? Ok(dbDevice) : StatusCode(500);
    }
    
    /// <summary>
    /// Get all devices of the user
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("all"), JwtAuthentication]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(typeof(ConflictResult), 409)]
    [ProducesResponseType(typeof(StatusCodeResult), 500)]
    public async Task<IActionResult> GetAllAsync()
    {
        var (valid, user) = await _accessRightCheck.CheckAccessRights(HttpContext.Items["Validated_Token"] as ValidatedToken);
        
        if (!valid || user == null)
        {
            return BadRequest(new
            {
                status = 400,
                message = "The authorization header was empty or the logged in user does not exist!"
            });
        }

        var devices = await _database.Devices.Where(d => d.UId == user.UId).ToArrayAsync();
        if (!devices.Any())
        {
            return NotFound(new
            {
                status = 404,
                message = $"This user does not have any devices!"
            });
        }

        return Ok(devices);
    }

    /// <summary>
    /// Get a device by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}"), JwtAuthentication]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(typeof(ConflictResult), 409)]
    [ProducesResponseType(typeof(StatusCodeResult), 500)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var (valid, user) = await _accessRightCheck.CheckAccessRights(HttpContext.Items["Validated_Token"] as ValidatedToken);
        
        if (!valid || user == null)
        {
            return BadRequest(new
            {
                status = 400,
                message = "The authorization header was empty or the logged in user does not exist!"
            });
        }

        var device = await _database.Devices.FirstOrDefaultAsync(d => d.DId == id);
        if (device == null)
        {
            return NotFound(new
            {
                status = 404,
                message = $"The device with ID {id} could not be found!"
            });
        }

        if (device.UId != user.UId)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "You are not allowed to view this device!"
            });
        }
        
        return Ok(device);
    }

    /// <summary>
    /// Delete a device by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}"), JwtAuthentication]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(typeof(ConflictResult), 409)]
    [ProducesResponseType(typeof(StatusCodeResult), 500)]
    public async Task<IActionResult> DeleteByIdAsync(int id)
    {
        var (valid, user) = await _accessRightCheck.CheckAccessRights(HttpContext.Items["Validated_Token"] as ValidatedToken);
        
        if (!valid || user == null)
        {
            return BadRequest(new
            {
                status = 400,
                message = "The authorization header was empty or the logged in user does not exist!"
            });
        }

        var device = await _database.Devices.FirstOrDefaultAsync(d => d.DId == id);
        if (device == null)
        {
            return NotFound(new
            {
                status = 404,
                message = $"The device with ID {id} could not be found!"
            });
        }

        if (device.UId != user.UId)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "You are not allowed to view this device!"
            });
        }

        var del = await _database.DeleteAsync(device);
        
        return Ok(new
        {
            status = 200,
            message = $"Deleted device with ID {id}"
        });
    }

    /// <summary>
    /// Update a device by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:int}"), JwtAuthentication]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(typeof(ConflictResult), 409)]
    [ProducesResponseType(typeof(StatusCodeResult), 500)]
    public async Task<IActionResult> UpdateByIdAsync(int id, [FromBody] Device updatedDevice)
    {
        var (valid, user) = await _accessRightCheck.CheckAccessRights(HttpContext.Items["Validated_Token"] as ValidatedToken);
        
        if (!valid || user == null)
        {
            return BadRequest(new
            {
                status = 400,
                message = "The authorization header was empty or the logged in user does not exist!"
            });
        }

        var device = await _database.Devices.FirstOrDefaultAsync(d => d.DId == id);
        if (device == null)
        {
            return NotFound(new
            {
                status = 404,
                message = $"The device with ID {id} could not be found!"
            });
        }

        if (device.UId != user.UId)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "You are not allowed to view this device!"
            });
        }

        device.DeviceName = updatedDevice.DeviceName;
        device.DeviceType = updatedDevice.DeviceType;

        var updated = await _database.UpdateAsync(device);

        return Ok(new
        {
            status = 200,
            message = $"Updated device with Id {id}",
            updatedDevice = device
        });
    }
}