using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator.SlotUtility;


namespace Simulator.Games
{
    internal class HotTripleSevens
    {
        internal Random rand = new Random();

        internal const int WR = 0;
        internal const int H1 = 1;
        internal const int H2 = 2;
        internal const int H3 = 3;
        internal const int L1 = 4;
        internal const int L2 = 5;
        internal const int L3 = 6;
        internal const int DUMMY = 7;
        internal const int SC = 8;

        internal const int GRID_WIDTH = 3;
        internal const int GRID_HEIGHT = 5;

        internal const int BET_SIZE = 1;
        internal readonly int[][] payouts =
        {
            new int[] {0, 0, 0, 500},
            new int[] {0, 0, 0, 50},
            new int[] {0, 0, 0, 25},
            new int[] {0, 0, 0, 15},
            new int[] {0, 0, 0, 6},
            new int[] {0, 0, 0, 5},
            new int[] {0, 0, 0, 3},
        };
        internal readonly int mixPayout = 1;
        internal readonly int scPayout = 7;

        internal readonly int[][] payLines = {
            new int[] { 2, 2, 2},
            new int[] { 1, 1, 1},
            new int[] { 3, 3, 3},
            new int[] { 1, 2, 3},
            new int[] { 3, 2, 1}
        };

        internal const int fsMulti = 3;
        internal const int fsAmount = 10;

        /*internal static int[][][] baseReels = {
            new int[][] { 
                new int[] { H1, DUMMY, H2, DUMMY, L3, DUMMY, L3, DUMMY, H3, DUMMY, H3, DUMMY, L1, DUMMY, H1, DUMMY, H1, DUMMY, L2, DUMMY,
                            L2, DUMMY, H2, DUMMY, L3, DUMMY, L3, DUMMY, H3, DUMMY, H1, DUMMY, L1, DUMMY, L1, DUMMY, H2, DUMMY, H2, DUMMY,
                            WR, DUMMY, H1, DUMMY, L1, DUMMY, L1, DUMMY, H3, DUMMY, H3, DUMMY, L2, DUMMY, L2, DUMMY, H2, DUMMY, H2, DUMMY,
                            }
            }
        };*/



        internal readonly double[] LAND_DOUBLE_SYM_PROBA = { 0.5, 0.5, 0.5 };//{ 0.539, 0.451, 0.539 };
        internal readonly double[] LAND_DOUBLE_SYM_PROBA_FS = { 0.56, 0.43, 0.56 };//{ 0.5, 0.5, 0.5 };

        internal static RandomWeightArray symbolBGWeights = new RandomWeightArray(new int[,]
        {
            { H1, 8},
            { H2, 8},
            { H3, 8},
            { L1, 9},
            { L2, 9},
            { L3, 10},
            //{ H1, 10},
            //{ H2, 10},
            //{ H3, 10},
            //{ L1, 10},
            //{ L2, 10},
            //{ L3, 10},
        });

        internal static RandomWeightArray symbolFSWeights = new RandomWeightArray(new int[,]
        {
            { H1, 7},
            { H2, 8},
            { H3, 9},
            { L1, 11},
            { L2, 12},
            { L3, 13},
            //{ H1, 8},
            //{ H2, 8},
            //{ H3, 8},
            //{ L1, 10},
            //{ L2, 10},
            //{ L3, 10},
        });

        internal const double wrTeaseProba = 0.1;
        internal static RandomWeightArray WRAmountBGWeights = new RandomWeightArray(new int[,]
        {
            { 0, 13944},
            { 1, 5805},
            { 2, 240},
            { 3, 1},
        });

        internal static RandomWeightArray WRAmountFSWeights = new RandomWeightArray(new int[,]
        {
            { 0, 14900},
            { 1, 6088},
            { 2, 9},
            { 3, 1},
            { 4, 1},
            { 5, 1},
            //{ 6, 1},
        });

        internal const double SCProba = 0.153;

        internal readonly double[] bgRedrawThresholds = new double[]
        {
               0,    1, 3, 100
        };

        internal readonly double[] bgRedrawProbas = new double[]
        {
               0,   0,  0,   0
            //0.02, 0.02, 0, 0.2
        };

        internal readonly double[] fsRedrawThresholds = new double[]
        {
            0,      1,  5,  90
        };

        internal readonly double[] fsRedrawProbas = new double[]
        {
            0,     0,   0,   0
            //0.9, 0.01,  0, 0.85
        };

        // State Variable
        internal MathSummary summary = new MathSummary();

        internal int[,] SpinSymbols(bool basegame)
        {
            var symbolWeights = basegame ? symbolBGWeights : symbolFSWeights;
            var wrAmountWeights = basegame ? WRAmountBGWeights : WRAmountFSWeights;
            var landDoubleSymProba = basegame ? LAND_DOUBLE_SYM_PROBA : LAND_DOUBLE_SYM_PROBA_FS;

            int[,] symbols = new int[GRID_WIDTH,GRID_HEIGHT];
            List<Coordiante> possibleWR = new List<Coordiante>();
            for(int reel=0; reel<GRID_WIDTH; reel++)
            {
                bool doubleSym = rand.NextDouble() < landDoubleSymProba[reel];
                bool hasSC = false;
                for(int row=0; row<GRID_HEIGHT; row++)
                {
                    if ((doubleSym && row % 2 == 0) || (!doubleSym && row % 2 == 1)) {
                        symbols[reel, row] = DUMMY;
                    }
                    else
                    {
                        if(basegame && !hasSC && rand.NextDouble() < SCProba)
                        {
                            symbols[reel, row] = SC;
                            hasSC = true;
                        }
                        else
                        {
                            symbols[reel, row] = symbolWeights.rollSingleItem(rand);                           
                        }
                    }
                }
                for (int row = 0; row < GRID_HEIGHT; row++)
                {
                    if(!hasSC && symbols[reel, row] != DUMMY)
                    {
                        if (row == 0 || row == GRID_HEIGHT - 1)
                        {
                            if (rand.NextDouble() < wrTeaseProba)
                                possibleWR.Add(new Coordiante(reel, row));
                        }
                        else
                            possibleWR.Add(new Coordiante(reel, row));
                    }
                }
            }
            int wrAmount = wrAmountWeights.rollSingleItem(rand);
            wrAmount = Math.Min(wrAmount, possibleWR.Count);
            while(wrAmount > 0)
            {
                var index = rand.Next(possibleWR.Count);
                var wildCoor = possibleWR[index];
                possibleWR.RemoveAt(index);
                symbols[wildCoor.x, wildCoor.y] = WR;
                wrAmount--;
            }
            return symbols;
        }


        internal class SlotSpin
        {
            HotTripleSevens Game;
            internal int[,] SymbolMatrix;
            internal bool Basegame;
            internal List<SlotWin> Wins;
            internal bool TriggerFS;

            internal SlotSpin(HotTripleSevens game, bool basegame)
            {
                this.Game = game;
                Basegame = basegame;
                SymbolMatrix = game.SpinSymbols(Basegame);
                Wins = game.CheckWins(SymbolMatrix, Basegame ? 1 : fsMulti);
                var scWins = game.CheckSCWins(SymbolMatrix, Basegame ? 1 : fsMulti);
                if(scWins.Count>0)
                {
                    if (!Basegame)
                        Console.WriteLine("Scatter wins in FS");
                    this.TriggerFS = true;
                    Wins.AddRange(scWins);
                }
            }

            internal int GetTotalWin()
            {
                return Wins.Sum(x => x.win);
            }
        }

        internal SlotSpin GenerateMainSpin()
        {
            SlotSpin spin;// = new SlotSpin(this, true);
            double redrawProba = 0;
            do
            {
                spin = new SlotSpin(this, true);
                double win = (double)spin.GetTotalWin() / BET_SIZE;
                redrawProba = 0;// MathUtil.GetProbaByValue(bgRedrawThresholds, bgRedrawProbas, win);
            } while (rand.NextDouble() < redrawProba);
            return spin;
        }

        internal List<SlotSpin> GenerateFreeSpins()
        {
            List<SlotSpin> spins = new List<SlotSpin>();
            double redrawProba = 0;
            do
            {
                spins.Clear();
                for (int i = 0; i < fsAmount; i++)
                {
                    spins.Add(new SlotSpin(this, false));
                }
                double fsWin = (double)spins.Sum(x => x.GetTotalWin()) / BET_SIZE;

                redrawProba = 0;// MathUtil.GetProbaByValue(fsRedrawThresholds, fsRedrawProbas, fsWin);
                
            } while (rand.NextDouble() < redrawProba);
            return spins;
        }
        internal void PlayGame()
        {
            int win = 0;
            var mainSpin = GenerateMainSpin();
            win += mainSpin.GetTotalWin();
            //DebugUtil.PrintReels(mainSpin.SymbolMatrix);
            //Console.WriteLine($"{mainSpin.GetTotalWin()}");
            //Console.WriteLine("----------------");

            List<SlotSpin>? freeSpins = null;
            if(mainSpin.TriggerFS)
            {
                freeSpins = GenerateFreeSpins();
            }
            //Console.WriteLine(symbolWeights.rollSingleItem(rand, true));

            //collect statistics
            summary.Add(mainSpin, freeSpins);
            
        }

        internal class MathSummary
        {
            internal long SampleSize = 0;
            internal Average ReturnToPlayer = new();
            internal Average BaseRTP = new();
            internal Average HitRate = new();
            internal double MaxWin = 0;
            internal Average HitFSProba = new();
            internal Average AverageFSPayouts = new();
            internal Average AverageSquared = new();
            internal double[] SymbolWinDist = new double[9];
            internal Distribution WildInWinningAreaBG = new Distribution(new double[] { 0, 1.0, 2.0, 3.0});
            internal Distribution SCInWinningAreaBG = new Distribution(new double[] { 0, 1.0, 2.0, 3.0 });
            internal Distribution WinDistribution = new Distribution(new double[] { 0, 1.0, 5, 15, 30, 100, 1000});
            internal Distribution BGWinDistribution = new Distribution(new double[] { 0, 1.0, 5, 15, 30, 100 , 1000});
            internal Distribution FSWinDistribution = new Distribution(new double[] { 0, 1.0, 5, 15, 30, 100, 1000 });
            internal Distribution SingleFSWinDist = new Distribution(new double[] { 0, 1.0, 15, 100 });

            internal Distribution TriggeringFSWinDist = new Distribution(new double[] { 0, 1.0, 5, 15, 30, 50, 100 });
            internal void Add(SlotSpin mainSpin, List<SlotSpin>? freeSpins)
            {
                SampleSize++;
                int win = mainSpin.GetTotalWin() + (freeSpins == null ? 0 : freeSpins.Sum(x => x.GetTotalWin()));
                ReturnToPlayer.Add((double)win / BET_SIZE);
                AverageSquared.Add((double)win * win / BET_SIZE / BET_SIZE);
                if ((double)win / BET_SIZE > MaxWin)
                    MaxWin = (double)win / BET_SIZE;
                BaseRTP.Add((double)mainSpin.GetTotalWin() / BET_SIZE);
                BGWinDistribution.Add((double)mainSpin.GetTotalWin() / BET_SIZE);
                HitRate.Add(win>0 ? 1 : 0);
                HitFSProba.Add(mainSpin.TriggerFS ? 1 : 0);
                WinDistribution.Add((double)win / BET_SIZE);

                int wildInWinningAreaCount = 0;
                int scInWinningAreaCount = 0;
                for(int i=0; i<GRID_WIDTH; i++)
                {
                    for(int j=1; j<GRID_HEIGHT-1; j++)
                    {
                        if (mainSpin.SymbolMatrix[i,j] == WR)
                        {
                            wildInWinningAreaCount++;
                        }
                        if (mainSpin.SymbolMatrix[i, j] == SC)
                        {
                            scInWinningAreaCount++;
                        }

                    }
                }
                WildInWinningAreaBG.Add((double)wildInWinningAreaCount);
                SCInWinningAreaBG.Add((double)scInWinningAreaCount);

                foreach(SlotWin slotWin in mainSpin.Wins)
                {
                    SymbolWinDist[slotWin.symbolWinIndex] += (double)slotWin.win / BET_SIZE;
                }
                if(freeSpins != null)
                {
                    TriggeringFSWinDist.Add((double)mainSpin.GetTotalWin() / BET_SIZE);
                    AverageFSPayouts.Add((double)freeSpins.Sum(x => x.GetTotalWin()) / BET_SIZE);
                    FSWinDistribution.Add((double)freeSpins.Sum(x => x.GetTotalWin()) / BET_SIZE);
                    foreach(var spin in freeSpins)
                    {
                        SingleFSWinDist.Add((double)spin.GetTotalWin() / BET_SIZE);
                        foreach (SlotWin slotWin in spin.Wins)
                        {
                            SymbolWinDist[slotWin.symbolWinIndex] += (double)slotWin.win / BET_SIZE;
                            //if(SlotWin.symbolWinIndex)
                        }
                    }
                }
            }

            internal void WriteOnConsole()
            {
                Console.WriteLine($"Sample Size: {SampleSize}");
                Console.WriteLine($"Variance: {AverageSquared.Value - Math.Pow(ReturnToPlayer.Value,2)}");
                Console.WriteLine($"Max Win: {MaxWin}x");
                Console.WriteLine($"RTP: {ReturnToPlayer.Percentage}");
                Console.WriteLine($"Base RTP: {BaseRTP.Percentage}");
                Console.WriteLine($"FS RTP: {AverageFSPayouts.Value * HitFSProba.Value * 100}%");
                Console.WriteLine($"Hit Rate: {HitRate.Percentage}");
                Console.WriteLine($"Hit FS Probability: {HitFSProba.Percentage}");
                Console.WriteLine(string.Format("Average FS Payouts: {0,2:F}x", AverageFSPayouts.Value));
                Console.WriteLine("Symbol Win Distribution:");
                foreach(var symbolWin in SymbolWinDist)
                {
                    Console.WriteLine($"{symbolWin * 100 / SampleSize }%");
                }
                Console.WriteLine("--------------");
                Console.WriteLine("Win Distribution:");
                WinDistribution.PrintProbaOnConsole();
                WinDistribution.PrintValuesOnConsole();
                //WinDistribution.PrintCumulatedProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("Base Game Win Distribution:");
                BGWinDistribution.PrintProbaOnConsole();
                BGWinDistribution.PrintValuesOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("FS Win Distribution:");
                FSWinDistribution.PrintProbaOnConsole();
                FSWinDistribution.PrintValuesOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("Single FS Win Distribution:");
                SingleFSWinDist.PrintCumulatedProbaOnConsole();
                //Console.WriteLine("--------------");
                //Console.WriteLine("Triggering FS Base Game Win Distribution:");
                //TriggeringFSWinDist.PrintCumulatedProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("Wild in Winning Area BG:");
                WildInWinningAreaBG.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("SC in Winning Area BG:");
                SCInWinningAreaBG.PrintProbaOnConsole();
                Console.WriteLine("--------------");
            }
        }

        internal List<SlotWin> CheckSCWins(int[,] symbols, int multi)
        {
            List<SlotWin> wins = new List<SlotWin>();
            int scAmount = 0;
            for(int i=0; i<GRID_WIDTH; i++)
            {
                for(int j=1; j<=3; j++)
                {
                    if (symbols[i, j] == SC) 
                        scAmount++; 
                }
            }
            if(scAmount >= 3)
            {
                if (scAmount > 3)
                    Console.WriteLine("possibility of more than 3 scatters");
                wins.Add(new SlotWin { payline = -1, symbolWinIndex = SC, multi = multi, win = multi * scPayout });
            }
            return wins;
        }

        internal List<SlotWin> CheckWins(int[,] symbols, int multi)
        {
            List<SlotWin> wins = new List<SlotWin>();
            for(int i=0; i<payLines.Length; i++){
                int[] line = payLines[i];

                int wildAmount = 0;
                int[] highAmount = new int[3];
                int[] lowOrHighAmount = new int[3];
                for(int reel=0; reel<GRID_WIDTH; reel++)
                {
                    if (symbols[reel, line[reel]] == WR)
                        wildAmount++;
                    else if(symbols[reel, line[reel]] >= H1 && symbols[reel, line[reel]] <= H3)
                    {
                        highAmount[symbols[reel, line[reel]] - H1]++;
                        lowOrHighAmount[symbols[reel, line[reel]] - H1]++;
                    }
                    else if (symbols[reel, line[reel]] >= L1 && symbols[reel, line[reel]] <= L3)
                        lowOrHighAmount[symbols[reel, line[reel]] - L1]++;
                }
                if (wildAmount == GRID_WIDTH)
                {
                    wins.Add(new SlotWin { payline = i, symbolWinIndex = WR, multi = multi, win = multi*payouts[WR][GRID_WIDTH] });
                    continue;
                }
                else if(wildAmount + highAmount.Max() == GRID_WIDTH) 
                {
                    int sym = highAmount.ToList().IndexOf(highAmount.Max()) + H1;
                    wins.Add(new SlotWin { payline = i, symbolWinIndex = sym, multi = multi, win = multi * payouts[sym][GRID_WIDTH] });
                    continue;
                }
                else if (wildAmount + lowOrHighAmount.Max() == GRID_WIDTH)
                {
                    int sym = lowOrHighAmount.ToList().IndexOf(lowOrHighAmount.Max()) + L1;
                    wins.Add(new SlotWin { payline = i, symbolWinIndex = sym, multi = multi, win = multi * payouts[sym][GRID_WIDTH] });
                    continue;
                }
                else if (wildAmount + lowOrHighAmount.Count(x => x==1) == GRID_WIDTH)
                {
                    if (wildAmount >= 2)
                        Console.WriteLine("2+ wild in mixed pay");
                    int mixPayIndex = 7;
                    wins.Add(new SlotWin { payline = i, symbolWinIndex = mixPayIndex, multi = multi, win = multi * mixPayout });
                }
            }
            return wins;
        }

        internal class SlotWin
        {
            internal int payline;
            internal int symbolWinIndex;
            internal int multi;
            internal int win;
        }
    }
}
