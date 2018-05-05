using System;
using System.Collections.Generic;
using Facile.Models;

namespace Facile.Imports
{
    public class FacileJson <T>
    {
        public string Description { get; set; }
        public string Table { get; set; }
        public int Records { get; set; }
        public IList<T> Data { get; set; }
    }
}
