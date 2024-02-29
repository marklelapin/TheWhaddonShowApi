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
	//[ApiVersion("2.0")]
	public class PersonController : ControllerBase
	{

		private readonly IServerAPIControllerService<PersonUpdate> _controllerService;

		public PersonController(IServerAPIControllerService<PersonUpdate> controllerService)
		{
			_controllerService = controllerService;
		}

		// GET: api/v2/Person/latest/?ids=545A9495-DB58-44EC-BA47-FD0B7E478D4A,2B3FA075-D0B5-49AB-B897-DAB1428CA500
		/// <summary>
		/// Gets the latest updates of the Person Id(s) passed in.
		/// </summary>
		/// <remarks>
		///
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Person/latest/?ids=545A9495-DB58-44EC-BA47-FD0B7E478D4A,2B3FA075-D0B5-49AB-B897-DAB1428CA500'
		/// 
		/// 'api/v2/Person/latest/?ids=all'   will return the latest update for each and every Person.
		/// 
		///  
		/// 
		/// The API will respond with a 404 Not Found error if no persons relate to the Ids given.
		/// 
		/// Otherwise it will return a json string of PersonUpdates.
		/// 
		/// </remarks>
		[HttpGet("latest")]
		public async Task<IActionResult> GetLatest([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUpdates(ids, true);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}





		// GET: api/v2/Person/history/?ids=545A9495-DB58-44EC-BA47-FD0B7E478D4A,2B3FA075-D0B5-49AB-B897-DAB1428CA500
		/// <summary>
		/// Gets all of the updates made to a Person(s) passed in.
		/// </summary>
		/// <remarks>
		/// This gives a history of all updates made to the Person(s).
		/// 
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Person/history/?ids=545A9495-DB58-44EC-BA47-FD0B7E478D4A,2B3FA075-D0B5-49AB-B897-DAB1428CA500'
		/// 
		/// 'api/v2/Person/history/?ids=all'   will return all updates for all Persons.
		/// 
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if no persons relate to the Ids given.
		/// 
		/// Otherwise it will return a json string of PersonUpdates.
		/// 
		/// </remarks>
		[HttpGet("history")]
		public async Task<IActionResult> GetHistory([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUpdates(ids, false);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}






		// GET: api/v2/Person/conflicts/?ids=545A9495-DB58-44EC-BA47-FD0B7E478D4A,2B3FA075-D0B5-49AB-B897-DAB1428CA500
		/// <summary>
		/// Gets all of the updates conflicting with the latest update for the PersonId(s) passed in.
		/// </summary>
		/// <remarks>
		/// 
		/// To get data a guid or a comma separated list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Person/conflicts/?ids=545A9495-DB58-44EC-BA47-FD0B7E478D4A,2B3FA075-D0B5-49AB-B897-DAB1428CA500'
		/// 
		/// 'api/v2/Person/conflicts/?ids=all'    will return all currently conflicted updates for all Persons.
		/// 
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if none of the Persons have conflicting updates.
		/// 
		/// Otherwise it will return a json string of PersonUpdates.
		/// 
		/// </remarks>
		[HttpGet("conflicts")]
		public async Task<IActionResult> GetConflicts([FromQuery] string? ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetConflictedUpdates(ids);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}






		// GET api/v2/Person/unsynced/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// <summary>
		/// Gets all Person updates from the server that haven't been saved to the local copy.
		/// </summary>
		/// <remarks>
		/// A Guid (CopyId) identifying the unique local copy of the data needs to be passed in.
		/// 
		/// 'api/v2/Person/unsynced/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// 
		/// 
		/// The API will respond with a 404 Not Found error if no updates have been made since the local copy was last fully synced.
		/// 
		/// Otherwise it will return a json string of PersonUpdates.
		/// </remarks>

		[HttpGet("unsynced/{copyId}")]
		public async Task<IActionResult> GetUnsynced([FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.GetUnsyncedUpdates(copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}





		// POST api/v2/Person/updates
		/// <summary>
		/// Creates or Updates a Person(s) by posting a PersonUpdate. (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		/// <remarks>
		/// 
		/// Authorisation is required to write to the central database. Use Contact above to recieve relevant ClientIds etc.
		/// 
		/// This method is how you create or update a Person since in both cases this is done by adding an adddtional PersonUpdate that supercedes the current update in the system.
		/// If a new Person is being created a new Guid needs to be created for Id.
		/// 
		/// The CopyId of the local storage copy must be passed in the uri. e.g.   'api/v2/Person/updates/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// 
		/// Json Text containing all properties of the update to be made must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// [
		/// 
		///         {   
		///         
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
		///             "IsSample": true,
		///                          
		///             "IsActive":true,
		///             
		///             "FirstName":"Jim",
		///             
		///             "LastName":"Bowen",
		///             
		///             "Email":"notatrueemail@hotpost.com",
		///             
		///             "PictureRef":"/images/picture.png"             
		///                          
		///             "IsActor":true,
		///                                     
		///             "IsSinger":false,
		///                                        
		///             "IsWriter":false,
		///                                   
		///             "IsBand":false,
		///                                    
		///             "IsTechnical":false,
		///                                        
		///             "Tags":["Male","Serious"],
		///             
		///				"MSALLink": "sdfjkasdfsdsdf"
		///       },
		///       {   
		///         
		///             "Id":"08a0b93a-ee8f-4d4a-a120-d9ee6a2817dc",
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
		///             "FirstName":"Clarence",
		///             
		///             "LastName":"Hathaway",
		///             
		///             "Email":"clarence@reallyhotpost.com",
		///             
		///             "PictureRef":"/images/picture.png"             
		///                          
		///             "IsActor":true,
		///                                     
		///             "IsSinger":false,
		///                                        
		///             "IsWriter":false,
		///                                   
		///             "IsBand":false,
		///                                    
		///             "IsTechnical":false,
		///                                        
		///             "Tags":["Female","Comedy"],
		///             
		///				"MSALLink": "sdfjkasdfsdsdf"
		///       }
		///             
		/// ]
		/// The API will return ServerToLocalPostBack info in json that should be used to update local storage and confirm the save to server was successful.
		///  
		/// </remarks>
		[HttpPost("updates/{copyId}")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] List<PersonUpdate> updates, [FromRoute] Guid copyId)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PostUpdates(updates, copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };

		}

		// PUT api/v2/Person/conflicts/clear
		/// <summary>
		/// Changes all updates relating to the Id(s) passed in to IsConflicted = false.   (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		/// <remarks> 
		/// 
		/// Authorisation is required to write to the central database. Use Contact above to obtain relevant ClientIds etc.
		/// 
		/// A list of guids needs to be passed in as a QUERY as shown below:
		/// 
		/// 'api/v2/Person/conflicts/clear/?ids=545A9495-DB58-44EC-BA47-FD0B7E478D4A'
		/// 
		/// All Updates relating to the Ids passed in will have IsConfliced set to false.
		/// 
		/// </remarks>
		[HttpPut("conflicts/clear")]
        [Authorize]
        public async Task<IActionResult> PutConflicts([FromQuery] string ids)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PutClearConflicts(ids);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}





		// PUT api/v2/Person/updates/postbackfromlocal/27fc9657-3c92-6758-16a6-b9f82ca696b3
		/// <summary>
		/// Updates server to confirm the ids and created data have been successfully copied to local.       (AUTHORISATON Through Azure AdB2C required)
		/// </summary>
		///<remarks>
		///
		/// Authorisation is required to write to the central database. Use Contact above to obtain relevant ClientIds etc.
		/// 
		/// LocalToServerPostBacks come from saves to Local Storage as part of the syncing process and confirm that the save to local has been successful.
		/// 
		/// The CopyId of the local storage copy must be passed in the URL. e.g.   'api/v2/Person/updates/postbackfromlocal/27fc9657-3c92-6758-16a6-b9f82ca696b3'
		/// 
		/// Json Text containing LocalToServerPostBacks must be passed in the BODY of the text as shown below:
		/// 
		/// 
		/// </remarks>
		[HttpPut("updates/postbackfromlocal/{copyId}")]
        [Authorize]
        public async Task<IActionResult> PutPostBackFromLocal([FromRoute] Guid copyId, [FromBody] List<LocalToServerPostBack> postBacks)
		{
			(HttpStatusCode statusCode, string result) = await _controllerService.PutPostBackToServer(postBacks, copyId);

			return new ObjectResult(result) { StatusCode = (int)statusCode };
		}

		//TODO - Add in LocalToServerPostBacks to comments above.



		/// <summary>
		/// Resets Sample Data for PersonUpdates. (Deleting all PartUpdate where IsSample = 1 and adding back in the default Sample Data)
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


		////// DELETE api/v2/Person/
		////[HttpDelete("{updates}")]
		////public void Delete([FromBody] string updates)
		////{

		////    List<PersonUpdate> personUpdates = JsonSerializer.Deserialize<List<PersonUpdate>>(updates) ?? new List<PersonUpdate>();

		////    _serverDataAccess.DeleteFromServer(personUpdates);
		////}
	}
}
