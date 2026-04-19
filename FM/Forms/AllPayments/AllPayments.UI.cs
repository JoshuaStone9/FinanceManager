using System.Drawing.Drawing2D;

// AllPayments.UI.cs - Methods to build and manage the user interface of the AllPayments form

namespace FM
{
    public partial class AllPayments
    {
        private void BuildUi()
        {
            Controls.Clear();

            logo = new PictureBox
            {
                Image = System.Drawing.Image.FromFile("Resources/images/FM_Logo_Main_Menu.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(120, 120),
                Location = new Point((ClientSize.Width - 120) / 2, 0),
                BackColor = Color.Transparent
            };
            Controls.Add(logo);

            title = new Label
            {
                Text = "All Payments",
                Font = new System.Drawing.Font("Montserrat", 14F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point((ClientSize.Width - 160) / 2, 120)
            };
            Controls.Add(title);

            lblCurrentView = new Label
            {
                Text = $"Viewing: {DateTime.Today:MMMM yyyy}",
                Font = new System.Drawing.Font("Montserrat", 9F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point((ClientSize.Width - 150) / 2, 105)
            };
            Controls.Add(lblCurrentView);
            lblCurrentView.BringToFront();

            btnReload = MakePrimaryButton("Reload", new Point(16, 150), new Size(140, 40), (s, e) => ReloadAll());

            btnEditSelected = MakeSecondaryButton("Edit Selected", new Point(168, 150), new Size(140, 40), (s, e) => EditSelected());
            btnDeleteSelected = MakeSecondaryButton("Delete Selected", new Point(320, 150), new Size(140, 40), (s, e) => DeleteSelected());
            btnfilterByDate = MakeSecondaryButton("Filter by Date", new Point(472, 150), new Size(140, 40), FilterByDate_click);
            btnClearFilter = MakeSecondaryButton("Clear Filter", new Point(624, 150), new Size(140, 40), ClearFilter_click);
            btnAddPreviousBills = MakeSecondaryButton("Add Previous Bills", new Point(776, 150), new Size(180, 40), (s, e) => AddPreviousBills());
            btnAddPreviousInvestments = MakeSecondaryButton("Add Previous Investments", new Point(968, 150), new Size(180, 40), (s, e) => AddPreviousInvestments());
            btnDebtProjection = MakeSecondaryButton("Debt Projection", new Point(1160, 150), new Size(180, 40), (s, e) =>
            {
                using var form = new CarriedOverDebtProjection();
                form.ShowDialog(this);
            });

            Controls.Add(btnReload);
            Controls.Add(btnEditSelected);
            Controls.Add(btnDeleteSelected);
            Controls.Add(btnfilterByDate);
            Controls.Add(btnClearFilter);
            Controls.Add(btnAddPreviousBills);
            Controls.Add(btnAddPreviousInvestments);
            Controls.Add(btnDebtProjection);

            int left = 16;
            int width = ClientSize.Width - 32;

            int billsY = 230;
            int gridHeightLarge = 180;
            int gridHeightMedium = 150;
            int sectionGap = 55;

            SetupGrid(gridBills, "Bills", new Point(left, billsY), width, gridHeightLarge);

            int expensesY = billsY + gridHeightLarge + sectionGap;
            SetupGrid(gridExpenses, "Extra Expenses", new Point(left, expensesY), width, gridHeightLarge);

            int investmentsY = expensesY + gridHeightLarge + sectionGap;
            SetupGrid(gridInvestments, "Investments", new Point(left, investmentsY), width, gridHeightMedium);

            int savingsY = investmentsY + gridHeightMedium + sectionGap;
            SetupGrid(gridSavings, "Savings", new Point(left, savingsY), width, gridHeightLarge);

            Controls.AddRange(new Control[] { gridBills, gridExpenses, gridInvestments, gridSavings });

            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                Padding = new Padding(12, 12, 12, 20),
                BackColor = Color.Transparent
            };

            var totalLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 12, 0, 0),
                Margin = new Padding(0)
            };

            txtGrandTotal = new TextBox
            {
                ReadOnly = true,
                Width = 150,
                TextAlign = HorizontalAlignment.Right
            };

            lblGrandTotal = new Label
            {
                Text = "Grand Total:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 8, 0),
                BackColor = Color.Transparent
            };

            lblEmergencyFund = new Label
            {
                Text = "Emergency Fund:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.Transparent
            };

            txtEmergencyFund = new TextBox
            {
                ReadOnly = true,
                Width = 150,
                TextAlign = HorizontalAlignment.Right
            };

            editEmergencyFund = new Button
            {
                Text = "Edit",
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat
            };

            lblMonthlyAllowance = new Label
            {
                Text = "Monthly Allowance:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.Transparent
            };

            txtMonthlyAllowance = new TextBox
            {
                ReadOnly = true,
                Width = 150,
                TextAlign = HorizontalAlignment.Right
            };

            editMonthlyAllowance = new Button
            {
                Text = "Edit",
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat
            };

            lblRemainingFund = new Label
            {
                Text = "Remaining Fund:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.Transparent
            };

            txtRemainingFund = new TextBox
            {
                ReadOnly = true,
                Width = 150,
                TextAlign = HorizontalAlignment.Right
            };

            totalLayout.Controls.Add(txtRemainingFund);
            totalLayout.Controls.Add(lblRemainingFund);
            totalLayout.Controls.Add(txtGrandTotal);
            totalLayout.Controls.Add(lblGrandTotal);
            totalLayout.Controls.Add(txtEmergencyFund);
            totalLayout.Controls.Add(lblEmergencyFund);
            totalLayout.Controls.Add(editEmergencyFund);
            totalLayout.Controls.Add(txtMonthlyAllowance);
            totalLayout.Controls.Add(lblMonthlyAllowance);
            totalLayout.Controls.Add(editMonthlyAllowance);

            editEmergencyFund.Click += EditEmergencyFund_Click;
            editMonthlyAllowance.Click += EditMonthlyAllowance_click;

            bottomPanel.Controls.Add(totalLayout);
            Controls.Add(bottomPanel);
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

        private void SetupGrid(DataGridView grid, string caption, Point location, int width, int height)
        {
            var lbl = new Label
            {
                Text = caption,
                Location = new Point(location.X, location.Y - 24),
                AutoSize = true,
                Font = new System.Drawing.Font(Font, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            Controls.Add(lbl);

            grid.Location = location;
            grid.Size = new Size(width, height);
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 150, 150);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(Font, FontStyle.Bold);

            grid.DataBindingComplete += (s, e) => { FormatGrid(grid); UpdateTotal(); };
            grid.CellValueChanged += (s, e) => UpdateTotal();
            grid.RowsRemoved += (s, e) => UpdateTotal();

            AttachSingleSelectionBehavior(grid);
        }

        private void AttachSingleSelectionBehavior(DataGridView sourceGrid)
        {
            sourceGrid.MultiSelect = true;
            sourceGrid.ClearSelection();

            sourceGrid.SelectionChanged += (s, e) =>
            {
                try
                {
                    if (sourceGrid.SelectedRows.Count > 0)
                    {
                        if (!ReferenceEquals(sourceGrid, gridBills)) gridBills.ClearSelection();
                        if (!ReferenceEquals(sourceGrid, gridExpenses)) gridExpenses.ClearSelection();
                        if (!ReferenceEquals(sourceGrid, gridInvestments)) gridInvestments.ClearSelection();
                        if (!ReferenceEquals(sourceGrid, gridSavings)) gridSavings.ClearSelection();
                    }
                }
                catch
                {
                }
            };
        }

        private void AllPayments_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}