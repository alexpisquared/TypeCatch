using System.Windows.Forms.DataVisualization.Charting;

namespace TypingWpf.Vws
{
  partial class WinFormChartUsrCtrl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
      this.SuspendLayout();
      // 
      // chart1
      // 
      this.chart1.BackColor = System.Drawing.Color.Transparent;
      chartArea1.AxisX.LineColor = System.Drawing.Color.Maroon;
      chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.AxisX.MinorGrid.LineColor = System.Drawing.Color.Maroon;
      chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      //chartArea1.AxisY.Minimum = 125D;
      chartArea1.AxisY.MinorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
      chartArea1.Name = "ChartArea1";
      this.chart1.ChartAreas.Add(chartArea1);
      this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.chart1.Location = new System.Drawing.Point(0, 0);
      this.chart1.Name = "chart1";
      series1.ChartArea = "ChartArea1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
      series1.MarkerSize = 8;
      series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
      series1.Name = "Progress";
      series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
      series2.ChartArea = "ChartArea1";
      series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series2.MarkerSize = 20;
      series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star5;
      series2.Name = "CurCpM";
      series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
      this.chart1.Series.Add(series1);
      this.chart1.Series.Add(series2);
      this.chart1.Size = new System.Drawing.Size(554, 452);
      this.chart1.TabIndex = 0;
      this.chart1.Text = "chart1";
      // 
      // WinFormChartUsrCtrl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chart1);
      this.Name = "WinFormChartUsrCtrl";
      this.Size = new System.Drawing.Size(554, 452);
      this.Load += new System.EventHandler(this.WinFormChartUsrCtrl_Load);
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Chart chart1;
  }
}
