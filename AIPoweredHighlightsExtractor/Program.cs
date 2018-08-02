using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace AIPoweredHighlightsExtractor
{
	class Program
	{
		static void Main(string[] args)
		{
			STT stt = new AIPoweredHighlightsExtractor.STT();
			stt.SubscriptionKey = "7fb00c451af14991904ae0d0cd9a5490";
			Console.WriteLine("************************************* AI POWERED HIGHLIGHT EXTRACTOR *************************************");
			//Console.WriteLine();
			Console.WriteLine("		Hi.. I am show highlight exctractor. I will summarize any video for you!");
			Console.WriteLine("		What would you like to hear about?");
			Console.WriteLine();
			Console.WriteLine("		1. Satya's speech regarding importance of education and role of technology in education");
			Console.WriteLine("		2. Satya's interview");
			Console.WriteLine("		3. Batman");
			//Console.WriteLine();
			Console.WriteLine("************************************* AI POWERED HIGHLIGHT EXTRACTOR *************************************");
			int x = Convert.ToInt32(Console.ReadLine());
			Console.WriteLine("Please bear with me for sometime! I am a bit slow but really useful.. :D");
			switch (x)
			{
				case 1:
					stt.SendAudioHelper("satya2.wav");
					break;
				case 2:
					stt.SendAudioHelper("satya1.wav");
					break;
				case 3:
					stt.SendAudioHelper("batman.wav");
					break;
				default:
					stt.SendAudioHelper("satya2.wav");
					break;
			}
		}
	}
}
