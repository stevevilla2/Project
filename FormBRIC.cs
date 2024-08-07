﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace BARANGAY
{
    public partial class FormBRIC : Form
    {
        SQLiteConnection conn;
        SQLiteCommand cmdBRIC;
        SQLiteDataReader dr;
        public string _ID;

        public FormBRIC()
        {
            InitializeComponent();
            conn = new SQLiteConnection("Data Source=database.db;Version=3");
            this.Load += FormBRIC_Load; // Ensure LoadRecord is called on initialization
        }


        private void panelContainer_Paint_1(object sender, PaintEventArgs e)
        {
            // Your existing code
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dataGridView1.Columns[e.ColumnIndex].Name;
                if (colName == "btnEdit1")
                {
                    FrmAccountsBRIC f = new FrmAccountsBRIC(this);
                    f.btnSave.Enabled = false;
                    f._ID = dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString();
                    f.txtName.Text = dataGridView1.Rows[e.RowIndex].Cells["name"].Value.ToString();
                    f.txtAddress.Text = dataGridView1.Rows[e.RowIndex].Cells["address"].Value.ToString();
                    f.txtResidency.Text = dataGridView1.Rows[e.RowIndex].Cells["residency"].Value.ToString();
                    f.cboStatus.Text = dataGridView1.Rows[e.RowIndex].Cells["status"].Value.ToString();
                    f.cboCondition.Text = dataGridView1.Rows[e.RowIndex].Cells["condition"].Value.ToString();
                    f.dtBirthDate.Value = DateTime.Parse(dataGridView1.Rows[e.RowIndex].Cells["birth_date"].Value.ToString());
                    f.dtIssued.Value = DateTime.Parse(dataGridView1.Rows[e.RowIndex].Cells["date_issued"].Value.ToString());
                    f.dtValidUntil.Value = DateTime.Parse(dataGridView1.Rows[e.RowIndex].Cells["Valid_Until"].Value.ToString());
                    f.txtAdministeredBy.Text = dataGridView1.Rows[e.RowIndex].Cells["administered_by"].Value.ToString();
                    f.txtReference.Text = dataGridView1.Rows[e.RowIndex].Cells["Reference"].Value.ToString();
                    f.ShowDialog();
                }
                else if (colName == "btnDelete1")
                {
                    if (MessageBox.Show("Do you want to delete this?", clsvar._title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        conn.Open();
                        cmdBRIC = new SQLiteCommand("DELETE FROM residency WHERE id = @id", conn);
                        cmdBRIC.Parameters.AddWithValue("@id", dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString());
                        cmdBRIC.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Record has been successfully deleted", clsvar._title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, clsvar._title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FormBRIC_Load(object sender, EventArgs e)
        {
            try
            {
                LoadRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmAccountsBRIC f = new FrmAccountsBRIC(this);
            if (f.btnUpdate != null)
            {
                f.btnUpdate.Enabled = false;
            }
            else
            {
                MessageBox.Show("btnUpdate is not initialized properly in FrmAccountsBRIC.");
            }
            f.ShowDialog();
        }

        public void LoadRecord()
        {
            try
            {
                dataGridView1.Rows.Clear();
                conn.Open();
                cmdBRIC = new SQLiteCommand("SELECT * FROM residency", conn);
                dr = cmdBRIC.ExecuteReader();
                while (dr.Read())
                {
                    int v = dataGridView1.Rows.Add(dr["id"].ToString(), dr["name"].ToString(), DateTime.Parse(dr["birth_date"].ToString()).ToShortDateString(), dr["status"].ToString(), dr["address"].ToString(), dr["start_year"].ToString(), dr["condition"].ToString(), DateTime.Parse(dr["issued"].ToString()).ToShortDateString(), DateTime.Parse(dr["valid_until"].ToString()).ToShortDateString(), dr["administered_by"].ToString(), dr["reference"].ToString());
                }
                dr.Close();
                conn.Close();
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, clsvar._title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            FilterRecords(searchBox.Text);
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            FilterRecords(searchBox.Text);
        }

        private void FilterRecords(string searchTerm)
        {
            // Convert the search term to lowercase for case-insensitive comparison
            string lowerSearchTerm = searchTerm.ToLower();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool isVisible = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        // Convert cell value to lowercase for case-insensitive comparison
                        string lowerCellValue = cell.Value.ToString().ToLower();

                        if (lowerCellValue.Contains(lowerSearchTerm))
                        {
                            isVisible = true;
                            break;
                        }
                    }
                }
                row.Visible = isVisible;
            }
        }

    }
}
