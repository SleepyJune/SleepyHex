using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Amazon.S3.Model;

public class LevelSelector2 : MonoBehaviour
{
    public LevelDatabase levelDatabase;

    public enum SortType
    {
        DateModified,
        Name,
        Difficulty,
    }

    public Transform levelList;
    public Transform levelListParent;

    public LevelSelectButton levelSelectionButton;

    public LevelLoader levelLoader;

    public Button[] difficultyButtons;

    public SortType sortType = SortType.Difficulty;

    [NonSerialized]
    public int difficultyFilter = -1;

    public DialogWindow difficultyPanel;

    DialogWindow selectorPanel;

    public Dictionary<string, LevelSelectButton> buttonDatabase;

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        SoundManager.instance.PlayHomeMusic();
    }

    public void Initialize()
    {
        difficultyFilter = PlayerPrefs.GetInt("difficultyFilter", -1);

        selectorPanel = GetComponent<DialogWindow>();

        if (difficultyFilter != -1)
        {
            difficultyPanel.Close();
        }

        LoadLevels();

        //WriteLevelID();
    }

    void LoadLevels()
    {
        if (!LevelSelector.isLoaded)
        {
            //LoadLevelLocal();

            LoadLevelByDatabase(levelDatabase);
            RefreshList();

            LevelSelector.isLoaded = true;
        }
        else
        {
            RefreshList();
        }
    }

    public static void LoadLevelByDatabase(LevelDatabase levelDatabase)
    {
        foreach(var level in levelDatabase.levels)
        {
            LevelSelector.AddLevel(level);
        }

        LevelSelector.isLoaded = true;
    }

    public void SetDifficultyFilter(int difficulty)
    {
        difficultyFilter = difficulty;
        PlayerPrefs.SetInt("difficultyFilter", difficultyFilter);

        RefreshList();
    }

    void LoadLevelLocal()
    {
        if (!Directory.Exists(DataPath.savePath))
        {
            Directory.CreateDirectory(DataPath.savePath);
        }

        if (Application.platform != RuntimePlatform.WindowsEditor) //overwrite files if mobile platform
        {
            LoadLevelFromAsset();
        }
        else
        {
            LoadLevelFromFile();
        }
    }

    LevelTextAsset LoadLevelFromString(string str)
    {
        var levelTextAsset = new LevelTextAsset("new", 0, -1, DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString());
        levelTextAsset.text = str;

        Level level = Level.LoadLevel(levelTextAsset);
        if (level != null)
        {
            levelTextAsset.levelName = level.levelName;
            levelTextAsset.levelID = level.levelID;
            levelTextAsset.localVersion = level.version;
            levelTextAsset.hasSolution = level.hasSolution;
            levelTextAsset.difficulty = level.difficulty;

            levelTextAsset.dateCreated = level.dateCreated;
            levelTextAsset.dateModified = level.dateModified;

            if (level.hasSolution)
            {
                if (level.solution.bestScore == level.solution.worstScore
                    && level.solution.numBestSolutions != level.solution.numSolutions)
                {
                    levelTextAsset.hasSolution = false;
                }
            }

            return levelTextAsset;

            //upload versions
            /*LevelVersion version = new LevelVersion()
            {
                //category = "Default",
                levelName = levelName,
                levelID = level.levelID,
                version = level.version,
                solved = level.hasSolution,
                difficulty = level.difficulty,
                dateCreated = level.dateCreated,
                dateModified = levelTextAsset.dateModified.ToString(),
                //timestamp = levelTextAsset.dateModified.GetUnixEpoch(),

            };

            amazonHelper.UploadLevelVersion(version);*/
        }

        return null;
    }

    void LoadLevelFromAsset()
    {
        var levels = Resources.LoadAll("Levels", typeof(TextAsset));

        foreach (var obj in levels)
        {
            var asset = obj as TextAsset;

            //var path = DataPath.savePath + level.name + ".json";
            //File.WriteAllText(DataPath.savePath + level.name + ".json", level.text);

            var levelTextAsset = LoadLevelFromString(asset.text);

            LevelSelector.levelDatabase.Add(levelTextAsset.levelName, levelTextAsset);
        }
    }

    void LoadLevelFromFile()
    {
        var fileListPath = DataPath.savePath + DataPath.fileListFolder + DataPath.fileListName;

        DirectoryInfo d = new DirectoryInfo(DataPath.savePath);
        foreach (var file in d.GetFiles("*.json"))
        {
            var path = file.FullName;
            string str = File.ReadAllText(path);

            var levelTextAsset = LoadLevelFromString(str);

            LevelSelector.levelDatabase.Add(levelTextAsset.levelName, levelTextAsset);
        }
    }

    public void SetSortType(int sortType)
    {
        this.sortType = (SortType)sortType;
        RefreshList();
    }

    public void LoadLevel(string levelName)
    {
        GameManager.instance.LoadLevel(levelName);
        selectorPanel.Close();
    }

    public void SetButtonStars(Score score)
    {
        var storedStars = score.GetStoredStars();

        Debug.Log(score.stars + " vs " + storedStars);

        if (score.stars > storedStars)
        {
            LevelSelectButton button;

            Debug.Log("1");

            if (buttonDatabase.TryGetValue(score.level.levelName, out button))
            {
                button.SetStars(score.stars);

                Debug.Log("here");
            }
        }
    }

    public void RefreshList()
    {        
        foreach (Transform child in levelList)
        {
            Destroy(child.gameObject);
        }

        IEnumerable<LevelTextAsset> filteredLevels =
            difficultyFilter > 0 ? LevelSelector.levelDatabase.Values.Where(level => Math.Floor(level.difficulty) == difficultyFilter) 
                                        : new List<LevelTextAsset>();

        if (sortType == SortType.Difficulty)
        {
            var comparer = new NaturalComparer();
            LevelSelector.levelListDatabase = filteredLevels
                        .OrderBy(level => level.difficulty)
                        .ThenBy(level => level.levelName, comparer)
                        .ToList();
        }
        else if (sortType == SortType.Name)
        {
            //var comparer = new NaturalComparer();
            //levelListDatabase = filteredLevels.OrderByDescending(level => level.name, comparer).ToList();

            LevelSelector.levelListDatabase = filteredLevels.OrderBy(level => level.difficulty).ThenBy(level => level.levelID).ToList();
        }
        else
        {
            LevelSelector.levelListDatabase = filteredLevels.OrderBy(level => DateTime.Parse(level.dateCreated)).ToList();
        }

        Debug.Log("Num levels: " + LevelSelector.levelListDatabase.Count());

        buttonDatabase = new Dictionary<string, LevelSelectButton>();

        foreach (var level in LevelSelector.levelListDatabase)
        {
            if (level != null)
            {
                var newButton = Instantiate(levelSelectionButton, levelList);
                newButton.SetButton(level, this);

                buttonDatabase.Add(level.levelName, newButton);
            }
        }
    }    
}
