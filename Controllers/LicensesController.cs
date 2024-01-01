using Microsoft.AspNetCore.Mvc;
using UpdateNLicenceManagerAPI.Models;
using MongoDB.Driver;
// Gerekli using ifadeleri...

[ApiController]
[Route("[controller]")]
public class LicensesController : ControllerBase
{
    private readonly IMongoCollection<License> _licenses;

    public LicensesController(IMongoClient client)
    {
        var database = client.GetDatabase("your-database-name"); //TODO DB adı girilecek
        _licenses = database.GetCollection<License>("Licenses");
    }


    // POST: api/licenses/getbyid
    [HttpPost("getbyid")]
    public async Task<ActionResult<License>> GetById([FromBody] string id)
    {
        var license = await _licenses.Find<License>(l => l.Id == id).FirstOrDefaultAsync();
        if (license == null)
        {
            return NotFound();
        }
        return license;
    }

    // POST: api/licenses/update
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] License updateLicense)
    {
        var result = await _licenses.ReplaceOneAsync(l => l.Id == updateLicense.Id, updateLicense);
        if (result.IsAcknowledged && result.ModifiedCount > 0)
        {
            return Ok();
        }
        return NotFound();
    }

    // POST: api/licenses/delete
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] string id)
    {
        var result = await _licenses.DeleteOneAsync(l => l.Id == id);
        if (result.IsAcknowledged && result.DeletedCount > 0)
        {
            return Ok();
        }
        return NotFound();
    }

    // POST: api/licenses/checkactive
    [HttpPost("checkactive")]
    public async Task<ActionResult<bool>> CheckActive([FromBody] string key)
    {
        var license = await _licenses.Find<License>(l => l.LicenseKey == key).FirstOrDefaultAsync();
        return license != null && license.IsActive;
    }
}
