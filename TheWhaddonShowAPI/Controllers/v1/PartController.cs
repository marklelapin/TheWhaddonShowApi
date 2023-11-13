using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using MyClassLibrary.LocalServerMethods.Interfaces;
using MyClassLibrary.LocalServerMethods.Models;
using System.Net;
using TheWhaddonShowClassLibrary.Models;

//TODO change commentary on version 1 to v1 from v2 etc

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheWhaddonShowAPI.Controllers.v1
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersion("1.0")]
	public class PartController : ControllerBase
	{

		private readonly IServerAPIControllerService<PartUpdate> _controllerService;

		public PartController(IServerAPIControllerService<PartUpdate> controllerService)
		{
			_controllerService = controllerService;
		}



		// GET: api/Part/latest/?ids=0F93A0CF-F96E-4045-8CEB-12EDCAA3A15F,4384C339-F749-47A0-B684-C48C67F3C5D0
		/// <summary>
		/// Gets the latest updates of the Part Id(s) passed in.
		/// </summary>
		/// <remarks>
		///
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Part/latest/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E'
		/// 
		/// 'api/v2/Part/latest/?ids=all   will return the latest update for each and every Part.
		/// 
		///  
		/// 
		/// The API will respond with a 404 Not Found error if no parts relate to the Ids given.
		/// 
		/// Otherwise it will return a json string of PartUpdates.
		/// 
		/// </remarks>
		[HttpGet("latest")]
		public async Task<IActionResult> GetLatest([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUpdates(ids, true);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// GET: api/Part/history/?ids=0F93A0CF-F96E-4045-8CEB-12EDCAA3A15F,4384C339-F749-47A0-B684-C48C67F3C5D0
		/// <summary>
		/// Gets all of the updates made to a Part(s) passed in.
		/// </summary>
		/// <remarks>
		/// This gives a history of all updates made to the Part(s).
		/// 
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Part/history/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E'
		/// 
		/// 'api/v2/Part/history/?ids=all    will return all updatess for all Parts.
		/// 
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if no parts relate to the Ids given.
		/// 
		/// Otherwise it will return a json string of PartUpdates.
		/// 
		/// </remarks>
		[HttpGet("history")]
		public async Task<IActionResult> GetHistory([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUpdates(ids, false);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// GET: api/Part/conflicts/?ids=0F93A0CF-F96E-4045-8CEB-12EDCAA3A15F,4384C339-F749-47A0-B684-C48C67F3C5D0
		/// <summary>
		/// Gets all of the updates conflicting with the latest update for the PartId(s) passed in.
		/// </summary>
		/// <remarks>
		/// 
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Part/history/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E'
		/// 
		/// 'api/v2/Part/history/?ids=all    will return all currently conflicted updates for all Parts.
		/// 
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if none of the Parts have conflicting updaes.
		/// 
		/// Otherwise it will return a json string of PartUpdates.
		/// 
		/// </remarks>
		[HttpGet("conflicts")]
		public async Task<IActionResult> GetConflicts([FromQuery] string? ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetConflictedUpdates(ids);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// GET api/Part/unsynced/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// <summary>
		/// Gets all Part updates from the server that haven't been saved to the local copy.
		/// </summary>
		/// <remarks>
		/// A Guid (CopyId) identifying the unique local copy of the data needs to be passed in.
		/// 
		/// 'api/v2/Part/unsynced/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if no updates have been made since the local copy was last fully synced.
		/// 
		/// Otherwise it will return a json string of PartUpdates.
		/// </remarks>

		[HttpGet("unsynced/{copyId}")]
		public async Task<IActionResult> GetChanges([FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUnsyncedUpdates(copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// POST api/Part/updates
		/// <summary>
		/// Creates or Updates a Part(s) by posting a PartUpdate. (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		/// <remarks>
		/// 
		/// Authorisation is required to write to the central database. Use Contact above to recieve relevant ClientIds etc.
		/// 
		/// This method is how you create or update a Part since in both cases this is done by adding an adddtional PartUpdate that supercedes the current update in the system.
		/// If a new Part is being created a new Guid needs to be created for Id.
		/// 
		/// The CopyId of the local storage copy must be passed in the uri. e.g.   'api/Part/updates/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// 
		/// Json Text containing all properties of the update to be made must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// [
		/// 
		///         {   "Name":"Rodney",
		///         
		///             "Id":"08a0b93a-ee8f-4d4a-a120-d9ee6a2817dc",
		///             
		///             "PersonId":"b7b59aa8-34f7-4d2c-ac97-68a6efd4a3c9",
		///             
		///             "Tags":["Trotter","Side"],
		///             
		///             "ConflictId":null,
		///             
		///             "Created":"2023-05-15T12:16:10",
		///             
		///             "UpdatedOnServer":"",
		///             
		///             "CreatedBy":"mcarter",
		///             
		///             "IsActive":false
		///             
		///             },
		///             
		///          { "Name":"Uncle Albert",
		///          
		///             "PersonId":"bb2f3007-9c4c-4f41-a360-d1ea59f26f04",
		///             
		///             "Tags":[],
		///             
		///             "Id":"0b64f14f-3725-41e1-b981-778acab1ad8c",
		///             
		///             "ConflictId":null,
		///             
		///             "Created":"2023-05-15T12:16:11",
		///             
		///             "UpdatedOnServer":"",
		///             
		///             "CreatedBy":"mcarter",
		///             
		///             "IsActive":true
		///             
		///             } 
		///             
		/// ]
		/// The API will return ServerToLocalPostBack info in json that should be used to update local storage and confirm the save to server was successful.
		///  
		/// </remarks>
		[HttpPost("updates")]
		[Authorize]
		[RequiredScope("show.write")]
		public async Task<IActionResult> Post([FromBody] List<PartUpdate> updates, [FromQuery] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PostUpdates(updates, copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };

		}

		// PUT api/Part/conflicts/clear
		/// <summary>
		/// Changes all updates relating to the Id(s) passed in to IsConflicted = false.   (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		/// <remarks> 
		/// 
		/// Authorisation is required to write to the central database. Use Contact above to obtain relevant ClientIds etc.
		/// 
		/// A list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Part/conflicts/clear/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E'
		///
		/// </remarks>
		[HttpPut("conflicts/clear")]
		[Authorize]
		[RequiredScope("show.write")]
		public async Task<IActionResult> PutConflicts([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PutClearConflicts(ids);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// PUT api/Part/updates/postbackfromlocal/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// <summary>
		/// Updates server to confirm the ids and created data have been successfully copied to local.       (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		///<remarks>
		///
		/// Authorisation is required to write to the central database. Use Contact above to obtain relevant ClientIds etc.
		/// 
		/// The CopyId of the local storage copy must be passed in the uri. e.g.   'api/Part/updates/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// Json Text containing LocalToServerPostBacks must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// </remarks>
		[HttpPut("updates/postbackfromlocal")]
		[Authorize]
		[RequiredScope("show.write")]
		public async Task<IActionResult> PutPostBackFromLocal([FromBody] List<LocalToServerPostBack> postBacks, [FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PutPostBackToServer(postBacks, copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		//TODO - Add in LocalToServerPostBacks to comments above.





		////// DELETE api/Part/
		////[HttpDelete("{updates}")]
		////public void Delete([FromBody] string updates)
		////{

		////    List<PartUpdate> partUpdates = JsonSerializer.Deserialize<List<PartUpdate>>(updates) ?? new List<PartUpdate>();

		////    _serverDataAccess.DeleteFromServer(partUpdates);
		////}
	}
}
