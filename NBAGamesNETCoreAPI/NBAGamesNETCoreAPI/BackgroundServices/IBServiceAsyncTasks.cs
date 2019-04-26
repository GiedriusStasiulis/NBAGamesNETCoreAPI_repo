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
