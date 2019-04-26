using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Grpc.Auth;
using System.Threading;

namespace APITests
{
    class Launcher
    {
        static public void Main()
        {
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            //Console.WriteLine(homePath + "\\Downloads\\hellocanuhearmeplssendhelp.mp3");
            //Console.WriteLine(homePath + "\\Downloads\\hellocanuhearmeplssendhelp.mp3");
            fromAudioFile(homePath + "\\Downloads\\hellocanuhearmeplssendhelp.flac");
            fromAudioFile(homePath + "\\Downloads\\peter.flac");
            //fromAudioFile(homePath + "\\Downloads\\blueapple.mp3");
            //fromAudioFile(homePath + "\\Downloads\\brooklyn.flac");
            Console.WriteLine("part 2");
            var task = StreamingMicRecognizeAsync(10);
            Console.WriteLine("part 4");
            var res = task.Result;
            Console.WriteLine("part 5");
            var original = "gs://cloud-samples-tests/speech/brooklyn.flac";
        }

        static public SpeechClient getSpeechClient()
        {
            SpeechSettings settings = new SpeechSettings();

            GoogleCredential googleCredential;
            using (Stream m = new FileStream("./arcap.json", FileMode.Open))
                googleCredential = GoogleCredential.FromStream(m);
            var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.Host,
                googleCredential.ToChannelCredentials());
            return SpeechClient.Create(channel);
        }

        static public void fromAudioFile(String url)
        {
            var speech = getSpeechClient();
            var config = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                //Encoding = RecognitionConfig.Types.AudioEncoding.FLAC,
                //SampleRateHertz = 16000,
                LanguageCode = LanguageCodes.English.UnitedStates
            };
            //var audio = RecognitionAudio.FromStorageUri(url);
            Console.WriteLine("Gettingfrom url" + url);
            var audio = RecognitionAudio.FromFile(url);

            Console.WriteLine("calling recognize");
            var response = speech.Recognize(config, audio);
            Console.WriteLine("finished recognize");
            Console.WriteLine(response.ToString());

            foreach (var result in response.Results)
            {
                Console.WriteLine("res");
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine("alt");
                    Console.WriteLine(alternative.Transcript);
                }
            }
            Console.WriteLine("done");
        }

        static async Task<object> StreamingMicRecognizeAsync(int seconds)
        {
            if (NAudio.Wave.WaveIn.DeviceCount < 1)
            {
                Console.WriteLine("No microphone!");
                return -1;
            }
            var speech = getSpeechClient();
            var streamingCall = speech.StreamingRecognize();
            // Write the initial request with the config.
            await streamingCall.WriteAsync(
                new StreamingRecognizeRequest()
                {
                    StreamingConfig = new StreamingRecognitionConfig()
                    {
                        Config = new RecognitionConfig()
                        {
                            Encoding =
                            RecognitionConfig.Types.AudioEncoding.Linear16,
                            SampleRateHertz = 16000,
                            LanguageCode = "en",
                        },
                        InterimResults = true,
                    }
                });
            // Print responses as they arrive.
            Task printResponses = Task.Run(async () =>
            {
                while (await streamingCall.ResponseStream.MoveNext(
                    default(CancellationToken)))
                {
                    foreach (var result in streamingCall.ResponseStream
                        .Current.Results)
                    {
                        foreach (var alternative in result.Alternatives)
                        {
                            Console.WriteLine(alternative.Transcript);
                        }
                    }
                }
            });
            // Read from the microphone and stream to API.
            object writeLock = new object();
            bool writeMore = true;
            var waveIn = new NAudio.Wave.WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
            waveIn.DataAvailable +=
                (object sender, NAudio.Wave.WaveInEventArgs args) =>
                {
                    lock (writeLock)
                    {
                        if (!writeMore) return;
                        streamingCall.WriteAsync(
                            new StreamingRecognizeRequest()
                            {
                                AudioContent = Google.Protobuf.ByteString
                                    .CopyFrom(args.Buffer, 0, args.BytesRecorded)
                            }).Wait();
                    }
                };
            waveIn.StartRecording();
            Console.WriteLine("Speak now.");
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            // Stop recording and shut down.
            waveIn.StopRecording();
            lock (writeLock) writeMore = false;
            await streamingCall.WriteCompleteAsync();
            await printResponses;
            return 0;
        }
    }
}
