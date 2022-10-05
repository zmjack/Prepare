using System;
using System.Collections.Generic;
using System.Text;

namespace Prepare
{
    public class PrepareNameAttribute : Attribute
    {
        public string Name { get; set; }

        public PrepareNameAttribute(string name = null)
        {
            Name = name;
        }
    }
}
