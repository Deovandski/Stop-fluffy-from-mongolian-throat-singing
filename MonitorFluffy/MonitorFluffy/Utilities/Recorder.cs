using CSCore;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace MonitorFluffy.Utilities
{
    /// <summary>
    /// Meant For Recording Audio
    /// </summary>
    internal class Recorder
    {
        internal enum RECORDING_TYPE
        {
            TEST, BEFORE_AUDIO, AFTER_AUDIO
        }

        public Recorder()
        {
            Record(1, RECORDING_TYPE.TEST);
        }

        public void Record(int secondsToRecord, RECORDING_TYPE recording_type)
        {

            MMDeviceCollection devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Capture, DeviceState.Active);

            if (devices.Count > 0)
            {
                // https://github.com/filoe/cscore/blob/master/Samples/RecordWithSpecificFormat/Program.cs
                using (WasapiCapture soundIn = new WasapiCapture())
                {
                    // Slap that first device in and call it good
                    soundIn.Device = devices[0];

                    //initialize the soundIn instance
                    soundIn.Initialize();

                    //create a SoundSource around the the soundIn instance
                    //this SoundSource will provide data, captured by the soundIn instance
                    SoundInSource soundInSource = new SoundInSource(soundIn) { FillWithZeros = false };

                    //create a source, that converts the data provided by the
                    //soundInSource to any other format
                    //in this case the "Fluent"-extension methods are being used
                    IWaveSource convertedSource = soundInSource
                        .ChangeSampleRate(48000) // sample rate
                        .ToSampleSource()
                        .ToWaveSource(8); //bits per sample

                    //channels...
                    using (convertedSource = convertedSource.ToStereo())
                    {
                        //create a new wavefile
                        string fileName = "";
                        switch (recording_type)
                        {
                            case RECORDING_TYPE.AFTER_AUDIO:
                                fileName = "After Playing Audio";
                                break;
                            case RECORDING_TYPE.BEFORE_AUDIO:
                                fileName = "Before Playing Audio";
                                break;
                            case RECORDING_TYPE.TEST:
                                fileName = "Test Audio";
                                break;
                        }
                        using (WaveWriter waveWriter = new WaveWriter(Pathing.InTodayPath + "\\" + DateTime.Now.ToString("HH-mm tt") + " - " + fileName + ".wav", convertedSource.WaveFormat))
                        {

                            //register an event handler for the DataAvailable event of 
                            //the soundInSource
                            //Important: use the DataAvailable of the SoundInSource
                            //If you use the DataAvailable event of the ISoundIn itself
                            //the data recorded by that event might won't be available at the
                            //soundInSource yet
                            soundInSource.DataAvailable += (s, e) =>
                            {
                                //read data from the converedSource
                                //important: don't use the e.Data here
                                //the e.Data contains the raw data provided by the 
                                //soundInSource which won't have your target format
                                byte[] buffer = new byte[convertedSource.WaveFormat.BytesPerSecond / 2];
                                int read;

                                //keep reading as long as we still get some data
                                //if you're using such a loop, make sure that soundInSource.FillWithZeros is set to false
                                while ((read = convertedSource.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    //write the read data to a file
                                    // ReSharper disable once AccessToDisposedClosure
                                    waveWriter.Write(buffer, 0, read);
                                }
                            };

                            //we've set everything we need -> start capturing data
                            soundIn.Start();

                            Thread.Sleep(secondsToRecord * 1000);

                            soundIn.Stop();
                        }
                    }
                }
            }
            else
            {
                throw new Exception("No Recording Devices available");
            }
        }
    }
}
