using DSPSeedFastSearch.FastStarDatas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
using System.IO;
using System.Text.Json;
using System.Text.Unicode;
using DSPSeedFastSearch.GPUAccelerate;
using Vortice.Dxc;
using System.Runtime.InteropServices;

namespace DSPSeedFastSearch.Pages
{
    /// <summary>
    /// FastStarData.xaml 的交互逻辑
    /// </summary>
    public partial class FastStarDataPage : Page
    {
        public FastStarDataPage()
        {
            InitializeComponent();
            uiElements = new List<UIElement>()
            {
                ViewButton,
                benchmarkButton,
                gpuBenchmarkButton,
            };
        }
        public int mSeedMin { get; set; } = 0;
        public int mSeedMax { get; set; } = 1000000;
        public int mStarCount { get; set; } = 64;

        Task currentTask;
        CancellationTokenSource cancellationTokenSource;
        List<UIElement> uiElements;
        public void Deactive()
        {
            cancellationTokenSource?.Cancel();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Deactive();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            generateButton.Visibility = Visibility.Collapsed;
            foreach (var uiElement in uiElements)
            {
                uiElement.IsEnabled = false;
            }

            cancellationTokenSource = new CancellationTokenSource();
            currentTask = Task.Run((Action)DoSomeWork, cancellationTokenSource.Token);
            currentTask.ContinueWith((_) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    generateButton.Visibility = Visibility.Visible;
                    foreach (var uiElement in uiElements)
                    {
                        uiElement.IsEnabled = true;
                    }
                });
            });
        }

        private void BenchMarkButton_Click(object sender, RoutedEventArgs e)
        {
            int bufferSize = 8192;
            FastStarData[] result = new FastStarData[bufferSize];
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < bufferSize; i++)
            {
                result[i] = new FastStarData(i, mStarCount);
            }
            stopWatch.Stop();
            long cost1 = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            Parallel.For(0, bufferSize, (int i) => result[i] = new FastStarData(i, mStarCount));
            stopWatch.Stop();
            long cost2 = stopWatch.ElapsedMilliseconds;
            message.Text = string.Format("benchmark result:\n{2} star,{3} seeds,\nsingle core {0}ms,\nmulti core {1}ms,", cost1, cost2, mStarCount, bufferSize);
        }

        private void GPUBenchMarkButton_Click(object sender, RoutedEventArgs e)
        {
            var compileResult = DxcCompiler.Compile(DxcShaderStage.Compute, File.ReadAllText("Data/Shaders/FastStarData.hlsl"), "main");
            if (compileResult.GetStatus() != SharpGen.Runtime.Result.Ok)
            {
                MessageBox.Show(compileResult.GetErrors());
                return;
            }
            ComputeDevice device = new ComputeDevice();
            CommandList commandList = new CommandList();
            ComputeShader computeShader = new ComputeShader();
            computeShader.byteCode = compileResult.GetResult().ToArray();
            RootSignature rootSignature = new RootSignature();
            RWBuffer rwBuffer = new RWBuffer();
            ReadBackBuffer readBackBuffer = new ReadBackBuffer();
            RingUploadBuffer uploadBuffer = new RingUploadBuffer();
            int testBufferSize = 65536 * 128;
            int errorCount = 0;
            int big = 0;
            int small = 0;
            int bigError = 0;
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            string time = "";
            string CPUTime = "";
            try
            {
                device.Init();
                commandList.Init(device);
                device.CreateRootSignature(rootSignature, new[] { RootSignatureParamP.CBV, RootSignatureParamP.UAV, });
                device.CreateRWBuffer(rwBuffer, testBufferSize);
                device.CreateReadBackBuffer(readBackBuffer, testBufferSize);
                uploadBuffer.Initialize(device, 65536);

                commandList.BeginCommand();
                commandList.SetDescriptorHeapDefault();
                commandList.SetComputeRootSignature(rootSignature);
                commandList.SetComputeShader(computeShader);
                int batchCount = 64;
                int sizePerBatch = testBufferSize / batchCount / sizeof(int);
                for (int i = 0; i < batchCount; i++)
                {
                    int offset = uploadBuffer.Upload<int>(new int[] { i * sizePerBatch, i * sizePerBatch });
                    commandList.SetComputeUAV(rwBuffer, 0, 0);
                    uploadBuffer.SetComputeCBV(commandList, offset, 0);
                    commandList.Dispatch(sizePerBatch / 16, 1, 1);
                }
                commandList.Copy(rwBuffer, readBackBuffer);
                commandList.EndCommand();
                commandList.Execute();
                device.WaitForGpu();
                int[] testData1 = new int[testBufferSize / sizeof(int)];

                readBackBuffer.CopyTo(MemoryMarshal.Cast<int, byte>(testData1));
                stopwatch.Stop();
                time = stopwatch.ElapsedMilliseconds.ToString();
                stopwatch.Restart();
                //for (int i = 0; i < testData1.Length; i++)
                //{
                //    Algorithms.URandom1 random1 = new Algorithms.URandom1(i);
                //    for (int j = 0; j < 5; j++)
                //    {
                //        random1.Next();
                //    }
                //    int testData2 = random1.Next();
                //    int viewResult = testData1[i];
                //    if (viewResult != testData2)
                //    {
                //        if (viewResult > testData2)
                //            big++;
                //        if (viewResult < testData2)
                //            small++;
                //        if (Math.Abs(viewResult - testData2) > 1)
                //            bigError++;
                //        errorCount++;
                //    }
                //}
                Parallel.For(0, testData1.Length, (int i) =>
                  {
                      Algorithms.URandom1 random1 = new Algorithms.URandom1(i);
                      for (int j = 0; j < 5; j++)
                      {
                          random1.Next();
                      }
                      int testData2 = random1.Next();
                      int viewResult = testData1[i];
                      if (viewResult != testData2)
                      {
                          if (viewResult > testData2)
                              Interlocked.Increment(ref big);
                          if (viewResult < testData2)
                              Interlocked.Increment(ref small);
                          if (Math.Abs(viewResult - testData2) > 1)
                              Interlocked.Increment(ref bigError);
                          Interlocked.Increment(ref errorCount);
                      }
                  });

                stopwatch.Stop();
                CPUTime = stopwatch.ElapsedMilliseconds.ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
            finally
            {
                device.Dispose();
                rootSignature.Dispose();
                computeShader.Dispose();
                commandList.Dispose();
                rwBuffer.Dispose();
                uploadBuffer.Dispose();
                readBackBuffer.Dispose();
            }

            //message.Text = string.Format("benchmark result:\n{0} star,cost {1}ms",mStarCount, time);
            message.Text = string.Format("benchmark result:\n{0} random,\ncost {1}ms,\nCPU cost {2}ms, {3} error,\n{4} big, {5} small", testBufferSize / sizeof(int), time, CPUTime, errorCount, big, small);
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            FastStarData fastStarData = new FastStarData(mSeedMin, mStarCount);
            viewSeeds.ItemsSource = fastStarData.stars;
        }
        private void DoSomeWork()
        {
            CancellationToken token = cancellationTokenSource.Token;
            int bufferSize = 8192;
            FastStarData[] result = new FastStarData[bufferSize];
            for (int i = mSeedMin; i < mSeedMax; i += bufferSize)
            {
                if (token.IsCancellationRequested) break;
                Parallel.For(i, Math.Min(i + bufferSize, mSeedMax), (int j) =>
                {
                    result[j - i] = new FastStarData(j, mStarCount);
                });

                FileStream fileStream = new FileStream(string.Format("{0}-{1}.json", i, Math.Min(i + bufferSize, mSeedMax) - 1), FileMode.Create, FileAccess.Write);
                WriteObjects(fileStream, new Span<FastStarData>(result, 0, Math.Min(bufferSize, mSeedMax - i)));
                fileStream?.Flush();
                fileStream?.Close();
            }
        }

        private void WriteObjects(Stream stream, Span<FastStarData> starDatas)
        {
            var options = new JsonWriterOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
            Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(stream, options);
            WriteObjects(utf8JsonWriter, starDatas);
            utf8JsonWriter?.Flush();
            utf8JsonWriter?.Dispose();
        }
        private void WriteObjects(Utf8JsonWriter utf8JsonWriter, Span<FastStarData> starDatas)
        {
            float filterValue = 2.6f;
            utf8JsonWriter.WriteStartArray();
            for (int j = 0; j < starDatas.Length; j++)
            {
                bool hasStar = false;
                foreach (Star star in starDatas[j].stars)
                {
                    if (star.dysonLumino > filterValue)
                    {
                        hasStar = true;
                        break;
                    }
                }
                if (!hasStar)
                    continue;

                utf8JsonWriter.WriteStartObject();
                utf8JsonWriter.WriteNumber("seed", starDatas[j].seed);
                utf8JsonWriter.WriteStartArray("dysonLuminos");
                foreach (Star star in starDatas[j].stars)
                {
                    utf8JsonWriter.WriteNumberValue(star.dysonLumino);
                }
                utf8JsonWriter.WriteEndArray();
                utf8JsonWriter.WriteEndObject();
            }
            utf8JsonWriter.WriteEndArray();
        }
    }
}
