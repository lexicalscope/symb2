using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeorgeCloney;

namespace Symb2
{
    public class Trace : IEnumerable<TraceElement>
    {
        public static Trace Trace1;
        public static Trace Trace2;
        public static Trace CurTrace;

        private IList<TraceElement> TraceList;

        public Trace()
            : this(new List<TraceElement>())
        { }

        public Trace(IList<TraceElement> es)
        {
            TraceList = es;
        }

        public TraceElement TraceVCall(Object that, string meth, params object[] args)
        {
            var e = new TraceElement(that, null, meth, args);//.DeepClone();
            TraceList.Add(e);
            return e;
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
            Trace.Trace1 = new Trace();
            Trace.Trace2 = new Trace();
        }

        internal static void Begin1()
        {
            Trace.CurTrace = Trace.Trace1;
        }

        internal static void Begin2()
        {
            Trace.CurTrace = Trace.Trace2;
        }

        public static TraceElement TraceCall(Object that, string meth, params object[] args)
        {
            return Trace.CurTrace.TraceVCall(that, meth, args);
        }
    }
}
