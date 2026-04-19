using System.Data;
using System.Globalization;

// AllPayments.Totals.cs - Methods to calculate and update total amounts in AllPayments form

namespace FM
{
    public partial class AllPayments
    {
        private void UpdateTotal()
        {
            decimal total = 0m;

            total += SumAmountColumn(gridBills);
            total += SumAmountColumn(gridExpenses);
            total += SumAmountColumn(gridInvestments);

            if (txtGrandTotal != null)
                txtGrandTotal.Text = total.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
        }

        private decimal SumAmountColumn(DataGridView grid)
        {
            if (grid.DataSource is not DataTable dt || !dt.Columns.Contains("amount"))
                return 0m;

            decimal sum = 0m;
            foreach (DataRow r in dt.Rows)
            {
                if (r.IsNull("amount")) continue;
                if (r["amount"] is decimal dec) sum += dec;
                else if (decimal.TryParse(Convert.ToString(r["amount"]), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    sum += val;
                else if (decimal.TryParse(Convert.ToString(r["amount"]), System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out var val2))
                    sum += val2;
            }
            return sum;
        }

        private void RemainingFund()
        {
            if (decimal.TryParse(txtMonthlyAllowance.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var allowance) &&
                decimal.TryParse(txtGrandTotal.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var total))
            {
                decimal remaining = allowance - total;
                remaining += SumAmountColumn(gridSavings);
                txtRemainingFund.Text = remaining.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
            }
            else
            {
                txtRemainingFund.Text = "N/A";
            }
        }
    }
}