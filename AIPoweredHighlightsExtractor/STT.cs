using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace AIPoweredHighlightsExtractor
{
	public class STT
	{
		public STT()
		{
			this.FinalText = new StringBuilder();
		}
		/// <summary>
		/// Gets the default locale.
		/// </summary>
		/// <value>
		/// The default locale.
		/// </value>
		private string DefaultLocale
		{
			get { return "en-US"; }
		}
		/// <summary>
		/// The data recognition client
		/// </summary>
		public DataRecognitionClient dataClient;
		/// <summary>
		/// You can also put the primary key in app.config, instead of using UI.
		/// string subscriptionKey = ConfigurationManager.AppSettings["primaryKey"];
		/// </summary>
		public string subscriptionKey;

		/// <summary>
		/// Gets or sets subscription key
		/// </summary>
		public string SubscriptionKey
		{
			get
			{
				return this.subscriptionKey;
			}

			set
			{
				this.subscriptionKey = value;
			}
		}

		public StringBuilder FinalText;
		/// <summary>
		/// Creates a data client without LUIS intent support.
		/// Speech recognition with data (for example from a file or audio source).  
		/// The data is broken up into buffers and each buffer is sent to the Speech Recognition Service.
		/// No modification is done to the buffers, so the user can apply their
		/// own Silence Detection if desired.
		/// </summary>
		private void CreateDataRecoClient()
		{
				this.dataClient = SpeechRecognitionServiceFactory.CreateDataClient(
					SpeechRecognitionMode.LongDictation,
					this.DefaultLocale,
					this.SubscriptionKey);
				this.dataClient.AuthenticationUri = "";

				// Event handlers for speech recognition results
				//if (this.Mode == SpeechRecognitionMode.ShortPhrase)
				//{
				//	this.dataClient.OnResponseReceived += this.OnDataShortPhraseResponseReceivedHandler;
				//}
				//else
				//{
					this.dataClient.OnResponseReceived += this.OnDataDictationResponseReceivedHandler;
				//}

				//this.dataClient.OnPartialResponseReceived += this.OnPartialResponseReceivedHandler;
				//this.dataClient.OnConversationError += this.OnConversationErrorHandler;
		}
		/// <summary>
		/// Called when a final response is received;
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="SpeechResponseEventArgs"/> instance containing the event data.</param>
		private void OnDataDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
		{
			//Console.WriteLine("--- OnDataDictationResponseReceivedHandler ---");
			if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.EndOfDictation ||
				e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
			{
				if (this.FinalText != null)
				{
					System.IO.StreamWriter file = new System.IO.StreamWriter(@".\..\summary.txt");
					file.WriteLine(this.FinalText.ToString());
					file.Close();
					RunPythonScript(file);
					//Environment.Exit(0);
					Console.WriteLine("Phewww!!! At last I did it. That was a long run. Please check out summary.txt in your bin folder!");
				}
			}

			this.WriteResponseResult(e);
			return;
		}

		private void RunPythonScript(StreamWriter file)
		{
			ProcessStartInfo start = new ProcessStartInfo();
			//Location to your python.exe
			start.FileName = "C:\\Users\\saagraw\\AppData\\Local\\Programs\\Python\\Python37-32\\python.exe";
			start.Arguments = string.Format("{0}", "nlp.py");
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			using (Process process = Process.Start(start))
			{
				using (StreamReader reader = process.StandardOutput)
				{
					string result = reader.ReadToEnd();
					//Console.Write(result);
				}
			}
		}

		/// <summary>
		/// Writes the response result.
		/// </summary>
		/// <param name="e">The <see cref="SpeechResponseEventArgs"/> instance containing the event data.</param>
		private void WriteResponseResult(SpeechResponseEventArgs e)
		{
		    if (e.PhraseResponse.Results.Length == 0)
			{
				return;
				//Console.WriteLine("No phrase response is available.");
			}
			else
			{
				//Console.WriteLine("********* Final n-BEST Results *********");
				for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
				{
					//Console.WriteLine(
					//	"[{0}] Confidence={1}, Text=\"{2}\"",
					//	i,
					//	e.PhraseResponse.Results[i].Confidence,
					//	e.PhraseResponse.Results[i].DisplayText);
					FinalText.Append(e.PhraseResponse.Results[i].DisplayText + " ");
				}
				//Console.WriteLine();
			}
			return;
		}
		/// <summary>
		/// Sends the audio helper.
		/// </summary>
		/// <param name="wavFileName">Name of the wav file.</param>
		public void SendAudioHelper(string wavFileName)
		{
			if (dataClient == null)
			{
				this.CreateDataRecoClient();
			}
			using (FileStream fileStream = new FileStream(wavFileName, FileMode.Open, FileAccess.Read))
			{
				// Note for wave files, we can just send data from the file right to the server.
				// In the case you are not an audio file in wave format, and instead you have just
				// raw data (for example audio coming over bluetooth), then before sending up any 
				// audio data, you must first send up an SpeechAudioFormat descriptor to describe 
				// the layout and format of your raw audio data via DataRecognitionClient's sendAudioFormat() method.
				int bytesRead = 0;
				byte[] buffer = new byte[1024];

				try
				{
					do
					{
						// Get more Audio data to send into byte buffer.
						bytesRead = fileStream.Read(buffer, 0, buffer.Length);

						// Send of audio data to service. 
						try
						{
							this.dataClient.SendAudio(buffer, bytesRead);
						}
						catch (Exception ex)
						{
						}
					}
					while (bytesRead > 0);
				}
				finally
				{
					// We are done sending audio.  Final recognition results will arrive in OnResponseReceived event call.
					this.dataClient.EndAudio();
				}
				return;
			}
		}
	}
}
