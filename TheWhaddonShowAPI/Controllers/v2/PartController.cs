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
	public class PartController : ControllerBase
	{

		private readonly IServerAPIControllerService<PartUpdate> _controllerService;

		public PartController(IServerAPIControllerService<PartUpdate> controllerService)
		{
			_controllerService = controllerService;
			
		}


		// GET: api/v2/Part/latest/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E
		/// <summary>
		/// Gets the latest updates of the Part Id(s) passed in.
		/// </summary>
		/// <remarks>
		///
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Part/latest/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E'
		/// 
		/// 'api/v2/Part/latest/?ids=all'   will return the latest update for each and every Part.
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





		// GET: api/v2/Part/history/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E
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
		/// 'api/v2/Part/history/?ids=all'   will return all updates for all Parts.
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






		// GET: api/v2/Part/conflicts/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E
		/// <summary>
		/// Gets all of the updates conflicting with the latest update for the PartId(s) passed in.
		/// </summary>
		/// <remarks>
		/// 
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Part/conflicts/?ids=68417C12-80C3-48BC-8EBE-3F3F2A91B8E5,17822466-DD66-4F2D-B4A9-F7EAAD6EB08B,F380FD46-6E6E-450D-AD3E-23EEC0B6A75E'
		/// 
		/// 'api/v2/Part/conflicts/?ids=all'    will return all currently conflicted updates for all Parts.
		/// 
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if none of the Parts have conflicting updates.
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

		// GET api/v2/Part/unsynced/27fc9657-3c92-6758-16a6-b9f82ca696b3
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
		public async Task<IActionResult> GetUnsynced([FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUnsyncedUpdates(copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}


		// POST api/v2/Part/updates
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
		/// The CopyId of the local storage copy must be passed in the uri. e.g.   'api/v2/Part/updates/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// 
		/// Json Text containing all properties of the update to be made must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// [
		///
		///         {         
		///             "Id":"08a0b93a-ee8f-4d4a-a120-d9ee6a2817dc",
		///             
		///             "Created":"2023-05-15T12:16:10",
		///             
		///             "CreatedBy":"mcarter",
		///             
		///             "UpdatedOnServer":"",
		///             
		///             "IsConflicted":false,
		///             
		///             "IsActive":false,
		///             
		///             "IsSample":true,
		///             
		///             "Name": "Rodney",
		///             
		///             "PersonId" : null,
		///             
		///              "Tags":["Male","Test"],
		///             
		///             },
		///             
		///          {   
		///             
		///             "Id":"0b64f14f-3725-41e1-b981-778acab1ad8c",
		///             
		///             "Created":"2023-05-15T12:16:11",
		///             
		///             "CreatedBy":"mcarter",
		///                          
		///             "UpdatedOnServer":""
		///             
		///             "IsConflicted":false,
		///             
		///             "IsActive":true
		///             
		///             "IsSample" : true
		/// 
		///             "Name":"Uncle Albert",
		///             
		///             "PersonId":"bb2f3007-9c4c-4f41-a360-d1ea59f26f04",
		///             
		///             "Tags":[],
		///             } 
		///             
		/// ]
		/// The API will return ServerToLocalPostBack info in json that should be used to update local storage and confirm the save to server was successful.
		///  
		/// </remarks>
		[HttpPost("updates/{copyId}")]
		//[Authorize (Roles = Roles.Write)]
		public async Task<IActionResult> Post([FromBody] List<PartUpdate> updates, [FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PostUpdates(updates, copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };

		}

		// PUT api/v2/Part/conflicts/clear
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
		/// All Updates relating to the Ids passed in will have IsConfliced set to false.
		/// 
		/// </remarks>
		[HttpPut("conflicts/clear")]
       // [Authorize(Roles = Roles.Write)]
		public async Task<IActionResult> PutConflicts([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PutClearConflicts(ids);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}





		// PUT api/v2/Part/updates/postbackfromlocal/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// <summary>
		/// Updates server to confirm the ids and created data have been successfully copied to local.       (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		///<remarks>
		///
		/// Authorisation is required to write to the central database. Use Contact above to obtain relevant ClientIds etc.
		/// 
		/// LocalToServerPostBacks come from saves to Local Storage as part of the syncing process and confirm that the save to local has been successful.
		/// 
		/// The CopyId of the local storage copy must be passed in the URL. e.g.   'api/v2/Part/updates/postbackfromlocal/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// 
		/// [
		///  {
		///    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
		///    "created": "2023-06-15T09:34:32.939Z",
		///    "isConflicted": true
		///  }
		/// ]
		/// 
		/// Json Text containing LocalToServerPostBacks must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// </remarks>
		[HttpPut("updates/postbackfromlocal/{copyId}")]
//[Authorize(Roles = Roles.Write)]
		public async Task<IActionResult> PutPostBackFromLocal([FromRoute] Guid copyId, [FromBody] List<LocalToServerPostBack> postBacks)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PutPostBackToServer(postBacks, copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		//TODO - Check what happens if putpostbackfromlocal happens twice - errored out in api.


		/// <summary>
		/// Resets Sample Data for PartUpdates. (Deleting all PartUpdate where IsSample = 1 and adding back in the default Sample Data)
		/// </summary>
		/// <returns></returns>
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpDelete("resetsampledata")]
		[Authorize]
		public async Task<IActionResult> ResetSampleData()
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.ResetSampleData();

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		////// DELETE api/v2/Part/
		////[HttpDelete("{updates}")]
		////public void Delete([FromBody] string updates)
		////{

		////    List<PartUpdate> partUpdates = JsonSerializer.Deserialize<List<PartUpdate>>(updates) ?? new List<PartUpdate>();

		////    _serverDataAccess.DeleteFromServer(partUpdates);
		////}
	}
}
