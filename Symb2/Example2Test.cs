using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;
using Microsoft.Pex.Framework.Validation;

namespace Symb2
{
    namespace V1
    {
        class Infrastructure
        {
            internal void Call(string number)
            {
                Trace.TraceCall(this, "Call", number);
            }
        }

        class Scheduler
        {
            internal Agent Pick(Agent agent1, Agent agent2)
            {
                var e = Trace.TraceCall(this, "Pick", agent1, agent2);
                if (agent1.WaitTime <= agent2.WaitTime)
                {
                    e.Result = agent1;
                    return agent1;
                }
                e.Result = agent2;
                return agent2;
            }
        }

        class M2
        {
            Infrastructure infra = new Infrastructure();
            Scheduler sched = new Scheduler();

            public void Main(Agent agent1, Agent agent2)
            {
                infra.Call(sched.Pick(agent1, agent2).Number);
            }
        }
    }

    namespace V2
    {
        class Infrastructure
        {
            internal void Call(string number)
            {
                Trace.TraceCall(this, "Call", number);
            }
        }

        class Scheduler
        {
            internal Agent Pick(Agent agent1, Agent agent2)
            {
                var e = Trace.TraceCall(this, "Pick", agent1, agent2);
                if (agent1.WaitTime > agent2.WaitTime)
                {
                    e.Result = agent1;
                    return agent1;
                }
                e.Result = agent2;
                return agent2;
            }
        }

        class M2
        {
            Infrastructure infra = new Infrastructure();
            Scheduler sched = new Scheduler();

            public void Main(Agent agent1, Agent agent2)
            {
                infra.Call(sched.Pick(agent1, agent2).Number);
            }
        }
    }

    public class Agent
    {
        public int WaitTime { get; set; }
        public string Number { get; set; }

        [Pure]
        public override string ToString()
        {
            return "Agent{WaitTime: " + WaitTime + " Number: " + Number + "}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Agent other = obj as Agent;
            if ((System.Object)other == null)
            {
                return false;
            }    
            return WaitTime == other.WaitTime && Number == other.Number;
        }

        public override int GetHashCode()
        {
            return WaitTime ^ Number.GetHashCode();
        }
    }

    //[PexClass(typeof(Example2Test))]
    [TestClass]
    public partial class Example2Test
    {
        [PexMethod]
        [PexAllowedContractRequiresFailure]
        public void TM(
            Agent agent1,
            Agent agent2,
            Agent agent1p,
            Agent agent2p)
        {
            Contract.Requires(agent1 != null && agent2 != null);
            Contract.Requires(agent1p != null && agent2p != null);

            Contract.Requires(agent1.Number == agent2p.Number);
            Contract.Requires(agent2.Number == agent1p.Number);
            Contract.Requires(agent1.WaitTime == agent1p.WaitTime);
            Contract.Requires(agent2.WaitTime == agent2p.WaitTime);
            
            Trace.Begin();
            
            Trace.Begin1();
            new V1.M2().Main(agent1, agent2);

            Trace.Begin2();
            new V2.M2().Main(agent1p, agent2p);

            C();
        }

        private void C()
        {
            //PexObserve.Value<string>("condition", PexSymbolicValue.GetRawPathConditionString());

            var transTrace1 = Trace.Trace1.TransformTrace(
                es => es,
                e => true);

            /*var transTrace2 = Trace.Trace2.TransformTrace(
                es => es,
                e => true);*/

            var transTrace2 = Trace.Trace2.TransformTrace(
                es => {
                    var e0 = es.ElementAt(0);
                    Agent agent1 = (Agent) e0[0];
                    Agent agent2 = (Agent) e0[1];
                    
                    return new[] { 
                        new TraceElement(
                            null, 
                            e0.Result, 
                            "Pick", 
                            new Agent(){Number = agent2.Number, WaitTime = agent1.WaitTime}, 
                            new Agent(){Number = agent1.Number, WaitTime = agent2.WaitTime}), 
                        es.ElementAt(1)};
                },
                e => e.Meth == "Pick", 
                e => e.Meth == "Call");

            Assert.AreEqual(transTrace1, transTrace2);
        }
    }
}
