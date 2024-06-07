using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simulator.SlotUtility;


namespace Simulator.Games
{
    internal class HotMania
    {
        internal Random rand = new Random();

        internal const int S1 = 0;
        internal const int S2 = 1;
        internal const int S3 = 2;
        internal const int S4 = 3;
        internal const int S5 = 4;
        internal const int S6 = 5;
        internal const int S7 = 6;
        internal const int S8 = 7;

        internal const int GRID_WIDTH = 3;
        internal const int GRID_HEIGHT = 3;

        internal const int BET_SIZE = 1;
        internal readonly int[][] payouts =
        {
            new int[] {0, 0, 0, 200},
            new int[] {0, 0, 0, 50},
            new int[] {0, 0, 0, 20},
            new int[] {0, 0, 0, 10},
            new int[] {0, 0, 0, 8},
            new int[] {0, 0, 0, 5},
            new int[] {0, 0, 0, 3},
            new int[] {0, 0, 0, 1},
        };

        internal readonly int[][] payLines = {
            new int[] { 1, 1, 1},
            new int[] { 0, 0, 0},
            new int[] { 2, 2, 2},
            new int[] { 0, 1, 2},
            new int[] { 2, 1, 0}
        };

        internal readonly int[][] reels =
        {
            new int[] { S1, S1, S2, S2, S2, S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                            S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                            S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                            S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                            S3, S3, S3,             S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                                                                S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                                                                                        
                                                                                                        
                      },
            new int[] { S1, S1, S2, S2, S2, S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                        S1, S1, S2, S2, S2, S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                S2, S2, S2, S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                                                    S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                                                    S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8, 
                                                                    S5, S5, S5, 
                                                                                
                      },
            new int[] { S1, S1, S2, S2, S2, S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                S2, S2, S2, S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                            S3, S3, S3, S4, S4, S4, S5, S5, S5, S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                            S3, S3, S3, S4, S4, S4,             S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                                        S4, S4, S4,             S6, S6, S6, S7, S7, S7, S8, S8, S8,
                                                        S4, S4, S4,                         S7, S7, S7,
                      },
        };

        internal static RandomWeightArray fullReelSymbolWeights = new RandomWeightArray(new int[,]
        {
            { S3, 1},
            { S4, 3},
            { S5, 7},
            { S6, 15},
            { S7, 31},
            { S8, 58},
        });

        internal static RandomWeightArray fullReelAmountWeights = new RandomWeightArray(new int[,]
        {
            { 0, 175},
            { 1, 40},
            { 2, 10},
            { 3, 1},
        });

        internal const int WHEEL_ITEM_COUNT = 8;
        internal const int BONUS_END_COUNT = 2;
        internal const int BONUS_BOOST_COUNT = 1;

        internal static RandomWeightArray endBonusIndexWeights = new RandomWeightArray(new int[,]
        {
            { 0, 3},
            { 1, 30},
            { 2, 22},
            { 3, 16},
            { 4, 16},
            { 5, 8},
            { 6, 3},
            { 7, 1},
        });

        internal static RandomWeightArray boostBonusIndexWeights = new RandomWeightArray(new int[,]
        {
            { 0, 1},
            { 1, 1},
            { 2, 1},
            { 3, 1},
            { 4, 1},
            { 5, 1},
            { 6, 2},
            { 7, 2},
        });

        internal const double redrawDeadBoostProba = 0.25;

        // State Variable
        internal MathSummary summary = new MathSummary();

        internal int[,] SpinSymbols(bool basegame)
        {
            int[,] symbols = SlotUtil.SpinReels(rand, reels, GRID_HEIGHT);
            int fullReelAmount = fullReelAmountWeights.rollSingleItem(rand);
            if(fullReelAmount > 0)
            {
                List<int> reelIndexPool = new List<int>() { 0, 1, 2, };
                List<int> reelIndexes = new List<int>();
                for(int i = 0; i<fullReelAmount; i++)
                {
                    int tmpIndex = rand.Next(reelIndexPool.Count);
                    reelIndexes.Add(reelIndexPool[tmpIndex]);
                    reelIndexPool.Remove(reelIndexPool[tmpIndex]);
                }
                //tmp
                /*double proba = (double)fullReelAmount / 3;
                for (int i = 0; i < fullReelAmount; i++)
                {
                    if(rand.NextDouble() < proba)
                        reelIndexes.Add(i);
                }*/
                //tmp

                int fullReelSymbol = fullReelSymbolWeights.rollSingleItem(rand);  
                foreach(var reel in reelIndexes)
                {
                    for (int row = 0; row < GRID_HEIGHT; row++)
                    {
                        symbols[reel, row] = fullReelSymbol;
                    }
                }
            }

            return symbols;
        }


        internal class SlotSpin
        {
            HotMania Game;
            internal int[,] SymbolMatrix;
            internal bool Basegame;
            internal List<SlotWin> Wins;
            internal bool TriggerBonus;

            internal SlotSpin(HotMania game, bool basegame)
            {
                this.Game = game;
                Basegame = basegame;
                SymbolMatrix = game.SpinSymbols(Basegame);
                Wins = game.CheckWins(SymbolMatrix);
                TriggerBonus = game.IsTriggeringBonus(SymbolMatrix);
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

        internal BonusWin GenerateBonus(int symbolType, int basePayout)
        {
            var bonus = new BonusWin(symbolType, basePayout);
            var remainEndCount = BONUS_END_COUNT;

            //tmp
            //bool redraw1 = true;
            //while (redraw1)
            //{
            //    remainEndCount = 2;
            //    redraw1 = false;
            //    while (remainEndCount > 0)
            //    {
            //        var endIndex = endBonusIndexWeights.rollSingleItem(rand);
            //        bonus.Outcomes[endIndex] = WheelOutcome.End;
            //        if (bonus.Outcomes[endIndex] == WheelOutcome.Win)
            //        {
            //            redraw1 = true;
            //        }
            //        remainEndCount--;
            //    }
            //}
            //tmp
            while (remainEndCount > 0)
            {
                var endIndex = endBonusIndexWeights.rollSingleItem(rand);
                if (bonus.Outcomes[endIndex] == WheelOutcome.Win)
                {
                    bonus.Outcomes[endIndex] = WheelOutcome.End;
                    remainEndCount--;
                }
            }
            var remainBoostCount = BONUS_BOOST_COUNT;
            while (remainBoostCount > 0)
            {
                var boostIndex = boostBonusIndexWeights.rollSingleItem(rand);
                if (bonus.Outcomes[boostIndex] == WheelOutcome.Win)
                {
                    bool redraw = false;
                    if (boostIndex + 1 < bonus.Outcomes.Length && bonus.Outcomes[boostIndex + 1] == WheelOutcome.End)
                        if (rand.NextDouble() < redrawDeadBoostProba)
                            redraw = true;
                    if (!redraw)
                    {
                        bonus.Outcomes[boostIndex] = WheelOutcome.Boost;
                        remainBoostCount--;
                    }
                }
            }
            bonus.TotalWin = GetBonusTotalWin(bonus);
            return bonus ;
        }
        internal void PlayGame()
        {
            int win = 0;
            var mainSpin = GenerateMainSpin();
            win += mainSpin.GetTotalWin();
            //DebugUtil.PrintReels(mainSpin.SymbolMatrix);
            //Console.WriteLine($"{mainSpin.GetTotalWin()}");
            //Console.WriteLine("----------------");

            BonusWin? bonus = null;
            if(mainSpin.TriggerBonus)
            {
                bonus = GenerateBonus(mainSpin.SymbolMatrix[0,0], mainSpin.GetTotalWin());
            }
            //Console.WriteLine(symbolWeights.rollSingleItem(rand, true));

            //collect statistics
            summary.Add(mainSpin, bonus);
            
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
            internal Average HitBoostProba = new();
            internal Average AverageBoostPayouts = new();
            internal Average BoostHitRate = new();
            internal Average AverageSquared = new();
            internal double[] SymbolWinDist = new double[8];
            internal double[] BonusSymbolProba = new double[8];
            internal Distribution WinDistribution = new Distribution(new double[] { 0, 1.0, 5, 15, 30, 100, 1000});
            internal Distribution BGWinDistribution = new Distribution(new double[] { 0, 1.0, 5, 15, 30, 100 , 1000});
            internal Distribution FSWinDistribution = new Distribution(new double[] { 0, 1.0, 5, 15, 30, 100, 1000 });
            internal double[] boostTriggerRoundProba = new double[8];
            internal double[] bonusEndRoundProba = new double[8];
            internal double fs = 0;
            internal double firstBoost = 0;
            internal double endBonus = 0;

            internal void Add(SlotSpin mainSpin, BonusWin? bonus)
            {
                SampleSize++;
                int win = mainSpin.GetTotalWin() + (bonus == null ? 0 : bonus.TotalWin);
                ReturnToPlayer.Add((double)win / BET_SIZE);
                AverageSquared.Add((double)win * win / BET_SIZE / BET_SIZE);
                if ((double)win / BET_SIZE > MaxWin)
                    MaxWin = (double)win / BET_SIZE;
                BaseRTP.Add((double)mainSpin.GetTotalWin() / BET_SIZE);
                BGWinDistribution.Add((double)mainSpin.GetTotalWin() / BET_SIZE);
                HitRate.Add(win>0 ? 1 : 0);
                HitFSProba.Add(mainSpin.TriggerBonus ? 1 : 0);
                WinDistribution.Add((double)win / BET_SIZE);


                foreach(SlotWin slotWin in mainSpin.Wins)
                {
                    SymbolWinDist[slotWin.symbolWinIndex] += (double)slotWin.win / BET_SIZE;
                }
                if(bonus != null)
                {
                    BonusSymbolProba[bonus.SymbolType] += 1.0;
                    HitBoostProba.Add(bonus.TriggerBoost ? 1 : 0);
                    if (bonus.TriggerBoost)
                    {
                        BoostHitRate.Add(bonus.BoostModeWin>0 ? 1 : 0);
                        AverageBoostPayouts.Add((double)bonus.BoostModeWin / BET_SIZE);
                    }

                    AverageFSPayouts.Add((double)bonus.TotalWin / BET_SIZE);
                    FSWinDistribution.Add((double)bonus.TotalWin / BET_SIZE);

                    fs += 1;
                    for(int i = 0; i<bonus.Outcomes.Length; i++)
                    {
                        if (bonus.Outcomes[i] == WheelOutcome.End)
                        {
                            if (i == 6)
                                bonusEndRoundProba[5]++;
                            else
                                bonusEndRoundProba[i]++;
                            break;
                        }
                        else if (bonus.Outcomes[i] == WheelOutcome.Boost)
                            boostTriggerRoundProba[i]++;
                    }
                    if (bonus.Outcomes[6] == WheelOutcome.End && bonus.Outcomes[7] == WheelOutcome.End)
                        endBonus += 1;
                    if (bonus.Outcomes[0] == WheelOutcome.Boost)
                        firstBoost += 1;
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
                for(int i=0; i<BonusSymbolProba.Length; i++)
                {
                    Console.WriteLine($"P({i} | Bonus): {BonusSymbolProba[i]/ fs}");
                }
                Console.WriteLine("--------------");
                Console.WriteLine($"P(Hit Boost | Bonus): {HitBoostProba.Percentage}");
                Console.WriteLine($"Boost Mode Hit Rate: {BoostHitRate.Percentage}");
                Console.WriteLine(string.Format("Average Boost Payouts: {0,2:F}x", AverageBoostPayouts.Value));
                Console.WriteLine("--------------");
                for (int i = 0; i < bonusEndRoundProba.Length; i++)
                {
                    Console.WriteLine($"P({i}th end | Bonus): {bonusEndRoundProba[i] / fs}");
                }
                Console.WriteLine("--------------");
                for (int i = 0; i < boostTriggerRoundProba.Length; i++)
                {
                    Console.WriteLine($"P({i}th boost | Bonus): {boostTriggerRoundProba[i] / fs}");
                }
                Console.WriteLine("--------------");
                Console.WriteLine($"P(first boost | Bonus): {firstBoost / fs}");
                Console.WriteLine($"P(full end bonus | Bonus): {endBonus / fs}");
                Console.WriteLine("--------------");
            }
        }

        internal List<SlotWin> CheckWins(int[,] symbols)
        {
            List<SlotWin> wins = new List<SlotWin>();
            for(int i=0; i<payLines.Length; i++){
                int[] line = payLines[i];
                int symbolType = symbols[0, line[0]];
                int symbolSize = 1;
                for(int reel=1; reel<GRID_WIDTH; reel++)
                {
                    if (symbols[reel, line[reel]] == symbolType)
                        symbolSize++;
                    else
                        break;
                }
                if(symbolSize == GRID_WIDTH)
                {
                    wins.Add(new SlotWin { payline = i, symbolWinIndex = symbolType, win = payouts[symbolType][GRID_WIDTH] });

                }
            }
            return wins;
        }

        internal bool IsTriggeringBonus(int[,] symbols)
        {
            int targetSym = -1;
            for (int i = 0; i < GRID_WIDTH; i++)
            {
                for(int j=0; j < GRID_HEIGHT; j++)
                {
                    if(targetSym == -1 && symbols[i,j] >=S3 && symbols[i, j] <= S8)
                    {
                        targetSym = symbols[i, j];
                    }
                    if(targetSym != symbols[i, j])
                        return false;
                }
            }
            return true;
        }

        internal int GetBonusTotalWin(BonusWin bonus)
        {
            int totalWin = 0;
            int multiplier = 1;
            bool boostMode = false;
            foreach(var outcome in bonus.Outcomes)
            {
                if (outcome == WheelOutcome.End)
                    return totalWin;
                else if (outcome == WheelOutcome.Boost)
                {
                    totalWin += bonus.BasePayout;
                    bonus.TriggerBoost = true;
                    boostMode = true;
                    multiplier++;
                }
                else if (outcome == WheelOutcome.Win)
                {
                    if (boostMode)
                    {
                        bonus.BoostModeWin += bonus.BasePayout * multiplier;
                        totalWin += bonus.BasePayout * multiplier;
                        multiplier++;
                    }
                    else
                        totalWin += bonus.BasePayout;
                }
                else
                    Console.WriteLine("Unexpected outcome in wheel bonus");
            }
            return totalWin;
        }
        
        internal enum WheelOutcome
        {
            Win,
            End,
            Boost
        }

        internal class BonusWin
        {
            internal WheelOutcome[] Outcomes;
            internal int SymbolType;
            internal int BasePayout;
            internal int TotalWin;
            internal bool TriggerBoost;
            internal int BoostModeWin;
            internal BonusWin(int symbolType, int basePayout)
            {
                Outcomes = new WheelOutcome[WHEEL_ITEM_COUNT];
                SymbolType = symbolType;
                BasePayout = basePayout;
                TriggerBoost = false;
                BoostModeWin = 0;
            }
        }

        internal class SlotWin
        {
            internal int payline;
            internal int symbolWinIndex;
            internal int win;
        }
    }
}
