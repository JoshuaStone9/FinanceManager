using System.Globalization;

// AllPayments.Helpers.cs - Helper methods for formatting and editing records in AllPayments form

namespace FM
{
    public partial class AllPayments
    {
        private void FormatColumnHeaders(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (!col.Visible) continue;
                col.HeaderText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(col.HeaderText.ToLower());
            }
        }

        private void FormatGrid(DataGridView grid)
        {
            if (grid.Columns.Contains("amount"))
            {
                grid.Columns["amount"].DefaultCellStyle.Format = "C";
                grid.Columns["amount"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
                grid.Columns["amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (grid.Columns.Contains("date"))
            {
                grid.Columns["date"].DefaultCellStyle.Format = "d";
                grid.Columns["date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
    }

    // EditRecordDialog left as a separate class for reuse
    public class EditRecordDialog : Form
    {
        private readonly System.Collections.Generic.Dictionary<string, object?> initial;
        public readonly System.Collections.Generic.Dictionary<string, object?> Values = new();

        private readonly TextBox txtName = new() { Width = 300 };
        private readonly TextBox txtAmount = new() { Width = 120 };
        private readonly DateTimePicker dtDate = new() { Format = DateTimePickerFormat.Short };
        private readonly TextBox txtCategory = new() { Width = 200 };
        private readonly TextBox txtType = new() { Width = 120 };
        private readonly TextBox txtLength = new() { Width = 120 };
        private readonly TextBox txtNotes = new() { Width = 300, Multiline = true, Height = 80 };

        public EditRecordDialog(System.Collections.Generic.Dictionary<string, object?> values)
        {
            initial = values;
            Text = "Edit Record";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(420, 380);
            MaximizeBox = false;
            MinimizeBox = false;

            var y = 12;
            void AddLabel(string text) => Controls.Add(new Label { Text = text, Location = new Point(12, y), AutoSize = true });
            void IncY(int delta = 28) => y += delta;

            if (values.ContainsKey("name"))
            {
                AddLabel("Name");
                txtName.Location = new Point(12, y + 18);
                txtName.Text = Convert.ToString(values["name"]) ?? string.Empty;
                Controls.Add(txtName);
                IncY(60);
            }

            if (values.ContainsKey("amount"))
            {
                AddLabel("Amount");
                txtAmount.Location = new Point(12, y + 18);
                txtAmount.Text = Convert.ToString(values["amount"], CultureInfo.InvariantCulture) ?? string.Empty;
                Controls.Add(txtAmount);
                IncY(40);
            }

            if (values.ContainsKey("date"))
            {
                AddLabel("Date");
                dtDate.Location = new Point(12, y + 18);
                if (values["date"] is DateTime dt) dtDate.Value = dt.Date;
                Controls.Add(dtDate);
                IncY(40);
            }

            if (values.ContainsKey("category"))
            {
                AddLabel("Category");
                txtCategory.Location = new Point(12, y + 18);
                txtCategory.Text = Convert.ToString(values["category"]) ?? string.Empty;
                Controls.Add(txtCategory);
                IncY(40);
            }

            if (values.ContainsKey("type"))
            {
                AddLabel("Type");
                txtType.Location = new Point(12, y + 18);
                txtType.Text = Convert.ToString(values["type"]) ?? string.Empty;
                Controls.Add(txtType);
                IncY(40);
            }

            if (values.ContainsKey("length"))
            {
                AddLabel("Length");
                txtLength.Location = new Point(12, y + 18);
                txtLength.Text = Convert.ToString(values["length"]) ?? string.Empty;
                Controls.Add(txtLength);
                IncY(40);
            }

            if (values.ContainsKey("notes") || values.ContainsKey("description"))
            {
                AddLabel("Notes");
                txtNotes.Location = new Point(12, y + 18);
                var noteVal = values.ContainsKey("notes") ? values["notes"] : values.GetValueOrDefault("description");
                txtNotes.Text = Convert.ToString(noteVal) ?? string.Empty;
                Controls.Add(txtNotes);
                IncY(100);
            }

            var btnSave = new Button { Text = "Save", Location = new Point(240, ClientSize.Height - 44), Size = new Size(80, 30) };
            var btnCancel = new Button { Text = "Cancel", Location = new Point(330, ClientSize.Height - 44), Size = new Size(80, 30) };
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (Controls.Contains(txtAmount) && !string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out _)
                    && !decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                {
                    MessageBox.Show("Amount must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (Controls.Contains(txtName)) Values["name"] = txtName.Text.Trim();
            if (Controls.Contains(txtAmount)) Values["amount"] = string.IsNullOrWhiteSpace(txtAmount.Text) ? (object?)DBNull.Value : (object)decimal.Parse(txtAmount.Text, CultureInfo.CurrentCulture);
            if (Controls.Contains(dtDate)) Values["date"] = dtDate.Value.Date;
            if (Controls.Contains(txtCategory)) Values["category"] = string.IsNullOrWhiteSpace(txtCategory.Text) ? (object?)DBNull.Value : txtCategory.Text.Trim();
            if (Controls.Contains(txtType)) Values["type"] = string.IsNullOrWhiteSpace(txtType.Text) ? (object?)DBNull.Value : txtType.Text.Trim();
            if (Controls.Contains(txtLength)) Values["length"] = string.IsNullOrWhiteSpace(txtLength.Text) ? (object?)DBNull.Value : txtLength.Text.Trim();
            if (Controls.Contains(txtNotes)) Values["notes"] = string.IsNullOrWhiteSpace(txtNotes.Text) ? (object?)DBNull.Value : txtNotes.Text.Trim();

            DialogResult = DialogResult.OK;
        }
    }
}