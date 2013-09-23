using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symb2
{
    interface InstObj
    {
        int _rb_id { get; set; }
    }

    class ATraceElement 
    {
        String m;
        int[] ps;

        public ATraceElement(string m, int[] ps)
        {
            this.m = m;
            this.ps = ps;
        }
    }

    class ATrace
    {
        int count = 0;
        List<ATraceElement> trace = new List<ATraceElement>();

        public void log(String m, Object[] ps)
        {
            Array.ForEach(ps, p => { InstObj po = ((InstObj)p); if (po._rb_id == 0) { po._rb_id = ++count; } });
            trace.Add(new ATraceElement(m, Array.ConvertAll(ps, p => ((InstObj)p)._rb_id)));
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
    }
}
