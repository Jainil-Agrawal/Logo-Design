using GeaDesign001.Core.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Logo
{
    public partial class MainWindow : Window
    {
        public async Task LoadLogosFromDatabase()
        {
            try
            {
                DataTable dt = await SqlHelper.ExecuteDataTableAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "sp_GetAllLogos"
                );

                LogoList.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    string? imgPath = row["SelectedColor"].ToString();
                    if (!File.Exists(imgPath))
                        continue;

                    Image img = new ()
                    {
                        Source = new BitmapImage(new Uri(imgPath)),
                        Width = Convert.ToDouble(row["Width"]),
                        Height = Convert.ToDouble(row["Height"]),
                        Tag = row["Id"]
                    };

                    Canvas.SetLeft(img, Convert.ToDouble(row["PositionX"]));
                    Canvas.SetTop(img, Convert.ToDouble(row["PositionY"]));

                    float angle = Convert.ToSingle(row["Angle"]);
                    img.RenderTransform = new RotateTransform(angle)
                    {
                        CenterX = img.Width / 2,
                        CenterY = img.Height / 2
                    };

                    DesignCanvas.Children.Add(img);

                    LogoItem item = new ()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["LogoName"].ToString(),
                        ImagePath = imgPath,
                        ImageControl = img,
                        X = Convert.ToSingle(row["PositionX"]),
                        Y = Convert.ToSingle(row["PositionY"]),
                        W = Convert.ToSingle(row["Width"]),
                        H = Convert.ToSingle(row["Height"]),
                        Angle = angle
                    };

                    LogoDesign logoDesign = new ()
                    {
                        LogoName = row["LogoName"].ToString(),
                        DefaultImagePath = row["DefaultImagePath"].ToString(),
                        RedImagePath = row["RedImagePath"].ToString(),
                        BlueImagePath = row["BlueImagePath"].ToString(),
                        GreenImagePath = row["GreenImagePath"].ToString(),
                        SelectedColor = imgPath,
                        PositionX = Convert.ToSingle(row["PositionX"]),
                        PositionY = Convert.ToSingle(row["PositionY"]),
                        Width = Convert.ToSingle(row["Width"]),
                        Height = Convert.ToSingle(row["Height"]),
                        Angle = angle,
                        Id = Convert.ToInt32(row["Id"])
                    };

                    LogoList.Items.Add(item);
                    Images.Add(logoDesign);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logos: " + ex.Message);
            }
        }

        private void Open_File(object sender, RoutedEventArgs e)
        {
            string logoName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter Logo Name:",
                "Logo Name",
                "Logo " + (LogoList.Items.Count + 1)
            );

            if (string.IsNullOrWhiteSpace(logoName))
            {
                return;
            }

            LogoDesign logoImages = new ()
            {
                LogoName = logoName
            };

            OpenFileDialog dlg = new ()
            {
                Filter = "PNG Image (*.png)|*.png|All Files (*.*)|*.*",

                Title = "Select DEFAULT image"
            };
            if (dlg.ShowDialog() == true)
            {
                logoImages.DefaultImagePath = dlg.FileName;
                logoImages.SelectedColor = dlg.FileName;
            }
            else
            {
                return;
            }

            dlg.Title = "Select RED image";
            if (dlg.ShowDialog() == true)
                logoImages.RedImagePath = dlg.FileName;
            else
                return;

            dlg.Title = "Select BLUE image";
            if (dlg.ShowDialog() == true)
                logoImages.BlueImagePath = dlg.FileName;
            else
                return;

            dlg.Title = "Select GREEN image";
            if (dlg.ShowDialog() == true)
                logoImages.GreenImagePath = dlg.FileName;
            else
                return;
            logoImages.Angle = 0;
            logoImages.Height = 50;
            logoImages.Width = 50;
            logoImages.PositionX = 50;
            logoImages.PositionY = 50;
            AddLogoToCanvas(logoImages);
        }

        private void AddLogoToCanvas(LogoDesign? logo)
        {
            if (rotateTimer != null)
            {
                rotateTimer.Stop();
                rotateTimer = null;
            }
            BitmapImage bitmap = new (new Uri(logo.DefaultImagePath));

            int insertedId = Convert.ToInt32(
                SqlHelper.ExecuteScalar(
                    connectionString,
                    CommandType.StoredProcedure,
                    "sp_InsertLogoDesign",
                    new SqlParameter("@LogoName", logo.LogoName),
                    new SqlParameter("@DefaultImagePath", logo.DefaultImagePath),
                    new SqlParameter("@RedImagePath", logo.RedImagePath),
                    new SqlParameter("@BlueImagePath", logo.BlueImagePath),
                    new SqlParameter("@GreenImagePath", logo.GreenImagePath),
                    new SqlParameter("@SelectedColor", logo.SelectedColor),
                    new SqlParameter("@PositionX", logo.PositionX),
                    new SqlParameter("@PositionY", logo.PositionY),
                    new SqlParameter("@Width", logo.Width),
                    new SqlParameter("@Height", logo.Height),
                    new SqlParameter("@Angle", logo.Angle)
                )
            );
            Image logos = new ()
            {
                Source = bitmap,
                Width = 50,
                Height = 50,
                Tag = insertedId
            };
            LogoItem item = new ()
            {
                Id = insertedId,
                Name = logo.LogoName,
                ImagePath = logo.SelectedColor,
                ImageControl = logos,
                X = 50,
                Y = 50,
                W = 50,
                H = 50,
                Angle = 0
            };
            LogoList.Items.Add(item);
            logo.Id = insertedId;
            Images.Add(logo);
            logos.MouseLeftButtonDown += Logo_MouseDown;
            logos.MouseMove += Logo_MouseMove;
            logos.MouseLeftButtonUp += Logo_MouseUp;

            Canvas.SetLeft(logos, 50);
            Canvas.SetTop(logos, 50);

            DesignCanvas.Children.Add(logos);
            LogoList.SelectedItem = LogoList.Items[^1];
            selectImage = logos;
            rotateImages = [];
            img.Text = selectImage.Height.ToString();
            speed.Text = rotateSpeed.ToString();
            angle.Text = "0";
        }

        private Image? GetCanvasImage(Image sourceImage)
        {
            return DesignCanvas.Children
                    .OfType<Image>()
                    .FirstOrDefault(x => x.Tag != null && x.Tag.Equals(sourceImage.Tag));
        }
    }
}
