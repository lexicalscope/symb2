using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;

namespace Symb2
{
    namespace V1
    {
        class Infrastructure
        {
            internal void Call(Agent agent)
            {
                Trace.TraceCall(this, "Call", agent);
            }
        }

        class Scheduler
        {
            internal Agent Pick(Agent agent1, Agent agent2)
            {
                var e = Trace.TraceCall(this, "Pick", agent1, agent2);
                if (agent1.WaitTime < agent2.WaitTime)
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
                infra.Call(sched.Pick(agent1, agent2));
            }
        }
    }

    namespace V2
    {
        class Infrastructure
        {
            internal void Call(Agent agent)
            {
                Trace.TraceCall(this, "Call", agent);
            }
        }

        class Scheduler
        {
            internal Agent Pick(Agent agent1, Agent agent2)
            {
                var e = Trace.TraceCall(this, "Pick", agent1, agent2);
                if (agent1.WaitTime >= agent2.WaitTime)
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
                infra.Call(sched.Pick(agent1, agent2));
            }
        }
    }

    public class Agent
    {
        public int WaitTime { get; set; }
    }

    [TestClass]
    public partial class Example2Test
    {
        [PexMethod]
        public void TM(Agent agent1, Agent agent2)
        {
            Contract.Requires(agent1 != null);
            Contract.Requires(agent2 != null);

            Trace.Begin();
            
            Trace.Begin1();
            new V1.M2().Main(agent1, agent2);

            Trace.Begin2();
            new V2.M2().Main(agent1, agent2);

            C();
        }

        private void C()
        {
        }
    }
}
