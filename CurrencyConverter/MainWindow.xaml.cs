using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;

namespace CurrencyConverter
{

    public partial class MainWindow : Window
    {

        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adapter = new SqlDataAdapter();

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
            getData();
        }

        //used to open Connection to DB 
        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open();
        }

        
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrency.Focus();
                return;

            }
            else if(cmbFromCurrency.SelectedValue == null||cmbFromCurrency.SelectedIndex== 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            if(cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3"); 
            }
            else
            {
                ConvertedValue= (double.Parse(cmbFromCurrency.SelectedValue.ToString()))*double.Parse(txtCurrency.Text)/double.Parse(cmbToCurrency.SelectedValue.ToString());
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }



        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        //Clears from the main tab
        private void ClearAll()
        {
            txtCurrency.Text = String.Empty;
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

       //checks textbox values that are entered
        private void NumberValidationTextBox(Object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    //fills values data
        private void BindCurrency()
        {
            mycon();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("select Id, CurrencyName from Currency",con);
            cmd.CommandType = CommandType.Text;
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            DataRow dataRow = dt.NewRow();
            dataRow["Id"] = 0;
            dataRow["CurrencyName"] = "SELECT";
            dt.Rows.InsertAt(dataRow, 0);
            if(dt != null && dt.Rows.Count > 0)
            {
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;
       
            cmbToCurrency.DisplayMemberPath="CurrencyName";
            cmbToCurrency.SelectedValuePath="Id";
            cmbToCurrency.SelectedIndex = 0;
        }

  
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(txtAmmount == null || txtAmmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount","Information",MessageBoxButton.OK,MessageBoxImage.Information);
                    txtAmmount.Focus();
                    return;
                }
                else if(txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    if (CurrencyId > 0)
                    {
                        if(MessageBox.Show("Are you sure you want to update?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question)== MessageBoxResult.Yes)
                        {
                            mycon();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE CURRENCY SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id",con);
                            cmd.CommandType= CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id",CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();
                            MessageBox.Show("Data Updated Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to save?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            
                            cmd = new SqlCommand("INSERT INTO Currency(Amount, CurrencyName) Values(@Amount, @CurrencyName)",con);
                            cmd.CommandType=CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount",double.Parse(txtAmmount.Text));
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearData();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Erro",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void ClearData()
        {
            try
            {
                txtAmmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                getData();
                CurrencyId = 0;
                BindCurrency();
                txtAmmount.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //fills management table
        private void getData()
        {
            mycon();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("SELECT * FROM Currency", con);
            cmd.CommandType = CommandType.Text;
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
                dgCurrency.ItemsSource = dt.DefaultView;
            else
                dgCurrency.ItemsSource = null;
            con.Close();
        }

        private void dgCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;
                DataRowView row_Selected = grd.CurrentItem as DataRowView;
                if (row_Selected != null)
                {
                    if (dgCurrency.Items.Count > 0)
                    {
                        if(grd.SelectedCells.Count > 0)
                        {
                            CurrencyId = Int32.Parse(row_Selected["Id"].ToString());
                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                txtAmmount.Text = row_Selected["Amount"].ToString();
                                txtCurrencyName.Text = row_Selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";
                            }
                            if(grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                if(MessageBox.Show("Are you sure you want to delete?","Information",MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    mycon();
                                    DataTable dt =new DataTable();
                                    cmd = new SqlCommand("DELETE FROM Currency WHERE id = @Id", con);
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id",CurrencyId);
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearData();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearData();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void txtAmmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }

}

