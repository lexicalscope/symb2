using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Symb2
{
    [TestClass]
    public partial class SortingPexTestX
    {
        [PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]// All, MaxConstraintSolverTime = 8, MaxRuns=1000, MaxRunsWithoutNewTests=1000, Timeout = 240)]
        [PexAllowedContractRequiresFailure]
        public void TM([PexAssumeNotNull] String[] L0)
        {
            PexAssume.AreElementsNotNull(L0);
            PexAssume.AreDistinctValues(L0);
            PexAssume.IsTrue(L0.Length > 1);

            //String[] L1 = L0.UCopy().USort((a, b) => a.CompareTo(b));
            String[] L1 = L0.OrderBy(x => x).ToArray();
            //new String[L0.Length];
            //L0.UCopyTo(L1);
            //Array.Sort(L1);

            //PexAssert.AreElementsEqual(L0, L1);
            //System.Console.WriteLine(String.Join(",", L0));
            //System.Console.WriteLine("".Equals("\0"));
            //System.Console.WriteLine("\0".CompareTo("\0\0"));
            PexAssert.IsTrue(L0.SequenceEqual(L1));
        }

        //[TestMethod]
        public void t0()
        {
            TM(new String[] { "a", "b", "c" });
        }

        //[TestMethod]
        public void t1()
        {
            TM(new String[] { "c", "b", "a" });
        }

        //[TestMethod]
        public void foo()
        {
            Console.WriteLine(String.Join(",", new String[] { "c", "b", "a" }.USort((a,b) => a.CompareTo(b))));
        }

        //[TestMethod]
        public void t2()
        {
            TM(new String[] { "\0\0", "\0\0\0", "" });
        }
    }


}
