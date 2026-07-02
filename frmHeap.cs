using System.Drawing.Drawing2D;

namespace HeapPriorityQueue
{
    public partial class frmHeap : Form
    {
        
        private static readonly Color C_BG = Color.FromArgb(13, 17, 23);
        private static readonly Color C_PANEL = Color.FromArgb(22, 27, 34);
        private static readonly Color C_BORDER = Color.FromArgb(48, 54, 61);
        private static readonly Color C_CYAN = Color.FromArgb(0, 212, 255);
        private static readonly Color C_VIOLET = Color.FromArgb(124, 58, 237);
        private static readonly Color C_GREEN = Color.FromArgb(34, 197, 94);
        private static readonly Color C_YELLOW = Color.FromArgb(251, 191, 36);
        private static readonly Color C_RED = Color.FromArgb(239, 68, 68);
        private static readonly Color C_TEXT_DIM = Color.FromArgb(139, 148, 158);

        
        private readonly HeapEngine _heap = new();
        private int _opCount = 0;
        private int _flashIdx = -1;   // index of last-touched node to flash
        private Color _flashColor = Color.White;
        private readonly System.Windows.Forms.Timer _flashTimer = new() { Interval = 600 };

        
        private Panel pnlHeader = null!;
        private Panel pnlLeft = null!;
        private Panel pnlCenter = null!;
        private Panel pnlRight = null!;
        private Panel pnlBottom = null!;

        
        private TextBox txtName = null!;
        private NumericUpDown nudFloor = null!;
        private Button btnInsert = null!;
        private Button btnExtract = null!;
        private Button btnPeek = null!;
        private Button btnReset = null!;

        
        private Panel pnlTree = null!;

        
        private RichTextBox rtbLog = null!;

        
        private Label lblSize = null!, lblMin = null!, lblOps = null!;

        
        public frmHeap()
        {
            InitializeComponent();
            BuildUI();
            _flashTimer.Tick += (s, e) => { _flashIdx = -1; _flashTimer.Stop(); pnlTree.Invalidate(); };
            Log("🚀 Elevator Priority Queue ready.", C_CYAN);
        }

        
        private void BuildUI()
        {
            
            pnlHeader = MakePanel(0, 0, ClientSize.Width, 64, Color.FromArgb(16, 20, 28));
            var lblTitle = new Label
            {
                Text = "⬆  ELEVATOR PRIORITY QUEUE",
                ForeColor = C_CYAN,
                Font = new Font("Segoe UI", 17, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(24, 16)
            };
            var lblSub = new Label
            {
                Text = "Min-Heap Visualizer  |  Lower floor number = Higher Priority",
                ForeColor = C_TEXT_DIM,
                Font = new Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(26, 44)
            };
            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSub });
            
            pnlHeader.Paint += (s, e) =>
            {
                using var brush = new LinearGradientBrush(
                    new Point(0, 63), new Point(pnlHeader.Width, 63),
                    C_CYAN, C_VIOLET);
                e.Graphics.FillRectangle(brush, 0, 62, pnlHeader.Width, 2);
            };

            
            pnlBottom = MakePanel(0, ClientSize.Height - 44, ClientSize.Width, 44, Color.FromArgb(16, 20, 28));
            lblSize = MakeStatLabel("Heap Size: 0", 20);
            lblMin = MakeStatLabel("Min Floor: —", 240);
            lblOps = MakeStatLabel("Operations: 0", 460);
            pnlBottom.Controls.AddRange(new Control[] { lblSize, lblMin, lblOps });
            pnlBottom.Paint += (s, e) =>
            {
                using var pen = new Pen(C_BORDER);
                e.Graphics.DrawLine(pen, 0, 0, pnlBottom.Width, 0);
            };

            
            int leftW = 230;
            pnlLeft = MakePanel(0, 64, leftW, ClientSize.Height - 108, C_PANEL);
            pnlLeft.Paint += (s, e) =>
            {
                using var pen = new Pen(C_BORDER);
                e.Graphics.DrawLine(pen, pnlLeft.Width - 1, 0, pnlLeft.Width - 1, pnlLeft.Height);
            };
            BuildLeftControls();

            
            int rightW = 280;
            pnlRight = MakePanel(ClientSize.Width - rightW, 64, rightW, ClientSize.Height - 108, C_PANEL);
            pnlRight.Paint += (s, e) =>
            {
                using var pen = new Pen(C_BORDER);
                e.Graphics.DrawLine(pen, 0, 0, 0, pnlRight.Height);
            };
            BuildRightPanel();

            
            int cx = leftW, cw = ClientSize.Width - leftW - rightW;
            pnlCenter = MakePanel(cx, 64, cw, ClientSize.Height - 108, C_BG);

            var lblTreeTitle = new Label
            {
                Text = "HEAP TREE",
                ForeColor = C_TEXT_DIM,
                Font = new Font("Segoe UI", 8, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            pnlCenter.Controls.Add(lblTreeTitle);

            pnlTree = new Panel
            {
                Location = new Point(0, 30),
                Size = new Size(cw, ClientSize.Height - 138),
                BackColor = Color.Transparent
            };
            // Enable double-buffering to prevent flicker on tree redraws
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(pnlTree, true);
            pnlTree.Paint += PnlTree_Paint;
            pnlCenter.Controls.Add(pnlTree);

            // Add all panels to form
            Controls.AddRange(new Control[] { pnlHeader, pnlBottom, pnlLeft, pnlRight, pnlCenter });
        }

        private void BuildLeftControls()
        {
            int y = 20;

            AddSectionLabel(pnlLeft, "FLOOR REQUEST", ref y);

            var lblName = MakeFieldLabel(pnlLeft, "Passenger / Button Name", y); y += 22;
            txtName = new TextBox
            {
                Location = new Point(14, y),
                Size = new Size(200, 28),
                BackColor = Color.FromArgb(30, 36, 46),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10f),
                Text = "Btn1"
            };
            pnlLeft.Controls.Add(txtName);
            y += 34;

            var lblFloor = MakeFieldLabel(pnlLeft, "Floor Number (Priority)", y); y += 22;
            nudFloor = new NumericUpDown
            {
                Location = new Point(14, y),
                Size = new Size(200, 28),
                Minimum = 1,
                Maximum = 99,
                Value = 1,
                BackColor = Color.FromArgb(30, 36, 46),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlLeft.Controls.Add(nudFloor);
            y += 42;

            btnInsert = MakeButton(pnlLeft, "⬆  Add Request", C_CYAN, C_BG, y); y += 46;
            btnExtract = MakeButton(pnlLeft, "✔  Extract Min", C_GREEN, C_BG, y); y += 46;
            btnPeek = MakeButton(pnlLeft, "👁  Peek Min", C_YELLOW, C_BG, y); y += 46;

            AddSectionLabel(pnlLeft, "ACTIONS", ref y);
            btnReset = MakeButton(pnlLeft, "↺  Reset All", C_RED, C_BG, y);

            // Wire events
            btnInsert.Click += BtnInsert_Click;
            btnExtract.Click += BtnExtract_Click;
            btnPeek.Click += BtnPeek_Click;
            btnReset.Click += BtnReset_Click;

            
            y += 50;
            var btnDemo = MakeButton(pnlLeft, "▶  Load Demo", C_VIOLET, C_BG, y);
            btnDemo.Click += BtnDemo_Click;
        }

        private void BuildRightPanel()
        {
            var lblLog = new Label
            {
                Text = "OPERATION LOG",
                ForeColor = C_TEXT_DIM,
                Font = new Font("Segoe UI", 8, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(12, 10)
            };

            rtbLog = new RichTextBox
            {
                Location = new Point(8, 30),
                Size = new Size(pnlRight.Width - 16, pnlRight.Height - 38),
                BackColor = Color.FromArgb(13, 17, 23),
                ForeColor = C_TEXT_DIM,
                Font = new Font("Consolas", 8.5f),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true
            };

            pnlRight.Controls.AddRange(new Control[] { lblLog, rtbLog });
        }

        
        private void PnlTree_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var nodes = _heap.Nodes;
            int count = nodes.Count;

            if (count == 0)
            {
                DrawEmptyState(g);
                return;
            }

            // Calculate node positions
            var positions = new PointF[count];
            int w = pnlTree.Width;
            int h = pnlTree.Height;
            int nodeR = 30;

            // BFS level layout
            int depth = (int)Math.Floor(Math.Log2(count)) + 1;
            for (int i = 0; i < count; i++)
            {
                int level = (int)Math.Floor(Math.Log2(i + 1));
                int posInLevel = i - ((1 << level) - 1);
                int countInLevel = Math.Min(1 << level, count - ((1 << level) - 1));
                float cellW = (float)w / countInLevel;
                float x = cellW * posInLevel + cellW / 2;
                float y = 20 + level * (h - 40f) / depth;
                positions[i] = new PointF(x, y);
            }

            // Draw edges first
            using var edgePen = new Pen(C_BORDER, 1.5f);
            for (int i = 1; i < count; i++)
            {
                int parent = (i - 1) / 2;
                g.DrawLine(edgePen, positions[parent], positions[i]);
            }

            // Draw nodes
            for (int i = 0; i < count; i++)
            {
                var pt = positions[i];
                var rect = new RectangleF(pt.X - nodeR, pt.Y - nodeR, nodeR * 2, nodeR * 2);

                Color nodeColor = i == 0 ? C_CYAN : GetLevelColor((int)Math.Floor(Math.Log2(i + 1)));
                bool isFlash = (i == _flashIdx);

        
                if (i == 0 || isFlash)
                {
                    using var glow = new SolidBrush(Color.FromArgb(40, isFlash ? _flashColor : C_CYAN));
                    g.FillEllipse(glow, rect.X - 4, rect.Y - 4, rect.Width + 8, rect.Height + 8);
                }

        
                Color fill = isFlash ? _flashColor : nodeColor;
                using var nodeBrush = new SolidBrush(Color.FromArgb(40, fill));
                g.FillEllipse(nodeBrush, rect);

                // Node border
                using var borderPen = new Pen(fill, isFlash ? 2.5f : 1.8f);
                g.DrawEllipse(borderPen, rect);

                // Floor number (big)
                using var numFont = new Font("Segoe UI", 11, FontStyle.Bold, GraphicsUnit.Point);
                using var namFont = new Font("Segoe UI", 7, FontStyle.Regular, GraphicsUnit.Point);
                using var textBrush = new SolidBrush(fill);

                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(nodes[i].Priority.ToString(), numFont, textBrush,
                    new RectangleF(pt.X - nodeR, pt.Y - 10, nodeR * 2, 22), sf);
                g.DrawString(nodes[i].Name, namFont, new SolidBrush(C_TEXT_DIM),
                    new RectangleF(pt.X - nodeR, pt.Y + 10, nodeR * 2, 16), sf);
            }

        
            DrawLegend(g, w, h);
        }

        private void DrawEmptyState(Graphics g)
        {
            using var f = new Font("Segoe UI", 14, FontStyle.Regular, GraphicsUnit.Point);
            using var b = new SolidBrush(Color.FromArgb(60, 139, 148, 158));
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("🏢  No floor requests in queue\nAdd a request using the left panel",
                f, b, new RectangleF(0, 0, pnlTree.Width, pnlTree.Height), sf);
        }

        private void DrawLegend(Graphics g, int w, int h)
        {
            var items = new[] {
                ("Root (Min)", C_CYAN),
                ("Level 1",    GetLevelColor(1)),
                ("Level 2",    GetLevelColor(2)),
                ("Highlighted",C_YELLOW)
            };
            int lx = 10, ly = h - 20;
            using var lf = new Font("Segoe UI", 7.5f, GraphicsUnit.Point);
            foreach (var (label, color) in items)
            {
                using var b = new SolidBrush(color);
                g.FillEllipse(b, lx, ly - 6, 10, 10);
                using var tb = new SolidBrush(C_TEXT_DIM);
                g.DrawString(label, lf, tb, lx + 14, ly - 7);
                lx += 95;
            }
        }

        private static Color GetLevelColor(int level) => level switch
        {
            1 => Color.FromArgb(124, 58, 237),  // Violet
            2 => Color.FromArgb(16, 185, 129),  // Emerald
            3 => Color.FromArgb(245, 158, 11),   // Amber
            _ => Color.FromArgb(99, 102, 241),  // Indigo
        };

        
        private void BtnInsert_Click(object? sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { Log("⚠  Please enter a name.", C_YELLOW); return; }
            int floor = (int)nudFloor.Value;

            _heap.Insert(name, floor);
            _flashIdx = _heap.Count - 1;
            _flashColor = C_GREEN;

            Log($"⬆  Inserted: {name} → Floor {floor}", C_GREEN);
            _opCount++;
            nudFloor.Value = Math.Min(nudFloor.Maximum, nudFloor.Value + 1);
            RefreshUI();
        }

        private void BtnExtract_Click(object? sender, EventArgs e)
        {
            try
            {
                var min = _heap.ExtractMin();
                _flashIdx = 0;
                _flashColor = C_RED;
                Log($"✔  Extracted: {min.Name} (Floor {min.Priority}) — Elevator arrived!", C_CYAN);
                _opCount++;
                RefreshUI();
            }
            catch (InvalidOperationException)
            {
                Log("⚠  Heap is empty — nothing to extract.", C_YELLOW);
            }
        }

        private void BtnPeek_Click(object? sender, EventArgs e)
        {
            try
            {
                var min = _heap.Peek();
                _flashIdx = 0;
                _flashColor = C_YELLOW;
                Log($"👁  Next stop: {min.Name} (Floor {min.Priority})", C_YELLOW);
                RefreshUI();
            }
            catch (InvalidOperationException)
            {
                Log("⚠  Heap is empty — nothing to peek.", C_YELLOW);
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            _heap.Clear();
            _opCount = 0;
            _flashIdx = -1;
            Log("↺  Heap cleared. All floor requests removed.", C_RED);
            RefreshUI();
        }

        private void BtnDemo_Click(object? sender, EventArgs e)
        {
            _heap.Clear();
            _flashIdx = -1;
            var demo = new[] {
                ("Lobby",   1), ("Btn3", 3), ("Btn7", 7),
                ("Btn2", 2), ("Btn5", 5), ("Btn4", 4), ("Btn9", 9)
            };
            foreach (var (n, p) in demo) _heap.Insert(n, p);
            _opCount = demo.Length;
            Log("▶  Demo loaded: 7 floor requests inserted.", C_VIOLET);
            RefreshUI();
        }

        
        private void RefreshUI()
        {
            // Stats
            int cnt = _heap.Count;
            lblSize.Text = $"Heap Size: {cnt}";
            lblOps.Text = $"Operations: {_opCount}";
            try { lblMin.Text = $"Min Floor: {_heap.Peek().Priority}  ({_heap.Peek().Name})"; }
            catch { lblMin.Text = "Min Floor: —"; }

            // Restart flash timer
            _flashTimer.Stop();
            if (_flashIdx >= 0) _flashTimer.Start();

            pnlTree.Invalidate();
        }

        private void Log(string msg, Color color)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}]  {msg}\n";
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(line);
            rtbLog.ScrollToCaret();
        }

        
        private Panel MakePanel(int x, int y, int w, int h, Color bg)
        {
            var p = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = bg };
            return p;
        }

        private Label MakeStatLabel(string text, int x)
        {
            return new Label
            {
                Text = text,
                ForeColor = C_TEXT_DIM,
                Font = new Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(x, 14)
            };
        }

        private Label MakeFieldLabel(Panel parent, string text, int y)
        {
            var lbl = new Label
            {
                Text = text,
                ForeColor = C_TEXT_DIM,
                Font = new Font("Segoe UI", 8, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(14, y)
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private void AddSectionLabel(Panel parent, string text, ref int y)
        {
            y += 8;
            var lbl = new Label
            {
                Text = text,
                ForeColor = C_TEXT_DIM,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Location = new Point(14, y)
            };
            parent.Controls.Add(lbl);
            y += 20;
        }

        private Button MakeButton(Panel parent, string text, Color accent, Color textColor, int y)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(14, y),
                Size = new Size(200, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, accent),
                ForeColor = accent,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold, GraphicsUnit.Point),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(80, accent);
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, accent);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, accent);
            parent.Controls.Add(btn);
            return btn;
        }

        private void frmHeap_Load(object sender, EventArgs e)
        {

        }
    }
}
