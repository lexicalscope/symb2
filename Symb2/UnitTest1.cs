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
                Trace.trace1.TraceCall(this, "Receive", message);
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
                Trace.trace1.TraceCall(this, "Send", message, priority);
                if (priority < minimumPriority)
                {
                    return;
                }
                messageReceiver.Receive(message);
            }
        }

        class M
        {
            public void Main(String message, int priority)
            {
                MessageBus bus = new MessageBus(new MessageReceiver(), 5);
                bus.Send(message, priority);
            }
        }
    }

    namespace V2
    {
        class MessageReceiver
        {
            public void Receive(string message)
            {
                Trace.trace2.TraceCall(this, "Receive", message);
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
                Trace.trace2.TraceCall(this, "Send", message, priority);
                if (priority < minimumPriority)
                {
                    return;
                }
                messageReceiver.Receive(message);
            }
        }

        class M
        {
            public void Main(String message, int priority)
            {
                MessageBus bus = new MessageBus(new MessageReceiver(), 7);
                bus.Send(message, priority);
            }
        }
    }    

    [TestClass]
    public partial class UnitTest1
    {
        [PexMethod]
        public void TM(string message, int priority)
        {
            Trace.Begin();

            new V1.M().Main(message, priority);
            new V2.M().Main(message, priority);

            C();
        }

        [Pure]
        private void C() 
        {
            var transTrace1 = Trace.trace1.TransformTrace(
                es => es, 
                e => true);
            var transTrace2 = Trace.trace2.TransformTrace(
                es => new[] { es.First(), new TraceElement(new Object(), "Receive", es.First()[0]) },
                 e => e.Meth == "Send" && ((int)e[1]) >= 5 && ((int)e[1]) < 7);

            Assert.AreEqual(transTrace1, transTrace2);    
        }
    }
}
