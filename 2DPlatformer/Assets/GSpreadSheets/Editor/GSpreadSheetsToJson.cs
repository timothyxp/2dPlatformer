/*
 * Author: Trung Dong
 * www.trung-dong.com
 * Last update: 2017/08/18
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
*/
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;
using Newtonsoft.Json;

using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.Net;
using System.IO;

public class GSpreadSheetsToJson : EditorWindow {

	const string PREF_ACCESS_CODE = "accessCode";
	const string PREF_SHEET_KEY = "spreadSheetKey";
	
	static string ACCESS_TOKEN = "";

	static string REFRESH_TOKEN = "";

	static string CLIENT_ID = "871414866606-7b9687cp1ibjokihbbfl6nrjr94j14o8.apps.googleusercontent.com";

	static string CLIENT_SECRET = "zF_J3qHpzX5e8i2V-ZEvOdGV";

	static string SCOPE = "https://www.googleapis.com/auth/drive https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

	static string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

	static string TOKEN_TYPE = "refresh";

	public static GOAuth2RequestFactory RefreshAuthenticate() 
	{
		OAuth2Parameters parameters = new OAuth2Parameters()
		{
			RefreshToken = ACCESS_TOKEN,
			AccessToken = REFRESH_TOKEN,
			ClientId = CLIENT_ID,
			ClientSecret = CLIENT_SECRET,
			Scope = "https://www.googleapis.com/auth/drive https://spreadsheets.google.com/feeds",
			AccessType = "offline",
			TokenType = "refresh"
		};
		string authUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
		return new GOAuth2RequestFactory("spreadsheet", "MySpreadsheetIntegration-v1", parameters);
	}

	[SerializeField]
	private string accessCode = "";

	/// <summary>
	/// Key of the spreadsheet. Get from url of the spreadsheet.
	/// </summary>
	[SerializeField]
	private string spreadSheetKey = "";

	/// <summary>
	/// List of sheet names which want to download and convert to json file
	/// </summary>
	[SerializeField]
	private List<string> wantedSheetNames = new List<string>();

	/// <summary>
	/// Name of application.
	/// </summary>
	private string appName = "Unity";

	/// <summary>
	/// The root of spreadsheet's url.
	/// </summary>
	private string urlRoot = "https://spreadsheets.google.com/feeds/spreadsheets/";

	/// <summary>
	/// The directory which contain json files.
	/// </summary>
	[SerializeField]
	private string outputDir = "./Assets/Resources/JsonData/";

	/// <summary>
	/// The data types which is allowed to convert from sheet to json object
	/// </summary>
	private static List<string> allowedDataTypes = new List<string>(){"string", "int", "bool", "float", "string[]", "int[]", "bool[]", "float[]"};

	/// <summary>
	/// Position of the scroll view.
	/// </summary>
	private Vector2 scrollPosition;

	/// <summary>
	/// Progress of download and convert action. 100 is "completed".
	/// </summary>
	private float progress = 100;
	/// <summary>
	/// The message which be shown on progress bar when action is running.
	/// </summary>
	private string progressMessage = "";

	[MenuItem("Utility/GSheet to Json")]
	private static void ShowWindow()
	{
		GSpreadSheetsToJson window = EditorWindow.GetWindow(typeof(GSpreadSheetsToJson)) as GSpreadSheetsToJson;
		window.Init();
	}

	public void Init()
	{
		progress = 100;
		progressMessage = "";
		accessCode = PlayerPrefs.GetString(PREF_ACCESS_CODE, "");
		spreadSheetKey = PlayerPrefs.GetString(PREF_SHEET_KEY, "");
		ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
	}

	private void OnGUI()
	{
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.scrollView);
		GUILayout.BeginVertical();
		{
			GUILayout.Label("Settings", EditorStyles.boldLabel);
			spreadSheetKey = EditorGUILayout.TextField("Spread sheet key", spreadSheetKey);
			outputDir = EditorGUILayout.TextField("Path to store json files", outputDir);
			accessCode = EditorGUILayout.TextField("Access code", accessCode);
			GUILayout.Label("");
			GUILayout.Label("Sheet names", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("These sheets below will be downloaded. Let the list blank (remove all items) if you want to download all sheets", MessageType.Info);

			int _removeId = -1;
			for(int i = 0; i < wantedSheetNames.Count; i++)
			{
				GUILayout.BeginHorizontal();
				wantedSheetNames[i] = EditorGUILayout.TextField(string.Format("Sheet {0}",i), wantedSheetNames[i]);
				if(GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
				{
					_removeId = i;
				}
				GUILayout.EndHorizontal();
			}
			if(_removeId >= 0)
				wantedSheetNames.RemoveAt(_removeId);
			if(wantedSheetNames.Count <= 0)
			{
				GUILayout.Label("Download all sheets");
			}
			else
				GUILayout.Label(string.Format("Download {0} sheets", wantedSheetNames.Count));
			if(GUILayout.Button("Add sheet name", GUILayout.Width(130)))
			{
				wantedSheetNames.Add("");
			}
			GUILayout.Label("");
			GUI.backgroundColor = Color.green;
			if(GUILayout.Button("Download data \nthen convert to Json files"))
			{
				progress = 0;
				DownloadToJson();
			}
			GUI.backgroundColor = Color.white;
			if((progress < 100)&&(progress > 0))
			{
				if(EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100))
				{
					progress = 100;
					EditorUtility.ClearProgressBar();
				}
			}
			else
			{
				EditorUtility.ClearProgressBar();
			}
		}
		GUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}
	
	private void DownloadToJson()
	{
		//Validate input
		if(string.IsNullOrEmpty(spreadSheetKey))
		{
			Debug.LogError("spreadSheetKey can not be null!");
			return;
		}
			
		PlayerPrefs.SetString(PREF_SHEET_KEY, spreadSheetKey);
		PlayerPrefs.Save();

		if (ACCESS_TOKEN == "" && REFRESH_TOKEN == "")
		{
			EditorUtility.ClearProgressBar();
			GetAccessCode();
			return;
		}

		//Authenticate
		progressMessage = "Authenticating...";
		GOAuth2RequestFactory requestFactory = RefreshAuthenticate();
		SpreadsheetsService service = new SpreadsheetsService(appName);
		service.RequestFactory = requestFactory;

		progress = 5;
		EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);
		//Get the list of spreadsheets by the key
		SpreadsheetQuery query = new SpreadsheetQuery();
		query.Uri = new System.Uri(urlRoot + spreadSheetKey);
		progressMessage = "Get list of spreadsheets...";
		EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);
		SpreadsheetFeed feed = service.Query(query);
		if((feed == null)||(feed.Entries.Count <= 0))
		{
			Debug.LogError("Not found any data!");
			progress = 100;
			EditorUtility.ClearProgressBar();
			return;
		}
		progress = 10;
		//Get the first spreadsheet in result
		AtomEntry mySpreadSheet = feed.Entries[0];

		//Get data from that spreadsheet
		progressMessage = "Get entries from spreadsheets...";
		EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);
		AtomLink link = mySpreadSheet.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);
		WorksheetQuery sheetsQuery = new WorksheetQuery(link.HRef.ToString());
		WorksheetFeed sheetsFeed = service.Query(sheetsQuery);
		progress = 15;

		//For each sheet in received data, check the sheet name. If that sheet is the wanted sheet, create a json file
		foreach(WorksheetEntry sheet in sheetsFeed.Entries)
		{
			if((wantedSheetNames.Count <= 0) || (wantedSheetNames.Contains(sheet.Title.Text)))
			{
				progressMessage = string.Format("Processing {0}...", sheet.Title.Text);
				EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress/100);
				//Get all cell data in sheet
				AtomLink cellsLink = sheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);
				CellQuery cellsQuery = new CellQuery(cellsLink.HRef.ToString());
				CellFeed cellsFeed = service.Query(cellsQuery);
				//Create json file
				CreateJsonFile(sheet.Title.Text, outputDir, cellsFeed.Entries);
				if(wantedSheetNames.Count <= 0)
					progress += 85/(sheetsFeed.Entries.Count);
				else
					progress += 85/wantedSheetNames.Count;
			}
		}
		progress = 100;
        AssetDatabase.Refresh();
	}

	private void CreateJsonFile(string fileName, string outputDirectory, AtomEntryCollection cells)
	{
		//Get properties's name, data type and sheet data
		IDictionary<int, string> propertyNames = new Dictionary<int, string>();	//Dictionary of (column index, property name of that column)
		IDictionary<int, string> dataTypes = new Dictionary<int, string>();		//Dictionary of (column index, data type of that column)
		IDictionary<int, Dictionary<int, string>> values = new Dictionary<int, Dictionary<int, string>>();	//Dictionary of (row index, dictionary of (column index, value in cell))
		foreach(CellEntry cell in cells)
		{
			int columnIndex = (int)cell.Cell.Column - 1;
			int rowIndex = (int)cell.Cell.Row - 1;
			string value = cell.Cell.Value;
			if(rowIndex == 0)
			{//This row is properties's name row
				propertyNames.Add(columnIndex, value);
			}
			else if(rowIndex == 1)
			{//This row is properties's data type row
				dataTypes.Add(columnIndex, value);
			}
			else
			{//Data rows
				//Because first row is name row and second row is data type row, so we will minus 2 from rowIndex to make data index start from 0
				if(!values.ContainsKey(rowIndex - 2))
				{
					values.Add(rowIndex - 2, new Dictionary<int, string>());
				}
				values[rowIndex - 2].Add(columnIndex, value);
			}
		}

		//Create list of Dictionaries (property name, value). Each dictionary represent for a object in a row of sheet.
		List<object> datas = new List<object>();
		foreach(int rowId in values.Keys)	
		{
			bool thisRowHasError = false;
			Dictionary<string, object> data = new Dictionary<string, object>();
			foreach(int columnId in propertyNames.Keys)	
			{//Read through all columns in sheet, with each column, create a pair of property(string) and value(type depend on dataType[columnId])
				if(thisRowHasError) break;
				if((!dataTypes.ContainsKey(columnId))||(!allowedDataTypes.Contains(dataTypes[columnId])))
					continue;	//There is not any data type or this data type is strange. May be this column is used for comments. Skip this column.
				if(!values[rowId].ContainsKey(columnId))
				{
					values[rowId].Add(columnId, "");
				}

				string strVal = values[rowId][columnId];

				switch(dataTypes[columnId])
				{
					case "string":
					{
						data.Add(propertyNames[columnId], strVal);
						break;
					}
					case "int":
					{
						int val = 0;
						if(!string.IsNullOrEmpty(strVal))
						{
							try
							{
								val = int.Parse(strVal);
							}
							catch(System.Exception e)
							{
								Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}",  propertyNames[columnId], fileName, e.ToString()));
								thisRowHasError = true;
								continue;
							}
						}
						data.Add(propertyNames[columnId], val);
						break;
					}
					case "bool":
					{
						bool val = false;
						if(!string.IsNullOrEmpty(strVal))
						{
							try
							{
								val = bool.Parse(strVal);
							}
							catch(System.Exception e)
							{
								Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}",  propertyNames[columnId], fileName, e.ToString()));
								continue;
							}
						}
						data.Add(propertyNames[columnId], val);
						break;
					}
					case "float":
					{
						float val = 0f;
						if(!string.IsNullOrEmpty(strVal))
						{
							try
							{
								val = float.Parse(strVal);
							}
							catch(System.Exception e)
							{
								Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}",  propertyNames[columnId], fileName, e.ToString()));
								continue;
							}
						}
						data.Add(propertyNames[columnId], val);
						break;
					}
					case "string[]":
					{
						string[] valArr = strVal.Split(new char[]{','});
						data.Add(propertyNames[columnId], valArr);
						break;
					}
					case "int[]":
					{
						string[] strValArr = strVal.Split(new char[]{','});
						int[] valArr = new int[strValArr.Length];
						if (string.IsNullOrEmpty (strVal.Trim ())) {
							valArr = new int[0];
						}
						bool error = false;
						for(int i = 0; i < valArr.Length; i++)
						{
							int val = 0;
							if(!string.IsNullOrEmpty(strValArr[i]))
							{
								try
								{
									val = int.Parse(strValArr[i]);
								}
								catch(System.Exception e)
								{
									Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}",  propertyNames[columnId], fileName, e.ToString()));
									error = true;
									break;
								}
							}
							valArr[i] = val;
						}
						if(error)
							continue;
						data.Add(propertyNames[columnId], valArr);
						break;
					}
					case "bool[]":
					{
						string[] strValArr = strVal.Split(new char[]{','});
						bool[] valArr = new bool[strValArr.Length];
						if (string.IsNullOrEmpty (strVal.Trim ())) {
							valArr = new bool[0];
						}
						bool error = false;
						for(int i = 0; i < valArr.Length; i++)
						{
							bool val = false;
							if(!string.IsNullOrEmpty(strValArr[i]))
							{
								try
								{
									val = bool.Parse(strValArr[i]);
								}
								catch(System.Exception e)
								{
									Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}",  propertyNames[columnId], fileName, e.ToString()));
									error = true;
									break;
								}
							}
							valArr[i] = val;
						}
						if(error)
							continue;
						data.Add(propertyNames[columnId], valArr);
						break;
					}
					case "float[]":
					{
						string[] strValArr = strVal.Split(new char[]{','});
						float[] valArr = new float[strValArr.Length];
						if (string.IsNullOrEmpty (strVal.Trim ())) {
							valArr = new float[0];
						}
						bool error = false;
						for(int i = 0; i < valArr.Length; i++)
						{
							float val = 0f;
							if(!string.IsNullOrEmpty(strValArr[i]))
							{
								try
								{
									val = float.Parse(strValArr[i]);
								}
								catch(System.Exception e)
								{
									Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}",  propertyNames[columnId], fileName, e.ToString()));
									error = true;
									break;
								}
							}
							valArr[i] = val;
						}
						if(error)
							continue;
						data.Add(propertyNames[columnId], valArr);
						break;
					}
					default: break;	//This data type is strange, may be this column is used for comments, not for store data, so do nothing and read next column.
				}
			}

			if(!thisRowHasError)
			{
				datas.Add(data);
			}
			else
			{
				Debug.LogError("There's error!");
			}
		}

		//Create json text
		string jsonText = JsonConvert.SerializeObject((object)datas);

		//Create directory to store the json file
		if(!outputDirectory.EndsWith("/"))
			outputDirectory += "/";
		Directory.CreateDirectory(outputDirectory);
		StreamWriter strmWriter = new StreamWriter(outputDirectory + fileName + ".txt", false, System.Text.Encoding.UTF8);
		strmWriter.Write(jsonText);
		strmWriter.Close();
	}


	void GetAccessCode()
	{
		// OAuth2Parameters holds all the parameters related to OAuth 2.0.
		OAuth2Parameters parameters = new OAuth2Parameters();

		parameters.ClientId = CLIENT_ID;

		parameters.ClientSecret = CLIENT_SECRET;

		parameters.RedirectUri = REDIRECT_URI;

		// Get the Authorization URL
		parameters.Scope = SCOPE;

		parameters.AccessType = "offline"; // IMPORTANT and was missing in the original

		parameters.TokenType = TOKEN_TYPE; // IMPORTANT and was missing in the original

		// Authorization url.

		string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
		Debug.Log(authorizationUrl);
		Debug.Log("Please visit the URL above to authorize your OAuth "
			+ "request token.  Once that is complete, type in your access code to "
			+ "continue...");

		parameters.AccessCode = accessCode;

		if (parameters.AccessCode == "")
		{
			Debug.LogWarning("Access code is blank!");
			EditorUtility.ClearProgressBar();
			Application.OpenURL(authorizationUrl);
			return;
		}

		Debug.Log("Get access token.");

		// Get the Access Token
		try{
			OAuthUtil.GetAccessToken(parameters);
			string accessToken = parameters.AccessToken;
			string refreshToken = parameters.RefreshToken;
			Debug.Log("OAuth Access Token: " + accessToken + "\n");
			ACCESS_TOKEN = accessToken;
			Debug.Log("OAuth Refresh Token: " + refreshToken + "\n");
			REFRESH_TOKEN = refreshToken;
			PlayerPrefs.SetString(PREF_ACCESS_CODE, accessCode);
			PlayerPrefs.Save();
			DownloadToJson();
		}
		catch(System.Exception e)
		{
			Debug.LogError(e.ToString());
			EditorUtility.ClearProgressBar();
			Application.OpenURL(authorizationUrl);
			return;
		}
	}

	public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
		bool isOk = true;
		// If there are errors in the certificate chain, look at each error to determine the cause.
		if (sslPolicyErrors != SslPolicyErrors.None) {
			for(int i=0; i<chain.ChainStatus.Length; i++) {
				if(chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown) {
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					bool chainIsValid = chain.Build((X509Certificate2)certificate);
					if(!chainIsValid) {
						isOk = false;
					}
				}
			}
		}
		return isOk;
	}
}
