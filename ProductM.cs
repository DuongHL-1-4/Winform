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
    public partial class frmProductM : Form
    {
        SqlConnection connection;
        public frmProductM()
        {
            InitializeComponent();
            connection = new SqlConnection("Server=DUC_DUONG;Database=product_management;Integrated Security = true;");
            frmProductM_Load();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmHome home = new frmHome();
            home.ShowDialog();
            this.Dispose();
        }

        private void frmProductM_Load(string searchTerm = null)
        {
            try
            {
                string query = "SELECT * FROM dbo.Product";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += " WHERE productID LIKE @SearchTerm OR productName LIKE @SearchTerm OR categoryID LIKE @SearchTerm";
                }

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                }

                DataTable productTable = new DataTable();
                adapter.Fill(productTable);
                dgvProduct.DataSource = productTable;
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
            frmProductM_Load(searchTerm);
        }

        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvProduct.Rows[e.RowIndex];
                txtProductID.Text = selectedRow.Cells["productIDDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtCategoryID.Text = selectedRow.Cells["categoryIDDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtProductName.Text = selectedRow.Cells["productNameDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtPrice.Text = selectedRow.Cells["priceDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtQuantity.Text = selectedRow.Cells["quantityDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductID", int.Parse(txtProductID.Text));
                command.Parameters.AddWithValue("@ProductName", txtProductName.Text);
                command.Parameters.AddWithValue("@Quantity", int.Parse(txtQuantity.Text));
                command.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));
                command.Parameters.AddWithValue("@CategoryID", int.Parse(txtCategoryID.Text));

                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    int rowsAffected = command.ExecuteNonQuery();
                    MessageBox.Show(rowsAffected > 0 ? "Operation successful." : "Operation failed.");
                    frmProductM_Load();
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
            ExecuteNonQuery("INSERT INTO dbo.Product (productID ,productName, quantity, price,categoryID) VALUES " +
                "(@ProductID , @ProductName, @Quantity, @Price, @CategoryID)");
            frmProductM_Load();

        }

        private void btnUpdata_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to update this product?", "Confirm update", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "UPDATE dbo.Product SET productID = @ProductID ,productName = @ProductName, quantity = @Quantity," +
                    " price = @Price, categoryID = @CategoryID WHERE productID = @ProductID";
                ExecuteNonQuery(query);
            }
            frmProductM_Load();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductID.Text))
            {
                MessageBox.Show("Please select a product to delete.");
                return;
            }

            var confirmResult = MessageBox.Show("Are you sure you want to delete this product?", "Confirm deletion", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "DELETE FROM dbo.Product WHERE productID = @ProductID";
                ExecuteNonQuery(query);
            }
            frmProductM_Load();
        }
    }
}
