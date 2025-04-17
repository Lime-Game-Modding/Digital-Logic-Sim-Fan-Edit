using DLS.Graphics;
using DLS.Simulation;
using Seb.Vis;
using Seb.Vis.UI;
using FanEdit.Sound;
using UnityEngine;

namespace DLS.Game
{
	public class UnityMain : MonoBehaviour
	{
		public static UnityMain instance;
		public bool openSaveDirectory;
		public AudioSource audioSource;

		[Header("Dev Settings (editor only)")]
		public bool openInMainMenu;

		public string testProjectName;
		public bool openA = true;
		public string chipToOpenA;
		public string chipToOpenB;

		[Header("Temp test vars")] public Vector2 testVecA;
		public Vector2 testVecB;
		public Vector2 testVecC;
		public Vector2 testVecD;
		public Color testColA;
		public Color testColB;
		public Color testColC;
		public Color testColD;
		public string testString;
		public string testString2;
		public ButtonTheme testButtonTheme;
		public bool testbool;
		public Anchor testAnchor;

		int SoundCount;

		void Awake()
		{
			instance = this;
			ResetStatics();
			InitSound();

			Main.Init();

			if (openInMainMenu || !Application.isEditor) Main.LoadMainMenu();
			else Main.CreateOrLoadProject(testProjectName, openA ? chipToOpenA : chipToOpenB);
		}

		void Update()
		{
			Main.Update();
            if (SoundCount <= 0)
            {
				SoundCount = 0;
				audioSource.Stop();
			}
		}

		void OnDestroy()
		{
			if (Project.ActiveProject != null) Project.ActiveProject.NotifyExit();
		}

		void OnValidate()
		{
			if (openSaveDirectory)
			{
				openSaveDirectory = false;
				Main.OpenSaveDataFolderInFileBrowser();
			}
		}

		// Ensure static stuff gets properly reset (on account of domain-reloading being disabled in editor)
		static void ResetStatics()
		{
			Simulator.Reset();
			UIDrawer.Reset();
			InteractionState.Reset();
			CameraController.Reset();
			WorldDrawer.Reset();
			FanEdit.Sound.SoundManager.Reset();
		}
		
		void InitSound()
        {
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.spatialBlend = 0;
			audioSource.Stop();
		}

		public void PlaySound(out int timeIndex)
        {
			timeIndex = 0;  //resets timer before playing sound
			audioSource.Play();
			SoundCount++;
		}

		public void StopPlaySound()
        {
			SoundCount--;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
			if (SoundManager.activePlayers.Count == 0) { return; }
			for(int i =0; i < SoundManager.activePlayers.Count; i++)
			{
				SoundManager.activePlayers[i].GenerateAudioFilterData(ref data, channels);
			}   
		}
    }
}