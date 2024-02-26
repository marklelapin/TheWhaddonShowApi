using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using MyClassLibrary.LocalServerMethods.Interfaces;
using MyClassLibrary.LocalServerMethods.Models;
using System.Net;
using TheWhaddonShowClassLibrary.Models;



namespace TheWhaddonShowAPI.Controllers.v2
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersion("2.0")]
	public class ScriptItemController : ControllerBase
	{

		private readonly IServerAPIControllerService<ScriptItemUpdate> _controllerService;

		public ScriptItemController(IServerAPIControllerService<ScriptItemUpdate> controllerService)
		{
			_controllerService = controllerService;
		}

		// GET: api/v2/ScriptItem/latest/?ids=FC97305D-8A92-42D5-94DB-6FC9F5FF1432,744BD79A-1A2B-425F-874F-315A3B3BA9F2,79E604CF-7CC2-41F6-B37F-F30C76AB5F34
		/// <summary>
		/// Gets the latest updates of the ScriptItem Id(s) passed in.
		/// </summary>
		/// <remarks>
		///
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/ScriptItem/latest/?ids=FC97305D-8A92-42D5-94DB-6FC9F5FF1432,744BD79A-1A2B-425F-874F-315A3B3BA9F2,79E604CF-7CC2-41F6-B37F-F30C76AB5F34'
		/// 
		/// 'api/v2/ScriptItem/latest/?ids=all'   will return the latest update for each and every ScriptItem.
		/// 
		///  
		/// 
		/// The API will respond with a 404 Not Found error if no scriptItems relate to the Ids given.
		/// 
		/// Otherwise it will return a json string of ScriptItemUpdates.
		/// 
		/// </remarks>
		[HttpGet("latest")]
		public async Task<IActionResult> GetLatest([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUpdates(ids, true);
			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}


		// GET: api/v2/ScriptItem/history/?ids=FC97305D-8A92-42D5-94DB-6FC9F5FF1432,744BD79A-1A2B-425F-874F-315A3B3BA9F2,79E604CF-7CC2-41F6-B37F-F30C76AB5F34
		/// <summary>
		/// Gets all of the updates made to a ScriptItem(s) passed in.
		/// </summary>
		/// <remarks>
		/// This gives a history of all updates made to the ScriptItem(s).
		/// 
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/ScriptItem/history/?ids=FC97305D-8A92-42D5-94DB-6FC9F5FF1432,744BD79A-1A2B-425F-874F-315A3B3BA9F2,79E604CF-7CC2-41F6-B37F-F30C76AB5F34'
		/// 
		/// 'api/v2/ScriptItem/history/?ids=all'   will return all updates for all ScriptItems.
		/// 
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if no scriptItems relate to the Ids given.
		/// 
		/// Otherwise it will return a json string of ScriptItemUpdates.
		/// 
		/// </remarks>
		[HttpGet("history")]
		public async Task<IActionResult> GetHistory([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUpdates(ids, false);
			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// GET: api/v2/ScriptItem/conflicts/?ids=ED789FA3-4B2B-41A0-A322-773ED7CE89FE
		/// <summary>
		/// Gets all of the updates conflicting with the latest update for the ScriptItemId(s) passed in.
		/// </summary>
		/// <remarks>
		/// 
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/ScriptItem/conflicts/?ids=ED789FA3-4B2B-41A0-A322-773ED7CE89FE'
		/// 
		/// 'api/v2/ScriptItem/conflicts/?ids=all'    will return all currently conflicted updates for all ScriptItems.
		/// 
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if none of the ScriptItems have conflicting updates.
		/// 
		/// Otherwise it will return a json string of ScriptItemUpdates.
		/// 
		/// </remarks>
		[HttpGet("conflicts")]
		public async Task<IActionResult> GetConflicts([FromQuery] string? ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetConflictedUpdates(ids);
			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// GET api/v2/ScriptItem/unsynced/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// <summary>
		/// Gets all ScriptItem updates from the server that haven't been saved to the local copy.
		/// </summary>
		/// <remarks>
		/// A Guid (CopyId) identifying the unique local copy of the data needs to be passed in.
		/// 
		/// 'api/v2/ScriptItem/unsynced/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if no updates have been made since the local copy was last fully synced.
		/// 
		/// Otherwise it will return a json string of ScriptItemUpdates.
		/// </remarks>

		[HttpGet("unsynced/{copyId}")]
		public async Task<IActionResult> GetUnsynced([FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUnsyncedUpdates(copyId);
			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// POST api/v2/ScriptItem/updates
		/// <summary>
		/// Creates or Updates a ScriptItem(s) by posting a ScriptItemUpdate. (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		/// <remarks>
		/// 
		/// Authorisation is required to write to the central database. Use Contact above to recieve relevant ClientIds etc.
		/// 
		/// This method is how you create or update a ScriptItem since in both cases this is done by adding an adddtional ScriptItemUpdate that supercedes the current update in the system.
		/// If a new ScriptItem is being created a new Guid needs to be created for Id.
		/// 
		/// The CopyId of the local storage copy must be passed in the uri. e.g.   'api/v2/ScriptItem/updates/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// 
		/// Json Text containing all properties of the update to be made must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// [
		/// 
		///         {   "Id":"53017F40-0D56-45E4-99BF-2D551E31329E",
		///             
		///             "Created":"2023-05-15T13:16:10",
		///             
		///             "CreatedBy":"mcarter",
		///                                          
		///             "UpdatedOnServer":"",
		///             
		///             "IsConflicted":false,
		///             
		///             "IsSample": true,
		///                          
		///             "IsActive":true,
		///             
		///             "ParentId":"1A78F03A-6326-4D4A-BBD3-677CF75EC49C",
		///             
		///             "OrderNo" : 1
		///             
		///             "Type" : "Dialogue"
		/// 
		///             "Text" : "Good Morning Vietnam!"
		/// 
		///             "PartIds" : ["E9089F6B-691E-4284-8A49-5DB9DE4C4B42"]
		///             
		///             "Tags":["Trotter","Side"]
		///             
		///             }
		///             
		///          { "Id":"1A78F03A-6326-4D4A-BBD3-677CF75EC49C",
		///             
		///             "Created":"2023-05-15T14:16:10",
		///             
		///             "CreatedBy":"mcarter",
		///                                          
		///             "UpdatedOnServer":"",
		///             
		///             "IsConflicted":false,
		///             
		///             "IsSample": true,
		///                          
		///             "IsActive":true,
		///              
		///             "ParentId":"33F622F6-DD0C-4437-9598-B462BBAE1B70",
		///             
		///             "OrderNo" : 1
		///             
		///             "Type" : "Dialogue"
		/// 
		///             "Text" : "Go back to bed!"
		/// 
		///             "PartIds" : ["E9089F6B-691E-4284-8A49-5DB9DE4C4B42"]
		///             
		///             "Tags":["Trotter","Side"]
		///             
		///             } 
		///             
		/// ]
		/// The API will return ServerToLocalPostBack info in json that should be used to update local storage and confirm the save to server was successful.
		///  
		/// </remarks>
		[HttpPost("updates/{copyId}")]
		[Authorize]
		[RequiredScope("show.write")]
		public async Task<IActionResult> Post([FromBody] List<ScriptItemUpdate> updates, [FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PostUpdates(updates, copyId);
			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		// PUT api/v2/ScriptItem/conflicts/clear
		/// <summary>
		/// Changes all updates relating to the Id(s) passed in to IsConflicted = false.   (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		/// <remarks> 
		/// 
		/// Authorisation is required to write to the central database. Use Contact above to obtain relevant ClientIds etc.
		/// 
		/// A list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/ScriptItem/conflicts/clear/?ids=FC97305D-8A92-42D5-94DB-6FC9F5FF1432'
		/// 
		/// All Updates relating to the Ids passed in will have IsConfliced set to false.
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

		// PUT api/v2/ScriptItem/updates/postbackfromlocal/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// <summary>
		/// Updates server to confirm the ids and created data have been successfully copied to local.       (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		///<remarks>
		///
		/// Authorisation is required to write to the central database. Use Contact above to obtain relevant ClientIds etc.
		/// 
		/// LocalToServerPostBacks come from saves to Local Storage as part of the syncing process and confirm that the save to local has been successful.
		/// 
		/// The CopyId of the local storage copy must be passed in the URL. e.g.   'api/v2/ScriptItem/updates/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// Json Text containing LocalToServerPostBacks must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// </remarks>
		[HttpPut("updates/postbackfromlocal/{copyId}")]
		[Authorize]
		[RequiredScope("show.write")]
		public async Task<IActionResult> PutPostBackFromLocal([FromRoute] Guid copyId, [FromBody] List<LocalToServerPostBack> postBacks)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PutPostBackToServer(postBacks, copyId);
			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		//TODO - Add in LocalToServerPostBacks to comments above.



		/// <summary>
		/// Resets Sample Data for ScriptItemUpdates. (Deleting all PartUpdate where IsSample = 1 and adding back in the default Sample Data)
		/// </summary>
		/// <returns></returns>
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpDelete("resetsampledata")]
		[Authorize]
		[RequiredScope("show.write")]
		public async Task<IActionResult> ResetSampleData()
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.ResetSampleData();

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}



		////// DELETE api/v2/ScriptItem/
		////[HttpDelete("{updates}")]
		////public void Delete([FromBody] string updates)
		////{

		////    List<ScriptItemUpdate> scriptItemUpdates = JsonSerializer.Deserialize<List<ScriptItemUpdate>>(updates) ?? new List<ScriptItemUpdate>();

		////    _serverDataAccess.DeleteFromServer(scriptItemUpdates);
		////}
	}
}
