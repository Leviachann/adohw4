using System.Data;
using System.Windows;
using System;

public partial class ProductsPage : Window
{
    private SqlConnection connection;
    private string connectionString = "YourConnectionString";

    public ProductsPage()
    {
        InitializeComponent();

        connection = new SqlConnection(connectionString);

        LoadBrands();
        LoadProducts();
    }

    private void LoadBrands()
    {
        try
        {
            string query = "SELECT DISTINCT BrandName FROM Cars";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable brandsTable = new DataTable();
            adapter.Fill(brandsTable);

            cmbBrand.ItemsSource = brandsTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error occurred while loading brands: " + ex.Message);
        }
    }

    private void LoadProducts()
    {
        try
        {
            string query = "SELECT * FROM Cars";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable productsTable = new DataTable();
            adapter.Fill(productsTable);

            dgProducts.ItemsSource = productsTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error occurred while loading products: " + ex.Message);
        }
    }

    private void btnShow_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string brandFilter = cmbBrand.SelectedValue?.ToString();
            bool isNewCarsFilter = chkNewCars.IsChecked ?? false;
            bool isOldCarsFilter = chkOldCars.IsChecked ?? false;
            string carTypeFilter = cmbCarType.Text;
            int? minMileage = ParseNullableInt(txtMinMileage.Text);
            int? maxMileage = ParseNullableInt(txtMaxMileage.Text);
            DateTime? minProductionDate = dpMinProductionDate.SelectedDate;
            DateTime? maxProductionDate = dpMaxProductionDate.SelectedDate;
            string colorFilter = cmbColor.Text;
            double? minPrice = ParseNullableDouble(txtMinPrice.Text);
            double? maxPrice = ParseNullableDouble(txtMaxPrice.Text);
            string fuelTypeFilter = cmbFuelType.Text;

            string query = "SELECT * FROM Cars WHERE 1 = 1";

            if (!string.IsNullOrEmpty(brandFilter))
            {
                query += " AND Brand = @Brand";
            }

            if (isNewCarsFilter && !isOldCarsFilter)
            {
                query += " AND IsNew = 1";
            }
            else if (!isNewCarsFilter && isOldCarsFilter)
            {
                query += " AND IsNew = 0";
            }

            if (!string.IsNullOrEmpty(carTypeFilter))
            {
                query += " AND CarType = @CarType";
            }

            if (minMileage.HasValue)
            {
                query += " AND Mileage >= @MinMileage";
            }

            if (maxMileage.HasValue)
            {
                query += " AND Mileage <= @MaxMileage";
            }

            if (minProductionDate.HasValue)
            {
                query += " AND ProductionDate >= @MinProductionDate";
            }

            if (maxProductionDate.HasValue)
            {
                query += " AND ProductionDate <= @MaxProductionDate";
            }

            if (!string.IsNullOrEmpty(colorFilter))
            {
                query += " AND Color = @Color";
            }

            if (minPrice.HasValue)
            {
                query += " AND Price >= @MinPrice";
            }

            if (maxPrice.HasValue)
            {
                query += " AND Price <= @MaxPrice";
            }

            if (!string.IsNullOrEmpty(fuelTypeFilter))
            {
                query += " AND FuelType = @FuelType";
            }

            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            adapter.SelectCommand.Parameters.AddWithValue("@Brand", brandFilter);
            adapter.SelectCommand.Parameters.AddWithValue("@CarType", carTypeFilter);
            adapter.SelectCommand.Parameters.AddWithValue("@MinMileage", minMileage ?? (object)DBNull.Value);
            adapter.SelectCommand.Parameters.AddWithValue("@MaxMileage", maxMileage ?? (object)DBNull.Value);
            adapter.SelectCommand.Parameters.AddWithValue("@MinProductionDate", minProductionDate ?? (object)DBNull.Value);
            adapter.SelectCommand.Parameters.AddWithValue("@MaxProductionDate", maxProductionDate ?? (object)DBNull.Value);
            adapter.SelectCommand.Parameters.AddWithValue("@Color", colorFilter);
            adapter.SelectCommand.Parameters.AddWithValue("@MinPrice", minPrice ?? (object)DBNull.Value);
            adapter.SelectCommand.Parameters.AddWithValue("@MaxPrice", maxPrice ?? (object)DBNull.Value);
            adapter.SelectCommand.Parameters.AddWithValue("@FuelType", fuelTypeFilter);

            DataTable filteredProductsTable = new DataTable();
            adapter.Fill(filteredProductsTable);

            dgProducts.ItemsSource = filteredProductsTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error occurred while filtering products: " + ex.Message);
        }
    }

    private int? ParseNullableInt(string value)
    {
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        return null;
    }

    private double? ParseNullableDouble(string value)
    {
        if (double.TryParse(value, out double result))
        {
            return result;
        }
        return null;
    }
}
