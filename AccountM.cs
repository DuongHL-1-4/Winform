using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainHome
{
    public partial class frmAccountM : Form
    {
        SqlConnection connection;

        public frmAccountM()
        {
            InitializeComponent();
            connection = new SqlConnection("Server=DUC_DUONG;Database=product_management;Integrated Security=true;");
            frmAccount_Load();
        }

        private void frmAccount_Load(string searchTerm = null)
        {
            try
            {
                string query = "SELECT * FROM Account";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += " WHERE accountID LIKE @SearchTerm OR customerID LIKE @SearchTerm " +
                             "OR employeeID LIKE @SearchTerm OR Role LIKE @SearchTerm";
                }

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                }

                DataTable accountTable = new DataTable();
                adapter.Fill(accountTable);
                dgvAccount.DataSource = accountTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            frmAccount_Load(searchTerm);
        }

        private void dgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvAccount.Rows[e.RowIndex];

                txtAccountID.Text = selectedRow.Cells["accountIDDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtUser.Text = selectedRow.Cells["usernameDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtPassword.Text = selectedRow.Cells["upasswordDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtRole.Text = selectedRow.Cells["roleDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;

                if (selectedRow.Cells["customerIDDataGridViewTextBoxColumn"].Value != DBNull.Value)
                {
                    txtCustomerID.Text = selectedRow.Cells["customerIDDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                    txtEmployeeID.Text = "NULL";
                }
                else
                {
                    txtEmployeeID.Text = selectedRow.Cells["employeeIDDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                    txtCustomerID.Text = "NULL";
                }
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AccountID", int.Parse(txtAccountID.Text));
                command.Parameters.AddWithValue("@Username", txtUser.Text);
                command.Parameters.AddWithValue("@Password", txtPassword.Text);
                command.Parameters.AddWithValue("@Role", txtRole.Text);

                if (string.IsNullOrWhiteSpace(txtCustomerID.Text))
                    command.Parameters.AddWithValue("@CustomerID", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@CustomerID", int.Parse(txtCustomerID.Text));

                if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
                    command.Parameters.AddWithValue("@EmployeeID", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@EmployeeID", int.Parse(txtEmployeeID.Text));

                try
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show(rowsAffected > 0 ? "Operation successful." : "Operation failed.");
                    frmAccount_Load();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text != "")
            {
                txtCustomerID.Clear();
            }
            else
            {
                txtEmployeeID.Clear();
            }
            string query = "INSERT INTO Account (accountID, username, u_password, Role, customerID, employeeID) " +
                           "VALUES (@AccountID, @Username, @Password, @Role, @CustomerID, @EmployeeID)";
            ExecuteNonQuery(query);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text != "")
            {
                txtCustomerID.Clear();
            }
            else
            {
                txtEmployeeID.Clear();
            }
            var confirmResult = MessageBox.Show("Are you sure you want to update this account?", "Confirm Update", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "UPDATE Account SET username = @Username, u_password = @Password, Role = @Role, " +
                               "customerID = @CustomerID, employeeID = @EmployeeID WHERE accountID = @AccountID";
                ExecuteNonQuery(query);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtEmployeeID.Text != "")
            {
                txtCustomerID.Clear();
            }
            else
            {
                txtEmployeeID.Clear();
            }
            if (string.IsNullOrWhiteSpace(txtAccountID.Text))
            {
                MessageBox.Show("Please select an account to delete.");
                return;
            }

            var confirmResult = MessageBox.Show("Are you sure you want to delete this account?", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "DELETE FROM Account WHERE accountID = @AccountID";
                ExecuteNonQuery(query);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmHome home = new frmHome();
            home.ShowDialog();
            this.Dispose();
        }
    }
}