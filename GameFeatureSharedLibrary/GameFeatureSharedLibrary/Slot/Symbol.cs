using System;
using System.Collections.Generic;
using System.Text;

namespace GameFeatureSharedLibrary.Slot
{
    internal class Symbol
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public bool IsWild { get; protected set; }
        public Symbol(int id, string name="", bool isWild = false)
        {
            Id = id;
            Name = name;
            IsWild = isWild;
        }
    }
}
