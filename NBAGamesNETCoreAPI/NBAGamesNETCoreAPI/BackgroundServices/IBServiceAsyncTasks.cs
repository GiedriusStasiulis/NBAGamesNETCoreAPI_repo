using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBAGamesNETCoreAPI.BackgroundServices
{
    public interface IBServiceAsyncTasks
    {
        Task SetUpFirestoreDbAsync();
        Task FetchDataFromWebAsync();
        Task CheckEntitiesNSendToFirestoreAsync();
    }
}
