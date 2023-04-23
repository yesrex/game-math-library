using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GameFeatureSharedLibrary.RandomNumber;


namespace FunctionTesting
{
    internal class Program
    {
        internal static void Main(string[] args) 
        {
            for (int i = -5; i < 5; i++) 
            {
                Console.WriteLine(i % 3);
            }
            
        }
    }
}
