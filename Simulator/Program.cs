// See https://aka.ms/new-console-template for more information
using Simulator.Games;
using System;

namespace Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HotTripleSevens game = new HotTripleSevens();
            for (int i = 0; i < 500_000_000; i++)
            {
                if ((i+1) % 1_000_000 == 0)
                    Console.WriteLine($"{i}");
                game.PlayGame();
            }
            game.summary.WriteOnConsole();
           
        }
    }
}