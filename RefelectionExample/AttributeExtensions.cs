using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefelectionExample
{
    public static class AttributeExtensions
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class UseHeaderAttribute : Attribute
        {

        }

        [AttributeUsage(AttributeTargets.Property)]
        public class UseAsLineAttribute : Attribute
        {

        }
        [AttributeUsage(AttributeTargets.Property)]
        public class DoubleLengthAttribute : Attribute
        {
            public int Length;
            public int AfteDot;

            public DoubleLengthAttribute(int length, int afteDot)
            {
                Length = length;
                AfteDot = afteDot;
            }
        }
    }
}
