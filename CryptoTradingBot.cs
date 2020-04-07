#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class CryptoTradingBot : Strategy
	{
        //Support / Reistance Indicator **BE SURE TO IMPORT THIS IN NINJATRADER, Go to Tools > Import > Ninjascript Add On and Select SRLines.zip**
        private DynamicSRLines SRLines = null;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"This trading bot will trade cryptocurrencies using support and resistance.";
				Name										= "CryptoTradingBot";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
			}
			else if (State == State.Configure)
			{
                //Add Indicator to Chart
                SRLines = DynamicSRLines(Close, 5, 300, 10, 3, 3, true, Brushes.Green, Brushes.Red);
                AddChartIndicator(SRLines);
			}
		}

		protected override void OnBarUpdate()
		{
            if (BarsInProgress != 0)
                return;
            if (Bars.Count < BarsRequiredToTrade)
                return;

            //Entry
            if (Position.MarketPosition == MarketPosition.Flat && Close[0] <= SRLines.L1SupportPrice)
                EnterShort();

            if (Position.MarketPosition == MarketPosition.Flat && Close[0] >= SRLines.L1ResistancePrice)
                EnterLong();

            //Exit
            if (Position.MarketPosition == MarketPosition.Long && High[0] >= SRLines.L1ResistancePrice)
                ExitLong();

            if (Position.MarketPosition == MarketPosition.Short && Low[0] <= SRLines.L1SupportPrice)
                ExitShort();
        }
	}
}
