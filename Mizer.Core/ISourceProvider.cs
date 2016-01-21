using System.Threading.Tasks;

namespace Mizer.Core
{
    public interface ISourceProvider
    {
        bool IsBusy { get; }

        Task<Set[]> ReadSetList();

        Task<Card[]> ReadCardList(Set set);
    }
}