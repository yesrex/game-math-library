using Simulator.SlotUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Games
{
    internal class CashCrew
    {
        internal static bool Debug = false;
        internal static Random rand = new Random();

        internal enum BetMode
        {
            Normal,
            BonusHunt,
            FeatureSpin,
            Buy,
            SuperBuy,
        }
        internal static readonly BetMode simulationBetMode = BetMode.Normal;
        internal static readonly int RtpVariantIndex = 1; //0 :98%, 1: 90%

        internal const int WR = 0;
        internal const int H1 = 1;
        internal const int H2 = 2;
        internal const int H3 = 3;
        internal const int H4 = 4;
        internal const int L5 = 5;
        internal const int L6 = 6;
        internal const int L7 = 7;
        internal const int L8 = 8;

        internal const int SC = 9;
        internal const int RETRIGGER =10;
        internal const int COIN = 11;
        internal const int COIN_COLLECT = 12;

        internal const int DUMMY = 13;

        internal const int GRID_WIDTH = 5;
        internal const int GRID_HEIGHT = 5;
        internal const int SC_TO_FS = 3;
        internal const int SC_TO_SFS = 4;
        internal const int START_FS = 6;
        internal const int START_SUPER_FS = 9;

        internal const int BET_SIZE = 10;
        internal static readonly int[][] Payouts =
        {
            new int[] {0, 0,  0,  0,   0, 250}, // WR
            new int[] {0, 0,  0, 10,  35, 100},
            new int[] {0, 0,  0,  7,  25,  70},
            new int[] {0, 0,  0,  6,  20,  60},
            new int[] {0, 0,  0,  5,  15,  50},
            new int[] {0, 0,  0,  2,   7,  20},
            new int[] {0, 0,  0,  2,   7,  20},
            new int[] {0, 0,  0,  2,   7,  20},
            new int[] {0, 0,  0,  2,   7,  20},
            new int[] {0, 0,  0,  0,   0,   0}, //SC
            new int[] {0, 0,  0,  0,   0,   0}, //RETRIGGER
            new int[] {0, 0,  0,  4,  10,  40}, //COIN
            new int[] {0, 0,  0,  0,   0,   0}, //COIN COLLECT
        };

        internal static readonly int[][] PayLines = {
            new int[] { 0, 0, 0, 0, 0},
            new int[] { 1, 1, 1, 1, 1},
            new int[] { 2, 2, 2, 2, 2},
            new int[] { 3, 3, 3, 3, 3},
            new int[] { 4, 4, 4, 4, 4},
            new int[] { 0, 1, 0, 1, 0},
            new int[] { 1, 2, 1, 2, 1},
            new int[] { 2, 3, 2, 3, 2},
            new int[] { 3, 4, 3, 4, 3},
            new int[] { 1, 0, 1, 0, 1},
            new int[] { 2, 1, 2, 1, 2},
            new int[] { 3, 2, 3, 2, 3},
            new int[] { 4, 3, 4, 3, 4},
            new int[] { 0, 1, 2, 3, 4},
            new int[] { 1, 2, 3, 2, 1},
            new int[] { 2, 3, 4, 3, 2},
            new int[] { 4, 3, 2, 1, 0},
            new int[] { 3, 2, 1, 2, 3},
            new int[] { 2, 1, 0, 1, 2},
        };

        internal static readonly RandomWeightArray SymbolWeights = new RandomWeightArray(new int[,]{
            {H1, 1},
            {H2, 1},
            {H3, 1},
            {H4, 1},
            {L5, 3},
            {L6, 3},
            {L7, 3},
            {L8, 3},
        });

        internal static readonly double CoinCollectProba = 0.049;
        internal static readonly double CoinCollectProbaFS = 0.45;

        internal static readonly RandomWeightArray CoinAmountWeights = new RandomWeightArray(new int[,]{
            {0,   450},
            {1,   100},
            {2,   120},
            {3,   120},
            {4,   90},
            {5,   50},
            {6,   50},
            {7,    5},
            {8,    5},
            {9,    5},
            {10,   5},
        });

        internal static readonly RandomWeightArray CoinAmountWeightsFeatureSpin = new RandomWeightArray(new int[,]{
            {5,   40},
            {6,   35},
            {7,   15},
            {8,   10},
            {9,   10},
            {10,  10},
        });

        internal static readonly RandomWeightArray CoinAmountWeightsFS = new RandomWeightArray(new int[,]{
            {0,   20},
            {1,   100},
            {2,   100},
            {3,   100},
            {4,   50},
            {5,   50},
            {6,   40},
            {7,   20},
            {8,   10},
            {9,    5},
            {10,   5},
        });

        internal static readonly RandomWeightArray CoinValueWeights = new RandomWeightArray(new int[,]{
            {1,    220},
            {2,    170},
            {3,    160},
            {4,    100},
            {5,    70},
            {10,   50},
            {15,   25},
            {20,   15},
            {25,   10},
            {50,   10},
            {100,   4},
            {250,   4},
            {500,   2},
        });

        internal static readonly RandomWeightArray CoinValueWeightsSFS = new RandomWeightArray(new int[,]{
            {5,    600},
            {10,   300},
            {15,    50},
            {20,    20},
            {25,    10},
            {50,    10},
            {100,    7},
            {250,    2},
            {500,    1},
        });

        internal static readonly RandomWeightArray ScAmountWeights = new RandomWeightArray(new int[,]{
            {0, 5700},
            {1, 870},
            {2, 400},
            {3, 29},
            {4, 1},
        });

        internal static readonly RandomWeightArray ScAmountWeightsBonusHunt = new RandomWeightArray(new int[,]{
            {0, 80},
            {1, 80},
            {2, 40},
            {3, 9},
            {4, 1},
        });

        internal static readonly RandomWeightArray RetriggerAmountWeights = new RandomWeightArray(new int[,]{
            {0, 120},
            {1, 9},
            {2, 1},
        });

        internal static readonly RandomWeightArray RetriggerValueWeights = new RandomWeightArray(new int[,]{
            {2, 3},
            {3, 3},
            {4, 2},
            {5, 1},
            {6, 1},
        });

        internal static readonly RandomWeightArray WrAmountWeights = new RandomWeightArray(new int[,]{
            {0, 17700},
            {1, 1500},
            {2, 750},
            {3, 45},
            {4, 4},
            {5, 1},
        });

        internal static readonly RandomWeightArray WrAmountWeightsFeatureSpin = new RandomWeightArray(new int[,]{
            {1, 200},
            {2, 95},
            {3, 45},
            {4, 4},
            {5, 1},
        });

        internal static readonly RandomWeightArray WrMultiplierWeights = new RandomWeightArray(new int[,]{
            {2, 600},
            {3, 200},
            {4, 100},
            {5, 45},
            {6, 25},
            {7, 10},
            {8, 10},
            {9, 6},
            {10, 1},
            {15, 1},
            {20, 1},
            {25, 1},
        });

        internal double[] deadSpinProbas = new double[] { 0.165, 0.233 };
        internal double[] deadSpinProbasBonusHunt = new double[] { 0.076, 0.151 };
        internal readonly double[] betterFeatureSpinProba = { 0.66, 0.33 };

        internal static readonly double[] bgRedrawThresholds =
        {
                          0,  0.1,   5,   20,   50,   100, 200,  1000,  5000,   11000
        };

        internal static readonly double[][] bgRedrawProbas =
        {
           new double[]{  0,    0,   0, 0.00, 0.00,  0.18, 0.50,  0.75, 0.90,    1 },
           new double[]{  0,    0,   0, 0.00, 0.00,  0.20, 0.60,  0.90, 0.95,    1 },
           new double[]{  0,    0,   0, 0.00, 0.00,  0.00, 0.00,  0.30,  0.9,    1 },
           new double[]{  0,    0,   0, 0.00, 0.00,  0.00, 0.50,  0.90, 0.95,    1 },
        };

        internal static readonly double[] betterFsProba = { 0.805, 0.395 };
        internal static readonly double[] bonusRedrawThresholds =
        {
                          0,     1,    10,     50,   100,   200,   500,   1000,   5000,  11000
        };

        internal static readonly double[][] bonusRedrawProbas = 
        {
           new double[]{  1,  0.8,   0.4,   0.00,   0.00,  0.60,  0.70,   0.80,   0.96,  1 },
           new double[]{  1,  0.8,   0.0,   0.00,   0.40,  0.70,  0.70,   0.80,   0.96,  1 },
        };

        internal static readonly double[] betterSuperFsProba = { 0.703, 0.47 };
        internal static readonly double[] superFsRedrawThresholds =
        {
                          0,     1,    10,   100,   200,   500,   1000,   2000,   5000,  11000
        };

        internal static readonly double[][] superFsRedrawProbas =
        {
           new double[]{  1, 0.95,   0.8,   0.00,   0.00,  0.10,  0.20,   0.30,   0.40,  1 },
           new double[]{  1, 0.95,   0.4,   0.00,   0.00,  0.75,  0.80,   0.90,   0.96,  1 },
        };

        // State Variable
        internal MathSummary summary = new MathSummary();

        internal class SymbolGrid
        {
            internal int[,] Symbols;
            internal int[,] Values;
            internal bool[,] LockedWilds;
            internal bool Basegame;
            internal bool SuperFS;
            internal bool BonusHuntMainSpin;
            internal bool FeatureSpin;
            internal int ForcedSC;
       
            internal SymbolGrid(bool basegame, bool superFS = false, bool[,]? lockedWilds = null)
            {
                this.Basegame = basegame;
                Symbols = new int[GRID_WIDTH, GRID_HEIGHT];
                Values = new int[GRID_WIDTH, GRID_HEIGHT];
                SuperFS = superFS;
                LockedWilds = lockedWilds == null ? new bool[GRID_WIDTH,GRID_HEIGHT] : lockedWilds;
            }
            internal SymbolGrid(BetMode mode)
            {
                Basegame = true;
                Symbols = new int[GRID_WIDTH, GRID_HEIGHT];
                Values = new int[GRID_WIDTH, GRID_HEIGHT];
                SuperFS = false;
                LockedWilds = new bool[GRID_WIDTH, GRID_HEIGHT];
                if (mode == BetMode.BonusHunt)
                    BonusHuntMainSpin = true;
                else if (mode == BetMode.FeatureSpin)
                    FeatureSpin = true;
                else if (mode == BetMode.Buy)
                    ForcedSC = SC_TO_FS;
                else if (mode == BetMode.SuperBuy)
                    ForcedSC = SC_TO_SFS;
            }

            internal void Spin()
            {
                int wrCount = 0;
                int coinCount = 0;
                int collectCount = 0;
                do
                {
                    wrCount = 0;
                    coinCount = 0;
                    collectCount = 0;
                    TrySpin();
                    if(FeatureSpin)
                        for (int i = 0; i < GRID_WIDTH; i++)
                        {
                            for (int j = 0; j < GRID_HEIGHT; j++)
                            {
                                if (Symbols[i, j] == WR)
                                    wrCount++;
                                else if (Symbols[i, j] == COIN)
                                    coinCount++;
                                else if (Symbols[i, j] == COIN_COLLECT)
                                    collectCount++;
                            }
                        }
                } while (FeatureSpin && (wrCount < 1 || coinCount < 5 || collectCount < 1));
            }

            internal void TrySpin()
            {
                Values = new int[GRID_WIDTH, GRID_HEIGHT];
                Symbols = ArrayUtil.GetConstantArray(DUMMY, GRID_WIDTH, GRID_HEIGHT);

                //Coin Collect
                bool coinCollectLanded = rand.NextDouble() < (Basegame ? CoinCollectProba : CoinCollectProbaFS);
                if (FeatureSpin)
                    coinCollectLanded = true;
                if (coinCollectLanded)
                {
                    int x = rand.Next(GRID_WIDTH);
                    int y = rand.Next(GRID_HEIGHT);
                    Symbols[x, y] = COIN_COLLECT;
                }

                // Coin
                int coinAmount = (Basegame ? CoinAmountWeights : CoinAmountWeightsFS).rollSingleItem(rand);
                if (FeatureSpin)
                    coinAmount = CoinAmountWeightsFeatureSpin.rollSingleItem(rand);
                while (coinAmount > 0)
                {
                    int x = rand.Next(GRID_WIDTH);
                    int y = rand.Next(GRID_HEIGHT);
                    if(Symbols[x, y] == DUMMY)
                    {
                        Symbols[x, y] = COIN;
                        Values[x, y] = (SuperFS ? CoinValueWeightsSFS : CoinValueWeights).rollSingleItem(rand);
                        coinAmount--;
                    }
                }

                //SC or Wild
                var reelsForWrOrSc = new List<int>() { 0, 1, 2, 3, 4};
                       // add SC or Retrigger
                var scAmount = Basegame ? ScAmountWeights.rollSingleItem(rand) : RetriggerAmountWeights.rollSingleItem(rand);
                if (BonusHuntMainSpin)
                    scAmount = ScAmountWeightsBonusHunt.rollSingleItem(rand);
                else if (ForcedSC > 0)
                    scAmount = ForcedSC;
                else if (FeatureSpin)
                    scAmount = 0;
                while (scAmount > 0 && reelsForWrOrSc.Count > 0)
                {
                    var tmpIndex = rand.Next(reelsForWrOrSc.Count);
                    Symbols[reelsForWrOrSc[tmpIndex], rand.Next(GRID_HEIGHT)] = Basegame ? SC : RETRIGGER;
                    reelsForWrOrSc.RemoveAt(tmpIndex);
                    scAmount--;
                }
                    // add wild
                var wrAmount = WrAmountWeights.rollSingleItem(rand);
                if (FeatureSpin)
                    wrAmount = WrAmountWeightsFeatureSpin.rollSingleItem(rand);
                while (wrAmount > 0 && reelsForWrOrSc.Count > 0)
                {
                    var tmpIndex = rand.Next(reelsForWrOrSc.Count);
                    Symbols[reelsForWrOrSc[tmpIndex], rand.Next(GRID_HEIGHT)] = WR;
                    reelsForWrOrSc.RemoveAt(tmpIndex);
                    wrAmount--;
                }

                for (int i = 0; i < GRID_WIDTH; i++)
                {
                    for (int j = 0; j < GRID_HEIGHT; j++)
                    {
                        if (LockedWilds[i, j])
                            Symbols[i, j] = WR;
                        if (Symbols[i, j] == DUMMY)
                            Symbols[i, j] = SymbolWeights.rollSingleItem(rand);

                        if (Symbols[i, j] == SC)
                            Values[i, j] = 0;
                        if (Symbols[i, j] == WR)
                            Values[i, j] = WrMultiplierWeights.rollSingleItem(rand);
                        if (Symbols[i, j] == RETRIGGER)
                        {
                            if (Basegame)
                                Console.WriteLine("Basegame should NOT have RETRIGGER");
                            Values[i, j] = RetriggerValueWeights.rollSingleItem(rand);
                        }
                    }
                }
            }
        }

        internal class SlotSpin
        {
            internal bool TriggerBonus { get { return GetScAmount() >= SC_TO_FS; } }
            internal bool TriggerSuperBonus { get { return GetScAmount() >= SC_TO_SFS; } }
            internal SymbolGrid Grid;
            internal bool Basegame;
            internal bool SuperFS;
            internal List<SlotWin> Wins;
            internal List<Coordinate> LandedWildCoors;
            //statistics
            internal bool BetterConfig;

            internal SlotSpin(BetMode mode)
            {
                Basegame = true;
                Grid = new SymbolGrid(mode);
                Wins = new List<SlotWin>();
                LandedWildCoors = new List<Coordinate>();
            }
            internal SlotSpin(bool basegame, bool superFs = false, bool[,]? lockedWilds = null)
            {
                Basegame = basegame;
                SuperFS = superFs;
                Grid = new SymbolGrid(basegame, superFs, lockedWilds);
                Wins = new List<SlotWin>();
                LandedWildCoors = new List<Coordinate>();
            }

            internal void Roll()
            {
                Grid.Spin();
                Wins = CheckWins(Grid.Symbols, Grid.Values);
                //coin collection
                var wildCoors = new List<Coordinate>();
                var coinCoors = new List<Coordinate>();
                var coinCollectCoors = new List<Coordinate>();
                for (int i = 0; i < GRID_WIDTH; i++)
                {
                    for (int j = 0; j < GRID_HEIGHT; j++)
                    {
                        if (Grid.Symbols[i, j] == WR)
                        {
                            if (Grid.Values[i, j] <= 0)
                                Console.Error.WriteLine("WR missing its multiplier!");
                            wildCoors.Add(new Coordinate(i, j));
                        }
                        if (Grid.Symbols[i, j] == COIN)
                        {
                            if (Grid.Values[i, j] <= 0)
                                Console.Error.WriteLine("COIN missing its multiplier!");
                            coinCoors.Add(new Coordinate(i, j));
                        }
                        if (Grid.Symbols[i, j] == COIN_COLLECT)
                        {
                            if (Grid.Values[i, j] != 0)
                                Console.Error.WriteLine("COIN_COLLECT should NOT have a value!");
                            coinCollectCoors.Add(new Coordinate(i, j));
                        }
                    }
                }
                LandedWildCoors = wildCoors;
                if (coinCollectCoors.Count > 0 && coinCoors.Count > 0)
                {
                    if (coinCollectCoors.Count > 1)
                        Console.Error.WriteLine("more than 1 coin collect landed!");
                    foreach (var wildCoor in wildCoors)
                    {
                        MultiplyCoinValue(Grid, wildCoor.x, wildCoor.y);
                    }
                    int coinWin = coinCoors.Sum(coin => Grid.Values[coin.x, coin.y]);
                    if (coinWin <= 0)
                        Console.Error.WriteLine("coin collect triggers with 0 coin win");
                    Wins.Add(new SlotWin(coinWin * BET_SIZE, COIN_COLLECT, 0, 1, 0, coinCoors));
                }
            }

            internal int GetTotalWin()
            {
                return Wins.Sum(x => x.Win);
            }

            internal int GetScAmount()
            {
                return SlotUtil.FindScatters(Grid.Symbols, SC).Count;
            }

            internal int GetRetriggerFSAmount()
            {
                return SlotUtil.FindScatters(Grid.Symbols, RETRIGGER).Sum(sym => Grid.Values[sym.x, sym.y]);
            }
        }

        internal SlotSpin GenerateBuyMainSpin(BetMode betMode)
        {
            SlotSpin spin;
            do
            {
                spin = new SlotSpin(betMode);
                spin.Roll();
            } while (spin.GetTotalWin() > 0);
            return spin;
        }
        internal SlotSpin GenerateMainSpin(BetMode betMode)
        {
            SlotSpin spin;// = new SlotSpin(this, true);
            int redrawProbaIndex = 0;
            bool betterConfig = false;
            double deadSpinProba = deadSpinProbas[RtpVariantIndex];
            if (betMode == BetMode.BonusHunt)
            {
                deadSpinProba = deadSpinProbasBonusHunt[RtpVariantIndex];
                redrawProbaIndex = 1;
            }    
            else if (betMode == BetMode.FeatureSpin)
            {
                deadSpinProba = 0;
                betterConfig = rand.NextDouble() < betterFeatureSpinProba[RtpVariantIndex];
                redrawProbaIndex = betterConfig ? 2 : 3;
            }
                
            bool deadSpin = rand.NextDouble() < deadSpinProba;
            double redrawProba = 0;
            bool redraw;
            do
            {
                redraw = false;
                spin = new SlotSpin(betMode);
                spin.BetterConfig = betterConfig;
                spin.Roll();
                double win = (double)spin.GetTotalWin() / BET_SIZE;
                if (deadSpin && (win > 0 || spin.TriggerBonus))
                    redraw = true;
                if (!deadSpin)
                {
                    redrawProba = MathUtil.GetProbaByValue(bgRedrawThresholds, bgRedrawProbas[redrawProbaIndex], win);
                    if (rand.NextDouble() < redrawProba)
                        redraw = true;
                }
            } while (redraw);
            return spin;
        }

        internal void PlayGame()
        {
            int win = 0;
            SlotSpin mainSpin;
            if (simulationBetMode == BetMode.Buy || simulationBetMode == BetMode.SuperBuy)
                mainSpin = GenerateBuyMainSpin(simulationBetMode);
            else
                mainSpin = GenerateMainSpin(simulationBetMode);
            win += mainSpin.GetTotalWin();
            if (Debug)
            {
                PrintReels(mainSpin.Grid.Symbols, mainSpin.Grid.Values, mainSpin, 3);
                Console.WriteLine($"{mainSpin.GetTotalWin()}");
                foreach (var slotWin in mainSpin.Wins)
                    Console.WriteLine($"{slotWin.SymbolWinIndex}({slotWin.Size}) * {slotWin.Multiplier}: {slotWin.Win} ({slotWin.Payline})");
                Console.WriteLine("----------------");
            }

            BonusWin? bonus = null;
            if (mainSpin.TriggerBonus)
            {
                bonus = GenerateBonus(mainSpin.TriggerSuperBonus);
            }
            //Console.WriteLine(symbolWeights.rollSingleItem(rand, true));

            //collect statistics
            summary.Add(mainSpin, bonus);

        }

        internal BonusWin GenerateBonus(bool SuperFS)
        {
            if (Debug)
                Console.WriteLine("FS starts");
            int redrawIndex = 1;
            if (simulationBetMode == BetMode.Buy)
            {
                if (SuperFS)
                    Console.Error.WriteLine("Buy trigger Super");
                redrawIndex = rand.NextDouble() < betterFsProba[RtpVariantIndex] ? 0 : 1;
            } 
            else if (simulationBetMode == BetMode.SuperBuy)
            {
                if (!SuperFS)
                    Console.Error.WriteLine("super Buy trigger Normal FS");
                redrawIndex = rand.NextDouble() < betterSuperFsProba[RtpVariantIndex] ? 0 : 1;
            }
                
            BonusWin bonus;
            double redrawProba = 0;
            int leftFS;
            do
            {
                bonus = new BonusWin();
                bonus.BetterConfig = redrawIndex == 0;
                leftFS = SuperFS ? START_SUPER_FS : START_FS;
                while (leftFS > 0)
                {
                    if (Debug)
                        Console.WriteLine($"Left FS: {leftFS}");
                    leftFS--;
                    var fs = new SlotSpin(false, SuperFS, bonus.LockedWilds);
                    fs.Roll();
                    if (Debug)
                    {
                        PrintReels(fs.Grid.Symbols, fs.Grid.Values, fs, 3);
                        Console.WriteLine($"{fs.GetTotalWin()}");
                        foreach (var slotWin in fs.Wins)
                            Console.WriteLine($"{slotWin.SymbolWinIndex}({slotWin.Size}) * {slotWin.Multiplier}: {slotWin.Win} ({slotWin.Payline})");
                        Console.WriteLine("----------------");
                    }
                    bonus.Spins.Add(fs);
                    leftFS += fs.GetRetriggerFSAmount();
                    foreach (var coor in fs.LandedWildCoors)
                        bonus.LockedWilds[coor.x, coor.y] = true;
                }
                double win = (double)bonus.TotalWin / BET_SIZE;
                redrawProba = 0;
                if (SuperFS)
                    redrawProba = MathUtil.GetProbaByValue(superFsRedrawThresholds, superFsRedrawProbas[redrawIndex], win);
                else
                    redrawProba = MathUtil.GetProbaByValue(bonusRedrawThresholds, bonusRedrawProbas[redrawIndex], win);
            } while (rand.NextDouble() < redrawProba);
            return bonus;
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
            internal Average CoinCollectHitRate = new();
            internal Average CoinCollectBasegameRTP = new();
            internal Average HitRateFs = new();
            internal Average CoinCollectHitRateFs = new();
            internal Average CoinCollectPayFs = new();
            internal Average FsCount = new();
            internal BreakDownTable SymbolWins = new(new double[] { WR, H1, H2, H3, H4, L5, L6, L7, L8, COIN, COIN_COLLECT});
            internal Distribution WinDistribution = new Distribution(new double[] { 0, 0.01, 1.0, 5, 15, 30, 100, 1000, 5000 });
            internal Distribution BGWinDistribution = new Distribution(new double[] { 0, 0.01, 1.0, 5, 15, 30, 100, 1000, 5000 });
            internal Distribution FSWinDistribution = new Distribution(new double[] { 0, 0.01, 1.0, 5, 15, 30, 100, 1000, 5000 });
            internal Average AveragePayBetter = new();
            internal Average AverageFSPayBetter = new();

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
                HitRate.Add(win > 0 ? 1 : 0);
                HitFSProba.Add(mainSpin.GetScAmount() >= SC_TO_FS ? 1 : 0);
                WinDistribution.Add((double)win / BET_SIZE);
                if (mainSpin.BetterConfig)
                    AveragePayBetter.Add((double)win / BET_SIZE);

                double coinCollectWin = 0;
                foreach (SlotWin slotWin in mainSpin.Wins)
                {
                    if (Debug)
                    {
                        if (slotWin.Size == 5)
                        {
                            PrintReels(mainSpin.Grid.Symbols, mainSpin.Grid.Values, mainSpin, 3);
                            Console.WriteLine($"{mainSpin.GetTotalWin()}");
                            foreach (var tmp in mainSpin.Wins)
                            {
                                Console.WriteLine($"{tmp.SymbolWinIndex}: {tmp.Win}");
                            }
                            Console.WriteLine();
                        }
                    }
                    SymbolWins.Add(slotWin.SymbolWinIndex, (double)slotWin.Win / BET_SIZE);
                    if (slotWin.SymbolWinIndex == COIN_COLLECT)
                        coinCollectWin += (double)slotWin.Win / BET_SIZE;
                }
                CoinCollectHitRate.Add(coinCollectWin > 0 ? 1 : 0);
                CoinCollectBasegameRTP.Add(coinCollectWin);

                if (bonus != null)
                {
                    double fsTotalWin = (double)bonus.TotalWin / BET_SIZE;
                    if (bonus.BetterConfig)
                        AverageFSPayBetter.Add(fsTotalWin);
                    AverageFSPayouts.Add(fsTotalWin);
                    FSWinDistribution.Add(fsTotalWin);
                    FsCount.Add(bonus.Spins.Count);

                    foreach (var fs in bonus.Spins)
                    {
                        HitRateFs.Add(fs.GetTotalWin() > 0 ? 1 : 0);
                        double coinCollectWinFs = 0;
                        foreach (SlotWin slotWin in fs.Wins)
                        {
                            SymbolWins.Add(slotWin.SymbolWinIndex, (double)slotWin.Win / BET_SIZE);
                            if (slotWin.SymbolWinIndex == COIN_COLLECT)
                                coinCollectWinFs += (double)slotWin.Win / BET_SIZE;
                        }
                        CoinCollectHitRateFs.Add(coinCollectWinFs > 0 ? 1 : 0);
                        if(coinCollectWinFs > 0)
                            CoinCollectPayFs.Add(coinCollectWinFs);
                    }
                }
            }

            internal void WriteOnConsole()
            {
                Console.WriteLine($"Sample Size: {SampleSize}");
                Console.WriteLine($"Variance: {AverageSquared.Value - Math.Pow(ReturnToPlayer.Value, 2)}");
                Console.WriteLine($"Max Win: {MaxWin}x");
                Console.WriteLine($"RTP: {ReturnToPlayer.Percentage}");
                Console.WriteLine($"Base RTP: {BaseRTP.Percentage}");
                Console.WriteLine($"FS RTP: {AverageFSPayouts.Value * HitFSProba.Value * 100}%");
                Console.WriteLine($"Hit Rate: {HitRate.Percentage}");
                Console.WriteLine($"Hit FS Probability: {HitFSProba.Percentage}");
                Console.WriteLine(string.Format("Average FS Payouts: {0,2:F4}x", AverageFSPayouts.Value));
                Console.WriteLine($"Hit Coin Collect Probability: {CoinCollectHitRate.Percentage}");
                Console.WriteLine($"RTP(Coin Collect | basegame): {CoinCollectBasegameRTP.Percentage}");
                Console.WriteLine($"P(Coin Collect | FS): {CoinCollectHitRateFs.Percentage}");
                Console.WriteLine($"E(Coin Collect | FS): {CoinCollectPayFs.Value}");
                Console.WriteLine($"P(Hit | each FS): {HitRateFs.Percentage}");
                Console.WriteLine($"Average FS: {FsCount.Value}");
                Console.WriteLine($"Base RTP | Better: {AveragePayBetter.Percentage}");
                Console.WriteLine(string.Format("FS Payouts | Better: {0,2:F4}x", AverageFSPayBetter.Value));
                Console.WriteLine("Symbol Win Distribution:");
                SymbolWins.PrintAverageValues(SampleSize);
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

            }
        }

        internal class BonusWin
        {
            internal List<SlotSpin> Spins;
            internal bool[,] LockedWilds;
            internal bool BetterConfig;
            internal int TotalWin
            {
                get { return Spins.Sum(x => x.GetTotalWin()); }
            }
            internal BonusWin()
            {
                Spins = new List<SlotSpin>();
                LockedWilds = new bool[GRID_WIDTH, GRID_HEIGHT];
            }
        }


        internal static List<SlotWin> CheckWins(int[,] symbols, int[,] multis)
        {
            var wins = new List<SlotWin>();
            for(int ind=0; ind < PayLines.Length; ind++)
            {
                var line = PayLines[ind];
                List<Coordinate> winCoors = new List<Coordinate>();
                int symSize = 1;
                int symType = symbols[0, line[0]];
                int multi = symbols[0, line[0]] == WR ? multis[0, line[0]] : 1;
                for (int i = 1; i < symbols.GetLength(0); i++)
                {
                    if (symType != symbols[i, line[i]] && symbols[i, line[i]] != WR)
                    {
                        if (symType == WR)
                            symType = symbols[i, line[i]];
                        else
                            break;
                    }
                    if (symbols[i, line[i]] == WR)
                        multi *= multis[i, line[i]];
                    symSize++;
                    winCoors.Add(new Coordinate(i, line[i]));
                }
                if (Payouts[symType][symSize] > 0)
                {
                    wins.Add(new SlotWin(Payouts[symType][symSize] * multi, symType, symSize, multi, ind, winCoors));
                }
            }
            return wins;
        }

        internal static void MultiplyCoinValue(SymbolGrid grid, int wildX, int wildY)
        {
            if (grid.Symbols[wildX, wildY] != WR)
                Console.Error.WriteLine("wrongly multiply coin value without wild");
            int multi = grid.Values[wildX, wildY];
            int startX = wildX - 1 >= 0 ? wildX - 1 : 0;
            int startY = wildY - 1 >= 0 ? wildY - 1 : 0;
            int endX = wildX + 1 < GRID_WIDTH ? wildX + 1 : GRID_WIDTH - 1;
            int endY = wildY + 1 < GRID_HEIGHT ? wildY + 1 : GRID_HEIGHT - 1;
            for(int i = startX; i <= endX; i++)
            {
                for(int j= startY; j <= endY; j++)
                {
                    if (grid.Symbols[i, j] == COIN)
                        grid.Values[i, j] *= multi;
                }
            }
        }

        internal static void PrintReels(int[,] symbols, int[,] values, SlotSpin spin, int digit = 2)
        {
            string printFormat = "{0," + digit + ":D}";
            for (int j = 0; j < symbols.GetLength(1); j++)
            {
                for (int i = 0; i < symbols.GetLength(0); i++)
                {
                    Console.Write(String.Format(printFormat, symbols[i, j]));
                }
                Console.Write("   ");
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    Console.Write(String.Format(printFormat, values[i, j]));
                }
                Console.Write("   ");
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    Console.Write(String.Format(printFormat, spin.Grid.LockedWilds[i, j] ? 1 : 0));
                }
               
                Console.WriteLine();
            }
        }
    }
}
