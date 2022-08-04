using MySql.Data.MySqlClient;
using StoreIT.v1.Data_Access;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ClosedXML.Report;

namespace StoreIT.v1
{
    public partial class Order : Form
    {
        MySqlConnection con;
        MySqlCommand cmd;
        MySqlDataReader sda;
        DBconnection clscon = new DBconnection();
        //string customer_id;
        public Order()
        {
            InitializeComponent();
            con = new MySqlConnection(clscon.dbconnect());
            LoadProduct();
            LoadCustomer();
            fillUser();
            fillProduct();
            fillSupplier();
            fillCustomer();
            
        }

        
        public void fillUser()
        {

            con.Open();
            cmd = new MySqlCommand("select user_id FROM user order by 1 asc", con);
            sda = cmd.ExecuteReader();


            while (sda.Read())
            {
                string getUsr = sda.GetString("user_id");
                cmbUser.Items.Add(getUsr);
                
            }
            con.Close();
            //  Clear();    
        }
        public void fillSupplier()
        {

            con.Open();
            cmd = new MySqlCommand("select supplier_id FROM supplier order by 1 asc", con);
            sda = cmd.ExecuteReader();


            while (sda.Read())
            {
                string getSupplier = sda.GetString("supplier_id");
                cmbSupplier.Items.Add(getSupplier);
            }

            con.Close();
            //  Clear();    
        }
        public void fillProduct()
        {

            con.Open();
            cmd = new MySqlCommand("select product_id FROM product order by 1 asc;", con);
            sda = cmd.ExecuteReader();


            while (sda.Read())
            {
                string getProduct = sda.GetString("product_id");
                cmbProduct.Items.Add(getProduct);
            }

            con.Close();
            //  Clear();    
        }
        public void fillCustomer()
        {

            con.Open();
            cmd = new MySqlCommand("select customer_id FROM customer order by 1 asc;", con);
            sda = cmd.ExecuteReader();


            while (sda.Read())
            {
                string getCustomer = sda.GetString("customer_id");
                cmbCustomer.Items.Add(getCustomer);
            }

            con.Close();
            //  Clear();    
        }
        public void Clear()
        {
            txtTotal.Clear();
            txtUnitPrice.Text = "0";
            txtTotal.Text = "0";
            txtOrder.Text = "";
            txtQty.Text = "";
            cmbProduct.Text = "";
            cmbCustomer.Text = "";
            cmbSupplier.Text = "";
            cmbTransac.Text = "";
            
            btnAdmit.Enabled = true;
            cmbCustomer.Enabled = true;
            cmbUser.Enabled = true;
            cmbSupplier.Enabled = true;
            
            //  datePick.Text 
            //btnSave.Enabled = false;

        }
        public void LoadProduct()
        {
            int i = 0;
            grid.Rows.Clear();
            con.Open();
            cmd = new MySqlCommand("select * from product", con); //display all products
            sda = cmd.ExecuteReader();

            while (sda.Read())
            {
                i += 1;
                grid.Rows.Add(i, sda[1].ToString(), sda[2].ToString(), sda[3].ToString(), sda[4].ToString(), sda[5].ToString(), sda[6].ToString()); // sda[6].ToString());
            }
            sda.Close();
            con.Close();
        }

        public void LoadCustomer()
        {
            int i = 0;
            gridcust.Rows.Clear();
            con.Open();
            cmd = new MySqlCommand("sp_customer_list", con); //stored proc display customer list
            sda = cmd.ExecuteReader();

            while (sda.Read())
            {
                i += 1; 
                gridcust.Rows.Add(i, sda[0].ToString(), sda[1].ToString(), sda[2].ToString());
            }
            sda.Close();
            con.Close();
        }



        private void cmbCustomer_Click_1(object sender, EventArgs e)
        {
            if (cmbSupplier != null)
            {
                cmbTransac.Text = "S";
                cmbSupplier.Enabled = false;
            }
        }

        private void cmbSupplier_Click_1(object sender, EventArgs e)
        {
            if (cmbCustomer != null)
            {
                cmbTransac.Text = "B";
                cmbCustomer.Enabled = false;
                
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Clear();
        }

       

        private void btnAdd_Click(object sender, EventArgs e)
        {

            

            string transaction = "";


            //stored proc starts here
            MySqlParameter[] pms = new MySqlParameter[8];

            

            pms[0] = new MySqlParameter("inOrderNo", MySqlDbType.Int32);
            pms[0].Value = txtOrder.Text;

            pms[1] = new MySqlParameter("inUserNo", MySqlDbType.Int32);
            pms[1].Value = cmbUser.Text;

            pms[2] = new MySqlParameter("inProdcode", MySqlDbType.VarChar);
            pms[2].Value = cmbProduct.Text;

            pms[3] = new MySqlParameter("inTrantype", MySqlDbType.VarChar);
            pms[3].Value = cmbTransac.Text;

            pms[4] = new MySqlParameter("inQuantity", MySqlDbType.Int32);
            pms[4].Value = txtQty.Text;

            pms[5] = new MySqlParameter("inSupplierNo", MySqlDbType.Int32);
            if (cmbCustomer != null)
            {
                pms[5].Value = DBNull.Value;
            }
            else
            {
                pms[5].Value = cmbSupplier.Text;
            }
            //Converts int to null if no input    
            pms[6] = new MySqlParameter("inCustomerNo", MySqlDbType.Int32);
            if (cmbSupplier != null)
            {
                pms[6].Value = DBNull.Value;
            }
            else
            {
                pms[6].Value = cmbCustomer.Text;
            }
            pms[7] = new MySqlParameter("inorderdate", MySqlDbType.Date);
            pms[7].Value = datePick.Text;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_buysellreturn"; // stored proc name
            cmd.Parameters.AddRange(pms);


            con.Open();
            if (cmd.ExecuteNonQuery() == 1)
            {
               
                MessageBox.Show("Yes");
            }
            else
            {
                MessageBox.Show("No");
            }
            con.Close();
            
             
            MessageBox.Show("Successfully added");

            LoadProduct();
            LoadCustomer();
        }


        private void txtTotal_TextChanged(object sender, EventArgs e)
        {
           // getTotalPrice();
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQty.Text) && !string.IsNullOrEmpty(txtUnitPrice.Text))
                txtTotal.Text = (Convert.ToInt32(txtQty.Text) + Convert.ToInt32(txtUnitPrice.Text)).ToString();
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQty.Text) && !string.IsNullOrEmpty(txtUnitPrice.Text))
                txtTotal.Text = (Convert.ToInt32(txtQty.Text) + Convert.ToInt32(txtUnitPrice.Text)).ToString();
        }

        /*private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel WorkBook| *.xlsx", Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtFileName.Text = ofd.FileName;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Please select your template file.", "StoreIT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }*/
    }
    }
     

