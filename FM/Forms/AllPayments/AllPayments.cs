
using System.Globalization;

// AllPayments.cs - Main form to view and manage all payments (bills, expenses, investments, savings) with filtering and editing capabilities

namespace FM
{
    public partial class AllPayments : Form
    {
        // Filter state
        private int currentMonth = DateTime.Today.Month;
        private int currentYear = DateTime.Today.Year;
        private bool isFiltered = false;

        // Grids
        private readonly DataGridView gridBills = new() { ReadOnly = true };
        private readonly DataGridView gridExpenses = new() { ReadOnly = true };
        private readonly DataGridView gridInvestments = new() { ReadOnly = true };
        private readonly DataGridView gridSavings = new() { ReadOnly = true };

        // UI fields
        private TextBox txtGrandTotal;
        private Label lblGrandTotal;
        private TextBox txtEmergencyFund;
        private Label lblEmergencyFund;
        private Button editEmergencyFund;
        private TextBox txtMonthlyAllowance;
        private Label lblMonthlyAllowance;
        private Button editMonthlyAllowance;
        private Label lblRemainingFund;
        private TextBox txtRemainingFund;
        private Panel bottomPanel;
        private PictureBox logo;
        private Label title;

        private Button btnReload;
        private Button btnEditSelected;
        private Button btnDeleteSelected;
        private Button btnSaveSelected;
        private Button btnfilterByDate;
        private Button btnClearFilter;
        private Button btnAddPreviousBills;
        private Button btnAddPreviousInvestments;
        private Button btnDebtProjection;

        // View label
        private Label lblCurrentView;

        // Computed values used across partials
        private decimal RemainingFundValue => decimal.TryParse(txtRemainingFund?.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var value) ? value : 0;
        private decimal CarriedOverDebtValue => Math.Max(0, 1200 - RemainingFundValue);

        public AllPayments()
        {
            //InitializeComponent();

            this.AutoScroll = true;
            this.AutoScrollMargin = new Size(0, 20);
            Text = "All Payments";
            ClientSize = new Size(1400, 1260);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Montserrat", 10, FontStyle.Regular);

            // Paint handler defined in UI partial
            Paint += AllPayments_Paint;

            // Build UI in UI partial
            BuildUi();

            // Load handler defined in data/filters partials
            Load += AllPayments_Load;
        }

        private void AllPayments_Load(object? sender, EventArgs e)
        {
            ReloadAll();
        }

        private void ReloadAll()
        {
            // If filtered, respect filter, else load current month data
            if (isFiltered)
            {
                FilterGridByMonth(currentMonth, currentYear);
            }
            else
            {
                currentMonth = DateTime.Today.Month;
                currentYear = DateTime.Today.Year;

                BillsDataGrid();
                ExtraExpenseDataGrid();
                InvestmentsDataGrid();
                SavingsDataGrid();
                UpdateTotal();
                EmergencyFundData();
                EditMonthlyAllowance();
                RemainingFund();

                lblCurrentView.Text = $"Viewing: {DateTime.Today:MMMM yyyy}";
            }
        }
    }
}