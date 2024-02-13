using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Models
{
    public interface ICopyable<T>
    {
        public T Copy();
    }
}
