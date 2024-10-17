using FFmpeg.AutoGen;
using System.Diagnostics;

namespace FfmpegTest
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private Process process;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => StartStream(cancellationTokenSource.Token));
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel(); // Dừng stream
        }

        private List<PictureBox> listPictureBoxes()
        {
            return new List<PictureBox>
            {
                pictureBox1,
                pictureBox2,
                pictureBox3,
                pictureBox4,
                pictureBox5,
                pictureBox6,
                pictureBox7,
                pictureBox8,
                pictureBox9,
                pictureBox10,
                pictureBox11,
                pictureBox12,
                pictureBox13,
                pictureBox14,
                pictureBox15,
                pictureBox16,
                pictureBox17,
                pictureBox18,
                pictureBox19,
                pictureBox20,
                pictureBox21,
                pictureBox22,
                pictureBox23,
                pictureBox24,
                pictureBox25,
                pictureBox26,
                pictureBox27,
                pictureBox28,
                pictureBox29,
                pictureBox30,
                pictureBox31,
                pictureBox32,
                pictureBox33,
                pictureBox34,
                pictureBox35,
                pictureBox36
            };
        }

        private List<TextBox> listTextBox()
        {
            return new List<TextBox>
            {
                textBox1,
                textBox2,
                textBox3,
                textBox4,
                textBox5,
                textBox6,
                textBox7,
                textBox8,
                textBox9,
                textBox10,
                textBox11,
                textBox12,
                textBox13,
                textBox14,
                textBox15,
                textBox16,
                textBox17,
                textBox18,
                textBox19,
                textBox20,
                textBox21,
                textBox22,
                textBox23,
                textBox24,
                textBox25,
                textBox26,
                textBox27,
                textBox28,
                textBox29,
                textBox30,
                textBox31,
                textBox32,
                textBox33,
                textBox34,
                textBox35,
                textBox36
            };
        }
        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                // Chuyển đổi dữ liệu đầu ra thành hình ảnh
                byte[] imageBytes = new byte[e.Data.Length]; // Đoạn này cần được sửa để đọc dữ liệu đúng cách
                using (var ms = new MemoryStream(imageBytes))
                {
                    var image = Image.FromStream(ms);
                    this.Invoke(new Action(() => pictureBox1.Image = image)); // Cập nhật PictureBox
                }
            }
        }


        private void StartStream(CancellationToken cancellationToken)
        {
            var ffmpegPath = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg\\bin\\ffmpeg.exe")).ToString();
            for (int i = 0; i < listPictureBoxes().Count; i++)
            {
                String url = listTextBox()[i].Text;
                PictureBox pictureBox = listPictureBoxes()[i];
                int width = pictureBox.Width;
                int height = pictureBox.Height;
                if (!string.IsNullOrWhiteSpace(url)) {
                    var ffmpegArgs = $"ffmpeg -hwaccel dxva2 -i {url} -f image2pipe -vf scale={width}:{height} -vcodec mjpeg -";
                    try
                    {
                        //var processStartInfo = new ProcessStartInfo
                        //{
                        //    FileName = ffmpegPath,
                        //    Arguments = ffmpegArgs,
                        //    RedirectStandardInput = true,
                        //    RedirectStandardOutput = true,
                        //    UseShellExecute = false,
                        //    CreateNoWindow = true
                        //};
                        process = new Process();
                        process.StartInfo.FileName = ffmpegPath;
                        process.StartInfo.Arguments = ffmpegArgs;
                        //process.StartInfo.RedirectStandardInput = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.OutputDataReceived += OnOutputDataReceived;
                        process.Start();
                        process.BeginOutputReadLine();

                        //using (var ffmpegProcess = Process.Start(processStartInfo))
                        //using (var outputStream = ffmpegProcess.StandardOutput.BaseStream)
                        //{
                        //    byte[] buffer = new byte[1024 * 1024]; // Buffer để chứa dữ liệu ảnh
                        //    MemoryStream memoryStream = new MemoryStream();

                        //    while (!cancellationToken.IsCancellationRequested)
                        //    {
                        //        int bytesRead = outputStream.Read(buffer, 0, buffer.Length);
                        //        if (bytesRead > 0)
                        //        {
                        //            memoryStream.Write(buffer, 0, bytesRead);

                        //            try
                        //            {
                        //                Image frame = Image.FromStream(memoryStream);
                        //                memoryStream.SetLength(0); // Reset lại MemoryStream sau khi lấy frame

                        //                // Hiển thị frame trên PictureBox
                        //                pictureBox.Invoke((MethodInvoker)(() => pictureBox.Image = frame));
                        //            }
                        //            catch (Exception)
                        //            {
                        //                // Bỏ qua frame nếu không hợp lệ
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                    }
                }
            }
        }
    }
}
