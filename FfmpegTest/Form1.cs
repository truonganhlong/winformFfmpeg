using FFmpeg.AutoGen;
using System.Diagnostics;
using System.Threading;

namespace FfmpegTest
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cancellationTokenSource;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            StartStream();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel(); // Dừng stream
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
        private async void StartStream()
        {
            for (int i = 0; i < listPictureBoxes().Count; i++) {
                if (!string.IsNullOrWhiteSpace(listTextBox()[i].Text))
                {
                    await Task.Run(() => RunFFmpeg(listTextBox()[i].Text, listPictureBoxes()[i]));
                }
            }
        }
        private void RunFFmpeg(string rtspUrl, PictureBox pictureBox)
        {
            int width = pictureBox.Width;
            int height = pictureBox.Height;
            // Đường dẫn tới ffmpeg.exe
            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "bin", "ffmpeg.exe");
            string arguments;
            // Các tham số để chạy FFmpeg với RTSP stream
            if (rtspUrl.Contains("rtsp"))
            {
                arguments = $"-hwaccel dxva2 -rtsp_transport tcp -i {rtspUrl} -r 15 -vf scale={width}:{height} -fflags nobuffer -analyzeduration 1000000 -f mjpeg -q:v 2 pipe:1";
            }
            else if (rtspUrl.Contains("m3u8"))
            {
                //arguments = $"-i {rtspUrl} -vf scale={width}:{height} -fflags nobuffer -f mjpeg -q:v 2 pipe:1";
                //arguments = $"-re -i {rtspUrl} -vf scale={width}:{height} -fflags +discardcorrupt -f mjpeg -q:v 2 pipe:1";
                arguments = $"-re -i {rtspUrl} -vf scale={width}:{height} -fflags +discardcorrupt -analyzeduration 5000000 -probesize 5000000 -f mjpeg -q:v 2 pipe:1";


            }
            else
            {
                throw new ArgumentException("Unsupported stream URL");
            }
            //string arguments = $"-hwaccel dxva2 -rtsp_transport tcp -i {rtspUrl} -r 15 -vf scale={width}:{height} -fflags nobuffer -analyzeduration 1000000 -f mjpeg -q:v 2 pipe:1";
            //string arguments = $"-i {rtspUrl} -vf scale={width}:{height} -fflags nobuffer -f mjpeg -q:v 2 pipe:1";

            using (Process ffmpegProcess = new Process())
            {
                ffmpegProcess.StartInfo.FileName = ffmpegPath;
                ffmpegProcess.StartInfo.Arguments = arguments;
                ffmpegProcess.StartInfo.UseShellExecute = false;
                ffmpegProcess.StartInfo.RedirectStandardOutput = true;
                ffmpegProcess.StartInfo.RedirectStandardError = true;
                ffmpegProcess.StartInfo.CreateNoWindow = true;

                ffmpegProcess.OutputDataReceived += (sender, e) =>
                {
                    Debug.WriteLine(e.Data);
                };

                // Khởi chạy tiến trình
                ffmpegProcess.Start();

                // Đọc luồng dữ liệu từ FFmpeg
                using (Stream output = ffmpegProcess.StandardOutput.BaseStream)
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            // Đọc luồng dữ liệu từ FFmpeg (từng khung hình dạng MJPEG)
                            var image = ReadFrame(output);
                            if (image != null)
                            {
                                // Cập nhật hình ảnh trên PictureBox trong UI thread
                                pictureBox.Invoke((MethodInvoker)delegate
                                {
                                    pictureBox.Image = image;
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error: " + ex.Message);
                            break;
                        }
                    }
                }

                ffmpegProcess.WaitForExit();
            }
        }

        private Image ReadFrame(Stream outputStream)
        {
            try
            {
                // Đọc header để xác định kích thước khung hình
                byte[] buffer = new byte[4096];
                int bytesRead = 0;

                // Đọc toàn bộ ảnh (frame) từ luồng FFmpeg
                using (MemoryStream ms = new MemoryStream())
                {
                    while ((bytesRead = outputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                        if (bytesRead < buffer.Length) break;
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading frame: {ex.Message}");
                return null;
            }
        }
    }
}
