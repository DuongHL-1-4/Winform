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
    public partial class frmEmployeeM : Form
    {
        SqlConnection connection;
        public frmEmployeeM()
        {
            InitializeComponent();
            connection = new SqlConnection("Server=DUC_DUONG;Database=product_management;Integrated Security = true;");
            frmEmployeeM_Load();
        }

        private void frmEmployeeM_Load(string searchTerm = null)
        {
            try
            {
                string query = "SELECT * FROM dbo.Employee";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += " WHERE employeeID LIKE @SearchTerm OR employeeName LIKE @SearchTerm OR e_phone LIKE @SearchTerm";
                }

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                }

                DataTable employeeTable = new DataTable();
                adapter.Fill(employeeTable);
                dgvEmployee.DataSource = employeeTable;
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
            frmEmployeeM_Load(searchTerm);
        }

        private void dgvEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvEmployee.Rows[e.RowIndex];
                txtEmployeeID.Text = selectedRow.Cells["employeeIDDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtEmployeeName.Text = selectedRow.Cells["employeeNameDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = selectedRow.Cells["eemailDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtPhone.Text = selectedRow.Cells["ephoneDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
                txtRole.Text = selectedRow.Cells["eroleDataGridViewTextBoxColumn"].Value?.ToString() ?? string.Empty;
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@EmployeeID", int.Parse(txtEmployeeID.Text));
                command.Parameters.AddWithValue("@EmployeeName", txtEmployeeName.Text);
                command.Parameters.AddWithValue("@Role", txtRole.Text);
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
                    frmEmployeeM_Load();
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
            ExecuteNonQuery("INSERT INTO dbo.Employee (employeeID , employeeName, e_role, e_phone, e_email) VALUES " +
                "(@EmployeeID , @EmployeeName, @Role, @Phone, @Email)");
            frmEmployeeM_Load();
        }

        private void btnUpdata_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to update this employee?", "Confirm update", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "UPDATE dbo.Employee SET employeeID = @EmployeeID , employeeName = @EmployeeName, e_role = @Role, e_phone = @Phone, " +
                    " e_email = @Email WHERE employeeID = @EmployeeID";
                ExecuteNonQuery(query);
            }
            frmEmployeeM_Load();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please select a employee to delete.");
                return;
            }

            var confirmResult = MessageBox.Show("Are you sure you want to delete this employee?", "Confirm deletion", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                string query = "DELETE FROM dbo.Employee WHERE employeeID = @EmployeeID";
                ExecuteNonQuery(query);
            }
            frmEmployeeM_Load();
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
