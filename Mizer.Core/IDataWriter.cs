using System.Collections.Generic;

namespace Mizer.Core
{
    public interface IDataWriter
    {
        void WriteSets(IEnumerable<Set> sets);

        void WriteCards(Set set);
    }
}