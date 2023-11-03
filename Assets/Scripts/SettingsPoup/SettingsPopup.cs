using System;
using Game.LevelSystem;
using Game.SavingService;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Settings
{
    public class SettingsPopup : MonoBehaviour
    {
        [SerializeField]
        private Button m_SaveButton;

        [SerializeField]
        private Button m_LoadButton;

        private const string LevelDataFileName = "level-data";

        private void Awake()
        {
            m_SaveButton.onClick.AddListener(Save);
            m_LoadButton.onClick.AddListener(Load);
        }

        private void OnEnable()
        {
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }

        public void ManageActive()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        private void Save()
        {
            Level level = LevelLoader.LevelLoaderInstance.GetLevelData();
            JsonDataService.SaveData(LevelDataFileName, level);
        }

        private void Load()
        {
            try
            {
                Level level = JsonDataService.LoadData<Level>(LevelDataFileName);
                LevelLoader.LevelLoaderInstance.DownloadLevel(level);
            }
            catch (Exception e)
            {
                Debug.LogError($"Something went wrong! Error : {e.Message} {e.StackTrace}");
            }
        }
    }
}