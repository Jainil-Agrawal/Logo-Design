using Azure;
using GeaDesign001.Core.Helper;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Logo
{
    public partial class MainWindow : Window
    {
        private void AddTextBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            string textBoxName = Microsoft.VisualBasic.Interaction.InputBox(
                            "Enter TextBox Label:",
                            "TextBox Label", "Label " + (TextBoxList.Items.Count + 1)
                                );
            Grid container = new ()
            {
                Width = 150,
                Height = 25,
                Margin = new Thickness(0)
            };
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Label lbl = new ()
            {
                Content = textBoxName,
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold
            };

            Grid.SetColumn(lbl, 0);
            container.Children.Add(lbl);
            TextBox tb = new ()
            {
                Text = "TextBox " + (TextBoxList.Items.Count + 1),
                AcceptsReturn = false,
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextWrapping = TextWrapping.Wrap
            };

            TextBoxes tbs = new ()
            {
                TextBoxName = textBoxName,
                Width = 150,
                Height = 25,
                Text = tb.Text,
                PositionX = 50,
                PositionY = 50,
            };

            Canvas.SetLeft(container, 50);
            Canvas.SetTop(container, 50);

            int insertedId = Convert.ToInt32(
              SqlHelper.ExecuteScalar(
                  connectionString,
                  CommandType.StoredProcedure,
                  "sp_InsertTextBox",
                  new SqlParameter("@TextBoxName", tbs.TextBoxName),
                  new SqlParameter("@Text", tbs.Text),
                  new SqlParameter("@PositionX", tbs.PositionX),
                  new SqlParameter("@PositionY", tbs.PositionY),
                  new SqlParameter("@Width", tbs.Width),
                  new SqlParameter("@Height", tbs.Height)
              )
          );

            Panel.SetZIndex(container, 1001);
            container.Tag = insertedId;
            container.PreviewMouseLeftButtonDown += Tb_PreviewMouseLeftButtonDown;
            container.PreviewMouseMove += Tb_PreviewMouseMove;
            container.PreviewMouseLeftButtonUp += Tb_PreviewMouseLeftButtonUp;
            tbs.Id = insertedId;

            Container item = new ()
            {
                Id = insertedId,
                Name = tbs.TextBoxName,
                TextControl = container,
                X = 50,
                Y = 50,
                W = 150,
                H = 25
            };
            
            TextBoxList.Items.Add(item);
            TextBoxList.SelectedItem = item;
            Grid.SetColumn(tb, 1);
            container.Children.Add(tb);
            DesignCanvas.Children.Add(container);
            selectContainer = container;
            TextBoxes.Add(tbs);
            txt.Text = selectContainer.Height.ToString();
            txtW.Text = selectContainer.Width.ToString();
        }

        private void Tb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedContainer = sender as Grid;

            TextBox? tb = null;
            foreach (var child in selectedContainer.Children)
            {
                if (child is TextBox)
                {
                    tb = child as TextBox;
                    break;
                }
            }


            if (TextBoxList.SelectedItem is not Container selectedItem || selectedItem.TextControl != selectedContainer)
            {
                if (tb != null)
                {
                    tb.IsReadOnly = true;
                }
            }
            else
            {
                if (e.ClickCount == 2 && tb != null)
                {
                    tb.IsReadOnly = false;
                    tb.Focus();
                    tb.CaretIndex = tb.Text.Length;
                    return;
                }
            }

            clickPosition = e.GetPosition(DesignCanvas);

            double left = Canvas.GetLeft(selectedContainer);
            double top = Canvas.GetTop(selectedContainer);

            offsetX = clickPosition.X - left;
            offsetY = clickPosition.Y - top;

            selectedContainer.CaptureMouse();
            e.Handled = true;
        }

        private void Tb_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (selectedContainer == null || !selectedContainer.IsMouseCaptured)
                return;
            if (selectedContainer == selectContainer)
            {
                Point mousePos = e.GetPosition(DesignCanvas);

                double newLeft = mousePos.X - offsetX;
                double newTop = mousePos.Y - offsetY;

                Canvas.SetLeft(selectedContainer, newLeft);
                Canvas.SetTop(selectedContainer, newTop);

                e.Handled = true;
            }
        }

        private void Tb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            selectedContainer?.ReleaseMouseCapture();
            selectedContainer = null;
            e.Handled = true;
        }

        private void TextBox_Height(object sender, RoutedEventArgs e)
        {
            if (selectContainer == null)
            {
                MessageBox.Show("Select a Container first.");
                return;
            }

            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();

            if (selectContainer == null) return;

            if (tag == "+")
            {
                selectContainer.Height += 2;
            }
            else
            {
                selectContainer.Height -= 2;
            }
            txt.Text = selectContainer.Height.ToString();

        }

        private void TextBox_Width(object sender, RoutedEventArgs e)
        {
            if (selectContainer == null)
            {
                MessageBox.Show("Select a Container first.");
                return;
            }

            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();

            if (selectContainer == null) return;

            if (tag == "+")
            {
                selectContainer.Width += 2;
            }
            else
            {
                selectContainer.Width -= 2;
            }
            txtW.Text = selectContainer.Width.ToString();
        }

        private void Edit_Label(object sender, RoutedEventArgs e)
        {
            if (selectContainer == null)
            {
                MessageBox.Show("Select a Container first.");
                return;
            }

            Label? lbl = null;
            foreach (var child in selectContainer.Children)
            {
                if (child is Label)
                {
                    lbl = child as Label;
                    break;
                }
            }

            string textBoxName = Microsoft.VisualBasic.Interaction.InputBox("Edit Label",
                                       "Enter Label", lbl.Content.ToString()
                            );
            lbl.Content = textBoxName;

            if (TextBoxList.SelectedItem is Container selectedItem && selectedItem.TextControl == selectContainer)
            {
                selectedItem.Name = textBoxName;
                TextBoxList.Items.Refresh();
            }
        }

        private void Collapse_Label(object sender, RoutedEventArgs e)
        {
            if (selectContainer == null)
            {
                MessageBox.Show("Select a Container first.");
                return;
            }

            Label? lbl = null;
            foreach (var child in selectContainer.Children)
            {
                if (child is Label)
                {
                    lbl = child as Label;
                    break;
                }
            }
            if (lbl?.Visibility != Visibility.Visible)
            {
                lbl.Visibility = Visibility.Visible;
            }
            else
            {
                lbl.Visibility = Visibility.Collapsed;
            }
        }

        private void Collapse_AllLabel(object sender, RoutedEventArgs e)
        {
            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();
            foreach (var child in DesignCanvas.Children)
            {
                if (child is Grid grid)
                {
                    Label? lbl = null;
                    foreach (var item in grid.Children)
                    {
                        if (item is Label)
                        {
                            lbl = item as Label;
                        }
                    }
                    if (tag == "collapse")
                    {
                        lbl.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        lbl.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void HideShowTxtbox_Click(object sender, RoutedEventArgs e)
        {
            if (selectContainer == null)
            {
                MessageBox.Show("Select a Container first.");
                return;
            }

            if (selectContainer.Visibility == Visibility.Hidden)
            {
                selectContainer.Visibility = Visibility.Visible;
            }
            else
            {
                selectContainer.Visibility = Visibility.Hidden;
            }
        }

        private void HideAllTxtbox_Click(object sender, RoutedEventArgs e)
        {
            string? tag = (sender as Button)?.Tag?.ToString()?.ToLower();
            foreach (var child in DesignCanvas.Children)
            {
                if (child is Grid)
                {
                    Grid? container = child as Grid;
                    if (tag == "hide")
                    {
                        container.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        container.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        //private void HighlightContainer(Grid? container)
        //{
        //    if (container == null)
        //        return;
        //    TextBox? tb = null;
        //    foreach (var child in container.Children)
        //    {
        //        if (child is TextBox)
        //        {
        //            tb = child as TextBox;
        //            break;
        //        }
        //    }

        //    tb.CaretIndex = tb.Text.Length;
        //    tb.Focus();
        //    foreach (var child in DesignCanvas.Children)
        //    {
        //        if (child is Grid grid)
        //        {
        //            grid.Margin = new Thickness(0);
        //        }
        //    }
        //    container.Margin = new Thickness(5);

        //}
    }
}

