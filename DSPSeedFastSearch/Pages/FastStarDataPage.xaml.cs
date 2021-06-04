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
        }
        public int mSeedMin { get; set; } = 0;
        public int mSeedMax { get; set; } = 1000000;
        public int mStarCount { get; set; } = 64;

        Task currentTask;
        CancellationTokenSource cancellationTokenSource;
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
            benchmarkButton.IsEnabled = false;
            //seeds.Clear();
            cancellationTokenSource = new CancellationTokenSource();
            currentTask = Task.Run((Action)DoSomeWork, cancellationTokenSource.Token);
            currentTask.ContinueWith((_) => { this.Dispatcher.Invoke(() => { generateButton.Visibility = Visibility.Visible; benchmarkButton.IsEnabled = true; }); });
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
            long coust1 = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            Parallel.For(0, bufferSize, (int i) => result[i] = new FastStarData(i, mStarCount));
            stopWatch.Stop();
            long coust2 = stopWatch.ElapsedMilliseconds;
            message.Text = string.Format("benchmark result:\n{2} star,{3} seeds,\nsingle core {0}ms,\nmulti core {1}ms,", coust1, coust2, mStarCount, bufferSize);
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
            FileStream fileStream = null;
            Utf8JsonWriter utf8JsonWriter = null;
            var options = new JsonWriterOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All);
            for (int i = mSeedMin; i < mSeedMax; i += bufferSize)
            {
                utf8JsonWriter?.Flush();
                utf8JsonWriter?.Dispose();
                utf8JsonWriter = null;
                fileStream?.Flush();
                fileStream?.Close();
                fileStream = null;
                if (token.IsCancellationRequested) break;
                Parallel.For(i, Math.Min(i + bufferSize, mSeedMax), (int j) =>
                {
                    result[j - i] = new FastStarData(j, mStarCount);
                });

                fileStream = new FileStream(string.Format("{0}-{1}.json", i, Math.Min(i + bufferSize, mSeedMax) - 1), FileMode.Create, FileAccess.Write);
                int resultLen = Math.Min(bufferSize, mSeedMax - i);
                utf8JsonWriter = new Utf8JsonWriter(fileStream, options);
                WriteObjects(utf8JsonWriter, new Span<FastStarData>(result, 0, resultLen));

            }
            utf8JsonWriter?.Flush();
            utf8JsonWriter?.Dispose();
            fileStream?.Flush();
            fileStream?.Close();
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
