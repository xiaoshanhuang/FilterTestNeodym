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
//using MathNet.SignalProcessing.Filter.FIR;
//using MathNet.SignalProcessing.Filter.IIR;
using MathNet.Numerics.Filtering.FIR;
using MathNet.Numerics.Filtering.IIR;

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
            double[] coefLowPass = FirCoefficients.LowPass(samplingRate, 50, 500);
            OnlineFirFilter filterLowPass = new OnlineFirFilter(coefLowPass);

            double[] coefHighPass = FirCoefficients.HighPass(samplingRate, 0.5, 1500);
            OnlineFirFilter filterHighPass = new OnlineFirFilter(coefHighPass);

            double[] coefBandStop = FirCoefficients.BandStop(samplingRate, 45, 55, 1500);
            OnlineFirFilter filterBandStop = new OnlineFirFilter(coefLowPass);

            double[] coefIIR = IirCoefficients.LowPass(samplingRate, 50, 1);

            // Filtering
            double[] tempHighPass = filterHighPass.ProcessSamples(data);
            double[] tempLowPass = filterLowPass.ProcessSamples(tempHighPass);
            double[] result = filterBandStop.ProcessSamples(tempLowPass);

            double[] coefPink = new double[8] { 0.049922035, -0.095993537, 0.050612699, -0.004408786, 1, -2.494956002, 2.017265875, -0.522189400 };
            int nT60 = 1430;
            IChannelSource whiteSource = new WhiteGaussianNoiseSource();
            double[] whiteNoise = new double[dataLength + nT60];
            for (int i = 0; i < whiteNoise.Length; i++)
            {
                whiteNoise[i] = whiteSource.ReadNextSample();
            }
            
            OnlineIirFilter filterPink = new OnlineIirFilter(coefPink);
            double[] tempData = filterLowPass.ProcessSamples(whiteNoise);
            double[] pinkNoise = new double[dataLength];
            Array.Copy(tempData, nT60, pinkNoise, 0, dataLength);

            StreamWriter coef = new StreamWriter("D:\\coef.txt");
            for (int i = 0; i < coefPink.Length; i++)
            {
                coef.WriteLine(coefPink[i]);
            }
            coef.Close();

            StreamWriter dataFile = new StreamWriter("D:\\data.txt");
            for (int i = 0; i < whiteNoise.Length; i++)
            {
                dataFile.WriteLine(whiteNoise[i]);
            }
            dataFile.Close();

            StreamWriter resultFile = new StreamWriter("D:\\result.txt");
            for (int i = 0; i < pinkNoise.Length; i++)
            {
                resultFile.WriteLine(pinkNoise[i]);
            }
            resultFile.Close();
        }
    }
}
