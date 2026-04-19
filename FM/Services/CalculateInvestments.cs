using FM;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

// CalculateInvestments.cs - Form to calculate monthly totals of investments and list them

namespace FM
{
    public partial class CalculateInvestments : Form
    {
        private static readonly string ConnStr = BuildConnStr();

        private static string BuildConnStr()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "STONEYMINI",
                InitialCatalog = "Finance_Manager",
                IntegratedSecurity = true,
                Encrypt = true,
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }

        // UI
        private DataGridView dgvInvestments;
        private Button btnRetrieveInvestments;
        private TextBox txtTotal;

        private Label lblJanuary; private TextBox txtJanuary;
        private Label lblFebruary; private TextBox txtFebruary;
        private Label lblMarch; private TextBox txtMarch;
        private Label lblApril; private TextBox txtApril;
        private Label lblMay; private TextBox txtMay;
        private Label lblJune; private TextBox txtJune;
        private Label lblJuly; private TextBox txtJuly;
        private Label lblAugust; private TextBox txtAugust;
        private Label lblSeptember; private TextBox txtSeptember;
        private Label lblOctober; private TextBox txtOctober;
        private Label lblNovember; private TextBox txtNovember;
        private Label lblDecember; private TextBox txtDecember;

        private Label lblInvestmentsList;
        private Label lblTitle;
        private PictureBox logo;

        public CalculateInvestments()
        {
            InitializeComponent();
            RestyleForm();     // <- NEW styling like the rest of your app
            BuildUi();
            WireMonthClicks();
        }

        private void RestyleForm()
        {
            Text = "Calculate Investments";
            ClientSize = new Size(900, 780); // a bit wider/taller for breathing room
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += CalculateInvestments_Paint; // gradient background
        }

        private Button MakePrimaryButton(string text, Point location, Size size, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(255, 120, 120),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        private Button MakeSecondaryButton(string text, Point location, Size size, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        private Label MakeTransparentLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        private TextBox MakeReadOnlyAmountBox(Point location)
        {
            return new TextBox
            {
                Location = location,
                Width = 110,
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Right
            };
        }

        private void BuildUi()
        {
            Controls.Clear();

            // Optional logo + title to match your other forms
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
                Text = "Calculate Investments",
                Font = new Font("Montserrat", 14, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point((ClientSize.Width - 260) / 2, 120)
            };
            Controls.Add(lblTitle);

            // Top controls
            btnRetrieveInvestments = MakePrimaryButton(
                "Retrieve Investments",
                new Point(20, 160),
                new Size(200, 40),
                RetrieveInvestments
            );
            Controls.Add(btnRetrieveInvestments);

            var lblTotal = MakeTransparentLabel("Grand Total:", new Point(240, 170));
            Controls.Add(lblTotal);

            txtTotal = new TextBox
            {
                Location = new Point(340, 166),
                Width = 140,
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Right
            };
            Controls.Add(txtTotal);

            // Month labels/textboxes (two rows)
            int left0 = 20, left1 = 240, left2 = 460, left3 = 680;
            int y1 = 210, y2 = 250, y3 = 290; // 3 rows of 4 months each for better spacing

            // Row 1
            lblJanuary = MakeTransparentLabel("January", new Point(left0, y1));
            txtJanuary = MakeReadOnlyAmountBox(new Point(left0, y2));
            lblFebruary = MakeTransparentLabel("February", new Point(left1, y1));
            txtFebruary = MakeReadOnlyAmountBox(new Point(left1, y2));
            lblMarch = MakeTransparentLabel("March", new Point(left2, y1));
            txtMarch = MakeReadOnlyAmountBox(new Point(left2, y2));
            lblApril = MakeTransparentLabel("April", new Point(left3, y1));
            txtApril = MakeReadOnlyAmountBox(new Point(left3, y2));

            // Row 2
            lblMay = MakeTransparentLabel("May", new Point(left0, y3));
            txtMay = MakeReadOnlyAmountBox(new Point(left0, y3 + 40));
            lblJune = MakeTransparentLabel("June", new Point(left1, y3));
            txtJune = MakeReadOnlyAmountBox(new Point(left1, y3 + 40));
            lblJuly = MakeTransparentLabel("July", new Point(left2, y3));
            txtJuly = MakeReadOnlyAmountBox(new Point(left2, y3 + 40));
            lblAugust = MakeTransparentLabel("August", new Point(left3, y3));
            txtAugust = MakeReadOnlyAmountBox(new Point(left3, y3 + 40));

            // Row 3
            int y4 = y3 + 80;
            lblSeptember = MakeTransparentLabel("September", new Point(left0, y4));
            txtSeptember = MakeReadOnlyAmountBox(new Point(left0, y4 + 40));
            lblOctober = MakeTransparentLabel("October", new Point(left1, y4));
            txtOctober = MakeReadOnlyAmountBox(new Point(left1, y4 + 40));
            lblNovember = MakeTransparentLabel("November", new Point(left2, y4));
            txtNovember = MakeReadOnlyAmountBox(new Point(left2, y4 + 40));
            lblDecember = MakeTransparentLabel("December", new Point(left3, y4));
            txtDecember = MakeReadOnlyAmountBox(new Point(left3, y4 + 40));

            Controls.AddRange(new Control[]
            {
                lblJanuary, txtJanuary, lblFebruary, txtFebruary, lblMarch, txtMarch, lblApril, txtApril,
                lblMay, txtMay, lblJune, txtJune, lblJuly, txtJuly, lblAugust, txtAugust,
                lblSeptember, txtSeptember, lblOctober, txtOctober, lblNovember, txtNovember, lblDecember, txtDecember
            });

            // Grid
            dgvInvestments = new DataGridView
            {
                AutoGenerateColumns = true,
                Location = new Point(20, y4 + 100),
                Size = new Size(ClientSize.Width - 40, 280),
                ReadOnly = true,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false
            };

            // Light styling on the grid for readability
            dgvInvestments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 150, 150);
            dgvInvestments.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvInvestments.ColumnHeadersDefaultCellStyle.Font = new Font(Font, FontStyle.Bold);

            dgvInvestments.DataBindingComplete += (s, e) => UpdateMonthlyAndList(); // fill totals + list current month
            dgvInvestments.CellValueChanged += (s, e) => UpdateMonthlyAndList();
            dgvInvestments.RowsRemoved += (s, e) => UpdateMonthlyAndList();

            Controls.Add(dgvInvestments);

            // Big multi-line label for listing investments of a month
            lblInvestmentsList = new Label
            {
                Location = new Point(20, dgvInvestments.Bottom + 10),
                Size = new Size(ClientSize.Width - 40, 120),
                AutoSize = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 255, 255, 220), // subtle white overlay
                Padding = new Padding(8)
            };
            Controls.Add(lblInvestmentsList);
        }

        private void WireMonthClicks()
        {
            txtJanuary.Click += (s, e) => UpdateMonthlyAndList(null, 1);
            txtFebruary.Click += (s, e) => UpdateMonthlyAndList(null, 2);
            txtMarch.Click += (s, e) => UpdateMonthlyAndList(null, 3);
            txtApril.Click += (s, e) => UpdateMonthlyAndList(null, 4);
            txtMay.Click += (s, e) => UpdateMonthlyAndList(null, 5);
            txtJune.Click += (s, e) => UpdateMonthlyAndList(null, 6);
            txtJuly.Click += (s, e) => UpdateMonthlyAndList(null, 7);
            txtAugust.Click += (s, e) => UpdateMonthlyAndList(null, 8);
            txtSeptember.Click += (s, e) => UpdateMonthlyAndList(null, 9);
            txtOctober.Click += (s, e) => UpdateMonthlyAndList(null, 10);
            txtNovember.Click += (s, e) => UpdateMonthlyAndList(null, 11);
            txtDecember.Click += (s, e) => UpdateMonthlyAndList(null, 12);
        }

        // Gradient background (LightCoral -> White)
        private void CalculateInvestments_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        // ----- Data/logic -----

        private void RetrieveInvestments(object? sender, EventArgs e)
        {
            using var conn = new SqlConnection(ConnStr);
            conn.Open();

            const string sql = @"
                SELECT
                    investments_id,
                    name,
                    amount,
                    [date] AS date,
                    category,
                    length,
                    notes
                FROM dbo.investments
                ORDER BY investments_id DESC;";

            using var da = new SqlDataAdapter(sql, conn);
            var dt = new DataTable();
            da.Fill(dt);

            dgvInvestments.DataSource = dt;

            if (dgvInvestments.Columns.Contains("amount"))
                dgvInvestments.Columns["amount"].DefaultCellStyle.Format = "N2";
            if (dgvInvestments.Columns.Contains("date"))
                dgvInvestments.Columns["date"].DefaultCellStyle.Format = "d";

            UpdateMonthlyAndList(null, DateTime.Today.Month);
        }

        private void UpdateMonthlyAndList(int? year = null, int? monthToList = null)
        {
            UpdateMonthlyTotals(year, out var totals);
            WriteTotalsToTextboxes(totals);
            UpdateLabelList(monthToList ?? DateTime.Today.Month, year ?? DateTime.Today.Year);
        }

        private void UpdateMonthlyTotals(int? year, out decimal[] totals)
        {
            int targetYear = year ?? DateTime.Today.Year;
            totals = new decimal[13]; // 1..12

            if (dgvInvestments.DataSource is DataTable dt)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r.IsNull("date") || r.IsNull("amount")) continue;
                    var d = (DateTime)r["date"];
                    if (d.Year != targetYear) continue;

                    decimal amt = r["amount"] is decimal dec ? dec : Convert.ToDecimal(r["amount"]);
                    totals[d.Month] += amt;
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvInvestments.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (DateTime.TryParse(row.Cells["date"].Value?.ToString(), out var d) &&
                        decimal.TryParse(row.Cells["amount"].Value?.ToString(), out var amt) &&
                        d.Year == targetYear)
                    {
                        totals[d.Month] += amt;
                    }
                }
            }
            txtTotal.Text = totals.Sum().ToString("N2");
        }

        private void WriteTotalsToTextboxes(decimal[] totals)
        {
            txtJanuary.Text = totals[1].ToString("N2");
            txtFebruary.Text = totals[2].ToString("N2");
            txtMarch.Text = totals[3].ToString("N2");
            txtApril.Text = totals[4].ToString("N2");
            txtMay.Text = totals[5].ToString("N2");
            txtJune.Text = totals[6].ToString("N2");
            txtJuly.Text = totals[7].ToString("N2");
            txtAugust.Text = totals[8].ToString("N2");
            txtSeptember.Text = totals[9].ToString("N2");
            txtOctober.Text = totals[10].ToString("N2");
            txtNovember.Text = totals[11].ToString("N2");
            txtDecember.Text = totals[12].ToString("N2");
        }

        private void UpdateLabelList(int month, int year)
        {
            var lines = new System.Collections.Generic.List<string>();

            if (dgvInvestments.DataSource is DataTable dt)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r.IsNull("date") || r.IsNull("amount")) continue;

                    var d = (DateTime)r["date"];
                    if (d.Month == month && d.Year == year)
                    {
                        string name = dt.Columns.Contains("name") && !r.IsNull("name") ? r["name"].ToString()! : "";
                        decimal amt = r["amount"] is decimal dec ? dec : Convert.ToDecimal(r["amount"]);
                        lines.Add($"{name} - {amt:N2} on {d:dd-MMM}");
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvInvestments.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (DateTime.TryParse(row.Cells["date"].Value?.ToString(), out var d) &&
                        decimal.TryParse(row.Cells["amount"].Value?.ToString(), out var amt) &&
                        d.Month == month && d.Year == year)
                    {
                        string name = dgvInvestments.Columns.Contains("name")
                                      ? row.Cells["name"].Value?.ToString() ?? ""
                                      : "";
                        lines.Add($"{name} - {amt:N2} on {d:dd-MMM}");
                    }
                }
            }

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            lblInvestmentsList.Text = lines.Count == 0
                ? $"No investments for {monthName} {year}"
                : $"{monthName} {year}:\n" + string.Join(Environment.NewLine, lines);
        }
    }
}
