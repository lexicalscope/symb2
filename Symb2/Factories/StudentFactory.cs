// <copyright file="StudentFactory.cs">Copyright ©  2013</copyright>

using System;
using Microsoft.Pex.Framework;
using Symb2;

namespace Symb2
{
    /// <summary>A factory for Symb2.Student instances</summary>
    public static partial class StudentFactory
    {
        /// <summary>A factory for Symb2.Student instances</summary>
        [PexFactoryMethod(typeof(Student))]
        public static Student Create([PexAssumeNotNull] string name_s)
        {
            PexAssume.IsNotNull(name_s);
            Student student = new Student(name_s);
            return student;

            // TODO: Edit factory method of Student
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
