using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MathNet.Numerics;
using NUnit.Framework;
using MathNet.SignalProcessing;
using MathNet.SignalProcessing.DataSources;
using MathNet.SignalProcessing.Channel;
using MathNet.SignalProcessing.Filter.FIR;

namespace FilterTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            // Generate test signal
            int samplingRate = 1000;
            double signalFreq = 10; double signalAmp = 10;
            double noiseFreq = 0.25; double noiseAmp = 10;
            double  utilityFreq = 50; double utilityAmp = 1000;

            IChannelSource signalSource = new SinusoidalSource(samplingRate, signalFreq, signalAmp, 0, 0, 0);
            IChannelSource noiseSource = new SinusoidalSource(samplingRate, noiseFreq, noiseAmp, 0, 0, 0);
            IChannelSource utilitySource = new SinusoidalSource(samplingRate, utilityFreq, utilityAmp, 0, 0, 0);

            int dataLength = 60 * samplingRate;

            double[] data = new double[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = signalSource.ReadNextSample() + noiseSource.ReadNextSample() + utilitySource.ReadNextSample();
            }

            // Filter initialization
            double[] coefLowPass = FirCoefficients.LowPass(samplingRate, 30, 1000);
            OnlineFirFilter filterLowPass = new OnlineFirFilter(coefLowPass);

            double[] coefHighPass = FirCoefficients.HighPass(samplingRate, 0.5, 3000);
            OnlineFirFilter filterHighPass = new OnlineFirFilter(coefHighPass);

            double[] coefBandStop = FirCoefficients.BandStop(samplingRate, 45, 55, 3000);
            OnlineFirFilter filterBandStop = new OnlineFirFilter(coefLowPass);

            // Filtering
            double[] tempHighPass = filterHighPass.ProcessSamples(data);
            double[] tempLowPass = filterLowPass.ProcessSamples(tempHighPass);
            double[] result = filterBandStop.ProcessSamples(tempLowPass);

            StreamWriter coef = new StreamWriter("D:\\coef.txt");
            for (int i = 0; i < coefBandStop.Length; i++)
            {
                coef.WriteLine(coefBandStop[i]);
            }
            coef.Close();

            StreamWriter dataFile = new StreamWriter("D:\\data.txt");
            for (int i = 0; i < data.Length; i++)
            {
                dataFile.WriteLine(data[i]);
            }
            dataFile.Close();

            StreamWriter resultFile = new StreamWriter("D:\\result.txt");
            for (int i = 0; i < result.Length; i++)
            {
                resultFile.WriteLine(result[i]);
            }
            resultFile.Close();
        }
    }
}
