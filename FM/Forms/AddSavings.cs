using FM.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using FM.Helpers;

// AddSavings.cs - Form to add a new savings record

namespace FM
{
    public partial class AddSavings : Form
    {
        private Label lblTitle;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblLength;
        private ComboBox cboLength;

        private Label lblDate;
        private DateTimePicker dtpDate;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnSave;
        private Button btnViewSavings;
        private Button btnViewAllPayments;
        private Button btnMainMenu;

        private Panel bottomPanel;
        private PictureBox logo;

        public AddSavings()
        {
            Text = "Add Savings";
            ClientSize = new Size(560, 620);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += AddSavings_Paint;

            SuspendLayout();

            logo = new PictureBox
            {
                Image = Image.FromFile("Resources/images/FM_Logo_Main_Menu.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(120, 120),
                Location = new Point((ClientSize.Width - 120) / 2, 0),
                BackColor = Color.Transparent
            };
            Controls.Add(logo);

            lblTitle = new Label
            {
                Text = "Add Savings",
                Location = new Point((ClientSize.Width - 160) / 2, 120),
                AutoSize = true,
                Font = new Font("Montserrat", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            lblName = new Label { Text = "Name", Location = new Point(20, 160), AutoSize = true, BackColor = Color.Transparent, TabIndex = 0 };
            txtName = new TextBox { Location = new Point(160, 156), Width = 330, TabIndex = 1 };

            lblAmount = new Label { Text = "Amount", Location = new Point(20, 200), AutoSize = true, BackColor = Color.Transparent, TabIndex = 2 };
            lblPound = new Label { Text = "£", Location = new Point(160, 200), AutoSize = true, BackColor = Color.Transparent };
            txtAmount = new TextBox { Location = new Point(175, 196), Width = 120, TabIndex = 3 };

            lblLength = new Label { Text = "Length", Location = new Point(20, 240), AutoSize = true, BackColor = Color.Transparent, TabIndex = 4 };
            cboLength = new ComboBox
            {
                Location = new Point(160, 236),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 5
            };
            cboLength.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" });

            lblDate = new Label { Text = "Date", Location = new Point(20, 280), AutoSize = true, BackColor = Color.Transparent, TabIndex = 6 };
            dtpDate = new DateTimePicker
            {
                Location = new Point(160, 276),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                TabIndex = 7
            };

            lblNotes = new Label { Text = "Notes", Location = new Point(20, 320), AutoSize = true, BackColor = Color.Transparent, TabIndex = 8 };
            txtNotes = new TextBox
            {
                Location = new Point(160, 316),
                Width = 330,
                Height = 150,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 9
            };

            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Transparent
            };

            btnSave = MakePrimaryButton("Save", new Point(20, 20), 150, 40, 10, BtnSave_Click);
            btnViewSavings = MakeSecondaryButton("View Savings", new Point(180, 20), 170, 40, 11, BtnViewSavings_Click);
            btnMainMenu = MakeSecondaryButton("Main Menu", new Point(360, 20), 150, 40, 12, BtnMainMenu_Click);
            btnViewAllPayments = MakeSecondaryButton("View All Payments", new Point(180, 70), 200, 40, 13, BtnViewAllPayments_Click);

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewSavings, btnMainMenu, btnViewAllPayments });

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblLength, cboLength,
                lblDate, dtpDate,
                lblNotes, txtNotes,
                bottomPanel
            });

            try { EnsureSchemaAndSeedCategories(); } catch { }

            ResumeLayout(false);
        }

        private Button MakePrimaryButton(string text, Point location, int width, int height, int tabIndex, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Width = width,
                Height = height,
                TabIndex = tabIndex,
                BackColor = Color.FromArgb(255, 120, 120),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        private Button MakeSecondaryButton(string text, Point location, int width, int height, int tabIndex, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Width = width,
                Height = height,
                TabIndex = tabIndex,
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        private void AddSavings_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            ResetFieldBackColors();

            string? error = ValidateInputs(out decimal amount);
            if (error != null)
            {
                MessageBox.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rec = new SavingsRecord
            {
                Name = txtName.Text.Trim(),
                Amount = amount,
                Length = cboLength.SelectedItem?.ToString() ?? "N/A",
                Date = dtpDate.Value.Date,
                Notes = txtNotes.Text.Trim()
            };

            try
            {
                var dt = CreateInsertAndLoadSavings(rec);

                SavingsStore.Savings.Add(rec);

                MessageBox.Show("Savings saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearFormForNext();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save savings:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewSavings_Click(object? sender, EventArgs e)
        {
        }

        private void BtnMainMenu_Click(object? sender, EventArgs e) => Close();

        private void BtnViewAllPayments_Click(object? sender, EventArgs e) => new AllPayments().Show();

        private string? ValidateInputs(out decimal amount)
        {
            amount = 0m;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.BackColor = Color.MistyRose;
                return "Please enter a Name.";
            }

            if (!decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out amount) || amount <= 0)
            {
                txtAmount.BackColor = Color.MistyRose;
                return "Please enter a valid Amount greater than 0.";
            }

            if (string.IsNullOrWhiteSpace(cboLength.Text))
            {
                cboLength.BackColor = Color.MistyRose;
                return "Please input a length of time.";
            }

            if (dtpDate.Value.Date > DateTime.Today)
            {
                dtpDate.CalendarMonthBackground = Color.MistyRose;
                return "Date cannot be in the future.";
            }

            return null;
        }

        private void ResetFieldBackColors()
        {
            txtName.BackColor = SystemColors.Window;
            txtAmount.BackColor = SystemColors.Window;
            cboLength.BackColor = SystemColors.Window;
            dtpDate.CalendarMonthBackground = SystemColors.Window;
        }

        private void ClearFormForNext()
        {
            txtName.Clear();
            txtAmount.Clear();
            cboLength.SelectedIndex = -1;
            dtpDate.Value = DateTime.Today;
            txtNotes.Clear();
            txtName.Focus();
        }

        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new SqlConnection(DatabaseHelper.BuildConnStr());
            conn.Open();

            using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[savings]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[savings](
        savings_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(200) NOT NULL,
        amount DECIMAL(12,2) NOT NULL,
        [length] NVARCHAR(50) NULL,
        [date] DATE NULL,
        notes NVARCHAR(MAX) NULL
    );
END;", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private DataTable CreateInsertAndLoadSavings(SavingsRecord rec)
        {
            using var conn = new SqlConnection(DatabaseHelper.BuildConnStr());
            conn.Open();

            using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[savings]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[savings](
        savings_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(200) NOT NULL,
        amount DECIMAL(12,2) NOT NULL,
        [length] NVARCHAR(50) NULL,
        [date] DATE NULL,
        notes NVARCHAR(MAX) NULL
    );
END;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqlCommand(@"
IF COL_LENGTH('dbo.savings','length') IS NULL
    ALTER TABLE dbo.savings ADD [length] NVARCHAR(50);
IF COL_LENGTH('dbo.savings','date') IS NULL
    ALTER TABLE dbo.savings ADD [date] DATE;
IF COL_LENGTH('dbo.savings','notes') IS NULL
    ALTER TABLE dbo.savings ADD notes NVARCHAR(MAX);", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.savings (name, amount, [length], [date], notes)
VALUES (@n, @a, @l, @d, @not);", conn))
            {
                cmd.Parameters.Add("@n", SqlDbType.NVarChar, 200).Value = rec.Name ?? (object)DBNull.Value;

                var pAmt = cmd.Parameters.Add("@a", SqlDbType.Decimal);
                pAmt.Precision = 12; pAmt.Scale = 2;
                pAmt.Value = rec.Amount;

                cmd.Parameters.Add("@l", SqlDbType.NVarChar, 50).Value = string.IsNullOrWhiteSpace(rec.Length) ? (object)DBNull.Value : rec.Length;
                cmd.Parameters.Add("@d", SqlDbType.Date).Value = rec.Date;
                cmd.Parameters.Add("@not", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(rec.Notes) ? (object)DBNull.Value : rec.Notes;

                cmd.ExecuteNonQuery();
            }

            var dt = new DataTable();
            using (var cmd = new SqlCommand("SELECT savings_id, name, amount, [date] FROM dbo.savings ORDER BY [date] DESC;", conn))
            using (var rdr = cmd.ExecuteReader())
                dt.Load(rdr);

            return dt;
        }
    }
}
