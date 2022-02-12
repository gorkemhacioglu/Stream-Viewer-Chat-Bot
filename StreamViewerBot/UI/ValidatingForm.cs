using System.Windows.Forms;

namespace StreamViewerBot.UI
{
    public partial class ValidatingForm : Form
    {
        public ProgressBar ProgressBar;

        public ValidatingForm()
        {
            InitializeComponent();
            ProgressBar = progressBar;
        }
    }
}