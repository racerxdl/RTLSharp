namespace rtlsdr {
  partial class SharpTest {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.trackBar3 = new System.Windows.Forms.TrackBar();
      this.offsetTrackBar = new System.Windows.Forms.TrackBar();
      this.trackBar5 = new System.Windows.Forms.TrackBar();
      this.label1 = new System.Windows.Forms.Label();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.lnaGain = new System.Windows.Forms.TrackBar();
      this.mixerGain = new System.Windows.Forms.TrackBar();
      this.vgaGain = new System.Windows.Forms.TrackBar();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.spectrumAnalyzer2 = new RadioComponents.Visual.SpectrumAnalyzer();
      this.spectrumAnalyzer1 = new RadioComponents.Visual.SpectrumAnalyzer();
      this.fftTimer = new System.Windows.Forms.Timer(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.offsetTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lnaGain)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.mixerGain)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.vgaGain)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(82, 109);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "Start";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(163, 109);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 1;
      this.button2.Text = "Stop";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // trackBar3
      // 
      this.trackBar3.Location = new System.Drawing.Point(404, 58);
      this.trackBar3.Maximum = 300;
      this.trackBar3.Minimum = 1;
      this.trackBar3.Name = "trackBar3";
      this.trackBar3.Size = new System.Drawing.Size(176, 45);
      this.trackBar3.TabIndex = 7;
      this.trackBar3.Value = 1;
      this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
      // 
      // offsetTrackBar
      // 
      this.offsetTrackBar.Location = new System.Drawing.Point(404, 12);
      this.offsetTrackBar.Maximum = 15;
      this.offsetTrackBar.Name = "offsetTrackBar";
      this.offsetTrackBar.Size = new System.Drawing.Size(176, 45);
      this.offsetTrackBar.TabIndex = 6;
      this.offsetTrackBar.Value = 1;
      this.offsetTrackBar.ValueChanged += new System.EventHandler(this.trackBar4_ValueChanged);
      // 
      // trackBar5
      // 
      this.trackBar5.Location = new System.Drawing.Point(12, 187);
      this.trackBar5.Maximum = 12000;
      this.trackBar5.Minimum = 8000;
      this.trackBar5.Name = "trackBar5";
      this.trackBar5.Size = new System.Drawing.Size(815, 45);
      this.trackBar5.TabIndex = 8;
      this.trackBar5.Value = 10630;
      this.trackBar5.ValueChanged += new System.EventHandler(this.trackBar5_ValueChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(456, 151);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 9;
      this.label1.Text = "label1";
      // 
      // trackBar1
      // 
      this.trackBar1.Location = new System.Drawing.Point(12, 12);
      this.trackBar1.Maximum = 100;
      this.trackBar1.Minimum = 1;
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size(176, 45);
      this.trackBar1.TabIndex = 4;
      this.trackBar1.Value = 1;
      this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
      // 
      // lnaGain
      // 
      this.lnaGain.Location = new System.Drawing.Point(651, 12);
      this.lnaGain.Maximum = 15;
      this.lnaGain.Name = "lnaGain";
      this.lnaGain.Size = new System.Drawing.Size(176, 45);
      this.lnaGain.TabIndex = 10;
      this.lnaGain.Value = 1;
      this.lnaGain.Scroll += new System.EventHandler(this.lnaGain_Scroll);
      // 
      // mixerGain
      // 
      this.mixerGain.Location = new System.Drawing.Point(651, 63);
      this.mixerGain.Maximum = 15;
      this.mixerGain.Name = "mixerGain";
      this.mixerGain.Size = new System.Drawing.Size(176, 45);
      this.mixerGain.TabIndex = 11;
      this.mixerGain.Value = 1;
      this.mixerGain.Scroll += new System.EventHandler(this.mixerGain_Scroll);
      // 
      // vgaGain
      // 
      this.vgaGain.Location = new System.Drawing.Point(651, 109);
      this.vgaGain.Maximum = 15;
      this.vgaGain.Name = "vgaGain";
      this.vgaGain.Size = new System.Drawing.Size(176, 45);
      this.vgaGain.TabIndex = 12;
      this.vgaGain.Value = 1;
      this.vgaGain.Scroll += new System.EventHandler(this.vgaGain_Scroll);
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point(283, 114);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(102, 17);
      this.checkBox1.TabIndex = 14;
      this.checkBox1.Text = "Enable WIndow";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
      // 
      // spectrumAnalyzer2
      // 
      this.spectrumAnalyzer2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.spectrumAnalyzer2.DisplayOffset = 0;
      this.spectrumAnalyzer2.DisplayRange = 130;
      this.spectrumAnalyzer2.Frequency = ((long)(0));
      this.spectrumAnalyzer2.Location = new System.Drawing.Point(12, 488);
      this.spectrumAnalyzer2.Name = "spectrumAnalyzer2";
      this.spectrumAnalyzer2.SampleRate = ((long)(0));
      this.spectrumAnalyzer2.Size = new System.Drawing.Size(855, 222);
      this.spectrumAnalyzer2.SpectrumScale = 1F;
      this.spectrumAnalyzer2.TabIndex = 13;
      // 
      // spectrumAnalyzer1
      // 
      this.spectrumAnalyzer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.spectrumAnalyzer1.DisplayOffset = 0;
      this.spectrumAnalyzer1.DisplayRange = 130;
      this.spectrumAnalyzer1.Frequency = ((long)(0));
      this.spectrumAnalyzer1.Location = new System.Drawing.Point(12, 238);
      this.spectrumAnalyzer1.Name = "spectrumAnalyzer1";
      this.spectrumAnalyzer1.SampleRate = ((long)(0));
      this.spectrumAnalyzer1.Size = new System.Drawing.Size(739, 244);
      this.spectrumAnalyzer1.SpectrumScale = 1F;
      this.spectrumAnalyzer1.TabIndex = 2;
      // 
      // fftTimer
      // 
      this.fftTimer.Interval = 25;
      this.fftTimer.Tick += new System.EventHandler(this.fftTimer_Tick);
      // 
      // SharpTest
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1445, 722);
      this.Controls.Add(this.checkBox1);
      this.Controls.Add(this.spectrumAnalyzer2);
      this.Controls.Add(this.vgaGain);
      this.Controls.Add(this.mixerGain);
      this.Controls.Add(this.lnaGain);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.trackBar5);
      this.Controls.Add(this.trackBar3);
      this.Controls.Add(this.offsetTrackBar);
      this.Controls.Add(this.trackBar1);
      this.Controls.Add(this.spectrumAnalyzer1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Name = "SharpTest";
      this.Text = "Form1";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.offsetTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lnaGain)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.mixerGain)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.vgaGain)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private RadioComponents.Visual.SpectrumAnalyzer spectrumAnalyzer1;
    private System.Windows.Forms.TrackBar trackBar3;
    private System.Windows.Forms.TrackBar offsetTrackBar;
    private System.Windows.Forms.TrackBar trackBar5;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TrackBar trackBar1;
    private System.Windows.Forms.TrackBar lnaGain;
    private System.Windows.Forms.TrackBar mixerGain;
    private System.Windows.Forms.TrackBar vgaGain;
    private RadioComponents.Visual.SpectrumAnalyzer spectrumAnalyzer2;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.Timer fftTimer;
  }
}

