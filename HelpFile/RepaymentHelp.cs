using System;
using System.Collections.Generic;
using System.Text;

namespace FQLCalculator
{
    class RepaymentHelp
    {
        /// <summary>
        /// 等额本息
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="InteresRate"></param>
        /// <param name="Installments"></param>
        /// <returns></returns>
        public static List<double> EqualLoanPayment(double Amount, double InteresRate, int Installments)
        {
            double x = 1 + InteresRate;
            double y = 1;
            var l = new List<double>();
            for (int i = 0; i < Installments; i++)
            {
                y *= x;
            }
            var j = Amount * (InteresRate * y / (y - 1));
            for (int i = 0; i < Installments; i++)
            {
                l.Add(j);
            }
            return l;
        }

        /// <summary>
        /// 等额本金
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="InteresRate"></param>
        /// <param name="Installments"></param>
        /// <returns></returns>
        public static List<double> EqualPrincipalPayment(double Amount, double InteresRate, int Installments)
        {
            double x = Amount / Installments;
            double y = 1;
            var l = new List<double>();
            for (int i = 0; i < Installments; i++)
            {
                y = x + (Amount - x * i) * InteresRate;
                l.Add(y);
            }
            return l;
        }
    }
}
