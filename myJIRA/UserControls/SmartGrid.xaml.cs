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

namespace myJIRA.UserControls
{
    /// <summary>
    /// Interaction logic for SmartGrid.xaml
    /// </summary>
    public partial class SmartGrid : UserControl
    {
        
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SmartGrid));
        
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public static readonly DependencyProperty SizeDefinitionsProperty = DependencyProperty.Register("SizeDefinitions", typeof(List<SizeDefinition>), typeof(SmartGrid));

        public List<SizeDefinition> SizeDefinitions
        {
            get
            {
                return (List<SizeDefinition>)GetValue(SizeDefinitionsProperty);
            }
            set
            {
                SetValue(SizeDefinitionsProperty, value);
            }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(List<UIElement>), typeof(SmartGrid));

        public List<UIElement> Content
        {
            get
            {
                return (List<UIElement>)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }


        public SmartGrid()
        {
            InitializeComponent();

            SizeDefinitions = new List<SizeDefinition>();
            Content = new List<UIElement>();
        }

        private DateTime? lastUpdated = null;

        private void CreateLayout()
        {
            try
            {
                var now = DateTime.Now;

                if (lastUpdated != null) //&& (now - lastUpdated.Value).Seconds < 3)
                    return;

                CreateKnownAxis();

                /*
                List<UIElement> items = new List<UIElement>();

                for (int i = 0; i < 100; i++)
                {
                    Label lbl = new Label() { Content = "Tests " + i, FontSize = 20 };

                    items.Add(lbl);
                }
                */

                AddElements(Content);

                lastUpdated = now;
            }
            catch (Exception e)
            {

            }
        }

        private void AddElements(List<UIElement> items)
        {
            double knownSize = (double)SizeDefinitions.Count;
            double maxAxis = Math.Ceiling(((double)items.Count) / knownSize); //TODO: Check for Manual Rows/Cols, if > Max

            //Create Definitions
            for (int i = 0; i <= maxAxis; i++)
            {
                if (Orientation == Orientation.Vertical)
                {
                    GridControl.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                }
                else
                {
                    GridControl.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto});
                }
            }

            int known = 0;
            int unknown = 0;

            foreach (var i in items)
            {
                int inc = 1;

                if (Orientation == Orientation.Horizontal)
                {
                    var cs = i.GetValue(Grid.ColumnSpanProperty);
                    if (cs is int)
                        inc = (int) cs;

                    if (Equals(i.GetValue(Grid.ColumnProperty), 0))
                    {
                        i.SetValue(Grid.ColumnProperty, known);
                        known += inc;
                    }

                    if (Equals(i.GetValue(Grid.RowProperty), 0))
                        i.SetValue(Grid.RowProperty, unknown);
                }
                else
                {
                    var rs = i.GetValue(Grid.RowSpanProperty);
                    if (rs is int)
                        inc = (int)rs;

                    if (Equals(i.GetValue(Grid.RowProperty),0))
                    {
                        i.SetValue(Grid.RowProperty, known);
                        known += inc;
                    }

                    if (Equals(i.GetValue(Grid.ColumnProperty),0))
                        i.SetValue(Grid.ColumnProperty, unknown);
                }

                GridControl.Children.Add(i);

                if (known >= knownSize)
                {
                    known = 0;
                    unknown++;
                }
            }

        }

        private void CreateKnownAxis()
        {
            foreach (var size in SizeDefinitions)
            {
                if (Orientation == Orientation.Horizontal)
                {

                    GridControl.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = size.Size,
                    });
                }
                else
                {
                    GridControl.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = size.Size
                    });
                }
            }

        }
        
        private void GridControl_Loaded(object sender, RoutedEventArgs e)
        {
            CreateLayout();
        }
    }

    public class SizeDefinition : FrameworkContentElement
    {
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(GridLength), typeof(SizeDefinition));

        public GridLength Size
        {
            get
            {
                return (GridLength)GetValue(SizeProperty);
            }
            set
            {
                SetValue(SizeProperty, value);
            }
        }
    }
}
