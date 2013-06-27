using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using System.Diagnostics.Contracts;
using Symb2.T3;

namespace Symb2
{
    namespace T3
    {
        interface NoAnsHdl
        {
            void NoAns(Call inCall, Infrastructure infrastructure);
        }

        class Billing
        {
            internal void Bill(Call inCall, Call outCall)
            {
                Trace.TraceCall(this, "Bill", inCall, outCall);
            }
        }

        class Call
        {
            public string Number { get; private set; }

            public Call(string number)
            {
                this.Number = number;
            }

            internal bool Answered()
            {
                TraceElement e = Trace.TraceCall(this, "Answered");
                var result = Example3Test.B0;
                e.Result = result;
                return result;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                Call other = obj as Call;
                if ((System.Object)other == null)
                {
                    return false;
                }
                return Number == other.Number;
            }

            public override int GetHashCode()
            {
                return Number.GetHashCode();
            }

            public override string ToString()
            {
                return "Call: '" + Number + "'";
            }
        }

        class Infrastructure
        {
            internal Call Call(string number)
            {
                TraceElement e = Trace.TraceCall(this, "Call", number);
                var result = new Call(number);
                e.Result = result;
                return result;
            }

            internal void Dispose(Call inCall)
            {
                Trace.TraceCall(this, "Dispose", inCall);
            }

            internal void Connect(Call inCall, Call outCall)
            {
                Trace.TraceCall(this, "Connect", inCall, outCall);
            }

            internal void Dispose2(Call inCall, Call outCall)
            {
                Trace.TraceCall(this, "Dispose2", inCall, outCall);
            }

            internal void PlayFile(Call inCall, string file)
            {
                Trace.TraceCall(this, "PlayFile", inCall, file);
            }

            internal void HangUp(Call inCall)
            {
                Trace.TraceCall(this, "HangUp", inCall);
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                Call other = obj as Call;
                if ((System.Object)other == null)
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }
    }

    namespace T3V1
    {
        class Caller
        {
            private Infrastructure Infra = new Infrastructure();
            private Billing Bill = new Billing();
            public NoAnsHdl NoAns { get; set; }

            public void ConnectCall(String fromNumber, String toNumber)
            {
                Call inCall = Infra.Call(fromNumber);
                /*if (!inCall.Answered())i
                {
                    Infra.Dispose(inCall);
                    return;
                }*/

                Call outCall = Infra.Call(toNumber);
                if (outCall.Answered())
                {
                    Infra.Connect(inCall, outCall);
                }
                else if (NoAns != null)
                {
                    NoAns.NoAns(inCall, Infra);
                }
                Infra.Dispose2(inCall, outCall);
                Bill.Bill(inCall, outCall);
            }
        }

        public class M
        {
            public void Main(string fromNumber, string toNumber)
            {
                new Caller().ConnectCall(fromNumber, toNumber);
            }
        }
    }


    namespace T3V2
    {
        class PlayFileNoAnsHdl : NoAnsHdl
        {
            public void NoAns(Call inCall, Infrastructure infrastructure)
            {
                infrastructure.PlayFile(inCall, "noans.wav");
                infrastructure.HangUp(inCall);
            }
        }

        class Caller
        {
            private Infrastructure Infra = new Infrastructure();
            private Billing Bill = new Billing();
            public NoAnsHdl NoAns { get; set; }

            public void ConnectCall(String fromNumber, String toNumber)
            {
                Call inCall = Infra.Call(fromNumber);
                /*if (!inCall.Answered())
                {
                    Infra.Dispose(inCall);
                    return;
                }*/

                Call outCall = Infra.Call(toNumber);
                if (outCall.Answered())
                {
                    Infra.Connect(inCall, outCall);
                }
                else if (NoAns != null)
                {
                    NoAns.NoAns(inCall, Infra);
                }
                Infra.Dispose2(inCall, outCall);
                Bill.Bill(inCall, outCall);
            }
        }

        public class M
        {
            public void Main(string fromNumber, string toNumber)
            {
                new Caller() { NoAns = new PlayFileNoAnsHdl() }.ConnectCall(fromNumber, toNumber);
            }
        }
    }

    [TestClass]
    public partial class Example3Test
    {
        public static bool B0;

        [PexMethod]
        [PexAllowedContractRequiresFailure]
        public void TM(string fromNumber, string toNumber, bool b0)
        {
            B0 = b0;

            Trace.Begin();
            Trace.Begin1();

            new T3V1.M().Main(fromNumber, toNumber);

            Trace.Begin2();

            new T3V2.M().Main(fromNumber, toNumber);

            C();
        }

        [Pure]
        private void C()
        {
            var transTraces = Trace.TransformTrace(
                (es1, es2) => Tuple.Create(
                    Trace.FromElements(
                        es1.ElementAt(0), 
                        es1.ElementAt(1),
                        es1.ElementAt(2),
                        es2.ElementAt(3),
                        es2.ElementAt(4)),
                     es2),
                Trace.Predicate(
                    "Call".IsMeth(),
                    "Call".IsMeth(),
                    "Answered".IsMeth().And(
                       false.IsResult())),
                Trace.Predicate(
                    "Call".IsMeth(),
                    "Call".IsMeth(),
                    "Answered".IsMeth().And(
                       false.IsResult()),
                    "PlayFile".IsMeth(),
                    "HangUp".IsMeth()
                ));

            if (!transTraces.AreEqual())
            {
                Assert.Fail();
            }
        }
    }

    public static class PredicateExtensions
    {
        public static Predicate<TraceElement> IsMeth(this String name) { return e => e.Meth == name; }
        public static Predicate<TraceElement> IsResult(this Object value) { return e => e.Result.Equals(value); }
        public static Predicate<TraceElement> And(this Predicate<TraceElement> l, Predicate<TraceElement> r) { return e => l(e) && r(e); }
        public static bool AreEqual<T>(this Tuple<T, T> tuple) { return tuple.Item1.Equals(tuple.Item2);} 
    }
}
