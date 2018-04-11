using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Citms.DailyStatistics
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            this.txtDbConn.Text = ConfigManage.SysConfig.DbConn;
        }

        private void btnSure_Click(object sender, EventArgs e)
        {
            string result = this.txtDbConn.Text.Trim();
            if (string.IsNullOrEmpty(result))
            {
                this.txtDbConn.BackColor = Color.Red;
            }
            else
            {
                ConfigManage.SysConfig.DbConn = this.txtDbConn.Text.Trim();
               // this.Close();
            }
        }
    }
}
