using CircleDraw;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace CircleMove
{
    public partial class Form1 : Form
    {
        int myFigure, timer_interval = 1000;
        public List<Figure> figures;
        MPolygon polygon;
        Figure currentFigureToDraw;
        List<Figure> currentFigure;
        bool areDrawing, shallRefresh, allPointMoving = false, is_saved = true;
        public RadiusChangerForm current_radius_form = null;
        public static Random random;
        System.Windows.Forms.Timer timer;
        string saverFilePath;
        Stack<List<Change>> historyBack, historyForward;
        ToolStripMenuItem backArrow;
        ToolStripMenuItem forwardArrow;
        Dictionary<int, Point> dPoint;
        List<Change> delChanges;
        private int stepCounter;

        public Form1()
        {
            delChanges = new List<Change>();
            random = new Random();
            currentFigure = new List<Figure>();
            myFigure = 1;
            stepCounter = 0;
            currentFigureToDraw = new Circle(0, 0);
            areDrawing = true;
            shallRefresh = false;
            figures = new List<Figure>();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = timer_interval;
            timer.Tick += timer_dynamics;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            saverFilePath = null;
            historyBack = new Stack<List<Change>>();
            historyForward = new Stack<List<Change>>();
            InitializeComponent();
            backArrow = new ToolStripMenuItem() { Checked = true, CheckOnClick = true, Enabled = false };
            forwardArrow = new ToolStripMenuItem() { Checked = true, CheckOnClick = true, Enabled = false };
            figures.Add(new Circle(this.Width / 2, this.Height / 2));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolStripDropDownItem file = new ToolStripMenuItem("Файл");
            ToolStripDropDownItem allVariants = new ToolStripMenuItem("Создать");
            ToolStripMenuItem new_saving = new ToolStripMenuItem("Новый") { CheckOnClick = true };
            new_saving.ShortcutKeys = Keys.Control | Keys.N;
            ToolStripMenuItem open_saving = new ToolStripMenuItem("Открыть") { CheckOnClick = true };
            open_saving.ShortcutKeys = Keys.Control | Keys.O;
            ToolStripMenuItem save = new ToolStripMenuItem("Сохранить") { CheckOnClick = true };
            save.ShortcutKeys = Keys.Control | Keys.S;
            ToolStripMenuItem save_as = new ToolStripMenuItem("Сохранить как") { CheckOnClick = true };
            save_as.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            ToolStripDropDownItem chooseAlgorithm = new ToolStripMenuItem("Алгоритм");
            ToolStripDropDownItem settings = new ToolStripMenuItem("Настройки");
            ToolStripDropDownItem colorPicker = new ToolStripMenuItem("Цвет");
            ToolStripMenuItem radiusPicker = new ToolStripMenuItem("Радиус") { CheckOnClick = true };
            ToolStripMenuItem chooseInsideColor = new ToolStripMenuItem("Внутренняя область") { CheckOnClick = true };
            ToolStripMenuItem chooseOutsideColor = new ToolStripMenuItem("Граница") { CheckOnClick = true };
            ToolStripMenuItem testButton = new ToolStripMenuItem("Тест") { CheckOnClick = true };
            ToolStripMenuItem chooseCircle = new ToolStripMenuItem("Круг") { Checked = true, CheckOnClick = true };
            ToolStripMenuItem chooseSquare = new ToolStripMenuItem("Квадрат") { Checked = false, CheckOnClick = true };
            ToolStripMenuItem chooseTriangle = new ToolStripMenuItem("Треугольник") { Checked = false, CheckOnClick = true };
            ToolStripMenuItem chooseTraditional = new ToolStripMenuItem("По определению") { Checked = false, CheckOnClick = true };
            ToolStripMenuItem chooseJarvis = new ToolStripMenuItem("Джарвис") { Checked = true, CheckOnClick = true };
            ToolStripMenuItem dynamicsController = new ToolStripMenuItem() { Checked = true, CheckOnClick = true };
            ToolStripMenuItem intervalChanger = new ToolStripMenuItem("Интервал") { Checked = false, CheckOnClick = true };
            dynamicsController.Image = Bitmap.FromFile(Directory.GetCurrentDirectory() + @"\images\play.png");
            backArrow.Image = Bitmap.FromFile(Directory.GetCurrentDirectory() + @"\images\back.png");
            backArrow.ShortcutKeys = Keys.Control | Keys.Z;
            forwardArrow.Image = Bitmap.FromFile(Directory.GetCurrentDirectory() + @"\images\forward.png");
            forwardArrow.ShortcutKeys = Keys.Control | Keys.Shift |Keys.Z;
            chooseCircle.Click += (buttonSender, args) =>
            {

                chooseSquare.Checked = false;
                chooseTriangle.Checked = false;
                chooseCircle.Checked = true;
                myFigure = 1;

            };
            chooseSquare.Click += (buttonSender, args) =>
            {
                chooseCircle.Checked = false;
                chooseTriangle.Checked = false;
                chooseSquare.Checked = true;
                myFigure = 2;
            };
            chooseTriangle.Click += (buttonSender, args) =>
            {
                chooseCircle.Checked = false;
                chooseSquare.Checked = false;
                chooseTriangle.Checked = true;
                myFigure = 3;
            };
            chooseInsideColor.Click += (buttonSender, args) =>
            {
                Color oldColor = Figure.insideColor;
                ColorDialog colorDialog = new ColorDialog() { FullOpen = true, Color = Figure.insideColor };
                colorDialog.ShowDialog();
                Figure.insideColor = colorDialog.Color;
                OnChange(new List<Change> { new ChangeInsideColor(this, oldColor, Figure.insideColor) });
                chooseInsideColor.Checked = false;
                is_saved = false;
                this.Refresh();
            };
            chooseOutsideColor.Click += (buttonSender, args) =>
            {
                Color oldColor = Figure.outsideColor;
                ColorDialog colorDialog = new ColorDialog() { FullOpen = true, Color = Figure.outsideColor };
                colorDialog.ShowDialog();
                Figure.outsideColor = colorDialog.Color;
                OnChange(new List<Change> { new ChangeOutsideColor(this, oldColor, Figure.outsideColor) });
                chooseOutsideColor.Checked = false;
                is_saved = false;
                this.Refresh();
            };
            chooseTraditional.Click += (buttonSender, args) =>
            {
                chooseJarvis.Checked = !chooseTraditional.Checked;
                MPolygon.isJarvis = false;
            };
            chooseJarvis.Click += (buttonSender, args) =>
            {
                chooseTraditional.Checked = !chooseJarvis.Checked;
                MPolygon.isJarvis = true;
            };
            intervalChanger.Click += (_sender, args) =>
            {
                intervalChanger.Checked = false;
                string answer = Microsoft.VisualBasic.Interaction.InputBox("Введите интервал (в мс):");
                try
                {
                    timer.Interval = int.Parse(answer);
                }
                catch (Exception) { }
            };
            testButton.Click += (buttonSender, args) =>
            {
                try
                {
                    string message = "";
                    string answer = Microsoft.VisualBasic.Interaction.InputBox("Введите количество точек:");
                    var points = new List<Point>();
                    Random rnd = new Random();
                    for (int i = 0; i < int.Parse(answer); i++)
                    {
                        points.Add(new Point(rnd.Next(0, 800), rnd.Next(0, 600)));
                    }
                    MPolygon test_polygon = new MPolygon(points);
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    test_polygon.CreatePolygonTraditionally(points);
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    message += "Время, затраченное на выполнение алгоритма по определению:\n";
                    message += ts.TotalMilliseconds.ToString() + " миллисекунд" + '\n';
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                    test_polygon.CreatePolygonJarvis(points);
                    stopWatch.Stop();
                    ts = stopWatch.Elapsed;
                    message += "Время, затраченное на выполнение алгоритма джарвиса:\n";
                    message += ts.TotalMilliseconds.ToString() + " миллисекунд\n\n\n";
                    message += "Нарисовать график? (это может занять некоторое время)";
                    DialogResult dialogResult = MessageBox.Show(message, "Для " + answer + " точек:", MessageBoxButtons.YesNoCancel);
                    if(dialogResult == DialogResult.Yes)
                    {
                        CreateDiagram(int.Parse(answer));
                    }
                }
                catch (Exception) { }
            };
            radiusPicker.Click += (_sender, args) =>
            {
                radiusPicker.Checked = false;
                if (current_radius_form == null)
                {
                    current_radius_form = new RadiusChangerForm(this, Figure.radius);
                    current_radius_form.radiusHandler += OnRadiusChanged;
                }
                current_radius_form.Show();
                current_radius_form.BringToFront();
                current_radius_form.WindowState = FormWindowState.Normal;
            };
            dynamicsController.Click += (_sender, args) =>
            {
                allPointMoving = !allPointMoving;
                if (allPointMoving)
                {
                    dynamicsController.Image = Bitmap.FromFile(Directory.GetCurrentDirectory() + @"\images\pause.png");
                    timer.Start();
                }
                if (!allPointMoving)
                {
                    dynamicsController.Image = Bitmap.FromFile(Directory.GetCurrentDirectory() + @"\images\play.png");
                    timer.Stop();
                }
            };
            new_saving.Click += (_sender, args) =>
            {
                new_saving.Checked = false;
                if (!is_saved)
                {
                    DialogResult dialogResult = MessageBox.Show("У вас имеются несохранённые данные. Сохранить?", "Предупреждение", MessageBoxButtons.YesNoCancel);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (saverFilePath == null) SaveAs();
                        else Serialize(saverFilePath);
                        is_saved = true;
                    }
                    if (dialogResult == DialogResult.Cancel) return;
                }
                figures = new List<Figure>();
                figures.Add(new Circle(this.Width / 2, this.Height / 2));
                polygon = null;
                timer = new System.Windows.Forms.Timer();
                timer.Interval = timer_interval;
                timer.Tick += timer_dynamics;
                Figure.radius = 45;
                Figure.insideColor = Color.Aquamarine;
                Figure.outsideColor = Color.Red;
                saverFilePath = null;
                historyBack = new Stack<List<Change>>();
                historyForward = new Stack<List<Change>>();
                EnableUndoRedoButtons();
                this.Refresh();
            };
            open_saving.Click += (_sender, args) =>
            {
                open_saving.Checked = false;
                if (!is_saved)
                {
                    DialogResult dialogResult = MessageBox.Show("У вас имеются несохранённые данные. Сохранить?", "Предупреждение", MessageBoxButtons.YesNoCancel);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (saverFilePath == null) SaveAs();
                        else Serialize(saverFilePath);
                        is_saved = true;
                    }
                    if (dialogResult == DialogResult.Cancel) return;
                }
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Binary File (*.bin)|*.bin";
                dialog.Title = "Open";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Deserealize(dialog.FileName);
                    saverFilePath = dialog.FileName;
                    historyBack = new Stack<List<Change>>();
                    historyForward = new Stack<List<Change>>();
                    EnableUndoRedoButtons();
                } 
            };
            save.Click += (_sender, args) =>
            {
                if (saverFilePath != null) Serialize(saverFilePath);
                else SaveAs();
                save.Checked = false;
            };
            save_as.Click += (_sender, args) =>
            {
                SaveAs();
                save_as.Checked = false;
            };
            backArrow.Click += (_sender, args) =>
            {
                var changes = historyBack.Pop();
                for (int i = changes.Count - 1; i >= 0; i--)
                {
                    changes[i].Undo();
                }
                historyForward.Push(changes);
                EnableUndoRedoButtons();
                this.Refresh();
            };
            forwardArrow.Click += (_sender, args) =>
            {
                var changes = historyForward.Pop();
                for (int i = 0; i < changes.Count; i++)
                {
                    changes[i].Redo();
                }
                historyBack.Push(changes);
                EnableUndoRedoButtons();
                this.Refresh();
            };
            allVariants.DropDownItems.AddRange(new[] { chooseCircle, chooseSquare, chooseTriangle });
            chooseAlgorithm.DropDownItems.AddRange(new[] { chooseTraditional, chooseJarvis });
            colorPicker.DropDownItems.AddRange(new[] { chooseInsideColor, chooseOutsideColor });
            settings.DropDownItems.AddRange(new[] { colorPicker, chooseAlgorithm, radiusPicker, intervalChanger });
            file.DropDownItems.AddRange(new[] { new_saving, open_saving, save, save_as });
            menuStrip1.Items.Add(backArrow);
            menuStrip1.Items.Add(dynamicsController);
            menuStrip1.Items.Add(forwardArrow);
            menuStrip1.Items.Add(file);
            menuStrip1.Items.Add(allVariants);
            menuStrip1.Items.Add(settings);
            menuStrip1.Items.Add(testButton);
            this.FormClosing += Form1_Closing;
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (!is_saved)
            {
                DialogResult dialogResult = MessageBox.Show("У вас имеются несохранённые данные. Сохранить?", "Предупреждение", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Cancel) e.Cancel = true;
                if (dialogResult == DialogResult.Yes)
                {
                    if (saverFilePath != null) Serialize(saverFilePath);
                    else SaveAs();
                }
            }
        }

        private void SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Binary File (*.bin)|*.bin";
            dialog.FileName = "Untitled";
            dialog.Title = "Save as";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Serialize(dialog.FileName);
                saverFilePath = dialog.FileName;
            }
        }

        private void Serialize(string file_name)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(file_name, FileMode.OpenOrCreate);
            object[] data = { figures.ToArray(), Figure.radius, Figure.insideColor, Figure.outsideColor };
            bf.Serialize(fs, data);
            fs.Close();
            is_saved = true;
        }

        private void Deserealize(string file_name)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(file_name, FileMode.OpenOrCreate);
            object[] data = (object[])bf.Deserialize(fs);
            figures = new List<Figure>((Figure[])data[0]);
            Figure.radius = (int)data[1];
            Figure.insideColor = (Color)data[2];
            Figure.outsideColor = (Color)data[3];
            fs.Close();
            saverFilePath = file_name;
            is_saved = true;
            this.Refresh();
        }

        private void timer_dynamics(object sender, EventArgs e)
        {
            foreach(var point in figures)
            {
                point.Dynamics();
            }
            this.Refresh();
            if (polygon != null)
            {
                for (int i = 0; i < figures.Count; i++)
                {
                    if (!polygon.polygonPoints.Contains(figures[i].GetPoint()))
                    {
                        figures.RemoveAt(i);
                        i--;
                    }
                }
                this.Refresh();
            }
        }

        private void CreateDiagram(int num_points)
        {
            Form formPopUp = new Form();
            formPopUp.Size = new Size(1080, 680);
            Random rnd = new Random();
            Chart myChart = new Chart();
            myChart.Parent = formPopUp;
            myChart.Dock = DockStyle.Fill;
            myChart.ChartAreas.Add(new ChartArea("Algorithm work time"));
            Series traditPoints = new Series("По определению");
            traditPoints.ChartType = SeriesChartType.Line;
            traditPoints.ChartArea = "Algorithm work time";
            myChart.Legends.Add(traditPoints.Legend);
            Series jarvisPoints = new Series("Джарвис");
            jarvisPoints.ChartType = SeriesChartType.Line;
            jarvisPoints.ChartArea = "Algorithm work time";
            myChart.Legends.Add(jarvisPoints.Legend);
            for (int i = 1; i <= num_points; i++)
            {
                List<Point> points = new List<Point>();
                for (int j = 0; j <= i; j++)
                {
                    points.Add(new Point(rnd.Next(0, 800), rnd.Next(0, 600)));
                }
                MPolygon test_polygon = new MPolygon(points);
                Stopwatch stopWatch = new Stopwatch();
                TimeSpan ts;
                if(i==1)
                {
                    traditPoints.Points.AddXY(i, 0.8);
                }
                else
                {
                    stopWatch.Start();
                    test_polygon.CreatePolygonTraditionally(points);
                    stopWatch.Stop();
                    ts = stopWatch.Elapsed;
                    traditPoints.Points.AddXY(i, ts.TotalMilliseconds * 1000);
                }
                stopWatch = new Stopwatch();
                stopWatch.Start();
                test_polygon.CreatePolygonJarvis(points);
                stopWatch.Stop();
                ts = stopWatch.Elapsed;
                jarvisPoints.Points.AddXY(i, ts.TotalMilliseconds * 1000);
            }
            myChart.Series.Add(traditPoints);
            myChart.Series.Add(jarvisPoints);
            formPopUp.Show(this);
        }

        private void OnRadiusChanged(object sender, RadiusEventArgs e)
        {
            Figure.radius = e.radius;
            for(int i = 0; i < figures.Count; i++)
            {
                figures[i].x = figures[i].x - (e.radius / 2 - e.prevRadius / 2);
                figures[i].y = figures[i].y - (e.radius / 2 - e.prevRadius / 2);
                is_saved = false;
            }
            this.Refresh();
        }

        public void OnChange(List<Change> e)
        {
            if(historyForward.Count > 0)
            {
                historyForward = new Stack<List<Change>>();
            }
            if (e.Count > 0 && e != null)
            {
                historyBack.Push(e);
                EnableUndoRedoButtons();
            }
        }

        private void EnableUndoRedoButtons()
        {
            if (historyBack.Count > 0)
            {
                backArrow.Enabled = true;
                is_saved = false;
            }
            else
            {
                backArrow.Enabled = false;
                is_saved = true;
            }

            if (historyForward.Count > 0)
            {
                forwardArrow.Enabled = true;
                is_saved = false;
            }
            else
            {
                forwardArrow.Enabled = false;
                is_saved = false;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach(Figure figure in figures)
            {
                if (figure != null)
                    figure.Draw(e);
            }
            if (figures.Count >= 3)
            {
                List<Point> allFigurePoints = new List<Point>();
                foreach (Figure figure in figures)
                {
                    if (figure != null)
                        allFigurePoints.Add(figure.GetPoint());
                }
                if (allFigurePoints.Count >= 3)
                    polygon = new MPolygon(allFigurePoints);
                else
                    polygon = null;
            }
            else
                polygon = null;

            if (polygon != null)
                polygon.Draw(e);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stepCounter++;
            delChanges = new List<Change>();
            dPoint = new Dictionary<int, Point>();
            if (e.Button == MouseButtons.Left)
            {
                bool isPolygonMoving = true;
                foreach (Figure figure in figures)
                {
                    if (figure != null)
                    {
                        if (figure.IsInside(e.Location))
                        {
                            currentFigure.Add(figure);
                            areDrawing = false;
                            shallRefresh = true;
                            isPolygonMoving = false;
                        }
                    }
                }
                if (isPolygonMoving)
                {
                    if (polygon != null)
                    {
                        if (polygon.IsInside(e.Location))
                        {
                            foreach (Figure figure in figures)
                            {
                                figure.OnPolygonMove(e.Location);
                                currentFigure.Add(figure);
                            }
                            shallRefresh = true;
                            areDrawing = false;
                            is_saved = false;
                        }
                    }
                }
                foreach(var figure in currentFigure) 
                {
                    dPoint[figures.IndexOf(figure)] = new Point(figure.x, figure.y);
                }
                if (areDrawing)
                {
                    if (myFigure == 1)
                    {
                        currentFigureToDraw = new Circle(e.X - Figure.radius / 2, e.Y - Figure.radius / 2);
                    }
                    if (myFigure == 2)
                    {
                        currentFigureToDraw = new Square(e.X - Figure.radius / 2, e.Y - Figure.radius / 2);
                    }
                    if (myFigure == 3)
                    {
                        currentFigureToDraw = new Triangle(e.X - Figure.radius / 2, e.Y - Figure.radius / 2);
                    }
                    figures.Add(currentFigureToDraw);
                    is_saved = false;
                    delChanges.Add(new CreatePoint(figures.IndexOf(currentFigureToDraw), this, currentFigureToDraw));
                    this.Refresh();
                }               
            }
            if(e.Button == MouseButtons.Right)
            {
                bool are_deleting_polygon = true;
                List<Figure> figures_to_delete = new List<Figure>();
                foreach (Figure figure in figures)
                {
                    if (figure != null)
                    {
                        if (figure.IsInside(e.Location))
                        {
                            figures_to_delete.Add(figure);
                            delChanges.Add(new DeletePoint(figures.IndexOf(figure), this, figure));
                            are_deleting_polygon = false;
                            is_saved = false;
                        }
                    }
                }
                for(int i = 0; i < figures_to_delete.Count; i++)
                {
                    figures.Remove(figures_to_delete[i]);
                    is_saved = false;
                }
                if (are_deleting_polygon)
                {
                    if (polygon != null)
                    {
                        if (polygon.IsInside(e.Location))
                        {
                            var old_list = figures;
                            for (int i = 0; i < figures.Count; i++)
                            {
                                delChanges.Add(new DeletePoint(old_list.IndexOf(figures[i]), this, figures[i]));
                                figures[i] = null;
                                is_saved = false;
                            }
                        }
                    }
                }
                if(figures.Count < 3)
                {
                    polygon = null;
                }
                this.Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (currentFigure != null)
            {
                for (int i = 0; i < currentFigure.Count; i++)
                {
                    currentFigure[i].Stop();
                }
                currentFigure = new List<Figure>();
                if (dPoint != null && stepCounter > 0)
                {
                    foreach (KeyValuePair<int, Point> val in dPoint)
                    {
                        delChanges.Add(new MovePoint(val.Key, this, val.Value.X - figures[val.Key].x, val.Value.Y - figures[val.Key].y));
                    }
                }
            }
            areDrawing = true;
            if(figures.Count >= 3)
            {
                var allFigurePoints = new List<Point>();
                foreach(Figure figure in figures)
                {
                    if(figure != null)
                        allFigurePoints.Add(figure.GetPoint());
                }
                polygon = new MPolygon(allFigurePoints);
                this.Refresh();
            }
            else
            {
                polygon = null;
                this.Refresh();
            }
            if(polygon != null)
            {
                var old_list = figures;
                for (int i = 0; i < figures.Count; i++)
                {
                    if (figures[i] != null)
                    {
                        if (!polygon.polygonPoints.Contains(figures[i].GetPoint()))
                        {
                            if (stepCounter > 0)
                                delChanges.Add(new DeletePoint(old_list.IndexOf(figures[i]), this, figures[i]));
                            figures[i] = null;
                            is_saved = false;
                        }
                    }
                }
                this.Refresh();
            }
            OnChange(delChanges);
            shallRefresh = false;
            stepCounter = 0;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentFigure != null)
            {
                if(shallRefresh)
                {
                    foreach (Figure figure in currentFigure)
                    {
                        figure.Move(this, e);
                        is_saved = false;
                    }
                    this.Refresh();
                }
            }
        }
    }
}
