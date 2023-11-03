using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button LoadButton;

    private const string LevelDataFileName = "level-data";

    private void Awake()
    {
        SaveButton.onClick.AddListener(Save);
        LoadButton.onClick.AddListener(Load);
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

        try
        {
            JsonDataService.SaveData(LevelDataFileName, level);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save file! Error : {e.Message} {e.StackTrace}");
        }
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