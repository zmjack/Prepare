using System;
using System.Collections.Generic;
using System.Text;

namespace Prepare
{
    public interface IDesignTimePrepareFactory
    {
        void Prepare(PrepareProject project, string[] args);
    }
}
