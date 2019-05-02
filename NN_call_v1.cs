using System;
using System.Linq;

using System.IO;
using System.Diagnostics;

using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Text;
using System.Threading;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class NN_call_v1 : Robot
    {
        //based on example to call pyhton script from:
        //https://code.msdn.microsoft.com/windowsdesktop/C-and-Python-interprocess-171378ee

        // full path of python interpreter  
        string python = "C:\\Python27\\python.exe";

        // python app to call  
        string myPythonApp = "C:\\Python27\\myScripts\\ReadAndUseNNv4.py";

        // dummy parameters to send Python script: these are scaled hourly changes of last 10 bars
        //double x1 = -0.0795, x2 = -0.036, x3 = -0.081, x4 = 0.0825, x5 = -0.0885, x6 = -0.02, x7 = 0.0115, x8 = -0.032, x9 = -0.011, x10 = -0.0095;

        //to store last processed open time
        DateTime lastTimeStamp = DateTime.MinValue;

        private const string label = "0100111001001110";

        protected override void OnStart()
        {
        }

        protected override void OnTick()
        {
            // on new bar
            if (MarketSeries.OpenTime.LastValue != lastTimeStamp)
            {
                lastTimeStamp = MarketSeries.OpenTime.LastValue;

                //close any open positions (to test one bar prediction)
                foreach (var p in Positions.FindAll(label, Symbol))
                {
                    ClosePosition(p);
                }

                //get values for NN
                int index = MarketSeries.OpenTime.GetIndexByTime(lastTimeStamp);
                var argsString = "";

                //loop over last bars not including lastCompletedBar for values
                for (int i = index - 10; i < index; i++)
                {
                    var delta = (MarketSeries.Close[i] - MarketSeries.Close[i - 1]) / (Symbol.PipSize * 200);

                    argsString += ToLongString(Math.Round(delta, 8)) + " ";
                }

                //send values string to NN script
                // Create new process start info 
                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(python);

                // make sure we can read the output from stdout 
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.RedirectStandardOutput = true;

                // start python app with arguments
                myProcessStartInfo.Arguments = myPythonApp + " " + argsString;

                Process myProcess = new Process();

                // assign start information to the process 
                myProcess.StartInfo = myProcessStartInfo;

                Print("Calling Python script...");

                // start the process 
                myProcess.Start();

                // Read the standard output of the app we called.  
                // in order to avoid deadlock we will read output first 
                // and then wait for process terminate: 
                string nnOutputString;
                using (StreamReader myStreamReader = myProcess.StandardOutput)
                {
                    nnOutputString = myStreamReader.ReadToEnd();
                }
                // wait exit signal from the app we called and then close it. 
                myProcess.WaitForExit();
                myProcess.Close();
                Print("On " + MarketSeries.OpenTime[index] + " got from NN: " + nnOutputString);

                //trade signal for one bar
                int si;
                if (!int.TryParse(nnOutputString, out si))
                {
                    Print("Error parsing string!");
                }
                if (si == 1)
                {
                    ExecuteMarketOrder(TradeType.Buy, Symbol, 1000, label, null, null, null, "");
                }
                else if (si == -1)
                {
                    ExecuteMarketOrder(TradeType.Sell, Symbol, 1000, label, null, null, null, "");
                }
            }
        }


        //ToLongString() and GetZeros() from: http://stackoverflow.com/questions/1546113/double-to-string-conversion-without-scientific-notation
        private static string ToLongString(double input)
        {
            string str = input.ToString().ToUpper();

            // if string representation was collapsed from scientific notation, just return it:
            if (!str.Contains("E"))
                return str;

            bool negativeNumber = false;

            if (str[0] == '-')
            {
                str = str.Remove(0, 1);
                negativeNumber = true;
            }

            string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            char decSeparator = sep.ToCharArray()[0];

            string[] exponentParts = str.Split('E');
            string[] decimalParts = exponentParts[0].Split(decSeparator);

            // fix missing decimal point:
            if (decimalParts.Length == 1)
                decimalParts = new string[] 
                {
                    exponentParts[0],
                    "0"
                };

            int exponentValue = int.Parse(exponentParts[1]);

            string newNumber = decimalParts[0] + decimalParts[1];

            string result;

            if (exponentValue > 0)
            {
                result = newNumber + GetZeros(exponentValue - decimalParts[1].Length);
            }
            // negative exponent
            else
            {
                result = "0" + decSeparator + GetZeros(exponentValue + decimalParts[0].Length) + newNumber;

                result = result.TrimEnd('0');
            }

            if (negativeNumber)
                result = "-" + result;

            return result;
        }

        private static string GetZeros(int zeroCount)
        {
            if (zeroCount < 0)
                zeroCount = Math.Abs(zeroCount);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < zeroCount; i++)
                sb.Append("0");

            return sb.ToString();
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
