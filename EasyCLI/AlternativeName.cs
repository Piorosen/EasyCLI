using System;
namespace EasyCLI
{
    public class AlternativeNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public AlternativeNameAttribute(string name = null)
        {
            Name = name;
        }
    }
}
