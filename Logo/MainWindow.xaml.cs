using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Logo
{
    public partial class MainWindow : Window
    {

        private Image? selectedImage;
        private Grid? selectedContainer;
        private Grid? selectContainer;
        private Point clickPosition;
        private Image? selectImage;
        private List<Image> rotateImages = [];
        private List<LogoDesign> Images = [];
        private List<TextBoxes> TextBoxes = [];
        private DispatcherTimer? rotateTimer;
        private float currentAngle = 0;
        private int rotateSpeed = 250;
        private double offsetX;
        private double offsetY;
        private string connectionString = "server=DESKTOP-QI77AJ8\\SQLEXPRESS;database=LogoControl;Trusted_connection=True;Encrypt=False;";
        public MainWindow()
        {
            InitializeComponent();
            _ = LoadLogosFromDatabase();
            _ = LoadTextBoxesFromDatabase();
            UpdateSelectionMode();
        }
    }
}
