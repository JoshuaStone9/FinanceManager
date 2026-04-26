
// AllPayments.Filters.cs - Partial class for AllPayments form, handling filtering by month and year

using FM.Helpers;
using Microsoft.Data.SqlClient;

namespace FM
{
    public partial class AllPayments
    {
        private void FilterByDate_click(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter month to filter (1-12):",
                "Filter by Month",
                DateTime.Today.Month.ToString());

            InsertCarriedOverDebt();
            InsertCarriedOverExcess();

            if (!int.TryParse(input, out int month) || month < 1 || month > 12)
            {
                MessageBox.Show("Please enter a valid month number (1-12).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string yearInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter year:",
                "Filter by Year",
                DateTime.Today.Year.ToString());

            if (!int.TryParse(yearInput, out int year) || year < 2000 || year > 2100)
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentMonth = month;
            currentYear = year;
            isFiltered = true;

            FilterGridByMonth(month, year);
        }

        private void ClearFilter_click(object? sender, EventArgs e)
        {
            currentMonth = DateTime.Today.Month;
            currentYear = DateTime.Today.Year;
            isFiltered = false;

            ReloadAll();
            MessageBox.Show("Filter cleared. Showing all records.", "Filter Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void FilterGridByMonth(int month, int year)
        {
            try
            {
                using var con = new SqlConnection(DatabaseHelper.BuildConnStr());
                con.Open();

                string billsQuery = @"SELECT billid, name, amount, [date], type, length, description 
             FROM dbo.bills 
             WHERE MONTH([date]) = @month AND YEAR([date]) = @year 
             ORDER BY [date] DESC";
                LoadFilteredData(gridBills, billsQuery, con, month, year, "billid");

                string expensesQuery = @"SELECT extra_expense_id, name, amount, duedate AS [date], category, type, length, description 
                FROM dbo.extra_expenses 
                WHERE MONTH(duedate) = @month AND YEAR(duedate) = @year 
                ORDER BY duedate DESC";
                LoadFilteredData(gridExpenses, expensesQuery, con, month, year, "extra_expense_id");

                string investmentsQuery = @"SELECT investments_id, name, amount, [date], category, length, notes 
           FROM dbo.investments 
           WHERE MONTH([date]) = @month AND YEAR([date]) = @year 
           ORDER BY [date] DESC";
                LoadFilteredData(gridInvestments, investmentsQuery, con, month, year, "investments_id");

                string savingsQuery = @"SELECT savings_id, name, amount, length, [date], notes 
       FROM dbo.savings 
       WHERE MONTH([date]) = @month AND YEAR([date]) = @year 
       ORDER BY [date] DESC";
                LoadFilteredData(gridSavings, savingsQuery, con, month, year, "savings_id");

                LoadEmergencyFund();
                EditMonthlyAllowance();

                UpdateTotal();
                RemainingFund();

                lblCurrentView.Text = $"Viewing: {new DateTime(year, month, 1):MMMM yyyy}";

                MessageBox.Show($"Filtered to show records from {month}/{year}", "Filter Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filter failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}