using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework;


namespace Symb2
{
    public delegate IEnumerable<T> Transform<T>(IEnumerable<T> ts);

    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> FindSubsequence<T>(this IEnumerable<T> sequence, params Predicate<T>[] predicates)
        {
            IEnumerable<T> current = sequence;

            while (current.Count() > predicates.Length)
            {
                var window = current.
                    TakeWhile((elem, index) => 
                        index < predicates.Length && 
                        predicates[index](elem));
                if(window.Count() == predicates.Length)
                    yield return window;

                current = current.Skip(1);
            }
        }

        public static Predicate<TraceElement> AsPredicate(this string predicate, params object[] values)
        {
            System.Linq.Expressions.Expression<Func<TraceElement, bool>> lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda<TraceElement,bool>(predicate, values);
            var function = lambda.Compile();
            return e => function(e);
        }

        public static IList<T> ReplaceSubsequences<T>(
            this IEnumerable<T> sequence, 
            Transform<T> transform,
            params Predicate<T>[] predicates)
        {
            Console.WriteLine("input:" + string.Join(",", sequence));

            var result = new List<T>();
            IEnumerable<T> current = sequence;
            while (current.Count() >= predicates.Length)
            {
                var window = current.
                    TakeWhile((elem, index) => 
                        index < predicates.Length && 
                        predicates[index](elem));
                Console.WriteLine("loop:" + string.Join(",", window));
                if (window.Count() == predicates.Length)
                {
                    result.AddRange(transform(window));
                    current = current.Skip(predicates.Length);
                }
                else
                {
                    result.Add(current.First());
                    current = current.Skip(1);
                }
                
            }
            return result;
        }
    }

    public class TraceElement
    {
        private object that;
        public string Meth { get; private set; }
        public object[] Args { get; private set; }
                
        public TraceElement(object that, string meth, params object[] args)
        {
            this.that = that;
            this.Meth = meth;
            this.Args = args;
        }

        public object this[int i]
        {
            get { return Args[i]; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TraceElement other = obj as TraceElement;
            if ((System.Object)other == null)
            {
                return false;
            }
            return this.Meth.Equals(other.Meth) && 
                Enumerable.SequenceEqual(this.Args, other.Args);
        }

        public override int GetHashCode()
        {
            int code = Meth.GetHashCode();
            foreach (var item in Args)
            {
                code ^= item.GetHashCode();
            }
            return code;
        }

        public override string ToString()
        {
            return  Meth + "[" + string.Join(", ", Args) + "]";
        }
    }

    public class Trace : IEnumerable<TraceElement>
    {
        public static Trace trace1;
        public static Trace trace2;

        private IList<TraceElement> TraceList;
        private IList<TraceElement> iList;

        public Trace() : this(new List<TraceElement>())
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
    }

    class MessageReceiver1
    {
        public void Receive(string message)
        {
            Trace.trace1.TraceCall(this, "Receive", message);
        }
    }

    class MessageBus1
    {
        private MessageReceiver1 messageReceiver;
        private int minimumPriority;

        public MessageBus1(MessageReceiver1 messageReceiver, int minimumPriority)
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

    class MessageReceiver2
    {
        public void Receive(string message)
        {
            Trace.trace2.TraceCall(this, "Receive", message);
        }
    }

    class MessageBus2
    {
        private MessageReceiver2 messageReceiver;
        private int minimumPriority;

        public MessageBus2(MessageReceiver2 messageReceiver, int minimumPriority)
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

    class P
    {
        public void Main(String message, int priority)
        {
            MessageBus1 bus = new MessageBus1(new MessageReceiver1(), 5);
            bus.Send(message, priority);
        }
    }

    class Q
    {
        public void Main(String message, int priority)
        {
            MessageBus2 bus = new MessageBus2(new MessageReceiver2(), 7);
            bus.Send(message, priority);
        }
    }

    [TestClass]
    public partial class UnitTest1
    {
        [TestMethod]
        public void TestOne()
        {
            TM("foo", 5);
        }

        [PexMethod]
        public void TM(string message, int priority)
        {
            Trace.trace1 = new Trace();
            Trace.trace2 = new Trace();

            new P().Main(message, priority);
            new Q().Main(message, priority);
                        
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
