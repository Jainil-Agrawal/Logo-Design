using GeaDesign001.Core.Helper;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Logo
{
    public partial class MainWindow : Window
    {
        public async Task LoadTextBoxesFromDatabase()
        {
            try
            {
                DataTable dt = await SqlHelper.ExecuteDataTableAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "sp_GetAllTextBoxes"
                );

                foreach (DataRow row in dt.Rows)
                {
                    Grid container = new ()
                    {
                        Width = Convert.ToDouble(row["Width"]),
                        Height = Convert.ToDouble(row["Height"]),
                        Tag = row["Id"],
                        Margin = new Thickness(0)
                    };

                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });     
                    container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); 

                    Label lbl = new () {
                        Content = row["TextBoxName"].ToString(),
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Bold
                    };
                    
                    Grid.SetColumn(lbl, 0);
                    container.Children.Add(lbl);
                    TextBox tb = new ()
                    {
                        Text = row["Text"].ToString(),
                        Width = double.NaN,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        TextWrapping = TextWrapping.Wrap
                    };
                    Grid.SetColumn(tb, 1);
                    
                    container.Children.Add(tb);
                    Canvas.SetLeft(container, Convert.ToDouble(row["PositionX"]));
                    Canvas.SetTop(container, Convert.ToDouble(row["PositionY"]));

                    container.PreviewMouseLeftButtonDown += Tb_PreviewMouseLeftButtonDown;
                    container.PreviewMouseMove += Tb_PreviewMouseMove;
                    container.PreviewMouseLeftButtonUp += Tb_PreviewMouseLeftButtonUp;
                    Panel.SetZIndex(container, 1001);
                    
                    DesignCanvas.Children.Add(container);
                    TextBoxes textbox = new ()
                    {
                        TextBoxName = row["TextBoxName"].ToString(),
                        Text = row["Text"].ToString(),
                        PositionX = Convert.ToSingle(row["PositionX"]),
                        PositionY = Convert.ToSingle(row["PositionY"]),
                        Width = Convert.ToSingle(row["Width"]),
                        Height = Convert.ToSingle(row["Height"]),
                        Id = Convert.ToInt32(row["Id"])
                    };

                    TextBoxes.Add(textbox);

                    Container item = new ()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["TextBoxName"].ToString(),
                        TextControl = container,
                        X = Convert.ToSingle(row["PositionX"]),
                        Y = Convert.ToSingle(row["PositionY"]),
                        W = Convert.ToSingle(row["Width"]),
                        H = Convert.ToSingle(row["Height"])
                    };
                    TextBoxList.Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logos: " + ex.Message);
            }
        }
    }
}
