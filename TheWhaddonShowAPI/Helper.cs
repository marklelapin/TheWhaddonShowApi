
using MyClassLibrary.LocalServerMethods.Interfaces;
using TheWhaddonShowClassLibrary.Models;
using TheWhaddonShowClassLibrary.SampleData;

namespace TheWhaddonShowTesting.Tests
{
    public class Helper
    {
       
          
            private readonly IServerDataAccess<PartUpdate> _serverPartDataAccess;
          
            private readonly IServerDataAccess<PersonUpdate> _serverPersonDataAccess;
          
            private readonly IServerDataAccess<ScriptItemUpdate> _serverScriptItemDataAccess;

            public Helper(                            
                          IServerDataAccess<PartUpdate> serverPartDataAccess
                          ,IServerDataAccess<PersonUpdate> serverPersonDataAccess
                          ,IServerDataAccess<ScriptItemUpdate> serverScriptItemDataAccess
                          )
            {
             
                _serverPartDataAccess = serverPartDataAccess;
          
                _serverPersonDataAccess = serverPersonDataAccess;

                _serverScriptItemDataAccess = serverScriptItemDataAccess;
            }

            public async void ResetSampleData()
            {

                await _serverPartDataAccess.ResetSampleData(SamplePartData.ServerStartingData,SamplePartData.ServerSyncLogStartingData);

                await _serverPersonDataAccess.ResetSampleData(SamplePersonData.ServerStartingData, SamplePersonData.ServerSyncLogStartingData);

               await _serverScriptItemDataAccess.ResetSampleData(SampleScriptItemData.ServerStartingData, SampleScriptItemData.ServerSyncLogStartingData);

            }


        
    }

}
