using System.Collections.Generic;

namespace Mizer
{
    public interface IDataWriter
    {
        void WriteSets(IEnumerable<Set> sets);

        void WriteCards(Set set);
    }
}