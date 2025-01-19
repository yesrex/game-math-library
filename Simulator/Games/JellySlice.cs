using Simulator.SlotUtility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Simulator.Games
{
    internal class JellySlice
    {
        internal Random rand = new Random();
        internal enum BetMode
        {
            Normal,
            BonusHunt,
            FeatureSpin,
            Buy,
        }
        internal static readonly BetMode simulationBetMode = BetMode.FeatureSpin;
        internal static readonly int RtpVariantIndex = 1; //0 :98%, 1: 90%

        internal static readonly bool debug = false;

        internal const int WR = 0;
        internal const int H1 = 1;
        internal const int M2 = 2;
        internal const int M3 = 3;
        internal const int M4 = 4;
        internal const int M5 = 5;
        internal const int L6 = 6;
        internal const int L7 = 7;
        internal const int L8 = 8;
        internal const int L9 = 9;
        internal const int L10 = 10;

        internal const int MR1 = 11;
        internal const int MR2 = 12;
        internal const int MR3 = 13;
        internal const int MR4 = 14;
        internal const int LR1 = 15;
        internal const int LR2 = 16;  
        internal const int LR3 = 17;  
        internal const int LR4 = 18;
        internal const int LR5 = 19;

        internal const int DUMMY = 20;
        internal const int SC = 21;

        internal const int GRID_WIDTH = 5;
        internal const int GRID_HEIGHT = 4;
        internal const int SC_TO_TRIGGER_FS = 5;

        internal const int BET_SIZE = 10;
        internal readonly int[][] payouts =
        {
            new int[] {0, 0, 0,  0,  0, 200}, // WR
            new int[] {0, 0, 0, 10, 30, 100}, // H1
            new int[] {0, 0, 0,  7, 15,  30},
            new int[] {0, 0, 0,  7, 15,  30},
            new int[] {0, 0, 0,  5, 10,  20},
            new int[] {0, 0, 0,  5, 10,  20},
            new int[] {0, 0, 0,  2,  5,  10}, // L6
            new int[] {0, 0, 0,  2,  5,  10},
            new int[] {0, 0, 0,  2,  5,  10},
            new int[] {0, 0, 0,  2,  5,  10},
            new int[] {0, 0, 0,  2,  5,  10},
        };

        internal static readonly int[][][] Reels =
        {
            new int[][]{
            new int[] { // no LR5
                        WR, LR2,LR2,LR3,LR4,LR1,LR3,MR1,MR4,H1,MR1,MR2,MR3,MR3,MR3,MR4,LR1,LR3,LR4,LR2,LR1,LR3,LR3,LR3,MR1,MR2,LR4,LR3,
                        SC, LR3,LR4,LR2,LR2,LR2,LR3,MR1,MR2,H1,MR4,MR3,MR1,LR1,LR3,LR4,LR2,LR1,MR2,MR2,LR2,LR3,MR1,MR3,LR4,LR3,LR1,LR2,
                        LR3, LR3,LR4,LR1,LR2,LR2,LR3,MR1,MR2,H1,MR4,LR1,MR3,MR3,MR2,LR1,LR3,LR4,LR2,LR1,LR1,LR1,LR1,MR4,LR2,LR3,MR1,MR1,MR1,MR1,LR4,LR3,
                        LR2,LR2,LR2,LR2,MR3,MR1,LR1,LR3,MR2,H1,H1,H1,MR4,LR1,MR3,MR3,MR2,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,LR1,MR2,LR4,LR3,
                      },
            new int[] {
                        LR5,LR1,LR1,LR3,LR4,LR5,MR1,MR1,MR4,H1,MR2,MR2,MR3,MR3,MR3,MR3,MR1,LR5,LR3,LR4,LR1,LR2,LR3,LR3,LR5,MR1,MR3,LR4,LR3,
                        SC, LR3,LR4,LR1,LR1,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR4,LR5,LR3,MR1,MR3,LR4,LR3,LR2,LR1,
                        WR, LR3,LR4,LR2,LR1,LR1,LR5,MR1,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR3,LR4,LR5,LR2,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                        LR1,LR1,LR1,LR1,LR4,MR3,MR1,LR2,LR5,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR3,LR4,LR1,LR5,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                      },
            new int[] { // no MR2
                        LR1,LR1,LR3,LR4,LR2,LR5,MR1,MR3,MR3,H1,MR1,MR4,MR4,MR4,MR3,MR1,LR2,LR3,LR4,LR5,LR2,LR3,LR3,LR5,MR1,MR4,LR4,LR3,
                        LR5,LR3,LR4,LR1,LR2,LR2,LR5,MR1,MR1,H1,MR3,MR4,MR1,LR2,LR3,LR4,LR1,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,LR2,LR1,
                        WR, LR3,LR2,LR4,LR4,LR4,LR5,MR1,MR1,H1,LR5,LR2,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR1,LR3,MR1,MR4,LR4,LR3,
                        LR1,LR1,LR4,LR4,MR4,MR1,LR2,LR5,MR1,H1,LR2,LR5,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR1,LR3,MR1,MR4,LR4,LR3,
                      },
            new int[] { // few LR4
                        LR4,LR5,LR3,LR2,LR1,LR5,MR1,MR2,MR3,H1,MR1,MR2,MR4,MR4,MR4,MR4,MR1,MR2,H1,LR2,LR1,LR5,LR3,LR3,LR2,MR2,H1,LR5,LR3,
                        SC, LR3,LR5,LR1,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR3,MR4,MR1,LR2,LR3,LR5,LR1,LR2,MR2,MR3,H1,LR3,MR1,MR4,LR5,LR3,LR2,LR1,
                        WR, LR3,LR5,LR2,LR2,LR2,LR5,MR1,MR2,H1,MR3,LR2,MR4,MR4,MR1,MR2,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                        LR1,LR5,LR5,LR5,MR4,MR1,LR2,LR3,MR2,H1,H1,H1,MR3,LR2,MR4,MR4,MR1,MR2,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                      },
            new int[] { LR2, LR1,LR1,LR3,LR4,LR5,LR5,MR2,MR1,MR4,H1,MR1,MR2,MR3,MR3,MR3,MR3,MR1,MR2,LR3,LR4,LR4,LR2,LR3,LR3,LR5,MR2,MR2,LR4,LR3,
                        LR5,LR5,LR4,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR4,LR2,LR1,
                        SC, LR3,LR4,LR1,LR5,LR5,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR5,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR3,LR2,LR5,
                        WR, LR3,LR2,LR4,LR4,LR1,LR5,MR1,MR2,H1,H1,H1,H1,MR4,LR1,MR3,MR2,MR1,LR5,LR3,LR4,LR1,LR2,MR2,H1,LR5,LR3,MR1,MR3,LR2,LR5,
                      },
            },
            new int[][]{
            new int[] { // no LR5, MR1
                        SC, LR3,LR4,LR2,LR1,LR3,LR4,MR4,MR2,H1,MR3,MR4,MR2,LR1,LR3,LR4,LR2,LR1,MR2,LR3,LR2,
                        SC, LR1,LR1,LR3,LR2,LR4,LR4,MR3,MR2,H1,MR4,MR3,MR2,LR1,LR3,LR4,LR2,LR1,MR2,MR2,LR2,
                        SC, LR2,LR4,LR3,MR3,MR1,LR1,LR3,MR2,H1,MR4,LR1,MR3,MR2,MR2,LR1,LR3,LR4,LR2,LR1,MR2,    
                      },
            new int[] { // no LR3
                        SC, LR1,LR2,LR4,LR5,MR1,MR2,MR4,MR1,H1,MR2,MR3,MR4,MR1,MR1,LR4,LR4,LR5,LR1,LR2,LR4,
                        SC, LR4,LR2,LR1,LR1,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR5,LR4,LR1,LR2,MR2,MR4,LR5,
                        SC, LR1,LR2,LR4,MR3,MR1,LR2,LR5,MR2,H1,MR4,LR4,MR3,MR3,MR1,LR2,LR4,LR4,LR1,LR5,MR2,
                        SC, LR1,LR2,LR4,MR3,MR1,LR2,LR5,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR4,LR1,LR5,LR5,MR2,
                        WR, LR4,LR5,LR2,LR1,LR1,LR5,MR1,MR2,H1,MR4,LR2,MR3,MR1,MR1,LR2,LR4,LR5,LR5,LR2,MR2,
                      },
            new int[] { // no MR2
                        SC, LR1,LR3,LR4,LR2,LR5,MR1,MR4,MR3,H1,MR1,MR1,MR1,MR4,MR3,MR1,LR2,LR3,LR4,LR5,LR2,
                        SC, LR3,LR3,LR4,LR1,LR5,LR5,MR1,MR1,H1,MR3,MR4,MR1,LR2,LR3,LR4,LR1,LR2,MR1,MR4,LR5,
                        SC, LR3,LR1,LR4,MR4,MR1,LR2,LR5,MR1,H1,LR2,LR5,MR4,MR1,MR1,LR2,LR3,LR4,LR5,LR2,MR1,
                        SC, LR1,LR3,LR4,MR4,MR1,LR2,LR1,MR1,H1,LR2,LR5,MR4,MR1,MR1,LR2,LR3,LR4,LR5,LR2,MR1,
                        SC, LR3,LR4,LR4,LR2,LR1,LR5,MR1,MR1,H1,LR5,LR2,LR3,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,
                      },
            new int[] { // no LR4
                        SC, LR5,LR3,LR2,LR1,LR5,MR2,MR1,MR3,H1, MR1,MR2,MR2,MR4,MR1,MR1,MR2,LR2,LR3,LR1,LR5,
                        SC, LR3,LR5,LR5,LR1,LR3,LR3,MR1,MR2,H1, MR3,MR4,MR1,LR2,LR3,LR5,LR1,LR2,MR2,MR2,MR3,
                        SC, LR3,LR5,LR2,LR1,LR3,LR5,MR1,MR2,H1, MR3,LR2,MR4,MR1,MR1,MR2,LR3,LR5,LR1,LR2,MR2,
                        SC, LR5,LR5,LR5,MR4,MR1,LR2,LR3,MR2,H1,MR3,LR2,MR4,MR1,MR2,MR2,LR3,LR5,LR1,LR2,MR2,
                      },
            new int[] { SC, LR1,LR3,LR4,LR5,LR5,MR3,MR1,MR4,SC, MR2,MR2,MR2,MR3,MR1,MR2,LR3,LR3,LR4,LR4,LR2,
                        SC, LR5,LR4,LR3,LR3,LR3,LR5,MR1,MR2,SC, MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR2,LR5,
                        SC, LR3,LR4,LR1,LR5,LR5,LR5,MR1,MR2,H1, MR4,MR3,MR1,LR5,LR3,LR4,LR1,LR2,MR2,MR2,LR5,
                        SC, LR3,LR2,LR4,LR4,LR1,LR5,MR1,MR2,H1,MR4,LR1,MR3,MR2,MR1,LR5,LR3,LR4,LR1,LR2,LR4,
                      },
            },
            new int[][]{
            new int[] { // LR1,LR2,LR3,MR1,MR4
                        SC,LR3,LR1,LR2,LR3,LR3,SC,MR4,MR1,MR1,LR2,LR2,LR1,LR3,MR4,MR4,SC,LR2,LR1,LR3,LR1,LR2,LR1,
                      },
            new int[] { // LR1,LR3,LR4,MR1,MR2
                        SC,LR1,LR4,LR3,SC,LR1,MR2,MR1,LR4,LR3,MR2,MR2,SC,LR4,LR1,LR1,SC,LR4,LR3,LR3,
                      },
            new int[] { // LR2,LR4,LR5,MR3,MR4
                        SC,LR2,LR4,LR5,SC,LR2,MR3,MR4,LR5,LR4,MR3,MR3,SC,LR4,LR2,LR5,SC,LR4,MR3,MR3,
                      },
            new int[] {
                        SC,LR2,LR4,LR1,SC,LR2,H1,MR4,LR4,LR3,MR2,MR3,SC,LR4,LR1,LR1,SC,LR3,MR1,MR3,
                      },
            new int[] {
                        SC,LR1,LR2,LR5,SC,LR4,H1,MR4,LR1,LR5,MR2,MR1,SC,LR3,LR2,LR5,SC,LR1,MR3,MR2,
                      },
            },
            new int[][]{
            new int[] { // LR1,LR2,LR3,MR1,MR4
                        SC,LR3,LR1,LR2,SC,LR3,LR3,MR1,SC,MR4,MR4,MR1,SC,LR2,LR1,LR3,SC,LR2,LR1,LR1,
                      },
            new int[] { // LR1,LR3,LR4,MR1,MR2
                        SC,LR1,LR4,LR3,SC,LR1,MR2,MR1,SC,LR3,MR2,MR2,SC,LR4,LR1,LR1,SC,LR4,LR3,LR3,
                      },
            new int[] { // LR2,LR4,LR5,MR3,MR4
                        SC,LR2,LR4,LR5,SC,LR2,MR3,MR4,SC,LR4,MR3,MR3,SC,LR4,LR2,LR5,SC,LR4,MR3,MR3,
                      },
            new int[] { 
                        SC,LR2,LR4,LR1,SC,LR2,H1,MR4,SC,LR3,MR2,MR3,SC,LR4,LR1,LR1,SC,LR3,MR1,MR3,
                      },
            new int[] { 
                        SC,LR1,LR2,LR5,SC,LR4,H1,MR4,SC,LR5,MR2,MR1,SC,LR3,LR2,LR5,SC,LR1,MR3,MR2,
                      },
            },
            new int[][]{
            new int[] { // no LR5
                        WR, LR2,LR2,LR3,LR4,LR1,LR3,MR1,MR4,H1,MR1,MR2,MR3,MR3,MR3,MR4,LR1,LR3,LR4,LR2,LR1,LR3,LR3,LR3,MR1,MR2,LR4,LR4,
                        LR3,LR3,LR3,LR3,LR2,LR2,LR2,MR1,MR2,H1,MR4,MR3,MR1,LR1,LR3,LR4,LR2,LR1,MR2,MR2,LR2,LR3,MR1,MR3,LR4,LR3,LR1,LR2,
                        LR3,LR3,LR4,LR1,LR2,LR2,LR3,MR1,MR2,H1,MR4,LR1,MR3,MR3,MR2,LR1,LR3,LR4,LR2,LR1,LR1,LR1,LR1,MR4,LR2,LR3,MR1,MR1,MR1,MR1,LR4,LR3,
                        LR2,LR2,LR2,LR2,MR3,MR1,LR1,LR3,MR2,H1,H1,H1,MR4,LR1,MR3,MR3,MR2,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,LR1,MR2,LR4,LR3,
                      },
            new int[] {
                        LR5,LR1,LR1,LR3,LR4,LR5,MR1,MR1,MR4,H1,MR2,MR2,MR3,MR3,MR3,MR3,MR1,LR5,LR3,LR4,LR1,LR2,LR3,LR3,LR5,MR1,MR3,LR4,LR4,
                        LR3,LR3,LR3,LR3,LR1,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR4,LR5,LR3,MR1,MR3,LR4,LR3,LR2,LR1,
                        WR, LR3,LR4,LR2,LR1,LR1,LR5,MR1,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR3,LR4,LR5,LR2,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                        LR1,LR1,LR1,LR1,LR4,MR3,MR1,LR2,LR5,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR3,LR4,LR1,LR5,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                      },
            new int[] { // no MR2
                        LR1,LR1,LR3,LR4,LR2,LR5,MR1,MR3,MR3,H1,MR1,MR4,MR4,MR4,MR3,MR1,LR2,LR2,LR4,LR5,LR3,LR3,LR3,LR3,MR1,MR4,LR4,LR5,
                        LR5,LR3,LR4,LR1,LR2,LR2,LR5,MR1,MR1,H1,MR3,MR4,MR1,LR2,LR3,LR4,LR1,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,LR2,LR1,
                        WR, LR3,LR2,LR4,LR4,LR4,LR5,MR1,MR1,H1,LR5,LR2,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR1,LR3,MR1,MR4,LR4,LR3,
                        LR1,LR1,LR4,LR4,MR4,MR1,LR2,LR5,MR1,H1,LR2,LR5,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR1,LR3,MR1,MR4,LR4,LR3,
                      },
            new int[] { // few LR4
                        LR4,LR5,LR2,LR2,LR1,LR5,MR1,MR2,MR3,H1,MR1,MR2,MR4,MR4,MR4,MR4,MR1,MR2,H1,LR2,LR1,LR3,LR3,LR3,LR3,MR2,H1,LR5,LR2,
                        LR3,LR5,LR1,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR3,MR4,MR1,LR2,LR3,LR5,LR1,LR2,MR2,MR3,H1,LR3,MR1,MR4,LR5,LR3,LR2,LR1,
                        WR, LR3,LR5,LR2,LR2,LR2,LR5,MR1,MR2,H1,MR3,LR2,MR4,MR4,MR1,MR2,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                        LR1,LR5,LR5,LR5,MR4,MR1,LR2,LR3,MR2,H1,H1,H1,MR3,LR2,MR4,MR4,MR1,MR2,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                      },
            new int[] { LR2, LR1,LR1,LR4,LR4,LR5,LR5,MR2,MR1,MR4,H1,MR1,MR2,MR3,MR3,MR3,MR3,MR1,MR2,LR2,LR4,LR3,LR3,LR3,LR3,LR5,MR2,MR2,LR4,LR2,
                        LR5,LR5,LR4,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR4,LR2,LR1,
                        LR3,LR4,LR1,LR5,LR5,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR5,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR3,LR2,LR5,
                        WR, LR3,LR2,LR4,LR4,LR1,LR5,MR1,MR2,H1,H1,H1,H1,MR4,LR1,MR3,MR2,MR1,LR5,LR3,LR4,LR1,LR2,MR2,H1,LR5,LR3,MR1,MR3,LR2,LR5,
                      },
            },
        };

        internal static readonly int[][][] FS_Reels =
        {
            new int[][]{
            new int[] { // no LR5
                        WR, LR2,LR2,LR3,LR4,LR1,LR3,MR4,MR4,H1,MR1,MR1,MR3,MR3,MR3,MR3,MR4,LR1,LR3,LR4,LR2,LR1,LR3,LR3,LR3,MR1,MR2,LR4,LR3,
                        LR1,LR3,LR4,LR2,LR2,LR2,LR3,MR1,MR2,H1,MR4,MR3,MR1,LR1,LR3,LR4,LR2,LR1,MR2,MR2,LR2,LR3,MR1,MR3,LR4,LR3,LR1,LR2,
                        LR3,LR3,LR4,LR1,LR2,LR2,LR3,MR1,MR1,H1,MR4,LR1,MR3,MR3,MR2,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,MR2,MR3,LR4,LR3,
                        LR2,LR2,LR2,LR2,MR3,MR1,LR1,LR3,H1 ,H1,H1, MR4,LR1,MR1,MR1,MR1,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,MR1,MR2,LR4,LR3,
                      },
            new int[] {
                        LR5,LR5,LR1,LR3,LR4,LR5,MR1,MR1,MR4,H1,MR2,MR2,MR3,MR3,MR3,MR3,MR1,LR5,LR3,LR4,LR1,LR2,LR3,LR3,LR5,MR2,MR2,LR4,LR3,
                        WR, LR5,LR3,LR4,LR1,LR1,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR5,LR5,LR4,LR1,LR2,MR2,MR4,LR5,LR3,MR1,MR3,LR4,LR5,LR5,LR1,
                        WR, LR3,LR4,LR2,LR1,LR1,LR5,MR1,MR2,H1,MR4,LR2,MR3,MR2,MR1,LR2,LR3,LR4,LR5,LR2,LR2,LR2,LR2,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                        WR, LR5,LR1,LR1,LR4,MR3,MR1,LR2,LR5,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR3,LR4,LR1,LR5,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                      },
            new int[] { // no MR2
                        LR1,LR1,LR3,LR4,LR5,LR5,MR1,MR3,MR3,H1,MR1,MR4,MR4,MR4,MR3,MR1,LR2,LR3,LR5,LR5,LR2,LR3,LR3,LR5,MR1,MR4,LR4,LR3,
                        LR5,LR3,LR4,LR1,LR2,LR2,LR5,MR1,MR1,H1,MR3,MR4,MR1,LR2,LR3,LR4,LR1,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,LR2,LR1,
                        WR, LR3,LR2,LR4,LR4,LR4,LR5,MR1,MR1,H1,LR5,LR2,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR1,LR3,MR1,MR4,LR4,LR3,
                        WR, LR1,LR1,LR4,LR4,MR4,MR1,LR2,LR5,MR1,H1,LR2,LR5,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,
                      },
            new int[] { // few LR4
                        LR4,LR5,LR3,LR2,LR1,LR5,MR1,MR2,MR3,H1,MR1,MR2,MR4,MR4,MR4,MR4,MR1,MR2,H1,LR2,LR1,LR1,LR3,LR3,LR2,MR2,H1,LR5,LR3,
                        SC, LR3,LR5,LR1,LR1,LR1,LR5,MR1,H1,H1,MR3,MR4,MR1,LR2,LR3,LR5,LR1,LR2,MR2,MR3,H1,LR3,MR1,MR4,LR5,LR3,LR2,LR1,
                        WR, LR3,LR5,LR2,LR2,LR2,LR5,MR1,H1,H1,H1,MR3,LR2,MR4,MR4,MR1,MR3,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                        WR, LR1,LR5,LR5,LR5,MR4,MR1,LR2,LR3,H1,H1,MR3,LR2,MR4,MR4,MR1,MR3,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                      },
            new int[] { LR2, LR1,LR1,LR3,LR4,LR5,LR5,MR2,MR1,MR1,H1,MR4,MR2,MR3,MR1,MR1,MR1,MR2,LR3,LR4,LR4,LR2,LR3,LR3,LR5,MR2,MR2,LR4,LR3,
                        LR2, LR5,LR4,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR4,LR2,LR1,
                        SC, LR3,LR4,LR1,LR5,LR5,LR5,MR1,H1,H1,MR4,MR3,MR1,LR5,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR3,LR2,LR5,
                        WR, LR3,LR2,LR4,LR4,LR1,LR5,MR1,H1,H1,H1,MR4,LR1,MR3,MR2,MR1,LR5,LR3,LR4,LR1,LR2,MR2,H1,LR5,LR3,MR1,MR3,LR2,LR5,
                      },
            },
            new int[][]{
            new int[] { // no LR5
                        WR, LR2,LR2,LR3,LR4,LR1,LR3,MR1,MR4,H1,MR1,MR2,MR3,MR3,MR3,MR4,LR1,LR3,LR4,LR2,LR1,LR3,LR3,LR3,MR1,MR2,LR4,LR3,
                        SC,LR3,LR4,LR2,LR2,LR2,LR3,MR1,MR2,H1,MR4,MR3,MR1,LR1,LR3,LR4,LR2,LR1,MR2,MR2,LR2,LR3,MR1,MR3,LR4,LR3,LR1,LR2,
                        LR3,LR3,LR4,LR1,LR2,LR2,LR3,MR1,MR2,H1,MR4,LR1,MR3,MR3,MR2,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,MR2,MR3,LR4,LR3,
                        LR2,LR2,LR2,LR2,MR3,MR1,LR1,LR3,MR2,H1,MR4,LR1,MR2,MR2,MR2,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,MR1,MR2,LR4,LR3,
                      },
            new int[] {
                        LR5,LR1,LR1,LR3,LR4,LR5,MR1,MR1,MR4,H1,MR2,MR2,MR3,MR3,MR3,MR1,LR5,LR3,LR4,LR1,LR2,LR3,LR3,LR5,MR2,MR2,LR4,LR3,
                        SC,LR3,LR4,LR1,LR1,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR5,LR5,LR4,LR1,LR2,MR2,MR4,LR5,LR3,MR1,MR3,LR4,LR5,LR5,LR1,
                        WR, LR3,LR4,LR2,LR1,LR1,LR5,MR1,MR2,H1,MR4,LR2,MR3,MR2,MR1,LR2,LR3,LR4,LR5,LR2,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                        LR5,LR1,LR1,LR4,MR3,MR1,LR2,LR5,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR3,LR4,LR1,LR5,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                      },
            new int[] { // no MR2
                        LR1,LR1,LR3,LR4,LR5,LR5,MR1,MR3,MR3,H1,MR1,MR4,MR4,MR4,MR3,MR1,LR2,LR3,LR5,LR5,LR2,LR3,LR3,LR5,MR1,MR4,LR4,LR3,
                        SC,LR3,LR4,LR1,LR2,LR2,LR5,MR1,MR1,H1,MR3,MR4,MR1,LR2,LR3,LR4,LR1,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,LR2,LR1,
                        WR, LR3,LR2,LR4,LR4,LR4,LR5,MR1,MR1,H1,LR5,LR2,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR1,LR3,MR1,MR4,LR4,LR3,
                        LR1,LR1,LR4,LR4,MR4,MR1,LR2,LR5,MR1,H1,LR2,LR5,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,
                      },
            new int[] { // few LR4
                        LR4,LR5,LR3,LR2,LR1,LR5,MR1,MR2,MR3,H1,MR1,MR2,MR4,MR4,MR4,MR1,MR2,H1,LR2,LR1,LR5,LR3,LR3,LR2,MR2,H1,LR5,LR3,
                        SC, LR3,LR5,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR3,MR4,MR1,LR2,LR3,LR5,LR1,LR2,MR2,MR3,H1,LR3,MR1,MR4,LR5,LR3,LR2,LR1,
                        WR, LR3,LR5,LR2,LR2,LR2,LR5,MR1,MR2,H1,MR3,LR2,MR4,MR4,MR1,MR2,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                        LR1,LR5,LR5,LR5,MR4,MR1,LR2,LR3,MR2,H1,MR3,LR2,MR4,MR4,MR1,MR2,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                      },
            new int[] { LR2, LR1,LR1,LR3,LR4,LR5,LR5,MR2,MR1,MR4,H1,MR1,MR2,MR3,MR3,MR3,MR1,MR2,LR3,LR4,LR4,LR2,LR3,LR3,LR5,MR2,MR2,LR4,LR3,
                        LR2, LR5,LR4,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR4,LR2,LR1,
                        SC, LR3,LR4,LR1,LR5,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR5,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR3,LR2,LR5,
                        WR, LR3,LR2,LR4,LR4,LR1,LR5,MR1,MR2,H1,H1,MR4,LR1,MR3,MR2,MR1,LR5,LR3,LR4,LR1,LR2,MR2,H1,LR5,LR3,MR1,MR3,LR2,LR5,
                      },
            },
                        new int[][]{
            new int[] { // no LR5
                        WR, LR2,LR2,LR3,LR4,LR1,LR3,MR4,MR4,H1,MR1,MR1,MR3,MR3,MR3,MR3,MR4,LR1,LR4,LR2,LR1,LR3,LR3,LR3,LR3,MR1,MR2,LR4,LR3,
                        LR1,LR3,LR4,LR2,LR2,LR2,LR3,MR1,MR2,H1,MR4,MR3,MR1,LR1,LR3,LR4,LR2,LR1,MR2,MR2,LR2,LR3,MR1,MR3,LR4,LR3,LR1,LR2,
                        LR3,LR3,LR4,LR1,LR2,LR2,LR3,MR1,MR1,H1,MR4,LR1,MR3,MR3,MR2,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,MR2,MR3,LR4,LR3,
                        LR2,LR2,LR2,LR2,MR3,MR1,LR1,LR3,H1 ,H1,H1, MR4,LR1,MR1,MR1,MR1,LR1,LR3,LR4,LR2,LR1,MR2,MR4,LR2,LR3,MR1,MR2,LR4,LR3,
                      },
            new int[] {
                        LR5,LR5,LR1,LR3,LR4,LR5,MR1,MR1,MR4,H1,MR2,MR2,MR3,MR3,MR3,MR3,MR1,LR5,LR3,LR4,LR1,LR2,LR3,LR3,LR5,MR2,MR2,LR4,LR3,
                        WR, LR5,LR3,LR4,LR1,LR1,LR5,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR5,LR5,LR4,LR1,LR2,MR2,MR4,LR5,LR3,MR1,MR3,LR4,LR5,LR5,LR1,
                        WR, LR3,LR3,LR3,LR3,LR4,LR2,LR1,LR1,LR5,MR1,MR2,H1,MR4,LR2,MR3,MR2,MR1,LR2,LR4,LR5,LR2,LR2,LR2,LR2,MR2,MR4,LR1,MR1,MR3,LR4,
                        WR, LR5,LR1,LR1,LR4,MR3,MR1,LR2,LR5,MR2,H1,MR4,LR2,MR3,MR3,MR1,LR2,LR3,LR4,LR1,LR5,MR2,MR4,LR1,LR3,MR1,MR3,LR4,LR3,
                      },
            new int[] { // no MR2
                        LR1,LR1,LR3,LR4,LR5,LR5,MR1,MR3,MR3,H1,MR1,MR4,MR4,MR4,MR3,MR1,LR2,LR3,LR5,LR5,LR2,LR3,LR3,LR5,MR1,MR4,LR4,LR3,
                        LR5,LR3,LR4,LR1,LR2,LR2,LR5,MR1,MR1,H1,MR3,MR4,MR1,LR2,LR3,LR4,LR1,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,LR2,LR1,
                        WR, LR3,LR3,LR3,LR3,LR2,LR4,LR4,LR4,LR5,MR1,MR1,H1,LR5,LR2,MR4,MR4,MR1,LR2,LR4,LR5,LR2,MR1,MR4,LR1,MR1,MR4,LR4,
                        WR, LR1,LR1,LR4,LR4,MR4,MR1,LR2,LR5,MR1,H1,LR2,LR5,MR4,MR4,MR1,LR2,LR3,LR4,LR5,LR2,MR1,MR4,LR5,LR3,MR1,MR4,LR4,LR3,
                      },
            new int[] { // few LR4
                        LR4,LR5,LR2,LR1,LR5,MR1,MR2,MR3,H1,MR1,MR2,MR4,MR4,MR4,MR4,MR1,MR2,H1,LR2,LR1,LR1,LR3,LR3,LR3,LR3,LR2,MR2,H1,LR5,
                        SC, LR3,LR5,LR1,LR1,LR1,LR5,MR1,H1,H1,MR3,MR4,MR1,LR2,LR3,LR5,LR1,LR2,MR2,MR3,H1,LR3,MR1,MR4,LR5,LR3,LR2,LR1,
                        WR, LR3,LR5,LR2,LR2,LR2,LR5,MR1,H1,H1,H1,MR3,LR2,MR4,MR4,MR1,MR3,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                        WR, LR1,LR5,LR5,LR5,MR4,MR1,LR2,LR3,H1,H1,MR3,LR2,MR4,MR4,MR1,MR3,LR3,LR5,LR1,LR2,MR2,MR3,LR1,LR3,MR1,MR4,LR2,LR3,
                      },
            new int[] { LR2, LR1,LR1,LR3,LR4,LR5,LR5,MR2,MR1,MR1,H1,MR4,MR2,MR3,MR1,MR1,MR1,MR2,LR4,LR4,LR2,LR3,LR3,LR3,LR3,LR5,MR2,MR2,LR4,
                        LR2, LR5,LR4,LR1,LR1,LR1,LR5,MR1,MR2,H1,MR4,MR3,MR1,LR2,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR4,LR2,LR1,
                        SC, LR3,LR4,LR1,LR5,LR5,LR5,MR1,H1,H1,MR4,MR3,MR1,LR5,LR3,LR4,LR1,LR2,MR2,MR2,LR5,LR5,MR1,MR3,LR4,LR3,LR2,LR5,
                        WR, LR3,LR2,LR4,LR4,LR1,LR5,MR1,H1,H1,H1,MR4,LR1,MR3,MR2,MR1,LR5,LR3,LR4,LR1,LR2,MR2,H1,LR5,LR3,MR1,MR3,LR2,LR5,
                      },
            },
        };

        internal static int winCap = 10000;

        internal static RandomWeightArray basegameReelWeights = new RandomWeightArray(new int[,]
        {
            { 0, 925},
            { 1,  62},
            { 2,  13},
            { 4,  10},
        });
        internal static RandomWeightArray basegameReelWeightsBonusHunt = new RandomWeightArray(new int[,]
        {
            { 0, 760},
            { 1, 200},
            { 2,  20},
            { 3,  10},
            { 4,  10},
        });
        internal static RandomWeightArray basegameReelWeightsFeatureSpin = new RandomWeightArray(new int[,]
        {
            { 4,  10},
        });
        internal const int FeatureBuyReelsIndex = 3;
        internal static RandomWeightArray fsReelWeights = new RandomWeightArray(new int[,]
        {
            //{ 0, 300},
            { 0, 895},
            { 1, 100},
            { 2,   5},
        });


        internal const int FeatureBuySlicerAmount = 5;
        internal static RandomWeightArray[] slicerAmountWeight = new RandomWeightArray[]
        {
            new RandomWeightArray(new int[,] 
            {
                { 0, 900},
                { 1,  50},
                { 2,  30},
                { 3,  10},
                { 4,   5},
                { 5,   5},
            }),
            new RandomWeightArray(new int[,] 
            {
                { 1,  40},
                { 2,  30},
                { 3,  20},
                { 4,   7},
                { 5,   3},
            }),
            new RandomWeightArray(new int[,]
            {
                { 1,  50},
                { 2,  25},
                { 3,  10},
                { 4,  10},
                { 5,   5},
            }),
            new RandomWeightArray(new int[,]
            {
                { 1,   5},
                { 2,  10},
                { 3,  30},
                { 4,  30},
                { 5,  25},
            }),
            new RandomWeightArray(new int[,]
            {
                { 0, 900},
                { 1,  50},
                { 2,  30},
                { 3,  10},
                { 4,   5},
                { 5,   5},
            }),
        };

        internal static RandomWeightArray slicerAmountWeightFeatureSpin = new RandomWeightArray(new int[,]
        {
            { 1,   1},
            { 2,   1},
            { 3,  34},
            { 4,  32},
            { 5,  32},
        });

        internal static RandomWeightArray slicerAmountWeightFS = new RandomWeightArray(new int[,]
        {
            { 0,  20},
            { 1,  20},
            { 2,  20},
            { 3,  15},
            { 4,  15},
            { 5,  10},
        });

        internal const int FeatureBuySlicerRow = 0;
        internal static RandomWeightArray slicerRowIndexWeight = new RandomWeightArray(new int[,]
        {
            { 0, 1},
            { 1, 2},
            { 2, 3},
            { 3, 4},
        });

        internal static RandomWeightArray slicerRowIndexWeightBonusHunt = new RandomWeightArray(new int[,]
        {
            { 0, 1},
            { 1, 1},
            { 2, 1},
            { 3, 1},
        });

        internal static RandomWeightArray slicerValueWeight = new RandomWeightArray(new int[,]
        {
            { 4, 25},
            { 3, 25},
            { 2, 50},
        });

        internal static RandomWeightArray[] slicerValueWeightsFeatureBuy = new RandomWeightArray[]
        {
            new RandomWeightArray(new int[,] // 98%
            {
                { 4, 19},
                { 3, 31},
                { 2, 50},
            }),
            new RandomWeightArray(new int[,] // 90%
            {
                { 4, 12},
                { 3, 23},
                { 2, 65},
            }),
        };

        internal static double redrawNoSliceWinProba = 0.10;
        internal static double redrawNoSliceWinProbaBonusHunt = 0.2;
        internal static double redrawNoSliceWinProbaFeatureSpin = 0.75;
        internal static double redrawNoSliceWinProbaFS = 0.3;

        internal double[] deadSpinProbas = new double[] { 0.442, 0.488 };
        internal double[] deadSpinProbasBonusHunt = new double[] { 0.19, 0.255 };
        internal double[] deadSpinProbasFeatureSpin = new double[] { 0.026, 0.105 };

        internal static readonly double[] bgRedrawThresholds =
        {
                          0,  0.1,   5,   20,   50,   100, 200,  1000,  5000,   
        };

        internal static readonly double[][] bgRedrawProbas =
        {
           new double[]{  0,    0,   0, 0.10, 0.20,  0.20, 0.30,  0.40,  0.40,   },
           new double[]{  0,    0,   0, 0.00, 0.00,  0.10, 0.40,  0.60,  0.60,   },
           new double[]{  0,  0.4, 0.2, 0.00, 0.00,  0.00, 0.00,  0.00,  0.10,   },
        };

        internal static readonly double[] bonusRedrawThresholds =
        {
             0,     1,    10,     30,    50,   100,   200,   500,   1000,   2000,  5000, 
        };

        internal static readonly double[] bonusRedrawProbas = new double[]
        {
             1,  0.00,  0.00,   0.00,  0.00,  0.00,  0.05,  0.20,   0.40,   0.60,  0.80, 
        };

        // State Variable
        internal MathSummary summary = new MathSummary();


        internal class SlotSpin
        {
            JellySlice Game;
            internal SymbolGrid Grid;
            internal bool Basegame;
            internal List<SlotWin> Wins;
            internal int CascadingCount;
            //statistics
            internal int InitialSliceWinSymbol;
            internal int InitialLandWildAmount;

            internal SlotSpin(JellySlice game, bool basegame, bool deadSpin)
            {
                this.Game = game;
                Basegame = basegame;
                Grid = new SymbolGrid(basegame, Game.rand);
                Wins = new List<SlotWin>();
                CascadingCount = 0;

                double usedRedrawNoSliceWinProba = redrawNoSliceWinProba;
                if (Basegame)
                {
                    if (simulationBetMode == BetMode.BonusHunt)
                        usedRedrawNoSliceWinProba = redrawNoSliceWinProbaBonusHunt;
                    else if (simulationBetMode == BetMode.FeatureSpin)
                        usedRedrawNoSliceWinProba = redrawNoSliceWinProbaFeatureSpin;
                }
                else
                    usedRedrawNoSliceWinProba = redrawNoSliceWinProbaFS;

                //start spin
                bool redraw = false;
                do
                {
                    redraw = false;
                    Grid.Initialize();
                    if (deadSpin)
                    {
                        if (Game.CheckWins(Grid.Symbols, Grid.Multipliers).Count() > 0 || GetFinalSC() >= SC_TO_TRIGGER_FS)
                            redraw = true;
                    }
                    else
                    {
                        var destroyed = GetDestroyed(Grid.Symbols, Game.CheckWins(Grid.Symbols, Grid.Multipliers));
                        bool sliceWin = false;
                        for (int i = 0; i < GRID_WIDTH; i++)
                        {
                            for (int j = 0; j < GRID_HEIGHT; j++)
                            {
                                if (Grid.Multipliers[i, j] > 0 && (destroyed[i, j] || Grid.Symbols[i, j] == SC))
                                {
                                    sliceWin = true;
                                    break;
                                }
                            }
                            if (sliceWin)
                                break;
                        }
                        if (!sliceWin && Game.rand.NextDouble() < usedRedrawNoSliceWinProba)
                            redraw = true;
                    }
                } while (redraw);
                InitialLandWildAmount = SlotUtil.FindScatters(Grid.Symbols, WR).Count;
                while (true)
                {
                    var cascadeWins = Game.CheckWins(Grid.Symbols, Grid.Multipliers);
                    
                    if (debug)
                    {
                        Console.WriteLine("==================");
                        PrintReels(Grid.Symbols, Grid.Multipliers);
                        Console.WriteLine("==================");
                    }

                    if (cascadeWins.Count == 0)
                        break;

                    if (debug)
                    {
                        foreach (var win in cascadeWins)
                            Console.WriteLine($"{win.SymbolWinIndex}({win.Size}) * {win.Multiplier}: {win.Win} ");
                    }

                    Wins.AddRange(cascadeWins);
                    CascadingCount++;
                    var destroyed = GetDestroyed(Grid.Symbols, cascadeWins);
                    for (int i = 0; i < destroyed.GetLength(0); i++)
                        for (int j = 0; j < destroyed.GetLength(1); j++)
                            if (destroyed[i, j])
                                Grid.Symbols[i, j] = DUMMY;

                    if (CascadingCount == 1)
                    {
                        int sliceWinSymbols = 0;
                        for(int i=0; i<GRID_WIDTH; i++)
                        {
                            for(int j=0; j<GRID_HEIGHT; j++)
                                if (destroyed[i, j] && Grid.Multipliers[i, j]>0)
                                    sliceWinSymbols++;
                        }
                        InitialSliceWinSymbol = sliceWinSymbols;
                    }
                    Grid.Cascade();
                }
            }

            internal int GetFinalSC()
            {
                int scAmount = 0;
                for (int i = 0; i < GRID_WIDTH; i++)
                    for (int j = 0; j < GRID_HEIGHT; j++)
                        if (Grid.Symbols[i, j] == SC)
                            scAmount += Grid.Multipliers[i, j] > 0 ? Grid.Multipliers[i, j] : 1;
                return scAmount;
            }

            internal int GetTotalWin()
            {
                return Wins.Sum(x => x.Win);
            }
        }

        internal SlotSpin GenerateMainSpin()
        {
            SlotSpin spin;// = new SlotSpin(this, true);
            bool deadSpin = false;
            int index = 0;
            if (simulationBetMode == BetMode.Normal)
                deadSpin = rand.NextDouble() < deadSpinProbas[RtpVariantIndex];
            if (simulationBetMode == BetMode.BonusHunt)
            {
                deadSpin = rand.NextDouble() < deadSpinProbasBonusHunt[RtpVariantIndex];
                index = 1;
            }
            else if (simulationBetMode == BetMode.FeatureSpin)
            {
                deadSpin = rand.NextDouble() < deadSpinProbasFeatureSpin[RtpVariantIndex];
                index = 2;
            }
            double redrawProba = 0;
            do
            { 
                spin = new SlotSpin(this, true, deadSpin);
                double win = (double)spin.GetTotalWin() / BET_SIZE;
                redrawProba = MathUtil.GetProbaByValue(bgRedrawThresholds, bgRedrawProbas[index], win);
            } while (rand.NextDouble() < redrawProba);

            return spin;
        }

        internal List<SlotSpin> GenerateFreeSpins(int fsAmount, int bgWin)
        {
            var freeSpins = new List<SlotSpin>();
            double redrawProba = 0;
            do
            {
                int leftFS = fsAmount;
                freeSpins = new List<SlotSpin>();
                while (leftFS > 0)
                {
                    var fs = new SlotSpin(this, false, false);
                    leftFS--;
                    leftFS += fs.GetFinalSC();
                    freeSpins.Add(fs);
                }
                double win = (double)freeSpins.Sum(x => x.GetTotalWin()) / BET_SIZE;
                win = Math.Min(win, winCap - (double)bgWin / BET_SIZE);
                redrawProba = MathUtil.GetProbaByValue(bonusRedrawThresholds, bonusRedrawProbas, win);
            } while (rand.NextDouble() < redrawProba);
                
            return freeSpins;
        }

        internal void PlayGame()
        {
            int win = 0;
            var mainSpin = GenerateMainSpin();
            win += mainSpin.GetTotalWin();
            //DebugUtil.PrintReels(mainSpin.SymbolMatrix);
            //Console.WriteLine($"{(double)mainSpin.GetTotalWin() / BET_SIZE}");
            //Console.WriteLine("----------------");

            List<SlotSpin>? freeSpins = null; 
            if(mainSpin.GetFinalSC() >= SC_TO_TRIGGER_FS && win < winCap*BET_SIZE)
            {
                freeSpins = GenerateFreeSpins(mainSpin.GetFinalSC(), win);
            }

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
            internal double MaxWay = 0;
            internal Average HitFSProba = new();
            internal Average AverageFSPayouts = new();
            internal Average AverageSquared = new();
            internal BreakDownTable2D SymbolWins = new(new double[] { WR, H1, M2, M3, M4, M5, L6, L7, L8, L9, L10 }, new double[] { 3, 4, 5 });
            internal Distribution CascadeDistBG = new Distribution(new double[] { 0, 1, 2, 3, 4, 5 });
            internal Distribution CascadeDistFS = new Distribution(new double[] { 0, 1, 2, 3, 4, 5 });
            internal Distribution WinDistributionBG = new Distribution(new double[] { 0, 0.01, 1, 5, 15, 30, 100, 1000, 5000 });
            internal Distribution WinDistributionFS = new Distribution(new double[] { 0, 0.01, 1, 5, 15, 30, 100, 1000, 5000 });
            internal Distribution WinDistribution = new Distribution(new double[] { 0, 0.01, 1, 5, 15, 30, 100, 1000, 5000 });
            internal Distribution SliceWinSymbolDistBG = new Distribution(new double[] { 0, 1, 2, 3, 5, 8 });
            internal Distribution WRLandingDistBG = new Distribution(new double[] { 0, 1, 2, 3 });
            internal Distribution SCLandingDistBG = new Distribution(new double[] { 0, 1, 5, 10, 15, 21 });
            internal Distribution WRLandingDistFS = new Distribution(new double[] { 0, 1, 2, 3 });
            internal Distribution SCLandingDistFS = new Distribution(new double[] { 0, 1, 5, 10, 15, 21 });
            internal Distribution SliceWinSymbolDistFS = new Distribution(new double[] { 0, 1, 2, 3, 5, 8 });
            internal Average FSHitRate = new();

            internal Distribution FSAmuontDist = new Distribution(new double[] { 0, 5, 10, 15, 20, 30, 50 });

            internal void Add(SlotSpin mainSpin, List<SlotSpin>? freeSpins)
            {
                SampleSize++;
                //Basegame
                int win = Math.Min(mainSpin.GetTotalWin(), winCap * BET_SIZE);
                BaseRTP.Add((double)win / BET_SIZE);
                foreach (SlotWin slotWin in mainSpin.Wins)
                {
                    SymbolWins.Add(slotWin.SymbolWinIndex, slotWin.Size, (double)slotWin.Win / BET_SIZE);
                    if(slotWin.Multiplier > MaxWay)
                        MaxWay = slotWin.Multiplier;
                }
                WRLandingDistBG.Add(mainSpin.InitialLandWildAmount);
                CascadeDistBG.Add(mainSpin.CascadingCount);
                SCLandingDistBG.Add(mainSpin.GetFinalSC());
                SliceWinSymbolDistBG.Add(mainSpin.InitialSliceWinSymbol);
                WinDistributionBG.Add((double)win / BET_SIZE);

                //free spins
                HitFSProba.Add(freeSpins == null ? 0 : 1);
                if(freeSpins != null)
                {
                    int fsWin = freeSpins.Sum(x => x.GetTotalWin());
                    fsWin = Math.Min(fsWin, winCap * BET_SIZE - win);
                    win += fsWin;
                    AverageFSPayouts.Add((double)fsWin / BET_SIZE);
                    WinDistributionFS.Add((double)fsWin / BET_SIZE);
                    FSAmuontDist.Add(freeSpins.Count);
                    foreach(var fs in freeSpins)
                    {
                        FSHitRate.Add(fs.GetTotalWin()>0 ? 1 : 0);
                        foreach (SlotWin slotWin in fs.Wins)
                        {
                            SymbolWins.Add(slotWin.SymbolWinIndex, slotWin.Size, (double)slotWin.Win / BET_SIZE);
                        }
                        CascadeDistFS.Add(fs.CascadingCount);
                        WRLandingDistFS.Add(fs.InitialLandWildAmount);
                        SCLandingDistFS.Add(fs.GetFinalSC());
                        SliceWinSymbolDistFS.Add(fs.InitialSliceWinSymbol);
                    }
                }

                // Overall
                ReturnToPlayer.Add((double)win / BET_SIZE);
                AverageSquared.Add((double)win * win / BET_SIZE / BET_SIZE);
                if ((double)win / BET_SIZE > MaxWin)
                    MaxWin = (double)win / BET_SIZE;
                HitRate.Add(win > 0 ? 1 : 0);
                WinDistribution.Add((double)win / BET_SIZE);
            }

            internal void WriteOnConsole()
            {
                Console.WriteLine($"Sample Size: {SampleSize}");
                Console.WriteLine($"Variance: {AverageSquared.Value - Math.Pow(ReturnToPlayer.Value, 2)}");
                Console.WriteLine($"Max Win: {MaxWin}x");
                Console.WriteLine($"Max Way In BG: {MaxWay}");
                Console.WriteLine($"RTP: {ReturnToPlayer.Percentage}");
                Console.WriteLine($"Base RTP: {BaseRTP.Percentage}");
                Console.WriteLine($"FS RTP: {AverageFSPayouts.Value * HitFSProba.Value * 100}%");
                Console.WriteLine($"Hit Rate: {HitRate.Percentage}");
                Console.WriteLine($"Hit FS Probability: {HitFSProba.Percentage}");
                Console.WriteLine(string.Format("Average FS Payouts: {0,2:F}x", AverageFSPayouts.Value));
                Console.WriteLine("--------------");
                Console.WriteLine("Symbol Win Distribution:");
                SymbolWins.PrintAverageValues(SampleSize);
                Console.WriteLine("--------------");
                Console.WriteLine("Win Distribution:");
                WinDistribution.PrintProbaOnConsole();
                WinDistribution.PrintValuesOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("BG Win Distribution:");
                WinDistributionBG.PrintProbaOnConsole();
                WinDistributionBG.PrintValuesOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("cascade Distribution in basegame:");
                CascadeDistBG.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("Wild Landing Distribution in basegame:");
                WRLandingDistBG.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("SC Landing Distribution in basegame:");
                SCLandingDistBG.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("Slice Win Symbol Distribution in basegame:");
                SliceWinSymbolDistBG.PrintProbaOnConsole();
                Console.WriteLine("--------------");

                Console.WriteLine("fs Win Distribution:");
                WinDistributionFS.PrintProbaOnConsole();
                WinDistributionFS.PrintValuesOnConsole();

                Console.WriteLine("fs amount Distribution:");
                FSAmuontDist.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine($"FS Hit Rate: {FSHitRate.Percentage}");
                Console.WriteLine("--------------");
                Console.WriteLine("cascade Distribution in fs:");
                CascadeDistFS.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("Wild Landing Distribution in fs:");
                WRLandingDistFS.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("SC Landing Distribution in fs:");
                SCLandingDistFS.PrintProbaOnConsole();
                Console.WriteLine("--------------");
                Console.WriteLine("Slice Win Symbol Distribution in fs:");
                SliceWinSymbolDistFS.PrintProbaOnConsole();
                Console.WriteLine("--------------");


            }
        }

        internal List<SlotWin> CheckWins(int[,] symbols, int[,] multis)
        {
            var wins = new List<SlotWin>();
            foreach (int sym in new int[] { WR, H1, M2, M3, M4, M5, L6, L7, L8, L9, L10 })
            {
                List<Coordinate> winCoors = new List<Coordinate>();
                bool matchingSym = false;
                int symSize = 0;
                int wayNum = 0;
                for (int i = 0; i < symbols.GetLength(0); i++)
                {
                    int symNum = 0;
                    int wildNum = 0;
                    for (int j = 0; j < symbols.GetLength(1); j++)
                    {
                        if (symbols[i, j] == sym)
                        {
                            symNum += multis[i, j] > 0 ? multis[i, j] : 1;
                            winCoors.Add(new Coordinate(i, j));
                        }
                        else if (symbols[i, j] == WR)
                        {
                            wildNum += multis[i, j] > 0 ? multis[i, j] : 1; ;
                            winCoors.Add(new Coordinate(i, j));
                        }
                    }
                    if (symNum > 0)
                        matchingSym = true;
                    if (symNum + wildNum == 0)
                        break;
                    else
                    {
                        symSize++;
                        if (wayNum == 0)
                            wayNum = symNum + wildNum;
                        else
                            wayNum *= (symNum + wildNum);
                    }
                }
                if (matchingSym && payouts[sym][symSize] > 0)
                {
                    wins.Add(new SlotWin(payouts[sym][symSize] * wayNum, sym, symSize, wayNum, 0, winCoors));
                }
            }
            return wins;
        }

        internal class SymbolGrid
        {
            internal readonly bool Basegame;
            internal readonly Random RandReference;
            internal int[] Stops;
            internal int[][] UsedReels;
            internal int UsedReelsIndex;
            Dictionary<int, int> HighRandomMap;
            Dictionary<int, int> LowRandomMap;
            internal int[,] Symbols;
            internal int[,] Multipliers;

            internal SymbolGrid(bool basegame, Random randReference)
            {
                this.RandReference = randReference;
                this.Basegame = basegame;
                this.Stops = new int[GRID_WIDTH];
                
                this.Symbols = new int[GRID_WIDTH, GRID_HEIGHT];
                this.Multipliers = new int[GRID_WIDTH, GRID_HEIGHT];

                HighRandomMap = new Dictionary<int, int>();
                int highTS = randReference.Next(4) + M2;
                for (int i = 0; i < 4; i++)
                    HighRandomMap[i + MR1] = (highTS + i - M2) % (M5 - M2 + 1) + M2;
                LowRandomMap = new Dictionary<int, int>();
                int ts = randReference.Next(5) + L6;
                for (int i = 0; i < 5; i++)
                    LowRandomMap[i + LR1] = (ts + i - L6) % (L10 - L6 + 1) + L6;
            }

            internal void Initialize()
            {
                // decide used Reels
                int reelsIndex = 0;
                if (Basegame)
                {
                    if (simulationBetMode == BetMode.Normal)
                        reelsIndex = basegameReelWeights.rollSingleItem(RandReference);
                    else if (simulationBetMode == BetMode.BonusHunt)
                        reelsIndex = basegameReelWeightsBonusHunt.rollSingleItem(RandReference);
                    else if (simulationBetMode == BetMode.FeatureSpin)
                        reelsIndex = basegameReelWeightsFeatureSpin.rollSingleItem(RandReference);
                    else
                        reelsIndex = FeatureBuyReelsIndex;
                }
                else
                    reelsIndex = fsReelWeights.rollSingleItem(RandReference);
                this.UsedReels = Basegame ? Reels[reelsIndex] : FS_Reels[reelsIndex];
                UsedReelsIndex = reelsIndex;

                // slicers
                this.Multipliers = new int[GRID_WIDTH, GRID_HEIGHT];
                int slicerAmount = 0;  
                if (Basegame)
                {
                    if (simulationBetMode == BetMode.Buy)
                        slicerAmount = FeatureBuySlicerAmount;
                    else if (simulationBetMode == BetMode.FeatureSpin)
                        slicerAmount = slicerAmountWeightFeatureSpin.rollSingleItem(RandReference);
                    else
                        slicerAmount = slicerAmountWeight[UsedReelsIndex].rollSingleItem(RandReference);
                }
                else
                    slicerAmount = slicerAmountWeightFS.rollSingleItem(RandReference);

                if (slicerAmount > 0)
                {
                    List<int> reelList = new List<int>() { 0, 1, 2, 3, 4};
                    for(int i = 0; i < slicerAmount; i++)
                    {
                        int slicerReelIndex = RandReference.Next(reelList.Count);
                        int slicerReel = reelList[slicerReelIndex];
                        reelList.RemoveAt(slicerReelIndex);
                        int slicerRow = 0;
                        if(Basegame) 
                        {
                            if(simulationBetMode == BetMode.Buy)
                                slicerRow = FeatureBuySlicerRow;
                            else if(simulationBetMode == BetMode.BonusHunt)
                                slicerRow = slicerRowIndexWeightBonusHunt.rollSingleItem(RandReference);
                            else
                                slicerRow = slicerRowIndexWeight.rollSingleItem(RandReference);
                        }
                        else
                            slicerRow = slicerRowIndexWeight.rollSingleItem(RandReference);
                        int slicerValue = 0;
                        if (Basegame)
                        {
                            if (simulationBetMode == BetMode.Buy)
                                slicerValue = slicerValueWeightsFeatureBuy[RtpVariantIndex].rollSingleItem(RandReference);
                            else
                                slicerValue = slicerValueWeight.rollSingleItem(RandReference);
                        }
                        else
                            slicerValue = slicerValueWeight.rollSingleItem(RandReference);
                        for(int j=slicerRow; j<GRID_HEIGHT; j++)
                            Multipliers[slicerReel, j] = slicerValue;
                    }
                }
                    
                // symbols
                Stops = SlotUtil.GetReelStops(RandReference, UsedReels);
                this.Symbols = new int[GRID_WIDTH, GRID_HEIGHT];
                for (int i = 0; i < GRID_WIDTH; i++)
                    for (int j = 0; j < GRID_HEIGHT; j++)
                        Symbols[i, j] = DUMMY;
                SlotUtil.FillAvalancheSymbol(Symbols, UsedReels, Stops, DUMMY);
                
                // random symbol
                for(int i=0; i < GRID_WIDTH;i++)
                    for(int j=0; j < GRID_HEIGHT; j++)
                    {
                        if (HighRandomMap.ContainsKey(Symbols[i, j]))
                            Symbols[i, j] = HighRandomMap[Symbols[i, j]];
                        else if (LowRandomMap.ContainsKey(Symbols[i, j]))
                            Symbols[i, j] = LowRandomMap[Symbols[i, j]];
                    }
            }

            internal void Cascade()
            {
                Symbols = SlotUtil.GetAvalanche(Symbols, DUMMY, new int[] { });
                for (int i = 0; i < Symbols.GetLength(0); i++)
                {
                    for (int j = Symbols.GetLength(1) - 1; j >= 0; j--)
                    {
                        if (Symbols[i, j] == DUMMY)
                        {
                            Stops[i] = (Stops[i] - 1 + UsedReels[i].Length) % UsedReels[i].Length;
                            Symbols[i, j] = UsedReels[i][Stops[i]];
                            if (HighRandomMap.ContainsKey(Symbols[i, j]))
                                Symbols[i, j] = HighRandomMap[Symbols[i, j]];
                            else if (LowRandomMap.ContainsKey(Symbols[i, j]))
                                Symbols[i, j] = LowRandomMap[Symbols[i, j]];
                        }
                    }
                }
            }
        }
        internal static bool[,] GetDestroyed(int[,] symbols, List<SlotWin> wins)
        {
            bool[,] destroyed = new bool[symbols.GetLength(0), symbols.GetLength(1)];
            foreach (var win in wins)
            {
                foreach (var coor in win.WinCoordinates)
                {
                    destroyed[coor.x, coor.y] = true;
                }
            }
            return destroyed;
        }

        internal static void PrintReels(int[,] symbols, int[,] values, int digit = 2)
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
                Console.WriteLine();
            }
        }
    }
}
