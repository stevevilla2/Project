﻿using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using iText.Kernel.Exceptions;

namespace BARANGAY
{
    public partial class FrmAccountsBRIC : Form
    {
        SQLiteConnection conn;
        SQLiteCommand cmd;
        FormBRIC f;
        public string _ID;
        private FormBP formBP;
        Capture _capture;
        bool _streaming;

        public FrmAccountsBRIC(FormBRIC f)
        {
            InitializeComponent();
            conn = new SQLiteConnection("Data Source=database.db;Version=3");
            cmd = new SQLiteCommand();
            // Initialize the FormBRIC object
            this.f = f; // Corrected to use the passed-in form
            InitializeCamera();
        }
        private void InitializeCamera()
        {
            try
            {
                _capture = new Capture(); // Initialize camera capture
                if (_capture == null || _capture.Ptr == IntPtr.Zero)
                {
                    MessageBox.Show("Failed to open webcam. Please make sure it is connected and try again.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Application.Idle += Streaming; // Start streaming frames
                _streaming = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing webcam capture: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Streaming(object sender, EventArgs e)
        {
            try
            {
                var frame = _capture.QueryFrame()?.ToImage<Bgr, byte>();
                if (frame != null)
                {
                    var bmp = frame.Bitmap;
                    pictureBox1.Image = bmp; // Display frame in pictureBox1
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during streaming: {ex.Message}", "Streaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public FrmAccountsBRIC()
        {
            InitializeComponent(); // Added to initialize components
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you want to save this record?", "Save Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    conn.Open();
                    string sql = "INSERT INTO residency (last_name, first_name, middle_name, birth_date, status, address, Contact_Number, Condition, issued, valid_until,administered_by) " +
                                 "VALUES (@last_name, @first_name, @middle_name, @birth_date, @status, @address, @Contact_Number, @Condition, @issued, @valid_until, @administered_by)";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@last_name", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@middle_name", txtMiddleName.Text);
                    cmd.Parameters.AddWithValue("@birth_date", dtBirthDate.Value);
                    cmd.Parameters.AddWithValue("@status", cboStatus.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@Contact_Number", txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Condition", cboCondition.Text);
                    cmd.Parameters.AddWithValue("issued", dtIssued.Value);
                    cmd.Parameters.AddWithValue("valid_until", dtValidUntil.Value);
                    cmd.Parameters.AddWithValue("@administered_by", txtAdministeredBy.Text);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Record has been successfully saved!", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clear();
                    f.LoadRecord();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void clear()
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtAddress.Clear();
            txtContactNumber.Clear();
            cboStatus.SelectedIndex = 0;
            cboCondition.SelectedIndex = 0;
            dtBirthDate.Value = DateTime.Now;
            dtIssued.Value = DateTime.Now;
            dtValidUntil.Value = DateTime.Now;
            txtAdministeredBy.Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtLastName.Focus();
        }

        private void cboCondition_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            clear();
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you want to update this record?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    conn.Open();
                    string sql = "UPDATE residency SET last_name=@last_name, first_name=@first_name, middle_name=@middle_name, birth_date=@birth_date, status=@status, address=@address, Contact_Number=@Contact_Number, Condition=@Condition, issued=@issued, valid_until=@valid_until, administered_by=@administered_by WHERE id = @ID";
                    cmd = new SQLiteCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@last_name", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@middle_name", txtMiddleName.Text);
                    cmd.Parameters.AddWithValue("@birth_date", dtBirthDate.Value);
                    cmd.Parameters.AddWithValue("@status", cboStatus.Text);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@Contact_Number", txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Condition", cboCondition.Text);
                    cmd.Parameters.AddWithValue("@issued", dtIssued.Value);
                    cmd.Parameters.AddWithValue("@valid_until", dtValidUntil.Value);
                    cmd.Parameters.AddWithValue("@administered_by", txtAdministeredBy.Text);
                    cmd.Parameters.AddWithValue("@ID", _ID);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Record has been successfully updated!", "Update Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clear();
                    f.LoadRecord();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cboStatus_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btn_captureImg_Click(object sender, EventArgs e)
        {
            try
            {
                var frame = _capture.QueryFrame()?.ToImage<Bgr, byte>();
                if (frame != null)
                {
                    var bmp = frame.Bitmap;
                    pictureBox3.Image = bmp;
                    MessageBox.Show("Image captured successfully!", "Capture Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to capture image. Please try again.", "Capture Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during image capture: {ex.Message}", "Capture Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string savedImagePath;

        private void btn_imgSave_Click(object sender, EventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Your Photo",
                    Filter = "JPEG Image|*.jpg",
                    InitialDirectory = @"C:\Users\user\source\repos\Project - Copy\ID Images" // Set your desired folder path here
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (pictureBox3.Image != null)
                    {
                        pictureBox3.Image.Save(saveFileDialog.FileName, ImageFormat.Jpeg); // Save captured image
                        savedImagePath = saveFileDialog.FileName; // Store the path
                        MessageBox.Show("Picture Saved Successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No image to save.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during image save: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadImage(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    if (System.IO.File.Exists(imagePath))
                    {
                        pictureBox3.Image = Image.FromFile(imagePath); // Load image into pictureBox3
                    }
                    else
                    {
                        MessageBox.Show("Image file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Image path is empty or null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FrmAccounts_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_capture != null)
            {
                _capture.Dispose(); // Release camera capture resources
            }
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            PrintToPdf(txtLastName.Text, txtFirstName.Text, txtMiddleName.Text, txtAddress.Text, dtBirthDate.Text, cboStatus.Text, dtIssued.Text, dtIssued.Text, dtValidUntil.Text);
        }
        private void PrintToPdf(string last_name, string first_name, string middle_name, string address, string birth_date, string status, string issued, string issued1, string ValidUntil)
        {
            try
            {
                // Use an absolute path or ensure the relative path is correct
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"BRIC Template.pdf");
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string outputPath = Path.Combine(desktopPath, @"Certificate of Residency Template.pdf");
                // Ensure the output directory exists
                string outputDir = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                if (!File.Exists(templatePath))
                {
                    MessageBox.Show($"Template file not found at: {templatePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (PdfReader reader = new PdfReader(templatePath))
                using (PdfWriter writer = new PdfWriter(outputPath))
                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                {
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                    IDictionary<string, PdfFormField> fields = form.GetAllFormFields();

                    // Case-insensitive field lookup
                    string lastnameFieldName = fields.Keys.FirstOrDefault(k => k == "lastNameField");
                    string firstnameFieldName = fields.Keys.FirstOrDefault(k => k == "firstNameField");
                    string middlenameFieldName = fields.Keys.FirstOrDefault(k => k == "middleNameField");
                    string addressFieldName = fields.Keys.FirstOrDefault(k => k == "AddressField");
                    string birth_dateFieldName = fields.Keys.FirstOrDefault(k => k == "Birth_dateField");
                    string statusFieldName = fields.Keys.FirstOrDefault(k => k == "StatusField");
                    string issuedFieldName = fields.Keys.FirstOrDefault(k => k == "IssuedField");
                    string issued1FieldName = fields.Keys.FirstOrDefault(k => k == "IssuedField1");
                    string validuntilFieldName = fields.Keys.FirstOrDefault(k => k == "ValidUntilField");

                    if (string.IsNullOrEmpty(lastnameFieldName) || (string.IsNullOrEmpty(firstnameFieldName) || (string.IsNullOrEmpty(middlenameFieldName) || string.IsNullOrEmpty(birth_dateFieldName) || string.IsNullOrEmpty(statusFieldName) || string.IsNullOrEmpty(addressFieldName) || string.IsNullOrEmpty(issuedFieldName) || string.IsNullOrEmpty(issued1FieldName) || string.IsNullOrEmpty(validuntilFieldName))))
                    {
                        MessageBox.Show("One or more form fields are missing in the template PDF.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Use more specific field setting methods if available
                    fields[lastnameFieldName].SetValue(last_name);
                    fields[firstnameFieldName].SetValue(first_name);
                    fields[middlenameFieldName].SetValue(middle_name);
                    fields[addressFieldName].SetValue(address);
                    fields[birth_dateFieldName].SetValue(birth_date);
                    fields[statusFieldName].SetValue(status);
                    fields[issuedFieldName].SetValue(issued);
                    fields[issued1FieldName].SetValue(issued1);
                    fields[validuntilFieldName].SetValue(ValidUntil);

                    form.FlattenFields();
                }

                MessageBox.Show("PDF filled and saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (PdfException pdfEx)
            {
                // Log the error for debugging
                // Provide more specific error messages based on the exception
                MessageBox.Show($"A PDF error occurred while filling the PDF: {pdfEx.Message}", "PDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioEx)
            {
                // Log the error for debugging
                // Provide more specific error messages based on the exception
                MessageBox.Show($"An IO error occurred while filling the PDF: {ioEx.Message}", "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                // Provide more specific error messages based on the exception
                MessageBox.Show($"An unknown error occurred while filling the PDF: {ex.Message}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            clear();
            this.Close();
        }

        private void dtIssued_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
