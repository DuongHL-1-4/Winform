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
    public partial class frmCustomerM : Form
    {
        SqlConnection connection;
        public frmCustomerM()
        {
            InitializeComponent();
            connection = new SqlConnection("Server=DUC_DUONG;Database=product_management;Integrated Security = true;");
            frmCustomerM_Load();
        }

        private void frmCustomerM_Load(string searchTerm = null)
        {
            try
            {
                string query = "SELECT * FROM dbo.Customer";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += " WHERE customerID LIKE @SearchTerm OR customerName LIKE @SearchTerm OR phone LIKE @SearchTerm";
                }

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                }

                DataTable customerTable = new DataTable();
                adapter.Fill(customerTable);
                dgvCustomer.DataSource = customerTable;
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
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            frmCustomerM_Load(searchTerm);
        }

        private void dgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvCustomer.Rows[e.RowIndex];
                txtCustomerID.Text = selectedRow.Cells["customerIDDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtCustomerName.Text = selectedRow.Cells["customerNameDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = selectedRow.Cells["cemailDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtPhone.Text = selectedRow.Cells["phoneDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtAddress.Text = selectedRow.Cells["caddressDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerID", int.Parse(txtCustomerID.Text));
                command.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                command.Parameters.AddWithValue("@Address", txtAddress.Text);
                command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                command.Parameters.AddWithValue("@Email", txtEmail.Text);

                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show(rowsAffected > 0 ? "Operation successful." : "Operation failed.");
                    frmCustomerM_Load();
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
            ExecuteNonQuery("INSERT INTO dbo.Customer (customerID , customerName, c_address, phone, c_email) VALUES " +
                "(@CustomerID , @CustomerName, @Address, @Phone, @Email)");
            frmCustomerM_Load();
        }

        private void btnUpdata_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to update this customer?", "Confirm update", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "UPDATE dbo.Customer SET customerID = @CustomerID , customerName = @CustomerName, c_address = @Address, " +
                    "phone = @Phone, c_email = @Email WHERE customerID = @CustomerID";
                ExecuteNonQuery(query);
            }
            frmCustomerM_Load();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerID.Text))
            {
                MessageBox.Show("Please select a customer to delete.");
                return;
            }

            var confirmResult = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm deletion", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "DELETE FROM dbo.Customer WHERE customerID = @CustomerID";
                ExecuteNonQuery(query);
            }
            frmCustomerM_Load();
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
