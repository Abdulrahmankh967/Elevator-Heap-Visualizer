namespace HeapPriorityQueue
{
    partial class frmHeap
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHeap));
            SuspendLayout();
            // 
            // frmHeap
            // 
            BackColor = Color.FromArgb(13, 17, 23);
            ClientSize = new Size(1180, 740);
            Font = new Font("Segoe UI", 9.5F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1000, 680);
            Name = "frmHeap";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Elevator Priority Queue — Heap Visualizer";
            Load += frmHeap_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
