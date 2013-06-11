using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using System.Diagnostics.Contracts;

namespace Symb2
{
    class SandMan
    {
        public void Chase()
        {
            Trace.TraceCall(this, "Chase");
        }

        public void Ignore()
        {
            Trace.TraceCall(this, "Ignore");
        }
    }

    namespace T4V1
    {
        class M
        {
            public void Main(int age)
            {
                SandMan sandMan = new SandMan();
                if (age < 21)
                {
                    sandMan.Chase();
                }
                else
                {
                    sandMan.Ignore();
                }
            }
        }
    }

    namespace T4V2
    {
        class M
        {
            public void Main(int age)
            {
                SandMan sandMan = new SandMan();
                if (age > 21)
                {
                    sandMan.Chase();
                }
                else
                {
                    sandMan.Ignore();
                }
            }
        }
    }

    [TestClass]
    public partial class Example4Test
    {
        public static bool B0;

        [PexMethod]
        [PexAllowedContractRequiresFailure]
        public void TM(int age)
        {
            Trace.Begin();
            Trace.Begin1();

            new T4V1.M().Main(age);

            Trace.Begin2();

            new T4V2.M().Main(age);

            C();
        }

        [Pure]
        private void C()
        {
            var transTrace1 = Trace.Trace1.
                TransformTrace(
                    es => new TraceElement[]{es.ElementAt(0).Meth == "Chase" 
                                ? new TraceElement(null, null, "Ignore")
                                : new TraceElement(null, null, "Chase")},
                    e => e.Meth == "Chase" || e.Meth == "Ignore");

            var transTrace2 = Trace.Trace2.TransformTrace(es => es);

            Assert.AreEqual(transTrace1, transTrace2);
        }
    }
}
