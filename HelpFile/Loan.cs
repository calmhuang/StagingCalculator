using System;
using System.Collections.Generic;
using System.Text;

namespace FQLCalculator
{
    class Loan
    {
        //用于计算
        public double interesRate
        {
            get
            { return InterestRate / 100 / (IsMonthlyInterest ? 1 : 12); }

        }
        //金额
        public double Amount { get; set; } = 0;
        /// <summary>
        /// 利率
        /// 用于显示
        /// </summary>
        public double InterestRate
        {
            get;
            set;
        }
        //总分期数
        public int SumTimes { get; set; } = 0;
        //剩余分期数
        public int Times { get; set; } = 0;
        /// <summary>
        /// 是否为月息
        /// </summary>
        public bool IsMonthlyInterest { get; set; } = true;

        public List<double> Repayment { get; set; }

        public bool CalMethod { get; set; } = false;

        public bool Min { get; set; } = false;

        public Loan DeepClone()
        {
            var ret = new Loan();
            ret.SumTimes = SumTimes;
            ret.InterestRate = InterestRate;
            ret.Repayment = Repayment;
            ret.IsMonthlyInterest = IsMonthlyInterest;
            ret.Amount = Amount;
            ret.CalMethod = CalMethod;
            ret.InterestRate = InterestRate;
            ret.Times = Times;
            return ret;
        }
    }
}
