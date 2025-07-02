using System;

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
