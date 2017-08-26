using System.Collections.Generic;

namespace Neutrino.Entities
{
    public class PageList<T>
    {
        public int AllRows { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public IList<T> Rows { get; set; }
    }
}