using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symb2
{
    public class Trace : IEnumerable<TraceElement>
    {
        public static Trace trace1;
        public static Trace trace2;

        private IList<TraceElement> TraceList;
        private IList<TraceElement> iList;

        public Trace()
            : this(new List<TraceElement>())
        { }

        public Trace(IList<TraceElement> es)
        {
            TraceList = es;
        }

        public void TraceCall(Object that, string meth, params object[] args)
        {
            TraceList.Add(new TraceElement(that, meth, args));
        }

        public Trace TransformTrace(
            Transform<TraceElement> transform,
            params Predicate<TraceElement>[] predicates)
        {
            return new Trace(TraceList.ReplaceSubsequences(transform, predicates));
        }

        #region IEnumerable<T> Members
        public IEnumerator<TraceElement> GetEnumerator()
        {
            return TraceList.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return TraceList.GetEnumerator();
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Trace other = obj as Trace;
            if ((System.Object)other == null)
            {
                return false;
            }
            return Enumerable.SequenceEqual(this.TraceList, other.TraceList);
        }

        public override int GetHashCode()
        {
            int code = 0;
            foreach (var item in TraceList)
            {
                code ^= item.GetHashCode();
            }
            return code;
        }

        public override string ToString()
        {
            return string.Join(", ", TraceList);
        }

        internal static void Begin()
        {
            Trace.trace1 = new Trace();
            Trace.trace2 = new Trace();
        }
    }
}
