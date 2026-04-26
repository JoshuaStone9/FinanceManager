using FM.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

// AllPayments.Actions.cs - Contains event handlers and actions for the AllPayments form

namespace FM
{
    public partial class AllPayments
    {
        private DataGridView? GetActiveGridWithSelection()
        {
            if (gridBills.SelectedRows.Count > 0) return gridBills;
            if (gridExpenses.SelectedRows.Count > 0) return gridExpenses;
            if (gridInvestments.SelectedRows.Count > 0) return gridInvestments;
            if (gridSavings.SelectedRows.Count > 0) return gridSavings;
            return null;
        }

        private string GetTableNameForGrid(DataGridView grid)
        {
            if (grid == gridBills) return "bills";
            if (grid == gridExpenses) return "extra_expenses";
            if (grid == gridInvestments) return "investments";
            if (grid == gridSavings) return "savings";
            return string.Empty;
        }

        private string GetPrimaryKeyColumnForGrid(DataGridView grid)
        {
            if (grid.Columns.Contains("billid")) return "billid";
            if (grid.Columns.Contains("extra_expense_id")) return "extra_expense_id";
            if (grid.Columns.Contains("investments_id")) return "investments_id";
            if (grid.Columns.Contains("savings_id")) return "savings_id";
            if (grid == gridBills) return "billid";
            if (grid == gridExpenses) return "extra_expense_id";
            if (grid == gridInvestments) return "investments_id";
            if (grid == gridSavings) return "savings_id";
            return string.Empty;
        }

        private void DeleteSelected()
        {
            var grid = GetActiveGridWithSelection();
            if (grid == null)
            {
                MessageBox.Show("Select a row in one of the grids first.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string table = GetTableNameForGrid(grid);
            string pk = GetPrimaryKeyColumnForGrid(grid);
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(pk))
            {
                MessageBox.Show("Could not determine table/primary key for the selected grid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ids = grid.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r =>
                {
                    var v = r.Cells[pk].Value;
                    if (v == null || v == DBNull.Value) return (int?)null;
                    try { return Convert.ToInt32(v); } catch { return (int?)null; }
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                MessageBox.Show("Selected row(s) do not contain primary key values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to permanently delete {ids.Count} record(s)?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                using var con = new SqlConnection(DatabaseHelper.BuildConnStr());
                con.Open();
                using var tx = con.BeginTransaction();
                using var cmd = con.CreateCommand();
                cmd.Transaction = tx;

                var paramNames = ids.Select((id, idx) => "@id" + idx).ToArray();
                cmd.CommandText = $"DELETE FROM dbo.[{table}] WHERE {pk} IN ({string.Join(", ", paramNames)})";

                for (int i = 0; i < ids.Count; i++)
                    cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

                int rows = cmd.ExecuteNonQuery();
                tx.Commit();

                MessageBox.Show(rows > 0 ? $"Deleted {rows} row(s)." : "No rows deleted.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditSelected()
        {
            var grid = GetActiveGridWithSelection();
            if (grid == null)
            {
                MessageBox.Show("Select a row in one of the grids first.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string table = GetTableNameForGrid(grid);
            string pk = GetPrimaryKeyColumnForGrid(grid);
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(pk))
            {
                MessageBox.Show("Could not determine table/primary key for the selected grid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var row = grid.SelectedRows[0];
            var idVal = row.Cells[pk].Value;
            if (idVal == null)
            {
                MessageBox.Show("Selected row does not contain a primary key value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fields = new[] { "name", "amount", "date", "category", "type", "length", "notes", "description" };
            var values = fields.Where(f => grid.Columns.Contains(f)).ToDictionary(f => f, f => row.Cells[f].Value);

            using var dlg = new EditRecordDialog(values);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var newValues = dlg.Values;
            if (newValues.Count == 0)
            {
                MessageBox.Show("No changes to save.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using var con = new SqlConnection(DatabaseHelper.BuildConnStr());
                con.Open();
                using var cmd = con.CreateCommand();

                var setClauses = new List<string>();
                foreach (var kv in newValues)
                {
                    string columnName = kv.Key;

                    if (kv.Key == "notes" && (table == "bills" || table == "extra_expenses"))
                    {
                        columnName = "description";
                    }

                    if (kv.Key == "date" && table == "extra_expenses")
                    {
                        columnName = "duedate";
                    }

                    setClauses.Add($"[{columnName}] = @{kv.Key}");
                    cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value ?? DBNull.Value);
                }

                cmd.CommandText = $"UPDATE dbo.[{table}] SET {string.Join(", ", setClauses)} WHERE {pk} = @id";
                cmd.Parameters.AddWithValue("@id", idVal);

                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "Updated." : "No rows updated.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditEmergencyFund_Click(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new emergency fund amount:",
                "Edit Emergency Fund",
                txtEmergencyFund.Text);

            if (string.IsNullOrWhiteSpace(input)) return;

            if (!decimal.TryParse(input, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.GetCultureInfo("en-GB"), out var newAmount))
            {
                MessageBox.Show("Invalid amount entered.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var con = new SqlConnection(DatabaseHelper.BuildConnStr());
                con.Open();

                using var cmd = con.CreateCommand();
                cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM dbo.emergency_fund)
    UPDATE dbo.emergency_fund SET amount = @amount, updated_at = GETDATE();
ELSE
    INSERT INTO dbo.emergency_fund (amount, updated_at) VALUES (@amount, GETDATE());";

                var p = cmd.Parameters.Add("@amount", System.Data.SqlDbType.Decimal);
                p.Precision = 12;
                p.Scale = 2;
                p.Value = newAmount;

                cmd.ExecuteNonQuery();

                txtEmergencyFund.Text = newAmount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                MessageBox.Show("Emergency fund updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update emergency fund: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditMonthlyAllowance_click(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new monthly allowance amount:",
                "Edit Monthly Allowance",
                txtMonthlyAllowance.Text);

            if (string.IsNullOrWhiteSpace(input)) return;

            if (!decimal.TryParse(input, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.GetCultureInfo("en-GB"), out var newAmount))
            {
                MessageBox.Show("Invalid amount entered.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int monthId = currentMonth;

            try
            {
                using var con = new SqlConnection(DatabaseHelper.BuildConnStr());
                con.Open();

                using var cmd = con.CreateCommand();
                cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM dbo.monthly_allowance WHERE month_id = @monthId)
    UPDATE dbo.monthly_allowance SET amount = @amount WHERE month_id = @monthId;
ELSE
    INSERT INTO dbo.monthly_allowance (month_id, amount) VALUES (@monthId, @amount);";

                var pAmt = cmd.Parameters.Add("@amount", System.Data.SqlDbType.Decimal);
                pAmt.Precision = 12;
                pAmt.Scale = 2;
                pAmt.Value = newAmount;

                cmd.Parameters.Add("@monthId", System.Data.SqlDbType.Int).Value = monthId;

                cmd.ExecuteNonQuery();

                txtMonthlyAllowance.Text = newAmount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                MessageBox.Show($"Monthly Allowance for {new DateTime(currentYear, currentMonth, 1):MMMM yyyy} updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update Monthly Allowance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}