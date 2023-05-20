using Grip.Bll.DTO;
using Grip.Bll.Services.Interfaces;
using Grip.DAL;
using Grip.DAL.Model;
using Grip.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace Grip.Controllers;

/// <summary>
/// Represents a controller for managing stations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StationController : ControllerBase
{
    private readonly IStationService _stationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="StationController"/> class.
    /// </summary>
    /// <param name="stationService">The service for managing stations.</param>
    public StationController( IStationService stationService)
    {
        _stationService = stationService;
    }

    /// <summary>
    /// Gets the secret key for a specific station.
    /// </summary>
    /// <param name="stationNumber">The number of the station.</param>
    /// <returns>An <see cref="ActionResult{T}"/> representing the result of the operation.</returns>
    /// <remarks>
    /// This function is accessible via HTTP GET request.
    /// It requires the station number to be provided as a route parameter.
    /// The response HTTP message is not chunked.
    /// The API key is validated using the ValidateApiKey middleware.
    /// Returns 200 OK if the operation is successful.
    /// Returns 400 Bad Request if the station number is invalid.
    /// Returns 401 Unauthorized if the API key is not valid.
    /// </remarks>
    [NotChunkedAttribute]
    [ValidateApiKeyAttribute]
    [HttpGet("{StationNumber}/SecretKey")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StationSecretKeyDTO>> GetKey([FromRoute] int StationNumber)
    {
        return Ok(await _stationService.GetSecretKey(StationNumber));
    }
}
