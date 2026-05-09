using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDFDeSecure
{
    public partial class PDFDeSecure : Form
    {
        PdfDocument pdf = new PdfDocument();
        PdfDocument outpdf = new PdfDocument();

        public PDFDeSecure()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 2)
            {
                RunAutoMode(args);
                Environment.Exit(0);
            }
        }

        private enum UnlockStrategy { Auto, Lossless, Render, Ocr }

        private void RunAutoMode(string[] args)
        {
            var input = args[1];
            var output = args[2];
            var strategy = UnlockStrategy.Auto;
            var writeReport = false;

            for (var i = 3; i < args.Length; i++)
            {
                if (args[i].StartsWith("--strategy=", StringComparison.OrdinalIgnoreCase))
                {
                    Enum.TryParse(args[i].Substring("--strategy=".Length), true, out strategy);
                }
                if (args[i].Equals("--report", StringComparison.OrdinalIgnoreCase))
                {
                    writeReport = true;
                }
            }

            DirectoryInfo di = new DirectoryInfo(input);
            var aryFi = di.GetFiles("*.pdf");
            var counter = 0;
            var error = 0;

            foreach (FileInfo fi in aryFi)
            {
                var report = string.Empty;
                try
                {
                    report = TryUnlock(fi.FullName, Path.Combine(output, fi.Name), strategy);
                    counter++;
                }
                catch (Exception ex)
                {
                    error++;
                    report += Environment.NewLine + ex;
                    File.WriteAllText(Path.Combine(output, "Error-" + fi.Name + ".log"), report);
                }

                if (writeReport)
                {
                    File.WriteAllText(Path.Combine(output, "Report-" + fi.Name + ".log"), report);
                }
            }

            MessageBox.Show("Unlocked " + counter + " files" + Environment.NewLine + "Failed " + error + " files" + Environment.NewLine + "Percentage " + counter + "/" + (counter + error) + " = " + ((float)counter / (counter + error) * 100).ToString("f2") + "%, Cheers!", "PDF file Unlocked! and Saved!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string TryUnlock(string inputPath, string outputPath, UnlockStrategy strategy)
        {
            if (strategy == UnlockStrategy.Lossless)
            {
                return LosslessUnlock(inputPath, outputPath);
            }

            if (strategy == UnlockStrategy.Render)
            {
                return RenderUnlock(inputPath, outputPath);
            }

            if (strategy == UnlockStrategy.Ocr)
            {
                return OcrUnlock(inputPath, outputPath);
            }

            // Auto strategy fallback chain
            try { return LosslessUnlock(inputPath, outputPath); }
            catch (Exception ex1)
            {
                try { return "Lossless failed: " + ex1.Message + Environment.NewLine + RenderUnlock(inputPath, outputPath); }
                catch (Exception ex2)
                {
                    return "Lossless failed: " + ex1.Message + Environment.NewLine + "Render failed: " + ex2.Message + Environment.NewLine + OcrUnlock(inputPath, outputPath);
                }
            }
        }

        private string LosslessUnlock(string inputPath, string outputPath)
        {
            outpdf = new PdfDocument();
            using (var stream = new FileInfo(inputPath).OpenRead())
            {
                pdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                foreach (PdfPage page in pdf.Pages)
                {
                    outpdf.AddPage(page);
                }
            }
            using (var outStream = new FileInfo(outputPath).Open(FileMode.Create, FileAccess.Write))
            {
                outpdf.Save(outStream, true);
            }
            return "Pass: Lossless(PdfSharp)";
        }

        private string RenderUnlock(string inputPath, string outputPath)
        {
            var gs = "gs";
            var cmd = $"-dBATCH -dNOPAUSE -sDEVICE=pdfwrite -sOutputFile=\"{outputPath}\" \"{inputPath}\"";
            RunProcess(gs, cmd);
            return "Pass: Render(Ghostscript)";
        }

        private string OcrUnlock(string inputPath, string outputPath)
        {
            // Requires ocrmypdf installed in runtime environment.
            RunProcess("ocrmypdf", $"--force-ocr \"{inputPath}\" \"{outputPath}\"");
            return "Pass: OCR(ocrmypdf)";
        }

        private void RunProcess(string fileName, string arguments)
        {
            var p = new Process();
            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            var stdout = p.StandardOutput.ReadToEnd();
            var stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                throw new InvalidOperationException(fileName + " failed. " + stdout + " " + stderr);
            }
        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Select PDF File";
            openFileDialog1.DefaultExt = "pdf";
            openFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pdffile.Text = openFileDialog1.FileName.ToString();
                btnunlock.Enabled = true;
            }
        }

        private void btnunlock_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save Unlocked PDF File";
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var report = TryUnlock(pdffile.Text, saveFileDialog1.FileName, UnlockStrategy.Auto);
                MessageBox.Show("PDF file Unlocked! and Saved!" + Environment.NewLine + report, "Unlocked & Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
