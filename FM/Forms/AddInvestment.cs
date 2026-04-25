using FM.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;

// AddInvestment.cs - Form to add a new investment record

namespace FM
{
    public partial class AddInvestment : Form
    {
        private Label lblTitle;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblCategory;
        private ComboBox cbCategory;

        private Label lblLength;
        private ComboBox cbLength;

        private Label lblDate;
        private DateTimePicker dtDate;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnSave;
        private Button btnViewBills;
        private Button btnViewAllPayments;
        private Button btnMainMenu;
        private Button btnCalculateInvestments;

        private Panel bottomPanel;
        private PictureBox logo;

        public AddInvestment()
        {
            Text = "Add Investment";
            ClientSize = new Size(560, 620);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += AddInvestment_Paint;

            this.Load += AddInvestment_Load;

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
                Text = "Add Investment",
                Location = new Point((ClientSize.Width - 190) / 2, 120),
                AutoSize = true,
                Font = new Font("Montserrat", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            lblName = new Label { Text = "Name", Location = new Point(20, 160), AutoSize = true, BackColor = Color.Transparent, TabIndex = 0 };
            txtName = new TextBox { Location = new Point(160, 156), Width = 330, TabIndex = 1 };

            lblAmount = new Label { Text = "Amount", Location = new Point(20, 200), AutoSize = true, BackColor = Color.Transparent, TabIndex = 2 };
            lblPound = new Label { Text = "£", Location = new Point(160, 200), AutoSize = true, BackColor = Color.Transparent };
            txtAmount = new TextBox { Location = new Point(175, 196), Width = 120, TabIndex = 3 };

            lblCategory = new Label { Text = "Category", Location = new Point(20, 240), AutoSize = true, BackColor = Color.Transparent, TabIndex = 4 };
            cbCategory = new ComboBox
            {
                Location = new Point(160, 236),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 5
            };

            lblLength = new Label { Text = "Length", Location = new Point(20, 280), AutoSize = true, BackColor = Color.Transparent, TabIndex = 6 };
            cbLength = new ComboBox
            {
                Location = new Point(160, 276),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 7
            };
            cbLength.Items.AddRange(new object[] { "One-time", "Monthly", "Quarterly", "Yearly" });

            lblDate = new Label { Text = "Date", Location = new Point(20, 320), AutoSize = true, BackColor = Color.Transparent, TabIndex = 8 };
            dtDate = new DateTimePicker
            {
                Location = new Point(160, 316),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                TabIndex = 9
            };

            lblNotes = new Label { Text = "Notes", Location = new Point(20, 360), AutoSize = true, BackColor = Color.Transparent, TabIndex = 10 };
            txtNotes = new TextBox
            {
                Location = new Point(160, 356),
                Width = 330,
                Height = 150,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 11
            };

            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Transparent
            };

            btnSave = MakePrimaryButton("Save", new Point(20, 20), 150, 40, 12, btnSave_Click);
            btnViewBills = MakeSecondaryButton("View Investments", new Point(180, 20), 170, 40, 13, btnViewBills_Click);
            btnMainMenu = MakeSecondaryButton("Main Menu", new Point(360, 20), 150, 40, 14, BtnMainMenu_Click);
            btnViewAllPayments = MakeSecondaryButton("View All Payments", new Point(80, 70), 200, 40, 15, btnViewAllPayments_Click);
            btnCalculateInvestments = MakeSecondaryButton("Calculate Investments", new Point(290, 70), 200, 40, 16, btnCalculateInvestments_Click);

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewBills, btnMainMenu, btnViewAllPayments, btnCalculateInvestments });

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblCategory, cbCategory,
                lblLength, cbLength,
                lblDate, dtDate,
                lblNotes, txtNotes,
                bottomPanel
            });

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

        private void AddInvestment_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private void AddInvestment_Load(object? sender, EventArgs e)
        {
            try
            {
                EnsureSchemaAndSeedCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load categories: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private sealed class CategoryItem
        {
            public int Id { get; }
            public string Name { get; }
            public CategoryItem(int id, string name) { Id = id; Name = name; }
            public override string ToString() => Name;
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cbCategory.DataSource == null || cbCategory.Items.Count == 0)
                    EnsureSchemaAndSeedCategories();

                if (cbCategory.SelectedIndex < 0 && cbCategory.Items.Count > 0)
                    cbCategory.SelectedIndex = 0;

                var item = cbCategory.SelectedItem as CategoryItem;
                int investment_category_id = item?.Id ?? 0;
                string investment_category = item?.Name ?? string.Empty;

                var rec = new InvestmentRecord
                {
                    Investment_ID = "",
                    Name = txtName.Text.Trim(),
                    Amount = txtAmount.Text.Trim(),
                    Date = dtDate.Value.Date,
                    Category = investment_category,
                    Length = cbLength.SelectedItem?.ToString() ?? cbLength.Text ?? "One-time",
                    Description = txtNotes.Text?.Trim() ?? ""
                };

                if (string.IsNullOrWhiteSpace(rec.Name))
                    throw new InvalidOperationException("Please enter a Name.");

                if (string.IsNullOrWhiteSpace(rec.Amount))
                    throw new InvalidOperationException("Please enter an Amount.");

                if (investment_category_id <= 0)
                    throw new InvalidOperationException("Selected category is invalid.");

                InsertInvestmentsToDb(rec, investment_category_id, investment_category);

                InvestmentStore.Investments.Add(rec);

                MessageBox.Show("Investment saved successfully.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving investment: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnViewBills_Click(object? sender, EventArgs e)
        {
            var investements = new InvestmentsRecord();
            investements.Show();
        }

        private void BtnMainMenu_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewAllPayments_Click(object? sender, EventArgs e)
        {
            var allPayments = new AllPayments();
            allPayments.Show();
        }

        private void btnCalculateInvestments_Click(object? sender, EventArgs e)
        {
            var calculate = new CalculateInvestments();
            calculate.Show();
        }

        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new SqlConnection(DatabaseHelper.BuildConnStr());
            conn.Open();

            using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[investment_categories]') AND type = 'U')
BEGIN
    CREATE TABLE dbo.investment_categories(
        id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(200) NOT NULL UNIQUE
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[investments]') AND type = 'U')
BEGIN
    CREATE TABLE dbo.investments(
        investments_id INT IDENTITY(1,1) PRIMARY KEY,
        name           NVARCHAR(200) NOT NULL,
        amount         DECIMAL(12,2) NOT NULL
    );
END;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqlCommand(@"
IF COL_LENGTH('dbo.investments','date') IS NULL
    ALTER TABLE dbo.investments ADD [date] DATE;
IF COL_LENGTH('dbo.investments','category_id') IS NULL
    ALTER TABLE dbo.investments ADD category_id INT;
IF COL_LENGTH('dbo.investments','category') IS NULL
    ALTER TABLE dbo.investments ADD category NVARCHAR(200);
IF COL_LENGTH('dbo.investments','length') IS NULL
    ALTER TABLE dbo.investments ADD [length] NVARCHAR(100);
IF COL_LENGTH('dbo.investments','notes') IS NULL
    ALTER TABLE dbo.investments ADD notes NVARCHAR(MAX);
", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_investments_category')
BEGIN
    ALTER TABLE dbo.investments
    ADD CONSTRAINT FK_investments_category FOREIGN KEY (category_id) REFERENCES dbo.investment_categories(id);
END;", conn))
            {
                try { cmd.ExecuteNonQuery(); } catch { }
            }

            string[] inv = { "Stocks", "ETF", "Crypto", "Real Estate" };
            using (var tx = conn.BeginTransaction())
            {
                foreach (var c in inv)
                {
                    using var upsert = new SqlCommand(
                        "IF NOT EXISTS (SELECT 1 FROM dbo.investment_categories WHERE name = @name) INSERT INTO dbo.investment_categories(name) VALUES(@name);",
                        conn, tx);
                    upsert.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = c;
                    upsert.ExecuteNonQuery();
                }
                tx.Commit();
            }

            using (var cmd = new SqlCommand(@"
UPDATE i
SET category_id = ic.id
FROM dbo.investments i
JOIN dbo.investment_categories ic ON LOWER(i.category) = LOWER(ic.name)
WHERE i.category_id IS NULL AND i.category IS NOT NULL AND i.category <> '';

UPDATE i
SET category = ic.name
FROM dbo.investments i
JOIN dbo.investment_categories ic ON i.category_id = ic.id
WHERE (i.category IS NULL OR i.category = '');", conn))
            {
                cmd.ExecuteNonQuery();
            }

            var list = new List<CategoryItem>();
            using (var cmd = new SqlCommand("SELECT id, name FROM dbo.investment_categories ORDER BY name;", conn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    list.Add(new CategoryItem(rdr.GetInt32(0), rdr.GetString(1)));
            }

            cbCategory.DataSource = null;
            cbCategory.DisplayMember = nameof(CategoryItem.Name);
            cbCategory.ValueMember = nameof(CategoryItem.Id);
            cbCategory.DataSource = list;

            if (cbCategory.Items.Count > 0 && cbCategory.SelectedIndex < 0)
                cbCategory.SelectedIndex = 0;
        }

        private void InsertInvestmentsToDb(InvestmentRecord rec, int categoryId, string categoryName)
        {
            using var conn = new SqlConnection(DatabaseHelper.BuildConnStr());
            conn.Open();

            if (!decimal.TryParse(rec.Amount, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) &&
                !decimal.TryParse(rec.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
            {
                throw new FormatException("Amount must be a number, e.g. 1234.56");
            }

            using var cmd = new SqlCommand(@"
INSERT INTO dbo.investments
    (name, amount, [date], category_id, category, [length], notes)
OUTPUT INSERTED.investments_id
VALUES
    (@n, @a, @d, @cat_id, @cat, @l, @notes);", conn);

            cmd.Parameters.Add("@n", SqlDbType.NVarChar, 200).Value = (object?)rec.Name?.Trim() ?? DBNull.Value;

            var pAmt = cmd.Parameters.Add("@a", SqlDbType.Decimal);
            pAmt.Precision = 12; pAmt.Scale = 2;
            pAmt.Value = amount;

            cmd.Parameters.Add("@d", SqlDbType.Date).Value = rec.Date.Date;
            cmd.Parameters.Add("@cat_id", SqlDbType.Int).Value = categoryId;
            cmd.Parameters.Add("@cat", SqlDbType.NVarChar, 200).Value = categoryName ?? (object)DBNull.Value;
            cmd.Parameters.Add("@l", SqlDbType.NVarChar, 100).Value = rec.Length ?? (object)DBNull.Value;
            cmd.Parameters.Add("@notes", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(rec.Description) ? (object)DBNull.Value : rec.Description;

            var newIdObj = cmd.ExecuteScalar();
            if (newIdObj != null && newIdObj != DBNull.Value)
                rec.Investment_ID = Convert.ToString(newIdObj, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }

    public class InvestmentRecord
    {
        public string Investment_ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Amount { get; set; } = "";
        public DateTime Date { get; set; }
        public string Category { get; set; } = "";
        public string Length { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
