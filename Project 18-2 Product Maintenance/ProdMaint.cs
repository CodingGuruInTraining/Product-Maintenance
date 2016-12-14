// This program connects to a data source and displays the table's data in
// individual fields. Users can modify the entries.
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

namespace Project_18_2_Product_Maintenance
{
    public partial class ProdMaint : Form
    {
        public ProdMaint()
        {
            InitializeComponent();
        }
        private bool newEntry = false;
        private void productsBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            // Adds exception handling
            try
            {
                // Checks if saving a new entry or a modification
                if (newEntry == true)
                {
                    // Stores textbox value
                    string prodVal = productCodeTextBox.Text;
                    // Validates product code if it is a new entry
                    if (!checkEmpty(prodVal, "Product Code") && checkLength(prodVal, "Product Code"))
                        return;
                    // Removes access to the product code field
                    prodCodeToggle(2);
                }
                // Stores textbox values
                string nameVal = nameTextBox.Text;
                string versVal = versionTextBox.Text;
                string dateVal = releaseDateTextBox.Text;
                // Validates inputs
                if (checkEmpty(nameVal, "Name") && checkLength(nameVal, "Name", 50) && checkNumber(versVal, "Version") && checkDate(dateVal, "Date"))
                {
                    // Auto-populated code
                    this.Validate();
                    this.productsBindingSource.EndEdit();
                    this.tableAdapterManager.UpdateAll(this.techSupport_DataDataSet);
                }
            }
            // Catch for concurrency errors
            catch (DBConcurrencyException err)
            {
                MessageBox.Show("Database error # " + err.ToString() + ": " + err.Message, err.GetType().ToString());
                this.productsTableAdapter.Fill(this.techSupport_DataDataSet.Products);
            }
            // Catch for empty fields
            catch (NoNullAllowedException err)
            {
                MessageBox.Show("Database error # " + err.ToString() + ": " + err.Message, err.GetType().ToString());
            }
            // Catch for general ADO.NET errors
            catch (DataException err)
            {
                MessageBox.Show(err.Message, err.GetType().ToString());
                productsBindingSource.CancelEdit();
            }
        }

        private void ProdMaint_Load(object sender, EventArgs e)
        {
            // Adds exception handling
            try
            { 
                // TODO: This line of code loads data into the 'techSupport_DataDataSet.Products' table. You can move, or remove it, as needed.
                this.productsTableAdapter.Fill(this.techSupport_DataDataSet.Products);
            }
            // Catch for SQL errors
            catch (SqlException err)
            {
                MessageBox.Show("Database error # " + err.Number + ": " + err.Message, err.GetType().ToString());
            }
            // Catch for unknown errors
            catch (Exception)
            {
                MessageBox.Show("Database error # unknown");
            }
        }

        private void fillByProdCodeToolStripButton_Click(object sender, EventArgs e)
        {
            // Adds exception handling
            try
            {
                // Stores textbox value
                string prodCode = productCodeToolStripTextBox.Text;
                // Validates product code search entry
                if (checkEmpty(prodCode, "Product Code") && checkLength(prodCode, "Product Code"))
                {
                    // Fills table adapter with one result line
                    this.productsTableAdapter.FillByProdCode(this.techSupport_DataDataSet.Products, productCodeToolStripTextBox.Text);                   
                }
            }
            // Catch for all exceptions
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }
        //private void fillBy_productCodeToolStripButton_Click(object sender, EventArgs e)
        //{
        //    // Adds exception handling
        //    try
        //    {
        //        // Stores textbox value
        //        string prodCode = productCodeToolStripTextBox.Text;
        //        // Validates product code search entry
        //        if (checkEmpty(prodCode, "Product Code") && checkLength(prodCode, "Product Code"))
        //        {
        //            // Fills table adapter with one result line
        //            //this.productsTableAdapter.FillByProdCode(this.techSupport_DataDataSet.Products, productCodeToolStripTextBox.Text);
        //            this.productsTableAdapter.FillBy_productCode(this.techSupport_DataDataSet.Products, productCodeToolStripTextBox.Text);
        //            //this.productsTableAdapter.Fill(this.techSupport_DataDataSet.Products);
        //        }
        //    }
        //    // Catch for all exceptions
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, ex.GetType().ToString());
        //    }   
        //}

        private bool checkEmpty(string value, string vname)
        {
            // Checks if the textboxes are empty
            if (value == "")
            {
                MessageBox.Show("Please enter a value in the " + vname + " field.");
                return false;
            }
            else
                return true;
        }
        private bool checkNumber(string value, string vname)
        {
            // Checks if textbox value can be converted correctly
            decimal val;
            bool result = Decimal.TryParse(value, out val);

            if (result == true)
                return true;
            else
            {
                MessageBox.Show("Please only enter valid numbers in the " + vname + " field.");
                return false;
            }
        }
        private bool checkLength(string value, string vname, int limit = 10)
        {
            // Checks if textbox value is within the specified range.
            int number = value.Length;
            if (number > limit)
            {
                MessageBox.Show("The " + vname + " needs to be " + limit.ToString() + " or less characters.");
                return false;
            }
            return true;
        }

        private bool checkDate(string value, string vname)
        {
            DateTime aDate;
            bool result = DateTime.TryParse(value, out aDate);
            if (result == false)
            {
                MessageBox.Show("The " + vname + " field needs to be a valid date.");
                return false;
            }
            return true;
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            newEntry = true;
            prodCodeToggle(1);
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            prodCodeToggle(2);
        }

        private void prodCodeToggle(byte choice)
        {
            // Changes properties of product code textbox that allows the user to enter
            // text into it or restrict access.
            switch (choice)
            {
                case 1: // Grant access
                    productCodeTextBox.ReadOnly = false;
                    productCodeTextBox.TabStop = true;
                    productCodeTextBox.Focus();
                    break;
                case 2: // Remove access
                    productCodeTextBox.ReadOnly = true;
                    productCodeTextBox.TabStop = false;
                    nameTextBox.Focus();
                    newEntry = false;
                    break;
            }              
        }

        //private void fillByProductCodeToolStripButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        this.productsTableAdapter.FillByProductCode(this.techSupport_DataDataSet.Products, productCodeToolStripTextBox1.Text);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(ex.Message);
        //    }

        //}
    }
}
