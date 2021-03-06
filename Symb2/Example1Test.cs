﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework;
using System.Diagnostics.Contracts;


namespace Symb2
{
    namespace V1
    {
        class MessageReceiver
        {
            public void Receive(string message)
            {
                Trace.TraceCall(this, "Receive", message);
            }
        }

        class MessageBus
        {
            private MessageReceiver messageReceiver;
            private int minimumPriority;

            public MessageBus(MessageReceiver messageReceiver, int minimumPriority)
            {
                this.messageReceiver = messageReceiver;
                this.minimumPriority = minimumPriority;
            }

            public void Send(string message, int priority)
            {
                Trace.Trace1.TraceVCall(this, "Send", message, priority);
                if (priority < minimumPriority)
                {
                    return;
                }
                messageReceiver.Receive(message);
            }
        }

        class M1
        {
            public void Main(
                string message1, int priority1,
                string message2, int priority2)
            {
                MessageBus bus = new MessageBus(new MessageReceiver(), 5);
                bus.Send(message1, priority1);
                bus.Send(message2, priority2);
            }
        }
    }

    namespace V2
    {
        class MessageReceiver
        {
            public void Receive(string message)
            {
                Trace.Trace2.TraceVCall(this, "Receive", message);
            }
        }

        class MessageBus
        {
            private MessageReceiver messageReceiver;
            private int minimumPriority;

            public MessageBus(MessageReceiver messageReceiver, int minimumPriority)
            {
                this.messageReceiver = messageReceiver;
                this.minimumPriority = minimumPriority;
            }

            public void Send(string message, int priority)
            {
                Trace.Trace2.TraceVCall(this, "Send", message, priority);
                if (priority < minimumPriority)
                {
                    return;
                }
                messageReceiver.Receive(message);
            }
        }

        class M1
        {
            public void Main(
                string message1, int priority1,
                string message2, int priority2)
            {
                MessageBus bus = new MessageBus(new MessageReceiver(), 7);
                bus.Send(message1, priority1);
                bus.Send(message2, priority2);

            }
        }
    }    

    [TestClass]
    public partial class Example1Test
    {
        [PexMethod]
        public void TM(
            string message1, int priority1,
            string message2, int priority2)
        {
            Trace.Begin();

            Trace.Begin1();
            new V1.M1().Main(
                message1, priority1,
                message2, priority2);

            Trace.Begin2();
            new V2.M1().Main(
                message1, priority1,
                message2, priority2);

            C();
        }

        [Pure]
        private void C() 
        {
            var transTrace1 = Trace.Trace1.TransformTrace(
                es => es, 
                e => true);
            var transTrace2 = Trace.Trace2.TransformTrace(
                es => new[] { es.First(), new TraceElement(new Object(), null, "Receive", es.First()[0]) },
                 e => e.Meth == "Send" && ((int)e[1]) >= 5 && ((int)e[1]) < 7);

            Assert.AreEqual(transTrace1, transTrace2);
        }
    }
}
