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
      this.frequencyTrackBar = new System.Windows.Forms.TrackBar();
      this.fftTimer = new System.Windows.Forms.Timer(this.components);
      this.panel2 = new System.Windows.Forms.Panel();
      this.stopButton = new System.Windows.Forms.Button();
      this.startButton = new System.Windows.Forms.Button();
      this.scrollPanel = new System.Windows.Forms.Panel();
      this.controlPanel = new System.Windows.Forms.Panel();
      this.displayRangeBar = new System.Windows.Forms.TrackBar();
      this.offsetTrackBar = new System.Windows.Forms.TrackBar();
      this.spectrumScaleBar = new System.Windows.Forms.TrackBar();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.vgaGain = new System.Windows.Forms.TrackBar();
      this.mixerGain = new System.Windows.Forms.TrackBar();
      this.lnaGain = new System.Windows.Forms.TrackBar();
      this.ifSpectrumAnalizer = new RadioComponents.Visual.SpectrumAnalyzer();
      this.mainSpectrumAnalyzer = new RadioComponents.Visual.SpectrumAnalyzer();
      ((System.ComponentModel.ISupportInitialize)(this.frequencyTrackBar)).BeginInit();
      this.panel2.SuspendLayout();
      this.scrollPanel.SuspendLayout();
      this.controlPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.displayRangeBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.offsetTrackBar)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.spectrumScaleBar)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.vgaGain)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.mixerGain)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.lnaGain)).BeginInit();
      this.SuspendLayout();
      // 
      // frequencyTrackBar
      // 
      this.frequencyTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.frequencyTrackBar.Location = new System.Drawing.Point(289, 12);
      this.frequencyTrackBar.Maximum = 12000;
      this.frequencyTrackBar.Minimum = 8000;
      this.frequencyTrackBar.Name = "frequencyTrackBar";
      this.frequencyTrackBar.Size = new System.Drawing.Size(798, 45);
      this.frequencyTrackBar.TabIndex = 8;
      this.frequencyTrackBar.Value = 10630;
      this.frequencyTrackBar.ValueChanged += new System.EventHandler(this.frequencyTrackBar_ValueChanged);
      // 
      // fftTimer
      // 
      this.fftTimer.Interval = 25;
      this.fftTimer.Tick += new System.EventHandler(this.fftTimer_Tick);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.stopButton);
      this.panel2.Controls.Add(this.startButton);
      this.panel2.Location = new System.Drawing.Point(12, 9);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(271, 51);
      this.panel2.TabIndex = 17;
      // 
      // stopButton
      // 
      this.stopButton.Enabled = false;
      this.stopButton.Location = new System.Drawing.Point(138, 14);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(75, 23);
      this.stopButton.TabIndex = 3;
      this.stopButton.Text = "Stop";
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButtonClick);
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(57, 14);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 2;
      this.startButton.Text = "Start";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButtonClick);
      // 
      // scrollPanel
      // 
      this.scrollPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.scrollPanel.AutoScroll = true;
      this.scrollPanel.Controls.Add(this.controlPanel);
      this.scrollPanel.Location = new System.Drawing.Point(12, 66);
      this.scrollPanel.Name = "scrollPanel";
      this.scrollPanel.Size = new System.Drawing.Size(271, 530);
      this.scrollPanel.TabIndex = 18;
      // 
      // controlPanel
      // 
      this.controlPanel.AutoSize = true;
      this.controlPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.controlPanel.Controls.Add(this.displayRangeBar);
      this.controlPanel.Controls.Add(this.offsetTrackBar);
      this.controlPanel.Controls.Add(this.spectrumScaleBar);
      this.controlPanel.Controls.Add(this.checkBox1);
      this.controlPanel.Controls.Add(this.groupBox1);
      this.controlPanel.Location = new System.Drawing.Point(3, 3);
      this.controlPanel.Name = "controlPanel";
      this.controlPanel.Size = new System.Drawing.Size(244, 361);
      this.controlPanel.TabIndex = 17;
      // 
      // displayRangeBar
      // 
      this.displayRangeBar.Location = new System.Drawing.Point(35, 262);
      this.displayRangeBar.Maximum = 300;
      this.displayRangeBar.Minimum = 1;
      this.displayRangeBar.Name = "displayRangeBar";
      this.displayRangeBar.Size = new System.Drawing.Size(176, 45);
      this.displayRangeBar.TabIndex = 20;
      this.displayRangeBar.Value = 1;
      this.displayRangeBar.Scroll += new System.EventHandler(this.displayRangeBar_Scroll);
      // 
      // offsetTrackBar
      // 
      this.offsetTrackBar.Location = new System.Drawing.Point(35, 313);
      this.offsetTrackBar.Maximum = 15;
      this.offsetTrackBar.Name = "offsetTrackBar";
      this.offsetTrackBar.Size = new System.Drawing.Size(176, 45);
      this.offsetTrackBar.TabIndex = 19;
      this.offsetTrackBar.Value = 1;
      this.offsetTrackBar.ValueChanged += new System.EventHandler(this.offsetTrackBar_ValueChanged);
      // 
      // spectrumScaleBar
      // 
      this.spectrumScaleBar.Location = new System.Drawing.Point(35, 220);
      this.spectrumScaleBar.Maximum = 100;
      this.spectrumScaleBar.Minimum = 1;
      this.spectrumScaleBar.Name = "spectrumScaleBar";
      this.spectrumScaleBar.Size = new System.Drawing.Size(176, 45);
      this.spectrumScaleBar.TabIndex = 18;
      this.spectrumScaleBar.Value = 1;
      this.spectrumScaleBar.ValueChanged += new System.EventHandler(this.spectrumScaleBar_ValueChanged);
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Checked = true;
      this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
      this.checkBox1.Location = new System.Drawing.Point(72, 181);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(101, 17);
      this.checkBox1.TabIndex = 17;
      this.checkBox1.Text = "Enable Window";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.vgaGain);
      this.groupBox1.Controls.Add(this.mixerGain);
      this.groupBox1.Controls.Add(this.lnaGain);
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(238, 172);
      this.groupBox1.TabIndex = 16;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Gains";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(14, 121);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(54, 13);
      this.label4.TabIndex = 18;
      this.label4.Text = "VGA Gain";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(14, 72);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(57, 13);
      this.label3.TabIndex = 17;
      this.label3.Text = "Mixer Gain";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(14, 23);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(53, 13);
      this.label2.TabIndex = 16;
      this.label2.Text = "LNA Gain";
      // 
      // vgaGain
      // 
      this.vgaGain.Location = new System.Drawing.Point(74, 121);
      this.vgaGain.Maximum = 15;
      this.vgaGain.Name = "vgaGain";
      this.vgaGain.Size = new System.Drawing.Size(158, 45);
      this.vgaGain.TabIndex = 15;
      this.vgaGain.Value = 1;
      this.vgaGain.Scroll += new System.EventHandler(this.vgaGain_Scroll);
      // 
      // mixerGain
      // 
      this.mixerGain.Location = new System.Drawing.Point(74, 70);
      this.mixerGain.Maximum = 15;
      this.mixerGain.Name = "mixerGain";
      this.mixerGain.Size = new System.Drawing.Size(158, 45);
      this.mixerGain.TabIndex = 14;
      this.mixerGain.Value = 1;
      this.mixerGain.Scroll += new System.EventHandler(this.mixerGain_Scroll);
      // 
      // lnaGain
      // 
      this.lnaGain.Location = new System.Drawing.Point(74, 19);
      this.lnaGain.Maximum = 15;
      this.lnaGain.Name = "lnaGain";
      this.lnaGain.Size = new System.Drawing.Size(158, 45);
      this.lnaGain.TabIndex = 13;
      this.lnaGain.Value = 1;
      this.lnaGain.Scroll += new System.EventHandler(this.lnaGain_Scroll);
      // 
      // ifSpectrumAnalizer
      // 
      this.ifSpectrumAnalizer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ifSpectrumAnalizer.DisplayOffset = 0;
      this.ifSpectrumAnalizer.DisplayRange = 130;
      this.ifSpectrumAnalizer.Frequency = ((long)(0));
      this.ifSpectrumAnalizer.Location = new System.Drawing.Point(289, 313);
      this.ifSpectrumAnalizer.Name = "ifSpectrumAnalizer";
      this.ifSpectrumAnalizer.SampleRate = ((long)(0));
      this.ifSpectrumAnalizer.Size = new System.Drawing.Size(798, 283);
      this.ifSpectrumAnalizer.SpectrumScale = 1F;
      this.ifSpectrumAnalizer.TabIndex = 13;
      this.ifSpectrumAnalizer.Title = "IF Spectrum";
      // 
      // mainSpectrumAnalyzer
      // 
      this.mainSpectrumAnalyzer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mainSpectrumAnalyzer.DisplayOffset = 0;
      this.mainSpectrumAnalyzer.DisplayRange = 130;
      this.mainSpectrumAnalyzer.Frequency = ((long)(0));
      this.mainSpectrumAnalyzer.Location = new System.Drawing.Point(289, 63);
      this.mainSpectrumAnalyzer.Name = "mainSpectrumAnalyzer";
      this.mainSpectrumAnalyzer.SampleRate = ((long)(0));
      this.mainSpectrumAnalyzer.Size = new System.Drawing.Size(797, 244);
      this.mainSpectrumAnalyzer.SpectrumScale = 1F;
      this.mainSpectrumAnalyzer.TabIndex = 2;
      this.mainSpectrumAnalyzer.Title = "Radio Spectrum";
      // 
      // SharpTest
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1091, 600);
      this.Controls.Add(this.scrollPanel);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.ifSpectrumAnalizer);
      this.Controls.Add(this.frequencyTrackBar);
      this.Controls.Add(this.mainSpectrumAnalyzer);
      this.Name = "SharpTest";
      this.Text = "RTLSharp";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.frequencyTrackBar)).EndInit();
      this.panel2.ResumeLayout(false);
      this.scrollPanel.ResumeLayout(false);
      this.scrollPanel.PerformLayout();
      this.controlPanel.ResumeLayout(false);
      this.controlPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.displayRangeBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.offsetTrackBar)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.spectrumScaleBar)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.vgaGain)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.mixerGain)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.lnaGain)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private RadioComponents.Visual.SpectrumAnalyzer mainSpectrumAnalyzer;
    private System.Windows.Forms.TrackBar frequencyTrackBar;
    private RadioComponents.Visual.SpectrumAnalyzer ifSpectrumAnalizer;
    private System.Windows.Forms.Timer fftTimer;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Panel scrollPanel;
    private System.Windows.Forms.Panel controlPanel;
    private System.Windows.Forms.TrackBar displayRangeBar;
    private System.Windows.Forms.TrackBar offsetTrackBar;
    private System.Windows.Forms.TrackBar spectrumScaleBar;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TrackBar vgaGain;
    private System.Windows.Forms.TrackBar mixerGain;
    private System.Windows.Forms.TrackBar lnaGain;
  }
}

