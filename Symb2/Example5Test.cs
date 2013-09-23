using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;
using Microsoft.Pex.Framework.Settings;

namespace Symb2
{
    public class Student
    {
        private string name;

        public Student(String name)
        {
            this.name = name;
        }

        internal string Name()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Student that = obj as Student;
            if ((System.Object)that == null)
            {
                return false;
            }

            return this.Name() == that.Name();
        }

        public override int GetHashCode()
        {
            return Name().GetHashCode();
        }
    }

    namespace T5V1
    {
        public class StudentDb
        {
            internal Student[] OrderedByGrade()
            {
                return Test51.L0p;
            }
        }

        public class Logger
        {
            internal static void Considered(Student[] students)
            {
                foreach (Student s in students)
                {
                    System.Console.WriteLine("considered " + s.Name());
                }
            }
        }

        public class Prizes
        {
            internal Student[] AwardTo(Student[] students)
            {
                Contract.Requires(students.Length >= 3);

                return new Student[] { students[0], students[1], students[2] };
            }
        }
        
        public class M
        {
            StudentDb db = new StudentDb();
            Logger log = new Logger();
            Prizes prizes = new Prizes();

            public Student[] Main()
            {
                Student[] students = db.OrderedByGrade();
                Logger.Considered(students);
                return prizes.AwardTo(students);
            }
        }
    }

    namespace T5V2
    {
        public class StudentDb
        {
            internal Student[] OrderedByGrade()
            {
                return Test51.L0q;
            }
        }

        public class Logger
        {
            internal static void Considered(Student[] students)
            {
                Array.Sort(students, (a, b) => a.Name().CompareTo(b.Name()));
                foreach (Student s in students)
                {
                    System.Console.WriteLine("considered " + s.Name());
                }
            }
        }

        public class Prizes
        {
            internal Student[] AwardTo(Student[] students)
            {
                Contract.Requires(students.Length >= 3);

                return new Student[] { students[0], students[1], students[2] };
            }
        }

        public class M
        {
            StudentDb db = new StudentDb();
            Logger log = new Logger();
            Prizes prizes = new Prizes();

            public Student[] Main()
            {
                Student[] students = db.OrderedByGrade();
                Logger.Considered(students);
                return prizes.AwardTo(students);
            }
        }
    }

    [TestClass]
    public partial class Test51
    {
        public static Student[] L0p;
        public static Student[] L0q;

        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexAllowedContractRequiresFailure]
        public void TM([PexAssumeNotNull] String[] L0)
        {
            PexAssume.AreElementsNotNull(L0);
            PexAssume.AreDistinctValues(L0);
            PexAssume.IsTrue(L0.Length >= 3);

            Test51.L0p = Array.ConvertAll(L0, n => new Student(n));
            Test51.L0q = Array.ConvertAll(L0, n => new Student(n));

            ATracer.begin1();
            new T5V1.M().Main();
            ATracer.begin2();
            new T5V2.M().Main();

            //ATrace.Begin();
            //PexAssert.AreBehaviorsEqual(() => new T5V1.M().Main(),
            //                            () => new T5V2.M().Main());
            //ATrace.End();
        }

        /*
        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexAllowedContractRequiresFailure]
        public void TM(Student[] L0p, Student[] L0q)
        {
            PexAssume.IsNotNull(L0p);
            PexAssume.IsNotNull(L0q);
            PexAssume.AreElementsNotNull(L0p);
            PexAssume.AreElementsNotNull(L0q);

            //Student[] ss = new Student[L0p.Length + L0q.Length];
            //L0p.CopyTo(ss, 0);
            //L0q.CopyTo(ss, L0p.Length);
 
            //PexAssume.AreDistinctReferences(ss);
            PexAssume.AreDistinct(L0p, (a,b) => !a.Name().Equals(b.Name()));

            PexAssume.AreElementsEqual<Student>(L0p, L0q, (a,b) => a.Equals(b));
            PexAssume.IsTrue(L0p.Length >= 3);

            //Contract.Requires(L0p.SequenceEqual(L0q));
            
            //Contract.Requires(Contract.ForAll<Student>(L0p, s => s != null));

            Test51.L0p = L0p;
            Test51.L0q = L0q;

            //ATrace.Begin();
            PexAssert.AreBehaviorsEqual(() => new T5V1.M().Main(),
                                        () => new T5V2.M().Main());
            //ATrace.End();
        }*/

        [TestMethod]
        public void SimpleTest()
        {
            TM(new String[] { "David", "Nigel", "Derek" });
        }
    }

    /*
    [TestClass]
    public class Test52
    {
        public static List<Student> L0;

        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexAllowedContractRequiresFailure]
        public void TM(List<Student> L0)
        {
            Contract.Requires(L0 != null);
            Contract.Requires(Contract.ForAll<Student>(L0, s => s != null));

            Test52.L0 = L0;
            //Test.L0q = L0q;

            //ATrace.Begin();
            new T5V2.M().Main();
            //ATrace.End();
        }
    }
    */
}
