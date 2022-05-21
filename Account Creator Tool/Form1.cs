using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Account_Creator_Tool
{
    public partial class accountCreator : Form
    {

        string stringConn = string.Empty;
        string stringConn2 = string.Empty;
        SqlConnection conn = null;
        SqlConnection conn2 = null;
        SqlCommand mySqlCommand = null;

        public accountCreator()
        {
            InitializeComponent();
        }

        private void accountCreator_Load(object sender, EventArgs e)
        {
            btnDisconnect.Enabled = false;
            txtAccountName.Text = "AccountServer";
            txtAccountName.ForeColor = Color.Gray;
            txtGameName.Text = "GameDB";
            txtGameName.ForeColor = Color.Gray;
            txtIPDB.Text = "IP Server";
            txtIPDB.ForeColor = Color.Gray;
        }

        private static bool isServerConnected(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }

        
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(txtIPDB.Text != "" && txtAccountName.Text != "" && txtUserDB.Text != "")
            {
                string serverDat = txtIPDB.Text;
                string databaseDat = txtAccountName.Text;
                string databaseDat2 = txtGameName.Text;
                string userDat = txtUserDB.Text;
                string passwordDat = txtPasswordDB.Text;

                try
                {
                    stringConn = @"Server=" + serverDat + ";Database=" + databaseDat + ";User Id=" + userDat + ";Password=" + passwordDat;
                    stringConn2 = @"Server=" + serverDat + ";Database=" + databaseDat2 + ";User Id=" + userDat + ";Password=" + passwordDat;
                    conn = new SqlConnection(stringConn);
                    conn2 = new SqlConnection(stringConn2);
                    //conn.Open();

                    if(isServerConnected(stringConn) && isServerConnected(stringConn2)){
                        lblInfoDB.Text = "ON";
                        lblInfoDB.ForeColor = Color.Green;
                        btnDisconnect.Enabled = true;
                        btnConnect.Enabled = false;
                        txtIPDB.Enabled = false;
                        txtAccountName.Enabled = false;
                        txtUserDB.Enabled = false;
                        txtPasswordDB.Enabled = false;
                    }
                    
                }
                catch (SqlException ex)
                {
                    lblInfoDB.Text = "ERROR";
                    lblInfoDB.ForeColor = Color.Red;
                }
            }
            else
            {
                MessageBox.Show("Empty fields, please fill IP, DBName and User.");
            }

            

        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Close();
                conn2.Close();
                lblInfoDB.Text = "OFF";
                lblInfoDB.ForeColor = Color.Red;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                txtIPDB.Enabled = true;
                txtAccountName.Enabled = true;
                txtUserDB.Enabled = true;
                txtPasswordDB.Enabled = true;
            }
            catch (SqlException ex)
            {
                lblInfoDB.Text = "ERROR";
                lblInfoDB.ForeColor = Color.Red;
            }
        }

        private void btnInsertUser_Click(object sender, EventArgs e)
        {
            string nameUser = txtUser.Text;
            string passUser = txtUserPassword.Text;


            try
            {
                if(chBGM.Checked == true)
                {
                    if (isServerConnected(stringConn) && isServerConnected(stringConn2))
                    {
                        conn.Open();
                        string sqlQuery = "INSERT INTO account_login (name, password) VALUES (@Name, @Password)";
                        mySqlCommand = new SqlCommand(sqlQuery, conn);
                        mySqlCommand.Parameters.AddWithValue("Name", nameUser);
                        mySqlCommand.Parameters.AddWithValue("Password", MD5Hash(passUser).ToUpper());
                        mySqlCommand.ExecuteNonQuery();
                        conn.Close();
                        string sqlQuery2 = "INSERT INTO account (act_id, gm, act_name) VALUES ((SELECT MAX(act_id) + 1 FROM account), 99, @Act_name)";
                        conn2.Open();
                        mySqlCommand = new SqlCommand(sqlQuery2, conn2);
                        mySqlCommand.Parameters.AddWithValue("Act_name", nameUser);
                        mySqlCommand.ExecuteNonQuery();
                        conn2.Close();
                    }
                    
                }
                else
                {
                    conn.Open();
                    string sqlQuery = "INSERT INTO account_login (name, password) VALUES (@Name, @Password)";
                    mySqlCommand = new SqlCommand(sqlQuery, conn);
                    mySqlCommand.Parameters.AddWithValue("Name", nameUser);
                    mySqlCommand.Parameters.AddWithValue("Password", MD5Hash(passUser).ToUpper());
                    mySqlCommand.ExecuteNonQuery();
                    conn.Close();

                    string sqlQuery2 = "INSERT INTO account (act_id, act_name) VALUES ((SELECT MAX(act_id) + 1 FROM account), @Act_name)";
                    conn2.Open();
                    mySqlCommand = new SqlCommand(sqlQuery2, conn2);
                    mySqlCommand.Parameters.AddWithValue("Act_name", nameUser);
                    mySqlCommand.ExecuteNonQuery();
                    conn2.Close();
                }

                //sqlQuery2 = "INSERT INTO account (act_id, act_name) VALUES ((SELECT MAX(act_id) + 1 FROM account), @Act_name)";
                

                MessageBox.Show("New user added!!, USER: " + nameUser + ", PASS: " + MD5Hash(passUser));
                txtUser.Text = "";
                txtUserPassword.Text = "";

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5Provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return hash.ToString();
        }

        private void txtAccountName_MouseEnter(object sender, EventArgs e)
        {
            if(txtAccountName.Text == "AccountServer")
            {
                txtAccountName.Text = "";
            }
        }

        private void txtAccountName_MouseLeave(object sender, EventArgs e)
        {
            if(txtAccountName.Text == "")
            {
                txtAccountName.Text = "AccountServer";
            }
        }

        private void txtGameName_MouseEnter(object sender, EventArgs e)
        {
            if(txtGameName.Text == "GameDB")
            {
                txtGameName.Text = "";
            }
        }

        private void txtGameName_MouseLeave(object sender, EventArgs e)
        {
            if(txtGameName.Text == "")
            {
                txtGameName.Text = "GameDB";
            }
        }

        private void txtIPDB_TextChanged(object sender, EventArgs e)
        {
            if(txtIPDB.Text == "")
            {
                txtIPDB.Text = "IP SERVER";
            }
        }
    }
}
