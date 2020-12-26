using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public abstract class BenchBase
    {
        public abstract bool Initialise(int rowcount);
        public abstract bool FindRows(int addcount);
        public abstract bool GetRows(int addcount);
        public abstract bool AddRows(int addcount);
        public abstract bool UpdateRows(int rowcount);
        public abstract bool DeleteRows(int rowcount);
        public abstract bool Save(int rowcount);

    }
}
