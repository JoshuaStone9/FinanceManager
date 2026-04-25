using Microsoft.Data.SqlClient;
using System.Data;

// AllPayments.Data.cs - Data access and loading logic for the AllPayments form

namespace FM
{
    public partial class AllPayments
    {

        private void EmergencyFundData()
        {
            LoadEmergencyFund();
        }

        private void LoadEmergencyFund()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT TOP 1 amount FROM dbo.emergency_fund ORDER BY updated_at DESC";
                using var cmd = new SqlCommand(query, con);

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    decimal amount = Convert.ToDecimal(result);
                    txtEmergencyFund.Text = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                }
                else
                {
                    txtEmergencyFund.Text = (0m).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Emergency fund error: " + ex.Message);
            }
        }

        private void EditMonthlyAllowance()
        {
            try
            {
                int monthId = currentMonth;

                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                using var cmd = new SqlCommand(
                    "SELECT TOP 1 amount FROM dbo.monthly_allowance WHERE month_id = @monthId",
                    con);

                cmd.Parameters.Add("@monthId", System.Data.SqlDbType.Int).Value = monthId;

                var result = cmd.ExecuteScalar();
                var amount = (result == null || result == DBNull.Value) ? 0m : Convert.ToDecimal(result);

                txtMonthlyAllowance.Text = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Monthly Allowance error: " + ex.Message);
            }
        }

        private void EditMoneyLeftOver()
        {
            try
            {
                int monthId = currentMonth;

                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                using var cmd = new SqlCommand(
                    "SELECT TOP 1 amount FROM dbo.money_left_over WHERE month_id = @monthId",
                    con);

                cmd.Parameters.Add("@monthId", System.Data.SqlDbType.Int).Value = monthId;

                var result = cmd.ExecuteScalar();
                var amount = (result == null || result == DBNull.Value) ? 0m : Convert.ToDecimal(result);

                txtMonthlyAllowance.Text = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Money Left Over error: " + ex.Message);
            }
        }

        private void BillsDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT billid, name, amount, [date], type, length, description FROM dbo.bills ORDER BY [date] DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridBills.DataSource = dt;

                if (gridBills.Columns.Contains("billid"))
                    gridBills.Columns["billid"].Visible = false;
                FormatColumnHeaders(gridBills);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bills error: " + ex.Message);
            }
        }

        private void InvestmentsDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT investments_id, name, amount, [date], category, length, notes FROM dbo.investments ORDER BY [date] DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridInvestments.DataSource = dt;

                if (gridInvestments.Columns.Contains("investments_id"))
                    gridInvestments.Columns["investments_id"].Visible = false;
                FormatColumnHeaders(gridInvestments);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Investments error: " + ex.Message);
            }
        }

        private void ExtraExpenseDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT extra_expense_id, name, amount, duedate AS [date], category, type, length, description FROM dbo.extra_expenses ORDER BY duedate DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridExpenses.DataSource = dt;

                if (gridExpenses.Columns.Contains("extra_expense_id"))
                    gridExpenses.Columns["extra_expense_id"].Visible = false;
                FormatColumnHeaders(gridExpenses);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Extra expenses error: " + ex.Message);
            }
        }

        private void SavingsDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT savings_id, name, amount, length, [date], notes FROM dbo.savings ORDER BY [date] DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridSavings.DataSource = dt;

                if (gridSavings.Columns.Contains("savings_id"))
                    gridSavings.Columns["savings_id"].Visible = false;
                FormatColumnHeaders(gridSavings);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Savings error: " + ex.Message);
            }
        }

        private void LoadFilteredData(DataGridView grid, string query, SqlConnection con, int month, int year, string idColumn)
        {
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@year", year);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            grid.DataSource = dt;

            if (grid.Columns.Contains(idColumn))
                grid.Columns[idColumn].Visible = false;

            FormatColumnHeaders(grid);
        }

        private void AddPreviousBills()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                var displayedDate = new DateTime(currentYear, currentMonth, 1);
                var previousMonth = displayedDate.AddMonths(-1);

                string selectQuery = @"SELECT name, amount, type, length, description 
                              FROM dbo.bills 
                              WHERE MONTH([date]) = @prevMonth AND YEAR([date]) = @prevYear";

                using var selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@prevMonth", previousMonth.Month);
                selectCmd.Parameters.AddWithValue("@prevYear", previousMonth.Year);

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(selectCmd))
                {
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"No bills found from {previousMonth:MMMM yyyy} to copy.",
                          "No Bills Found",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
                    return;
                }

                string insertQuery = @"INSERT INTO dbo.bills (name, amount, [date], type, length, description) 
                              VALUES (@name, @amount, @date, @type, @length, @description)";

                int insertedCount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    using var insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@name", row["name"]);
                    insertCmd.Parameters.AddWithValue("@amount", row["amount"]);
                    insertCmd.Parameters.AddWithValue("@date", displayedDate);
                    insertCmd.Parameters.AddWithValue("@type", row["type"] ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@length", row["length"] ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@description", row["description"] ?? DBNull.Value);

                    insertedCount += insertCmd.ExecuteNonQuery();
                }

                MessageBox.Show($"Successfully copied {insertedCount} bill(s) from {previousMonth:MMMM yyyy} to {displayedDate:MMMM yyyy}.",
                       "Bills Added",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Information);

                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Previous bills error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddPreviousInvestments()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                var displayedDate = new DateTime(currentYear, currentMonth, 1);
                var previousMonth = displayedDate.AddMonths(-1);

                string selectQuery = @"SELECT investments_id, name, amount, date, category_id, category, length, notes 
                              FROM dbo.investments 
                              WHERE MONTH([date]) = @prevMonth AND YEAR([date]) = @prevYear";

                using var selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@prevMonth", previousMonth.Month);
                selectCmd.Parameters.AddWithValue("@prevYear", previousMonth.Year);

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(selectCmd))
                {
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"No bills found from {previousMonth:MMMM yyyy} to copy.",
                          "No Investments Found",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
                    return;
                }

                string insertQuery = @"INSERT INTO dbo.investments (name, amount, date, category_id, category, length, notes) 
                              VALUES (@name, @amount, @date, @category_id, @category, @length, @notes)";

                int insertedCount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    using var insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@name", row["name"]);
                    insertCmd.Parameters.AddWithValue("@amount", row["amount"]);
                    insertCmd.Parameters.AddWithValue("@date", displayedDate);
                    insertCmd.Parameters.AddWithValue("@category_id", row.Table.Columns.Contains("category_id") ? row["category_id"] ?? DBNull.Value : DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@category", row["category"] ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@length", row["length"] ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@notes", row["notes"] ?? DBNull.Value);

                    insertedCount += insertCmd.ExecuteNonQuery();
                }

                MessageBox.Show($"Successfully copied {insertedCount} investments from {previousMonth:MMMM yyyy} to {displayedDate:MMMM yyyy}.",
                       "Investments Added",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Information);

                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Previous investments error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertCarriedOverDebt()
        {
            if (RemainingFundValue >= 1200)
                return;

            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                var displayedDate = new DateTime(currentYear, currentMonth, 1);
                var nextMonth = displayedDate.AddMonths(1);

                const string checkQuery = @"SELECT COUNT(1) FROM dbo.extra_expenses WHERE name = @name AND YEAR(duedate) = @year AND MONTH(duedate) = @month";

                using var checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@name", "Carried Over Debt");
                checkCmd.Parameters.AddWithValue("@year", nextMonth.Year);
                checkCmd.Parameters.AddWithValue("@month", nextMonth.Month);

                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    const string updateQuery = @"UPDATE dbo.extra_expenses SET amount = @amount, description = @description WHERE name = @name AND YEAR(duedate) = @year AND MONTH(duedate) = @month";

                    using var updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@amount", CarriedOverDebtValue);
                    updateCmd.Parameters.AddWithValue("@description", $"Auto-created shortfall from {displayedDate:MMMM yyyy}");
                    updateCmd.Parameters.AddWithValue("@name", "Carried Over Debt");
                    updateCmd.Parameters.AddWithValue("@year", nextMonth.Year);
                    updateCmd.Parameters.AddWithValue("@month", nextMonth.Month);

                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    const string insertQuery = @"INSERT INTO dbo.extra_expenses (name, amount, category_id, category, length, duedate, description) VALUES(@name, @amount, @category_id, @category, @length, @duedate, @description)";

                    using var cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@name", "Carried Over Debt");
                    cmd.Parameters.AddWithValue("@amount", CarriedOverDebtValue);
                    cmd.Parameters.AddWithValue("@category_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("@category", "Debt");
                    cmd.Parameters.AddWithValue("@length", DBNull.Value);
                    cmd.Parameters.AddWithValue("@duedate", nextMonth);
                    cmd.Parameters.AddWithValue("@description", $"Auto-created shortfall from {displayedDate:MMMM yyyy}");

                    cmd.ExecuteNonQuery();
                }

                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error inserting carried over debt: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InsertCarriedOverExcess()
        {
            if (RemainingFundValue <= 1200)
                return;

            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                var displayedDate = new DateTime(currentYear, currentMonth, 1);
                var nextMonth = displayedDate.AddMonths(1);

                const string carriedOverName = "Carried Over Excess";

                const string checkQuery = @"
                    SELECT COUNT(1) 
                    FROM dbo.savings 
                    WHERE name = @name 
                    AND YEAR(duedate) = @year 
                    AND MONTH(duedate) = @month";

                using var checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@name", carriedOverName);
                checkCmd.Parameters.AddWithValue("@year", nextMonth.Year);
                checkCmd.Parameters.AddWithValue("@month", nextMonth.Month);

                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    const string updateQuery = @"
                UPDATE dbo.savings 
                SET amount = @amount, 
                    description = @description 
                WHERE name = @name 
                AND YEAR(duedate) = @year 
                AND MONTH(duedate) = @month";

                    using var updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@amount", CarriedOverExcessValue);
                    updateCmd.Parameters.AddWithValue("@description", $"Auto-created excess from {displayedDate:MMMM yyyy}");
                    updateCmd.Parameters.AddWithValue("@name", carriedOverName);
                    updateCmd.Parameters.AddWithValue("@year", nextMonth.Year);
                    updateCmd.Parameters.AddWithValue("@month", nextMonth.Month);

                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    const string insertQuery = @"
                INSERT INTO dbo.savings 
                (name, amount, category_id, category, length, duedate, description) 
                VALUES 
                (@name, @amount, @category_id, @category, @length, @duedate, @description)";

                    using var cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@name", carriedOverName);
                    cmd.Parameters.AddWithValue("@amount", CarriedOverExcessValue);
                    cmd.Parameters.AddWithValue("@category_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("@category", "Savings");
                    cmd.Parameters.AddWithValue("@length", DBNull.Value);
                    cmd.Parameters.AddWithValue("@duedate", nextMonth);
                    cmd.Parameters.AddWithValue("@description", $"Auto-created excess from {displayedDate:MMMM yyyy}");

                    cmd.ExecuteNonQuery();
                }

                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error inserting carried over excess: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}