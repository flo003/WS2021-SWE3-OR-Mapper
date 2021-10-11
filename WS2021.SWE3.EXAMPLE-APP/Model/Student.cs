using System;
using WS2021.SWE3.OR_Mapper.ModelAttributes;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    /// <summary>This is a student implementation (from School example).</summary>
    [Entity(TableName = "STUDENTS")]
    public class Student: Person
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Gets or sets the student's grade.</summary>
        public int Grade { get; set; }
    }
}
