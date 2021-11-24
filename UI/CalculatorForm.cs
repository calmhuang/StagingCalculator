using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FQLCalculator
{
    public partial class CalculatorForm : Form
    {
        public double _Repayment { get; set; } = 0;
        public int _Period { get; set; } = 0;
        private Loan _FirstLoan;
        private List<List<Loan>> Loans;
        private int CalHelp => cbCal.Checked ? 1 : 12;
        private double sum = 0;

        public CalculatorForm()
        {
            InitializeComponent();
        }

        private void linklbExplanation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@$"用于一段时期内，无偿还能力，需要以贷养贷的情况下，计算该时间段内所有分期本金+利息的产生情况。
使用介绍（无法偿还期间）：
1、设置时间限制：多长时间内以贷养贷；
2、设置借款额度；
3、设置年利率或月息；
使用介绍（偿还期间 + 无法偿还期间）：
1、设置时间限制：一段时期；
2、设置借款额度；
3、设置年利率或月息；
4、设置每月偿还额度。", "说明", MessageBoxButtons.OK);
        }

        private void CalculatorForm_Load(object sender, EventArgs e)
        {
            _FirstLoan = new Loan();
            numInterestRate.DataBindings.Add("Value", _FirstLoan, "InterestRate", true, DataSourceUpdateMode.OnPropertyChanged);
            numAmount.DataBindings.Add("Value", _FirstLoan, "Amount", true, DataSourceUpdateMode.OnPropertyChanged);
            numRepayment.DataBindings.Add("Value", this, "_Repayment", true, DataSourceUpdateMode.OnPropertyChanged);
            numPeriod.DataBindings.Add("Value", this, "_Period", true, DataSourceUpdateMode.OnPropertyChanged);
            numTimes.DataBindings.Add("Value", _FirstLoan, "SumTimes", true, DataSourceUpdateMode.OnPropertyChanged);
            cbMethod.DataBindings.Add("Checked", _FirstLoan, "CalMethod", true, DataSourceUpdateMode.OnPropertyChanged);
            cbCal.DataBindings.Add("Checked", _FirstLoan, "IsMonthlyInterest", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            _FirstLoan.Times = _FirstLoan.SumTimes;
            Loans = new List<List<Loan>>();
            Loans.Add(new List<Loan>());
            _FirstLoan.Repayment = _FirstLoan.CalMethod ? RepaymentHelp.EqualLoanPayment(_FirstLoan.Amount, _FirstLoan.interesRate, _FirstLoan.SumTimes) : 
                RepaymentHelp.EqualPrincipalPayment(_FirstLoan.Amount, _FirstLoan.interesRate, _FirstLoan.SumTimes);
            Loans[0].Add(_FirstLoan.DeepClone());
            sum = 0;
            if (_Repayment != 0)
            {
                Cal(_Period, Loans, _Repayment);
            }
            else
            {
                Cal(_Period, Loans);
            }
            dgvFormart.RowCount = Loans.Count + 2;
            dgvFormart.ColumnCount = 600;
            dgvFormart.Rows[0].Frozen = true;
            dgvFormart.Columns[0].Frozen = true;
            dgvFormart.Invalidate();
        }

        #region dgv显示
        private void dgvFormart_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                if (e.ColumnIndex == 0)
                    return;
                e.Value = $"第{e.ColumnIndex}笔账单";
                return;
            }
            if (e.ColumnIndex == 0)
            {
                if (e.RowIndex == dgvFormart.RowCount - 1)
                {
                    e.Value = "共计需还：";
                    return;
                }
                if (e.RowIndex == 1)
                    e.Value = "本金";
                else
                    e.Value = $"第{e.RowIndex - 1}个月";
                return;
            }

            if (e.RowIndex == dgvFormart.RowCount - 1 && e.ColumnIndex == 1)
            {
                e.Value = sum;
                return;
            }

            if (e.RowIndex <= dgvFormart.RowCount - 2 && e.ColumnIndex <= Loans[e.RowIndex - 1].Count)
                e.Value = Loans[e.RowIndex - 1][e.ColumnIndex - 1].Amount;


        }

        private void dgvFormart_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0|| e.ColumnIndex == 0)
            {
                e.CellStyle.BackColor = Color.FromArgb(230, 245, 255);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                return;
            }

        }
        #endregion

        private void Cal(int Period, List<List<Loan>> loans1)
        {
            int i = 0;
            while (Period - ++i >= 0)
            {
                ChangeMonth(loans1, i);
            }
            foreach (var l in Loans[Period])
            {
                sum += l.Amount * (1 + l.interesRate);
            }
        }

        private void Cal(int Period, List<List<Loan>> loans1, double Repayment)
        {

        }

        private void ChangeMonth(List<List<Loan>> loans, int Moth)
        {
            List<Loan> CalLoans = new List<Loan>();
            List<Loan> _loans = new List<Loan>();
            loans.Add(new List<Loan>());

            foreach (var l in loans[Moth - 1])
            {
                _loans.Add(l.DeepClone());
            }

            foreach (var loan in _loans)
            {
                if (loan.Amount > 1)
                {
                    var l = new Loan();
                    l.Amount = loan.Repayment[loan.SumTimes - loan.Times];
                    l.InterestRate = loan.InterestRate;
                    l.SumTimes = loan.SumTimes;
                    l.Times = loan.SumTimes;
                    l.IsMonthlyInterest = loan.IsMonthlyInterest;
                    l.CalMethod = loan.CalMethod;
                    if (l.Amount < 100)
                        l.Min = true;
                    l.Repayment = l.CalMethod ? RepaymentHelp.EqualLoanPayment(l.Amount, l.interesRate, l.SumTimes) :
                        RepaymentHelp.EqualPrincipalPayment(l.Amount, l.interesRate, l.SumTimes);
                    CalLoans.Add(l);

                    loan.Amount -= (loan.Amount / loan.Times);
                    loan.Times--;
                }
            }

            var sum = 0.0;
            var InterestRate = loans[0][0].InterestRate;
            var SumTimes = loans[0][0].SumTimes;
            var IsMonthlyInterest = loans[0][0].IsMonthlyInterest;
            var CalMethod = loans[0][0].CalMethod;

            foreach (var loan in CalLoans)
            {
                if (loan.Min)
                {
                    sum += loan.Amount;
                }
            }
            CalLoans.RemoveAll(a=>a.Min);

            if (sum != 0)
            {
                var l = new Loan();
                l.Amount = sum;
                l.InterestRate = InterestRate;
                l.SumTimes = SumTimes;
                l.Times = SumTimes;
                l.IsMonthlyInterest = IsMonthlyInterest;
                l.CalMethod = CalMethod;
                l.Repayment = l.CalMethod ? RepaymentHelp.EqualLoanPayment(l.Amount, l.interesRate, l.SumTimes) :
                            RepaymentHelp.EqualPrincipalPayment(l.Amount, l.interesRate, l.SumTimes);
                CalLoans.Add(l);
            }

            for (int i = 0; i < _loans.Count; i++)
            {
                loans[Moth].Add(_loans[i]);
            }

            for (int i = 0; i < CalLoans.Count; i++)
            {
                loans[Moth].Add(CalLoans[i]);
            }
        }
    }
}
