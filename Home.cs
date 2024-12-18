using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainHome
{
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmSignIn signIn = new frmSignIn();
            signIn.ShowDialog();
            this.Dispose();
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmProductM productM = new frmProductM();
            productM.ShowDialog();
            this.Dispose();
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmCustomerM customerM = new frmCustomerM();
            customerM.ShowDialog();
            this.Dispose();
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmEmployeeM employeeM = new frmEmployeeM();
            employeeM.ShowDialog();
            this.Dispose();
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmAccountM accountM = new frmAccountM();
            accountM.ShowDialog();
            this.Dispose();
        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmBill bill = new frmBill();
            bill.ShowDialog();
            this.Dispose();
        }
    }
}
