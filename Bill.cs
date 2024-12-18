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
    public partial class frmBill : Form
    {
        SqlConnection connection;

        public frmBill()
        {
            InitializeComponent();
            connection = new SqlConnection("Server=DUC_DUONG;Database=product_management;Integrated Security=true;");
            frmBill_Load();
        }

        private void frmBill_Load(string searchTerm = null)
        {
            try
            {
                string query = @"
                    SELECT 
                        b.BillID,
                        b.customerID,
                        b.employeeID,
                        b.productID,
                        p.productName,
                        b.quantity,
                        p.price,
                        b.date_bill,
                        b.total_bill
                    FROM 
                        Bill b
                    JOIN 
                        Product p ON b.productID = p.productID
                    JOIN 
                        Customer c ON b.customerID = c.customerID";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " WHERE b.BillID LIKE @searchTerm OR c.customerName LIKE @searchTerm";
                }

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                }

                DataTable billTable = new DataTable();
                adapter.Fill(billTable);
                dgvBill.DataSource = billTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void dgvBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvBill.Rows[e.RowIndex];
                txtBillID.Text = selectedRow.Cells["BillID"].Value?.ToString() ?? string.Empty;
                txtCustomerID.Text = selectedRow.Cells["customerID"].Value?.ToString() ?? string.Empty;
                txtEmployeeID.Text = selectedRow.Cells["employeeID"].Value?.ToString() ?? string.Empty;
                txtProductID.Text = selectedRow.Cells["productID"].Value?.ToString() ?? string.Empty;
                dtTime.Text = selectedRow.Cells["date_bill"].Value?.ToString() ?? string.Empty;
                txtTotal.Text = selectedRow.Cells["total_bill"].Value?.ToString() ?? string.Empty;
                txtPName.Text = selectedRow.Cells["productName"].Value?.ToString() ?? string.Empty;
                txtPrice.Text = selectedRow.Cells["price"].Value?.ToString() ?? string.Empty;
                txtQuantity.Text = selectedRow.Cells["quantity"].Value?.ToString() ?? string.Empty;
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillID", int.Parse(txtBillID.Text));
                command.Parameters.AddWithValue("@CustomerID", int.Parse(txtCustomerID.Text));
                command.Parameters.AddWithValue("@EmployeeID", int.Parse(txtEmployeeID.Text));
                command.Parameters.AddWithValue("@ProductID", int.Parse(txtProductID.Text));
                command.Parameters.AddWithValue("@DateBill", DateTime.Parse(dtTime.Text));
                command.Parameters.AddWithValue("@TotalBill", decimal.Parse(txtTotal.Text));
                command.Parameters.AddWithValue("@ProductName", txtPName.Text);
                command.Parameters.AddWithValue("@Quantity", int.Parse(txtQuantity.Text));
                command.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));

                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show(rowsAffected > 0 ? "Operation successful." : "Operation failed.");
                    frmBill_Load();
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
            ExecuteNonQuery(@"INSERT INTO Bill (BillID, customerID, employeeID, productID, quantity, date_bill, total_bill) 
                      VALUES (@BillID, @CustomerID, @EmployeeID, @ProductID, @Quantity, @DateBill, @TotalBill)");
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to update this bill?", "Confirm Update", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                ExecuteNonQuery(@"UPDATE Bill 
                          SET customerID = @CustomerID, employeeID = @EmployeeID, productID = @ProductID, 
                              quantity = @Quantity, date_bill = @DateBill, total_bill = @TotalBill 
                          WHERE BillID = @BillID");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBillID.Text))
            {
                MessageBox.Show("Please select a bill to delete.");
                return;
            }

            var confirmResult = MessageBox.Show("Are you sure you want to delete this bill?", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                ExecuteNonQuery("DELETE FROM Bill WHERE BillID = @BillID");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            frmBill_Load(searchTerm);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmHome home = new frmHome();
            home.ShowDialog();
            this.Dispose();
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            UpdateTotal();  
        }

        private void UpdateTotal()
        {
            try
            {
                if (int.TryParse(txtQuantity.Text, out int quantity) && decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    decimal total = quantity * price;

                    txtTotal.Text = total.ToString("0.00");  
                }
                else
                {
                    txtTotal.Text = "0.00";  
                }
            }
            catch (Exception ex)
            {
                txtTotal.Text = "0.00";
                Console.WriteLine("Error calculating total: " + ex.Message);
            }
        }
    }
}
