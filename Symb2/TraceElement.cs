using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symb2
{
    public class TraceElement
    {
        private object that;
        public string Meth { get; private set; }
        public object[] Args { get; private set; }

        public TraceElement(object that, string meth, params object[] args)
        {
            this.that = that;
            this.Meth = meth;
            this.Args = args;
        }

        public object this[int i]
        {
            get { return Args[i]; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TraceElement other = obj as TraceElement;
            if ((System.Object)other == null)
            {
                return false;
            }
            return this.Meth.Equals(other.Meth) &&
                Enumerable.SequenceEqual(this.Args, other.Args);
        }

        public override int GetHashCode()
        {
            int code = Meth.GetHashCode();
            foreach (var item in Args)
            {
                code ^= item.GetHashCode();
            }
            return code;
        }

        public override string ToString()
        {
            return Meth + "[" + string.Join(", ", Args) + "]";
        }
    }
}
