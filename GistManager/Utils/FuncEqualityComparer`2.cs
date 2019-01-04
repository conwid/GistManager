using System;
using System.Collections.Generic;

namespace GistManager.Utils
{
    // https://dotnetfalcon.com/extending-linq2objects-groupby/
    public class FuncEqualityComparer<T, TValue> : IEqualityComparer<T>
    {
        private readonly Func<T, TValue> extractorFunc;
        public FuncEqualityComparer(Func<T, TValue> extractorFunc)
        {
            this.extractorFunc = extractorFunc;
        }
        public bool Equals(T x, T y) => extractorFunc(x).Equals(extractorFunc(y));
        public int GetHashCode(T obj) => extractorFunc(obj).GetHashCode();
    }
}
