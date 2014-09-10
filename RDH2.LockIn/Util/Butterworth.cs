using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.LockIn.Util
{
    /// <summary>
    /// The Butterworth class is used to perform filtering
    /// on the the LockIn.Amplifier data.
    /// </summary>
    internal class Butterworth
    {
        #region Member Variables
        private const Int32 _filterOrder = 5;
        private Int32 _samplingRate = 1000;
        private Double _cornerFrequency = 0.002;

        //Binomial Coefficients
        Double[] _dCoeffs = null;
        Int32[] _cCoeffs = null;
        Double _cScaleFactor = 1.0;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the Butterworth object
        /// </summary>
        /// <param name="samplingRate">The rate at which the data is sampled</param>
        /// <param name="cornerFrequency">The frequency in Hz at which the signal is attenuated 3 dB</param>
        public Butterworth(Int32 samplingRate, Double cornerFrequency)
        {
            //Save the input in the member variables
            this._samplingRate = samplingRate;

            //Calculate the frequency in fractions of PI
            this._cornerFrequency = (2 * cornerFrequency) / samplingRate;

            //Calculate the filter coefficients
            this.CalculateDCoefficients();
            this.CalculateCCoefficients();

            //Calculate the Scaling Coefficient
            this.CalculateScalingCoefficient();
        }
        #endregion


        #region LowPass Method
        /// <summary>
        /// LowPass performs a low-pass filter operation on the
        /// input Array of Doubles
        /// </summary>
        /// <param name="input">The Array of Doubles to filter with a low-pass</param>
        /// <returns>Double Array of low-pass filtered data</returns>
        public Double[] LowPass(Double[] input)
        {
            //Get the length of the input
            Int32 inputLength = input.GetLength(0);

            //Declare a variable to return
            Double[] rtn = new Double[inputLength];

            //Gain variable
            Double GAIN = 1.672358808e+04;

            //Carrier variables
            Double[] xv = new Double[Butterworth._filterOrder + 1];
            Double[] yv = new Double[Butterworth._filterOrder + 1];

            //Loop through the values and perform the filter
            for (Int32 i = 0; i < inputLength; i++)
            { 
                //Roll the X-Array
                xv[0] = xv[1]; 
                xv[1] = xv[2]; 
                xv[2] = xv[3]; 
                xv[3] = xv[4]; 
                xv[4] = xv[5]; 
                xv[5] = input[i] / GAIN;

                //Roll the Y-Array
                yv[0] = yv[1]; 
                yv[1] = yv[2]; 
                yv[2] = yv[3]; 
                yv[3] = yv[4]; 
                yv[4] = yv[5];
                yv[5] = (xv[0] + xv[5]) + 5 * (xv[1] + xv[4]) + 10 * (xv[2] + xv[3])
                      + (0.3599282451 * yv[0]) + (-2.1651329097 * yv[1])
                      + (5.2536151704 * yv[2]) + (-6.4348670903 * yv[3])
                      + (3.9845431196 * yv[4]);

                //Set the output value
                rtn[i] = yv[5];
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region Filter Coefficient Methods
        /// <summary>
        /// CalculateDCoefficients performs the calculation for
        /// the d-series coefficients.
        /// </summary>
        private void CalculateDCoefficients()
        {
            //Declare a variable to hold the binomial coefficients
            Double[] rCoeffs = new Double[Butterworth._filterOrder * 2];

            //Declare variables to hold theta values
            Double theta = Math.PI * this._cornerFrequency;
            Double sinTheta = Math.Sin(theta);
            Double cosTheta = Math.Cos(theta);

            for (Int32 i = 0; i < Butterworth._filterOrder; ++i)
            {
                //Calculate the pole angle
                Double poleAngle = Math.PI * (2d * i + 1d) / (2d * Butterworth._filterOrder);

                //Declare a temp variable to hold a calculation
                Double temp = 1.0 + (sinTheta * Math.Sin(poleAngle));

                //Calculate the binomial coefficients
                rCoeffs[2 * i] = -(cosTheta) / temp;
                rCoeffs[2 * i + 1] = -(sinTheta) * Math.Cos(poleAngle) / temp;
            }

            //Perform the binomial multiplication
            Double[] dCoeffs = this.BinomialMultiplication(Butterworth._filterOrder, rCoeffs);

            //Finally, set up the d-series Coefficients 
            dCoeffs[1] = dCoeffs[0];
            dCoeffs[0] = 1.0;

            for (Int32 j = 3; j <= Butterworth._filterOrder; ++j)
                dCoeffs[j] = dCoeffs[(2 * j) - 2];

            //Copy the coefficients to the Member Variable
            this._dCoeffs = new Double[Butterworth._filterOrder + 1];
            Array.Copy(dCoeffs, this._dCoeffs, Butterworth._filterOrder + 1);
        }


        /// <summary>
        /// CalculateCCoefficients performs the calculation for
        /// the c-series coefficients.
        /// </summary>
        private void CalculateCCoefficients()
        {
            //Create the new c-series Array
            this._cCoeffs = new Int32[Butterworth._filterOrder + 1];

            //Setup the initial coefficients
            this._cCoeffs[0] = 1;
            this._cCoeffs[1] = Butterworth._filterOrder;

            //Calculate the remaining coefficients
            for (Int32 i = 2; i <= Butterworth._filterOrder / 2; ++i)
            {
                this._cCoeffs[i] = (Butterworth._filterOrder - i + 1) * this._cCoeffs[i - 1] / i;
                this._cCoeffs[Butterworth._filterOrder - i] = this._cCoeffs[i];
            }

            //Set up the mirrored coefficients
            this._cCoeffs[Butterworth._filterOrder - 1] = Butterworth._filterOrder;
            this._cCoeffs[Butterworth._filterOrder] = 1;
        }


        /// <summary>
        /// BinomialMultiplication multiplies a set of binomials
        /// together based on the set of coefficients that is 
        /// passed in.
        /// </summary>
        /// <param name="numFuncs">The number of binomials to multiply</param>
        /// <param name="coefficients">The set of Coefficients to multiply</param>
        /// <returns>Coefficients of the multiplied binomials</returns>
        private Double[] BinomialMultiplication(Int32 numFuncs, Double[] coeffs)
        {
            //Declare a variable to return
            Double[] rtn = new Double[numFuncs * 2];

            //Iterate through the coefficients and multiply
            //the binomials
            for (Int32 i = 0; i < numFuncs; ++i)
            {
                for (Int32 j = i; j > 0; --j)
                {
                    rtn[2 * j] += coeffs[2 * i] * rtn[2 * (j - 1)] - coeffs[2 * i + 1] * rtn[2 * (j - 1) + 1];
                    rtn[2 * j + 1] += coeffs[2 * i] * rtn[2 * (j - 1) + 1] + coeffs[2 * i + 1] * rtn[2 * (j - 1)];
                }

                //Get the first two coefficients
                rtn[0] += coeffs[2 * i];
                rtn[1] += coeffs[2 * i + 1];
            }
            
            //Return the result
            return rtn;
        }
        #endregion


        #region Scaling Coefficient Methods
        /// <summary>
        /// CalculateScalingCoefficient performs the calculation 
        /// of the c-series scaling coefficient.
        /// </summary>
        private void CalculateScalingCoefficient()
        {
            //Calculate a couple of useful values
            Double omega = Math.PI * this._cornerFrequency;
            Double poleAngle0 = Math.PI / (2d * Butterworth._filterOrder);

            //Declare a temp value to hold intermediate scaling
            Double tempScaleFactor = 1.0;

            //Iterate through the poles to calculate
            //the temp scaling factor
            for (Int32 i = 0; i < Butterworth._filterOrder / 2; ++i)
                tempScaleFactor *= 1.0 + Math.Sin(omega) * Math.Sin((2d * i + 1d) * poleAngle0);

            //If the order of the filter is odd, reset
            //the temp scaling factor
            if (Butterworth._filterOrder % 2 == 1) 
                tempScaleFactor *= Math.Sin(omega / 2d) + Math.Cos(omega / 2d);

            //Finally, calculate the actual scaling factor
            this._cScaleFactor = Math.Pow((omega / 2d), Butterworth._filterOrder) / tempScaleFactor;
        }
        #endregion


        #region Nonoperational Code
            ////Get the number of points in the input array
            //Int32 inputLength = input.GetLength(0);

            ////Declare a variable to return
            //Double[] rtn = new Double[inputLength];

            ////Declare variables to hold the calc arrays
            //Double[] xVals = new Double[Butterworth._filterOrder + 1];
            //Double[] yVals = new Double[Butterworth._filterOrder + 1];

            ////Iterate through the input values and
            ////calculate the filtered values
            //for (Int32 j = 0; j < inputLength - Butterworth._filterOrder; j++)
            //{
            //    //Roll the xVals array
            //    xVals[0] = xVals[1];
            //    xVals[1] = xVals[2];
            //    xVals[2] = xVals[3];
            //    xVals[3] = xVals[4];
            //    xVals[4] = xVals[5];
            //    xVals[5] = input[Butterworth._filterOrder + j] * this._cScaleFactor;

            //    //Roll the yVals array
            //    yVals[0] = yVals[1];
            //    yVals[1] = yVals[2];
            //    yVals[2] = yVals[3];
            //    yVals[3] = yVals[4];
            //    yVals[4] = yVals[5];
            //    yVals[5] = 
            //        //Calculate the feedforward variable
            //        (this._cCoeffs[0] * xVals[5] +
            //         this._cCoeffs[1] * xVals[4] +
            //         this._cCoeffs[2] * xVals[3] +
            //         this._cCoeffs[3] * xVals[2] +
            //         this._cCoeffs[4] * xVals[1] +
            //         this._cCoeffs[5] * xVals[0]) -
            //        //Calculate the feedback variable
            //        (this._dCoeffs[1] * yVals[4] +
            //         this._dCoeffs[2] * yVals[3] +
            //         this._dCoeffs[3] * yVals[2] +
            //         this._dCoeffs[4] * yVals[1] +
            //         this._dCoeffs[5] * yVals[0]); 

            //    //Set the value in the return Array
            //    rtn[j] = yVals[5];
            //}


            //Gain variable
            //Double GAIN = 3.301002979e+12;

            ////Carrier variables
            //Double[] xv = new Double[Butterworth._filterOrder + 1];
            //Double[] yv = new Double[Butterworth._filterOrder + 1];

            ////Loop through the values and perform the filter
            //for (Int32 i = 0; i < inputLength; i++)
            //{ 
            //    //Roll the X-Array
            //    xv[0] = xv[1]; 
            //    xv[1] = xv[2]; 
            //    xv[2] = xv[3]; 
            //    xv[3] = xv[4]; 
            //    xv[4] = xv[5]; 
            //    xv[5] = input[i] / GAIN;

            //    //Roll the Y-Array
            //    yv[0] = yv[1]; 
            //    yv[1] = yv[2]; 
            //    yv[2] = yv[3]; 
            //    yv[3] = yv[4]; 
            //    yv[4] = yv[5]; 
            //    yv[5] = (xv[0] + xv[5]) + 5 * (xv[1] + xv[4]) + 10 * (xv[2] + xv[3])
            //                   + (  0.9798724625 * yv[0]) + ( -4.9192858681 * yv[1])
            //                   + (  9.8786215488 * yv[2]) + ( -9.9188753381 * yv[3])
            //                   + (  4.9796671950 * yv[4]);

            //    //Set the output value
            //    rtn[i] = yv[5];
            //}
        #endregion
    }
}
