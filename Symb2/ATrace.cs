using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Symb2
{
    interface InstObj
    {
        int _rb_id { get; set; }
    }

    class ATraceElement 
    {
        public String m;
        public int[] ps;

        public ATraceElement(string m, int[] ps)
        {
            this.m = m;
            this.ps = ps;
        }

        [Pure]
        public override String ToString()
        {
            return m + "(" + String.Join(",", ps) + ")";
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ATraceElement that = obj as ATraceElement;
            if ((System.Object)that == null)
            {
                return false;
            }

            return this.m.Equals(that.m) && Enumerable.SequenceEqual(this.ps, that.ps);
        }

        [Pure]
        public override int GetHashCode()
        {
            int hc = m.GetHashCode();
            foreach(int p in ps)
            {
                hc = (p * 17) + hc;
            }
            return hc;
        }
    }

    class RbId
    {
        public int id;

        public RbId(int id)
        {
            this.id = id;
        }
    }

    class ATrace
    {
        ConditionalWeakTable<Object, RbId> rb_id = new ConditionalWeakTable<Object, RbId>();
        int seenCount = 0;
        public List<ATraceElement> trace = new List<ATraceElement>();

        public void log(String m, Object[] ps)
        {
            //Array.ForEach(ps, p => { InstObj po = ((InstObj)p); if (po._rb_id == 0) { po._rb_id = ++seenCount; } });
            int[] ids = new int[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                ids[i] = rb_id.GetValue(ps[i], (k) => new RbId(++seenCount)).id;
            }
            trace.Add(new ATraceElement(m, ids));
        }

        [Pure]
        public override String ToString()
        {
            return String.Join(Environment.NewLine, trace);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ATrace that = obj as ATrace;
            if ((System.Object)that == null)
            {
                return false;
            }

            return this.seenCount.Equals(that.seenCount) && this.trace.SequenceEqual(that.trace);
        }

        [Pure]
        public override int GetHashCode()
        {
            int hc = trace.Count;
            foreach (ATraceElement e in trace)
            {
                hc = (e.GetHashCode() * 17) + hc;
            }
            return hc;
        }
    }

    class ATracer
    {
        public static ATrace trace1;
        public static ATrace trace2;
        public static ATrace trace;

        public static void begin1()
        {
            trace1 = new ATrace();
            trace = trace1;
        }

        public static void begin2()
        {
            trace2 = new ATrace();
            trace = trace2;
        }

        public static void log(String m, Object[] ps)
        {
            trace.log(m, ps);
        }
    }
}
