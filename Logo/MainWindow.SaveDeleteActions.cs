using GeaDesign001.Core.Helper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Logo
{
    public partial class MainWindow : Window
    {
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (rotateTimer != null && rotateTimer.IsEnabled)
            {
                StopRotation();
                UpdateSelectionMode();
            }
            foreach (var item in LogoList.Items.Cast<LogoItem>())
            {
                var logoItem = Images.FirstOrDefault(x => x.Id == item.Id);
                if (logoItem == null) continue;

                var canvasImg = GetCanvasImage(item.ImageControl);
                if (canvasImg == null) continue;

                double x = Canvas.GetLeft(canvasImg);
                double y = Canvas.GetTop(canvasImg);
                double w = canvasImg.Width;
                double h = canvasImg.Height;
                double angle = 0;
                if (canvasImg.RenderTransform is RotateTransform rt)
                    angle = rt.Angle;

                string? selectedColorPath = logoItem.SelectedColor;

                SqlHelper.ExecuteNonQuery(
                    connectionString,
                    CommandType.StoredProcedure,
                    "sp_UpdateLogoDesign",
                    new SqlParameter("@Id", logoItem.Id),
                    new SqlParameter("@SelectedColor", selectedColorPath ?? (object)DBNull.Value),
                    new SqlParameter("@PositionX", x),
                    new SqlParameter("@PositionY", y),
                    new SqlParameter("@Width", w),
                    new SqlParameter("@Height", h),
                    new SqlParameter("@Angle", angle)
                );
            }
            foreach (var item in TextBoxList.Items.Cast<Container>())
            {
                var container = TextBoxes.FirstOrDefault(x => x.Id == item.Id);
                if (container == null) continue;

                var container2 = DesignCanvas.Children
                    .OfType<Grid>()
                    .FirstOrDefault(x => x.Tag != null && x.Tag.Equals(item.TextControl.Tag));
                if (container2 == null) continue;

                double x = Canvas.GetLeft(container2);
                double y = Canvas.GetTop(container2);
                double w = container2.Width;
                double h = container2.Height;
                TextBox? tb = null;
                foreach (var child in container2.Children)
                {
                    if (child is TextBox)
                    {
                        tb = child as TextBox;
                        break;
                    }
                }
                string t = tb.Text;
                SqlHelper.ExecuteNonQuery(
                   connectionString,
                   CommandType.StoredProcedure,
                   "sp_UpdateTextBox",
                    new SqlParameter("@Id", item.Id),
                    new SqlParameter("@TextBoxName", item.Name),
                    new SqlParameter("@Text", t),
                    new SqlParameter("@PositionX", x),
                    new SqlParameter("@PositionY", y),
                    new SqlParameter("@Width", w),
                    new SqlParameter("@Height", h)
               );
            }

            MessageBox.Show("All logos and TextBoxes saved successfully!");
        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (selectImage == null)
            {
                MessageBox.Show("Select an image first.");
                return;
            }

            if (selectImage == null) return;
            MessageBoxResult result = MessageBox.Show("Are You sure, You want to delete this Image??", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                int index = LogoList.SelectedItems
                            .Cast<LogoItem>()
                            .ToList()
                            .FindIndex(x => x.Id.ToString() == selectImage.Tag.ToString());
                var selectedItem = LogoList.SelectedItems[index] as LogoItem;
                DesignCanvas.Children.Remove(selectImage);
                LogoList.Items.Remove(selectedItem);
                var logoItem = Images.FirstOrDefault(x => x.Id == selectedItem?.Id);
                Images.Remove(logoItem);
                SqlHelper.ExecuteNonQuery(
                       connectionString,
                       CommandType.StoredProcedure,
                       "sp_DeleteLogoDesign",
                       new SqlParameter("@Id", logoItem.Id)
                   );
                MessageBox.Show("Logo Deleted Successfully");
            }
        }

        private void Button_DeleteTextBox(object sender, RoutedEventArgs e)
        {
            if (selectContainer == null)
            {
                MessageBox.Show("Select a textbox first.");
                return;
            }

            if (selectContainer == null) return;
            MessageBoxResult result = MessageBox.Show("Are You sure, You want to delete this textbox??", "Delete TextBox", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                int index = TextBoxList.SelectedItems
                        .Cast<Container>()
                        .ToList()
                        .FindIndex(x => x.Id.ToString() == selectContainer.Tag.ToString());

                var selectedItem = TextBoxList.SelectedItems[index] as Container;

                DesignCanvas.Children.Remove(selectContainer);

                TextBoxList.Items.Remove(selectedItem);
                var textItem = TextBoxes.FirstOrDefault(x => x.Id == selectedItem?.Id);
                TextBoxes.Remove(textItem);
                SqlHelper.ExecuteNonQuery(
                       connectionString,
                       CommandType.StoredProcedure,
                       "sp_DeleteTextBox",
                       new SqlParameter("@Id", textItem.Id)
                   );
                MessageBox.Show("TextBox Deleted Successfully");
            }
        }
    }
}
