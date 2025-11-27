using System.Collections.Generic;

public class LocalisationSystem
{
	public enum Language
	{
		English = 0,
		Spanish = 1,
		French = 2,
		German = 3,
		Japanese = 4,
		TraditionalChinese = 5,
		SimplifiedChinese = 6,
		Russian = 7,
		Portuguese = 8,
		Polish = 9,
		Turkish = 10
	}

	public static Language language;

	public static Dictionary<string, string> localisedEN;

	public static Dictionary<string, string> localisedES;

	public static Dictionary<string, string> localisedFR;

	public static Dictionary<string, string> localisedDE;

	public static Dictionary<string, string> localisedJP;

	public static Dictionary<string, string> localisedTC;

	public static Dictionary<string, string> localisedSC;

	public static Dictionary<string, string> localisedRU;

	public static Dictionary<string, string> localisedPT;

	public static Dictionary<string, string> localisedPL;

	public static Dictionary<string, string> localisedTR;

	public static bool isInit;

	public static CSVLoader csvLoader;

	public static void Init()
	{
		csvLoader = new CSVLoader();
		csvLoader.LoadCSV();
		UpdateDictionaries();
		isInit = true;
	}

	public static void UpdateDictionaries()
	{
		localisedEN = csvLoader.GetDictionaryValues("en");
		localisedES = csvLoader.GetDictionaryValues("es");
		localisedFR = csvLoader.GetDictionaryValues("fr");
		localisedDE = csvLoader.GetDictionaryValues("de");
		localisedJP = csvLoader.GetDictionaryValues("jp");
		localisedTC = csvLoader.GetDictionaryValues("zh-hant");
		localisedSC = csvLoader.GetDictionaryValues("zh-hans");
		localisedRU = csvLoader.GetDictionaryValues("ru");
		localisedPT = csvLoader.GetDictionaryValues("pt");
		localisedPL = csvLoader.GetDictionaryValues("pl");
		localisedTR = csvLoader.GetDictionaryValues("tr");
	}

	public static Dictionary<string, string> GetDictionaryForEditor()
	{
		if (!isInit)
		{
			Init();
		}
		return language switch
		{
			Language.English => localisedEN, 
			Language.Spanish => localisedES, 
			Language.French => localisedFR, 
			Language.German => localisedDE, 
			Language.Japanese => localisedJP, 
			Language.TraditionalChinese => localisedTC, 
			Language.SimplifiedChinese => localisedSC, 
			Language.Russian => localisedRU, 
			Language.Portuguese => localisedPT, 
			Language.Polish => localisedPL, 
			Language.Turkish => localisedTR, 
			_ => localisedEN, 
		};
	}

	public static string GetLocalisedValue(string key)
	{
		if (!isInit)
		{
			Init();
		}
		string value = key;
		switch (language)
		{
		case Language.English:
			localisedEN.TryGetValue(key, out value);
			break;
		case Language.Spanish:
			localisedES.TryGetValue(key, out value);
			break;
		case Language.French:
			localisedFR.TryGetValue(key, out value);
			break;
		case Language.German:
			localisedDE.TryGetValue(key, out value);
			break;
		case Language.Japanese:
			localisedJP.TryGetValue(key, out value);
			break;
		case Language.TraditionalChinese:
			localisedTC.TryGetValue(key, out value);
			break;
		case Language.SimplifiedChinese:
			localisedSC.TryGetValue(key, out value);
			break;
		case Language.Russian:
			localisedRU.TryGetValue(key, out value);
			break;
		case Language.Portuguese:
			localisedPT.TryGetValue(key, out value);
			break;
		case Language.Polish:
			localisedPL.TryGetValue(key, out value);
			break;
		case Language.Turkish:
			localisedTR.TryGetValue(key, out value);
			break;
		}
		if (value != null && value.Contains("|"))
		{
			value = value.Replace('|', '\n');
		}
		return value;
	}
}
